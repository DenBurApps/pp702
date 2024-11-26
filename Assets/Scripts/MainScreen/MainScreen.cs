using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class MainScreen : MonoBehaviour
{
    [SerializeField] private List<PlantPlane> _planes;
    [SerializeField] private GameObject _emptyPlane;
    [SerializeField] private GameObject _topPlane;
    [SerializeField] private CongradulationScreen _congradulationScreen;
    [SerializeField] private NewPlant _newPlantScreen;
    [SerializeField] private NewPlant _editPlantScreen;
    [SerializeField] private OpenPlant _openPlant;
    [SerializeField] private CalendarScreen _calendarScreen;
    [SerializeField] private Settings _settings;
    [SerializeField] private TMP_InputField _search;
    [SerializeField] private ArticleScreen _articleScreen;

    private ScreenVisabilityHandler _screenVisabilityHandler;
    
    public event Action<PlantPlane> PlaneOpened;

    public IReadOnlyCollection<PlantPlane> Planes => _planes;

    private void Awake()
    {
        Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _congradulationScreen.SkipClicked += _screenVisabilityHandler.EnableScreen;
        _newPlantScreen.PlantDataSaved += EnablePlane;
        _newPlantScreen.BackClicked += _screenVisabilityHandler.EnableScreen;
        _openPlant.BackClicked += _screenVisabilityHandler.EnableScreen;
        _editPlantScreen.Edited += EnableAndSave;
        _openPlant.Deleted += DeletePlane;
        _calendarScreen.BackClicked += _screenVisabilityHandler.EnableScreen;
        _settings.MainScreenOpened += _screenVisabilityHandler.EnableScreen;
        _search.onValueChanged.AddListener(Search);
        _articleScreen.BackClicked += _screenVisabilityHandler.EnableScreen;
        
        foreach (var plane in _planes)
        {
            plane.PlaneOpened += OpenPlane;
        }
    }

    private void OnDisable()
    {
        _congradulationScreen.SkipClicked -= _screenVisabilityHandler.EnableScreen;
        _newPlantScreen.PlantDataSaved -= EnablePlane;
        _newPlantScreen.BackClicked -= _screenVisabilityHandler.EnableScreen;
        _openPlant.BackClicked -= _screenVisabilityHandler.EnableScreen;
        _editPlantScreen.Edited -= EnableAndSave;
        _openPlant.Deleted -= DeletePlane;
        _calendarScreen.BackClicked -= _screenVisabilityHandler.EnableScreen;
        _settings.MainScreenOpened -= _screenVisabilityHandler.EnableScreen;
        _search.onValueChanged.RemoveListener(Search);
        _articleScreen.BackClicked -= _screenVisabilityHandler.EnableScreen;
        
        foreach (var plane in _planes)
        {
            plane.PlaneOpened -= OpenPlane;
        }
    }

    private void Start()
    {
        _screenVisabilityHandler.EnableScreen();
        DisableAllPlanes();
        
        List<PlantData> savedPlantData = PlantDataSaveSystem.Load();
        foreach (var data in savedPlantData)
        {
            EnablePlane(data);
        }
    }

    private void EnableAndSave()
    {
        _screenVisabilityHandler.EnableScreen();
        SaveAllPlantData();
    }

    public void OpenSettings()
    {
        _settings.ShowSettings();
        _screenVisabilityHandler.SetTransperent();
    }
    
    private void Search(string text)
    {
        string adaptedText = text.ToLower();

        if (string.IsNullOrEmpty(adaptedText))
        {
            foreach (var plane in _planes)
            {
                if (plane.PlantData != null)
                {
                    plane.gameObject.SetActive(true);
                }
            }
            
            return;
        }

        foreach (var plane in _planes)
        {
            if (plane.PlantData != null)
            {
                if (string.Equals(plane.PlantData.Name, adaptedText, StringComparison.CurrentCulture))
                {
                    plane.gameObject.SetActive(true);
                }
                else
                {
                    plane.gameObject.SetActive(false);
                }
            }
        }
    }
    
    public void OnAddPlantClicked()
    {
        _newPlantScreen.EnableScreen();
        _screenVisabilityHandler.SetTransperent();
    }

    private void EnablePlane(PlantData data)
    {
        var availablePlane = _planes.FirstOrDefault(plane => !plane.IsActive);

        if (availablePlane != null)
        {
            availablePlane.Enable(data);
        }
        
        SaveAllPlantData();
        ToggleEmptyPlane();
    }

    private void DeletePlane(PlantPlane plane)
    {
        _screenVisabilityHandler.EnableScreen();
        plane.Disable();
        ToggleEmptyPlane();
        SaveAllPlantData();
    }
    
    private void DisableAllPlanes()
    {
        foreach (var plane in _planes)
        {
            plane.Disable();
        }

        ToggleEmptyPlane();
    }

    private void ToggleEmptyPlane()
    {
        bool isValid = _planes.All(plane => !plane.IsActive);

        _emptyPlane.SetActive(isValid);
        _topPlane.SetActive(!isValid);
    }

    private void OpenPlane(PlantPlane plane)
    {
        PlaneOpened?.Invoke(plane);
        _screenVisabilityHandler.DisableScreen();
    }
    
    private void SaveAllPlantData()
    {
        var petDataList = new List<PlantData>();

        foreach (var plane in _planes)
        {
            if (plane.PlantData != null)
            {
                petDataList.Add(plane.PlantData);
            }
        }

        PlantDataSaveSystem.Save(petDataList);
    }

}
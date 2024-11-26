using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class OpenPlant : MonoBehaviour
{
    [SerializeField] private PhotosController _photosController;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _day;
    [SerializeField] private TMP_Text _wateringFrequency;
    [SerializeField] private TMP_Text _wateringNextText;
    [SerializeField] private TMP_Text _sprayingFrequency;
    [SerializeField] private TMP_Text _sprayingNextText;
    [SerializeField] private TMP_Text _fertilizerFrequency;
    [SerializeField] private TMP_Text _fertilizerNextText;
    [SerializeField] private TMP_Text _replantingFrequency;
    [SerializeField] private TMP_Text _replantingNextText;
    [SerializeField] private TMP_Text _dateOfPlanting;
    [SerializeField] private MainScreen _mainScreen;

    [SerializeField] private GameObject _wateringPlane;
    [SerializeField] private GameObject _sprayingPlane;
    [SerializeField] private GameObject _fretilizerPlane;
    [SerializeField] private GameObject _replantingPlane;
    [SerializeField] private NewPlant _editPlant;

    private ScreenVisabilityHandler _screenVisabilityHandler;
    private PlantPlane _plantPlane;

    public event Action<PlantPlane> Deleted;
    public event Action BackClicked;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _mainScreen.PlaneOpened += Enable;
        _editPlant.BackClicked += _screenVisabilityHandler.EnableScreen;
    }

    private void OnDisable()
    {
        _mainScreen.PlaneOpened -= Enable;
        _editPlant.BackClicked -= _screenVisabilityHandler.EnableScreen;
    }

    private void Start()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    public void OnDelete()
    {
        Deleted?.Invoke(_plantPlane);
        _screenVisabilityHandler.DisableScreen();
    }

    public void OnEdit()
    {
        _editPlant.EnableScreenForEditing(_plantPlane);
        _screenVisabilityHandler.DisableScreen();
    }

    public void OnBackClicked()
    {
        BackClicked?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }

    private void Enable(PlantPlane plantPlane)
    {
        _screenVisabilityHandler.EnableScreen();
        _plantPlane = plantPlane;

        if (_plantPlane.PlantData.ImagePath != null)
        {
            _photosController.SetPhotos(_plantPlane.PlantData.ImagePath);
        }
        else
        {
            _photosController.ResetPhotos();
        }

        _name.text = _plantPlane.PlantData.Name;
        _plantPlane.PlantData.UpdateCreationDate();
        _day.text = _plantPlane.PlantData.DayCount.ToString();

        if (_plantPlane.PlantData.Watering != null && _plantPlane.PlantData.Watering.Number != default && !string.IsNullOrEmpty(_plantPlane.PlantData.Watering.Frequency))
        {
            _wateringFrequency.text = $"{_plantPlane.PlantData.Watering.Number} times a {_plantPlane.PlantData.Watering.Frequency}";
            _wateringNextText.text = CalculateNextCareText(_plantPlane.PlantData.Watering, "Watering");
        }
        else
        {
            _wateringPlane.SetActive(false);
        }
        
        if (_plantPlane.PlantData.Spraying != null && _plantPlane.PlantData.Spraying.Number != default && !string.IsNullOrEmpty(_plantPlane.PlantData.Spraying.Frequency))
        {
            Debug.Log("has spraying");
            _sprayingFrequency.text = $"{_plantPlane.PlantData.Spraying.Number} times a {_plantPlane.PlantData.Spraying.Frequency}";
            _sprayingNextText.text = CalculateNextCareText(_plantPlane.PlantData.Spraying, "Spraying");
        }
        else
        {
            Debug.Log("no spraying");
            _sprayingPlane.SetActive(false);
        }
        
        if (_plantPlane.PlantData.Fertilizer != null && _plantPlane.PlantData.Fertilizer.Number != default && !string.IsNullOrEmpty(_plantPlane.PlantData.Fertilizer.Frequency))
        {
            _fertilizerFrequency.text = $"{_plantPlane.PlantData.Fertilizer.Number} times a {_plantPlane.PlantData.Fertilizer.Frequency}";
            _fertilizerNextText.text = CalculateNextCareText(_plantPlane.PlantData.Fertilizer, "Fertilizer");
        }
        else
        {
            _fretilizerPlane.SetActive(false);
        }
        
        if (_plantPlane.PlantData.Replanting != null && _plantPlane.PlantData.Replanting.Number != default && !string.IsNullOrEmpty(_plantPlane.PlantData.Replanting.Frequency))
        {
            _replantingFrequency.text = $"{_plantPlane.PlantData.Replanting.Number} times a {_plantPlane.PlantData.Replanting.Frequency}";
            _replantingNextText.text = CalculateNextCareText(_plantPlane.PlantData.Replanting, "Replanting");
        }
        else
        {
            _replantingPlane.SetActive(false);
        }
        
        _dateOfPlanting.text = _plantPlane.PlantData.PlantingDate.ToString("dd.MMM.yyyy");
    }

    private string CalculateNextCareText(CareData careData, string careType)
    {
        if (careData == null || careData.Number <= 0 || string.IsNullOrEmpty(careData.Frequency))
        {
            return "No upcoming actions";
        }

        DateTime nextDate = CalculateNextCareDate(careData);

        if (nextDate == DateTime.MaxValue)
        {
            return "Invalid frequency";
        }

        return $"Next Care: {careType} {nextDate:MMM. dd, yyyy}";
    }
    
    private DateTime CalculateNextCareDate(CareData careData)
    {
        TimeSpan frequencySpan;
        switch (careData.Frequency.ToLower())
        {
            case "day":
                frequencySpan = TimeSpan.FromDays(1.0 / careData.Number);
                break;
            case "week":
                frequencySpan = TimeSpan.FromDays(7.0 / careData.Number);
                break;
            case "month":
                frequencySpan = TimeSpan.FromDays(30.0 / careData.Number);
                break;
            case "year":
                frequencySpan = TimeSpan.FromDays(365.0 / careData.Number);
                break;
            default:
                Debug.LogWarning($"Unknown frequency: {careData.Frequency}");
                return DateTime.MaxValue;
        }

        return careData.LastWatering.Add(frequencySpan);
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class NewPlant : MonoBehaviour
{
    [SerializeField] private PhotosController _photosController;
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private TMP_InputField _typeInput;
    [SerializeField] private CareInformation _watering;
    [SerializeField] private CareInformation _spraying;
    [SerializeField] private CareInformation _fertilizer;
    [SerializeField] private CareInformation _replanting;
    [SerializeField] private DateOfPlanting _dateOfPlanting;
    [SerializeField] private MainScreen _mainScreen;
    [SerializeField] private CongradulationScreen _congradulationScreen;

    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _backButton;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    private string _name;
    private string _type;
    private PlantPlane _editingPlantPlane;

    public event Action<PlantData> PlantDataSaved;
    public event Action BackClicked;
    public event Action Edited;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _nameInput.onValueChanged.AddListener(SetName);
        _typeInput.onValueChanged.AddListener(SetType);

        _saveButton.onClick.AddListener(Save);
        _backButton.onClick.AddListener(OnBackClicked);
    }

    private void OnDisable()
    {
        _nameInput.onValueChanged.RemoveListener(SetName);
        _typeInput.onValueChanged.RemoveListener(SetType);

        _saveButton.onClick.RemoveListener(Save);
        _backButton.onClick.RemoveListener(OnBackClicked);
    }

    private void Start()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    public void EnableScreen()
    {
        _editingPlantPlane = null;
        _screenVisabilityHandler.EnableScreen();
        ResetData();
        ToggleSaveButton();
    }

    public void EnableScreenForEditing(PlantPlane plantPlane)
    {
        _editingPlantPlane = plantPlane;
        _screenVisabilityHandler.EnableScreen();

        var plantData = plantPlane.PlantData;

        _name = plantData.Name;
        _type = plantData.Type;

        _nameInput.text = _name;
        _typeInput.text = _type;

        if (plantData.ImagePath != null)
        {
            _photosController.SetPhotos(plantData.ImagePath);
        }
        else
        {
            _photosController.ResetPhotos();
        }

        PopulateCareData(_watering, plantData.Watering);
        PopulateCareData(_spraying, plantData.Spraying);
        PopulateCareData(_fertilizer, plantData.Fertilizer);
        PopulateCareData(_replanting, plantData.Replanting);

        _dateOfPlanting.SetData(plantData.PlantingDate);

        ToggleSaveButton();
    }

    private void PopulateCareData(CareInformation careInformation, CareData careData)
    {
        if (careData != null)
        {
            careInformation.Populate(careData.Frequency, careData.Number.ToString(), careData.LastWatering, true);
        }
        else
        {
            careInformation.ResetValues();
        }
    }

    private void SetName(string name)
    {
        _name = name;
        ToggleSaveButton();
    }

    private void SetType(string type)
    {
        _type = type;
        ToggleSaveButton();
    }

    private void ToggleSaveButton()
    {
        _saveButton.interactable = !string.IsNullOrEmpty(_name) && !string.IsNullOrEmpty(_type) &&
                                   _dateOfPlanting.SelectedDate != default;
    }

    private void Save()
    {
        var dataToSave = new PlantData(_photosController.GetPhoto(), _name, _type, CreateCareData(_watering),
            CreateCareData(_spraying),
            CreateCareData(_fertilizer), CreateCareData(_replanting),
            _dateOfPlanting.SelectedDate);

        if (_editingPlantPlane != null)
        {
            _editingPlantPlane.Enable(dataToSave);
            Edited?.Invoke();
            _screenVisabilityHandler.DisableScreen();
            ResetData();
            return;
        }

        PlantDataSaved?.Invoke(dataToSave);
        _screenVisabilityHandler.DisableScreen();
        ResetData();
    }

    private CareData CreateCareData(CareInformation careInformation)
    {
        if (careInformation.IsEnabled)
        {
            return new CareData(int.Parse(careInformation.FrequencyNumber), careInformation.FrequencyName,
                careInformation.SelectedDate);
        }

        return null;
    }

    private void ResetData()
    {
        _watering.ResetValues();
        _spraying.ResetValues();
        _fertilizer.ResetValues();
        _replanting.ResetValues();
        _dateOfPlanting.ResetValues();

        _name = string.Empty;
        _type = string.Empty;
        _nameInput.text = _name;
        _typeInput.text = _type;
        _photosController.ResetPhotos();
        ToggleSaveButton();
    }

    private void OnBackClicked()
    {
        BackClicked?.Invoke();
        ResetData();
        _screenVisabilityHandler.DisableScreen();
    }
}
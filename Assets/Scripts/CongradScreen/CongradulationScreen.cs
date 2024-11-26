using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class CongradulationScreen : MonoBehaviour
{
    [SerializeField] private Sprite _selectedSprite;
    [SerializeField] private Sprite _unselectedSprite;

    [SerializeField] private TMP_Text _topText;
    [SerializeField] private TMP_Text _bottomText;
    [SerializeField] private PhotosController _photosController;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Button _wateringButton;
    [SerializeField] private Button _addNewPlant;
    [SerializeField] private Button _skipButton;
    [SerializeField] private NewPlant _newPlantScreen;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    private bool _isSelected;

    public event Action SkipClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _addNewPlant.onClick.AddListener(OnAddNewPlant);
        _skipButton.onClick.AddListener(OnSkipClicked);
        _wateringButton.onClick.AddListener(SetSelected);
        _newPlantScreen.PlantDataSaved += Enable;
    }

    private void OnDisable()
    {
        _addNewPlant.onClick.RemoveListener(OnAddNewPlant);
        _skipButton.onClick.RemoveListener(OnSkipClicked);
        _wateringButton.onClick.RemoveListener(SetSelected);
        _newPlantScreen.PlantDataSaved -= Enable;
    }

    private void Start()
    {
        ResetData();
        _screenVisabilityHandler.DisableScreen();
    }

    private void Enable(PlantData data)
    {
        _screenVisabilityHandler.EnableScreen();
        _nameText.text = data.Name;

        if (data.ImagePath != null)
        {
            _photosController.SetPhotos(data.ImagePath);
        }
        else
        {
            _photosController.ResetPhotos();
        }

        _addNewPlant.gameObject.SetActive(false);
        _bottomText.gameObject.SetActive(true);
        _topText.text = "Congratulations on the addition of the plant! Now try watering it";
        _wateringButton.image.sprite = _unselectedSprite;
        _isSelected = false;
    }

    private void SetSelected()
    {
        if (_isSelected)
        {
            _addNewPlant.gameObject.SetActive(false);
            _bottomText.gameObject.SetActive(true);
            _topText.text = "Congratulations on the addition of the plant! Now try watering it";
            _wateringButton.image.sprite = _unselectedSprite;
            _isSelected = false;
        }
        else
        {
            _addNewPlant.gameObject.SetActive(true);
            _bottomText.gameObject.SetActive(false);
            _topText.text = "Well done, you're doing a good job! Now you can add more plants";
            _wateringButton.image.sprite = _selectedSprite;
            _isSelected = true;
        }
    }

    private void OnAddNewPlant()
    {
        _newPlantScreen.EnableScreen();
        ResetData();
        _screenVisabilityHandler.DisableScreen();
    }

    private void OnSkipClicked()
    {
        SkipClicked?.Invoke();
        ResetData();
        _screenVisabilityHandler.DisableScreen();
    }

    private void ResetData()
    {
        _addNewPlant.gameObject.SetActive(false);
        _bottomText.gameObject.SetActive(true);
        _topText.text = "Congratulations on the addition of the plant! Now try watering it";
        _wateringButton.image.sprite = _unselectedSprite;
        _isSelected = false;
        _photosController.ResetPhotos();
    }
}
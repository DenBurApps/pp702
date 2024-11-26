using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CareInformation : MonoBehaviour
{
    [SerializeField] private Sprite _unselectedSprite;
    [SerializeField] private Sprite _selectedSprite;

    [SerializeField] private DateSelector _dateSelector;
    [SerializeField] private FrequencySelector _frequencySelector;
    [SerializeField] private GameObject[] _objectsToDisable;
    [SerializeField] private Button _openButton;

    [SerializeField] private Button _frequencyOpener;
    [SerializeField] private Button _dateOpener;

    [SerializeField] private TMP_Text _frequencyText;
    [SerializeField] private TMP_Text _dateText;

    private bool _isEnabled;

    private string _frequencyName;
    private string _frequencyNumber;

    private string _year;
    private string _day;
    private string _month;

    private DateTime _selectedDate;

    public string FrequencyName => _frequencyName;

    public string FrequencyNumber => _frequencyNumber;

    public string Year => _year;

    public string Day => _day;

    public string Month => _month;

    public DateTime SelectedDate => _selectedDate;
    public bool IsEnabled => _isEnabled;

    private void OnEnable()
    {
        _frequencySelector.TimesNumberInputed += SetFrequencyNumber;
        _frequencySelector.TimesNameInputed += SetFrequencyName;

        _frequencyOpener.onClick.AddListener(ToggleFrequency);
        _dateOpener.onClick.AddListener(ToggleDateSelector);

        _dateSelector.YearInputed += SetYear;
        _dateSelector.MonthInputed += SetMonth;
        _dateSelector.DayInputed += SetDay;

        _openButton.onClick.AddListener(ToggleButton);
    }

    private void OnDisable()
    {
        _frequencySelector.TimesNumberInputed -= SetFrequencyNumber;
        _frequencySelector.TimesNameInputed -= SetFrequencyName;

        _frequencyOpener.onClick.RemoveListener(ToggleFrequency);
        _dateOpener.onClick.RemoveListener(ToggleDateSelector);

        _dateSelector.YearInputed -= SetYear;
        _dateSelector.MonthInputed -= SetMonth;
        _dateSelector.DayInputed -= SetDay;

        _openButton.onClick.RemoveListener(ToggleButton);
    }

    private void Start()
    {
        ResetValues();
        UpdateUI();
    }
    
  

    private void ToggleButton()
    {
        if (_isEnabled)
        {
            SetSelection(_unselectedSprite, false);
        }
        else
        {
            SetSelection(_selectedSprite, true);
        }
    }

    private void SetSelection(Sprite sprite, bool activeStatus)
    {
        _openButton.image.sprite = sprite;

        foreach (var obj in _objectsToDisable)
        {
            obj.SetActive(activeStatus);
        }

        _isEnabled = activeStatus;
    }

    private void SetFrequencyName(string name)
    {
        _frequencyName = name;
        SetFrequency();
    }

    private void SetFrequencyNumber(string number)
    {
        _frequencyNumber = number;
        SetFrequency();
    }

    private void SetYear(string year)
    {
        _year = year;
        SetDate();
    }

    private void SetMonth(string month)
    {
        _month = month;
        SetDate();
    }

    private void SetDay(string day)
    {
        _day = day;
        SetDate();
    }

    private void SetFrequency()
    {
        if (string.IsNullOrEmpty(_frequencyName) || string.IsNullOrEmpty(_frequencyNumber))
            return;
        
        _frequencyText.text = $"{_frequencyNumber} times a {_frequencyName}";
    }

    private void SetDate()
    {
        if (string.IsNullOrEmpty(_year) || string.IsNullOrEmpty(_month) || string.IsNullOrEmpty(_day))
        {
            Debug.LogWarning("Date components are missing. Cannot set date.");
            return;
        }

        try
        {
            string dateString = $"{_year}-{_month}-{_day}";
            _selectedDate = DateTime.Parse(dateString);
            _dateText.text = _selectedDate.ToString("MMM. dd, yyyy");
        }
        catch (FormatException ex)
        {
            Debug.LogError($"Invalid date format: {_day}-{_month}-{_year}. Exception: {ex.Message}");
            _dateText.text = "Invalid Date";
        }
    }

    private void ToggleFrequency()
    {
        if (_frequencySelector.isActiveAndEnabled)
        {
            _frequencySelector.Disable();
        }
        else
        {
            _frequencySelector.Enable();
        }
    }

    private void ToggleDateSelector()
    {
        if (_dateSelector.isActiveAndEnabled)
        {
            _dateSelector.Disable();
        }
        else
        {
            _dateSelector.Enable();
        }
    }

    public void ResetValues()
    {
        _frequencyName = "day";
        _frequencyNumber = "3";

        _dateSelector.gameObject.SetActive(false);
        _frequencySelector.gameObject.SetActive(false);

        _selectedDate = DateTime.Now;
        _year = _selectedDate.Year.ToString();
        _month = _selectedDate.Month.ToString();
        _day = _selectedDate.Day.ToString();

        _isEnabled = false;
        SetSelection(_unselectedSprite, false);
    }

    private void UpdateUI()
    {
        SetFrequency();
        SetDate();
    }
    
    public void Populate(string frequencyName, string frequencyNumber, DateTime selectedDate, bool isEnabled)
    {
        _frequencyName = frequencyName;
        _frequencyNumber = frequencyNumber;

        _year = selectedDate.Year.ToString();
        _month = selectedDate.Month.ToString();
        _day = selectedDate.Day.ToString();
        _selectedDate = selectedDate;

        _isEnabled = isEnabled;
        SetSelection(isEnabled ? _selectedSprite : _unselectedSprite, isEnabled);

        UpdateUI();
    }
}
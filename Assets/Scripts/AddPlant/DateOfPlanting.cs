using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DateOfPlanting : MonoBehaviour
{
    [SerializeField] private DateSelector _dateSelector;
    [SerializeField] private Button _dateOpener;
    [SerializeField] private TMP_Text _dateText;
    
    private string _year;
    private string _day;
    private string _month;

    private DateTime _selectedDate;
    
    public string Year => _year;

    public string Day => _day;

    public string Month => _month;

    public DateTime SelectedDate => _selectedDate;
    
    private void OnEnable()
    {
        _dateOpener.onClick.AddListener(ToggleDateSelector);

        _dateSelector.YearInputed += SetYear;
        _dateSelector.MonthInputed += SetMonth;
        _dateSelector.DayInputed += SetDay;
    }

    private void OnDisable()
    {
        _dateOpener.onClick.RemoveListener(ToggleDateSelector);

        _dateSelector.YearInputed -= SetYear;
        _dateSelector.MonthInputed -= SetMonth;
        _dateSelector.DayInputed -= SetDay;
    }

    private void Start()
    {
        ResetValues();
        SetDate();
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
            _dateText.text = _selectedDate.ToString("dd.MM.yyyy");
        }
        catch (FormatException ex)
        {
            Debug.LogError($"Invalid date format: {_day}-{_month}-{_year}. Exception: {ex.Message}");
            _dateText.text = "Invalid Date";
        }
    }

    public void SetData(DateTime plantingDate)
    {
        _dateSelector.Enable();
        _selectedDate = plantingDate;
        _dateText.text = _selectedDate.ToString("dd.MM.yyyy");
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
        _dateSelector.gameObject.SetActive(false);
        _selectedDate = DateTime.Now;
        _year = _selectedDate.Year.ToString();
        _month = _selectedDate.Month.ToString();
        _day = _selectedDate.Day.ToString();
    }
}

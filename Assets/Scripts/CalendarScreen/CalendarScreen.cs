using System;
using System.Collections.Generic;
using System.Linq;
using Bitsplash.DatePicker;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class CalendarScreen : MonoBehaviour
{
    [SerializeField] private DatePickerSettings _datePicker;
    [SerializeField] private List<CalendarPlane> _planes;
    [SerializeField] private TMP_Text _todayText;
    [SerializeField] private TMP_Text _emptyText;
    [SerializeField] private TMP_Text _filledText;
    [SerializeField] private MainScreen _mainScreen;
    [SerializeField] private Settings _settings;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action BackClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _datePicker.Content.OnSelectionChanged.AddListener(DateSelected);
        _settings.CalendarOpened += EnableScreen;
    }

    private void OnDisable()
    {
        _datePicker.Content.OnSelectionChanged.RemoveListener(DateSelected);
        _settings.CalendarOpened -= EnableScreen;
    }

    private void Start()
    {
        DisableAllPlanes();
        _screenVisabilityHandler.DisableScreen();
    }

    public void OnBackClicked()
    {
        BackClicked?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }

    public void DateSelected()
    {
        var selectedDate = _datePicker.Content.Selection.GetItem(0).Date;
        Debug.Log($"Selected Date: {selectedDate:yyyy-MM-dd}");

        var matchingPlanes = _mainScreen.Planes
            .Where(plane => plane.IsActive && MatchesCareDate(plane, selectedDate))
            .ToList();

        Debug.Log($"Matching Planes Count: {matchingPlanes.Count}");

        DisableAllPlanes();

        for (int i = 0; i < matchingPlanes.Count && i < _planes.Count; i++)
        {
            var plantPlane = matchingPlanes[i];
            var calendarPlane = _planes[i];

            // Get all care types for the selected date
            string careTypes = GetCareTypesForDate(plantPlane, selectedDate);

            calendarPlane.Enable(plantPlane.PlantData.Name, careTypes, plantPlane.PlantData.ImagePath);
            Debug.Log($"Activated CalendarPlane for Plant: {plantPlane.PlantData.Name} with Care: {careTypes}");
        }

        ToggleEmptyText();
    }

    private bool MatchesCareDate(PlantPlane plantPlane, DateTime selectedDate)
    {
        // Check if any care date in UpcomingCareDates matches the selected date
        return plantPlane.UpcomingCareDates.Any(care => care.Date.Date == selectedDate.Date);
    }

    private string GetCareTypesForDate(PlantPlane plantPlane, DateTime selectedDate)
    {
        // Collect all care types matching the selected date
        var careTypes = plantPlane.UpcomingCareDates
            .Where(care => care.Date.Date == selectedDate.Date)
            .Select(care => care.CareType)
            .Distinct()
            .ToList();

        // Return care types as a comma-separated string
        return string.Join(", ", careTypes);
    }

    private void EnableScreen()
    {
        _screenVisabilityHandler.EnableScreen();
        _todayText.text = DateTime.Today.ToString("ddd, MMM dd");
    }

    private void DisableAllPlanes()
    {
        foreach (var plane in _planes)
        {
            plane.Disable();
        }

        ToggleEmptyText();
    }

    private void ToggleEmptyText()
    {
        bool isEmpty = _planes.All(p => !p.IsActive);

        _emptyText.gameObject.SetActive(isEmpty);
        _filledText.gameObject.SetActive(!isEmpty);
    }
}

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlantPlane : MonoBehaviour
{
    [SerializeField] private PhotosController _photosController;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _nextText;
    [SerializeField] private Button _openButton;

    public event Action<PlantPlane> PlaneOpened;

    public PlantData PlantData { get; private set; }
    public bool IsActive { get; private set; }
    public List<(DateTime Date, string CareType)> UpcomingCareDates { get; private set; } = new List<(DateTime, string)>();

    private void OnEnable()
    {
        _openButton.onClick.AddListener(OnPlaneOpened);
    }

    private void OnDisable()
    {
        _openButton.onClick.RemoveListener(OnPlaneOpened);
    }

    public void Enable(PlantData data)
    {
        gameObject.SetActive(true);

        PlantData = data;
        _name.text = PlantData.Name;
        CalculateUpcomingCareDates();
        CalculateNextAction();
        IsActive = true;

        if (PlantData.ImagePath != null && PlantData.ImagePath.Length > 0)
        {
            _photosController.SetPhotos(PlantData.ImagePath);
        }
        else
        {
            _photosController.ResetPhotos();
        }
    }

    public void Disable()
    {
        PlantData = null;
        _name.text = string.Empty;
        _nextText.text = string.Empty;
        IsActive = false;
        UpcomingCareDates.Clear();
        _photosController.ResetPhotos();
        gameObject.SetActive(false);
    }

    private void AddCareDates(CareData careData, string careType)
    {
        if (careData == null || careData.Number <= 0 || string.IsNullOrEmpty(careData.Frequency))
            return;

        DateTime nextDate = careData.LastWatering;
        TimeSpan frequencySpan = GetFrequencySpan(careData);

        for (int i = 0; i < 12; i++) // Generate up to 12 future dates
        {
            nextDate = nextDate.Add(frequencySpan);

            if (nextDate > DateTime.Now) // Only add future dates
            {
                UpcomingCareDates.Add((nextDate, careType));
            }
        }
    }

    private void CalculateUpcomingCareDates()
    {
        // Clear previous dates
        UpcomingCareDates.Clear();

        // Calculate care dates for all care types
        AddCareDates(PlantData.Watering, "Watering");
        AddCareDates(PlantData.Spraying, "Spraying");
        AddCareDates(PlantData.Fertilizer, "Fertilizer");
        AddCareDates(PlantData.Replanting, "Replanting");

        // Sort the dates in chronological order
        UpcomingCareDates.Sort((a, b) => a.Date.CompareTo(b.Date));
    }

    private void CalculateNextAction()
    {
        if (UpcomingCareDates.Count > 0)
        {
            var nextAction = UpcomingCareDates[0];
            _nextText.text = $"Next Care: {nextAction.CareType} {nextAction.Date:MMM. dd, yyyy}";
        }
        else
        {
            _nextText.text = "No upcoming actions";
        }
    }

    private TimeSpan GetFrequencySpan(CareData careData)
    {
        switch (careData.Frequency.ToLower())
        {
            case "day":
                return TimeSpan.FromDays(1.0 / careData.Number);
            case "week":
                return TimeSpan.FromDays(7.0 / careData.Number);
            case "month":
                return TimeSpan.FromDays(30.0 / careData.Number);
            case "year":
                return TimeSpan.FromDays(365.0 / careData.Number);
            default:
                Debug.LogWarning($"Unknown frequency: {careData.Frequency}");
                return TimeSpan.MaxValue;
        }
    }

    private void OnPlaneOpened()
    {
        PlaneOpened?.Invoke(this);
    }
}

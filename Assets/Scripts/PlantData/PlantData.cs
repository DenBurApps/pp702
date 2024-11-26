using System;

[Serializable]
public class PlantData
{
    public byte[] ImagePath;
    public string Name;
    public string Type;

    public CareData Watering;
    public CareData Spraying;
    public CareData Fertilizer;
    public CareData Replanting;
    public DateTime PlantingDate;
    public int DayCount;
    public DateTime CreationDate;

    public PlantData(byte[] imagePath, string name, string type, CareData watering, CareData spraying, CareData fertilizer, CareData replanting, DateTime plantingDate)
    {
        ImagePath = imagePath;
        Name = name;
        Type = type;
        Watering = watering;
        Spraying = spraying;
        Fertilizer = fertilizer;
        Replanting = replanting;
        PlantingDate = plantingDate;
        DayCount = 1;
        CreationDate = DateTime.Today;
    }

    public void UpdateCreationDate()
    {
        if (DateTime.Today.Day > CreationDate.Day)
        {
            DayCount++;
            CreationDate = DateTime.Today;
        }
    }
}

[Serializable]
public class CareData
{
    public int Number;
    public string Frequency;
    public DateTime LastWatering;

    public CareData(int number, string frequency, DateTime lastWatering)
    {
        Number = number;
        Frequency = frequency;
        LastWatering = lastWatering;
    }
    
    public DateTime GetLastCareDate()
    {
        if (Number <= 0 || string.IsNullOrEmpty(Frequency))
        {
            return DateTime.MinValue;
        }

        return LastWatering;
    }
}

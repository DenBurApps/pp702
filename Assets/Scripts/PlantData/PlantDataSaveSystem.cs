using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class PlantDataSaveSystem
{
    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "PlantData.json");

    public static void Save(List<PlantData> plantDatas)
    {
        PlantDataListWrapper wrapper = new PlantDataListWrapper(plantDatas);
        
        string json = JsonConvert.SerializeObject(wrapper, Formatting.Indented);
        File.WriteAllText(SavePath, json);
        Debug.Log($"Plant data saved to: {SavePath}");
    }

    public static List<PlantData> Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No save file found, returning empty list.");
            return new List<PlantData>();
        }

        string json = File.ReadAllText(SavePath);
        PlantDataListWrapper wrapper = JsonConvert.DeserializeObject<PlantDataListWrapper>(json);
        Debug.Log("Plant data loaded successfully.");
        return wrapper.PlantDataList;
    }

    [System.Serializable]
    private class PlantDataListWrapper
    {
        public List<PlantData> PlantDataList;

        public PlantDataListWrapper(List<PlantData> plantDataList)
        {
            PlantDataList = plantDataList;
        }
    }
}

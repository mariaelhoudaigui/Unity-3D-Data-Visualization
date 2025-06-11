using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class ScannedBuildingData
{
    public int id;
    public string buildingName;
    public int population;
    public float energyConsumption;
}

[System.Serializable]
public class ScannedDataList
{
    public List<ScannedBuildingData> scannedBuildings = new List<ScannedBuildingData>();
}


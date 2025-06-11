using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingData : MonoBehaviour
{
    [Header("Informations du bâtiment")]
    public int id;  // <-- ajoute cette ligne
    public string buildingName;
    public int population;
    public float energyConsumption;

    public string GetData()
    {
        return $"Bâtiment: {buildingName}\nPopulation: {population}\nConsommation d'énergie: {energyConsumption} kWh";
    }
}

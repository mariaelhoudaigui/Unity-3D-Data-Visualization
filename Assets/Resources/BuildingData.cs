using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingData : MonoBehaviour
{
    [Header("Informations du b�timent")]
    public int id;  // <-- ajoute cette ligne
    public string buildingName;
    public int population;
    public float energyConsumption;

    public string GetData()
    {
        return $"B�timent: {buildingName}\nPopulation: {population}\nConsommation d'�nergie: {energyConsumption} kWh";
    }
}

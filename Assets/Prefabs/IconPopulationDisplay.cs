using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class IconPopulationDisplay : MonoBehaviourPun
{
    public TextMeshProUGUI textComponent;

    public void SetPopulation(int population)
    {
        textComponent.text = population.ToString();
    }
}


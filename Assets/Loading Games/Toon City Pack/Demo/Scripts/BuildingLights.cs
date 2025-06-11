using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLights : MonoBehaviour {
    public int windowMaterialIndex;
    public Color lightColor;
    public bool areLightsOn;
    private Color defaultColor;
    private MeshRenderer mr;

    private void Start() {
        Material mat = GetComponent<Renderer>().material;
        if (mat.HasProperty("_Color"))
        {
            Color col = mat.color; // ou mat.GetColor("_Color");
                                   // ta logique avec la couleur
        }
        else
        {
            Debug.LogWarning($"Le matériau {mat.name} ne possède pas de propriété _Color.");
        }
    }

    public void SetLights(bool isOn) {
        mr.materials[windowMaterialIndex].shader = isOn ? Shader.Find("Unlit/Color") : Shader.Find("Standard");
        mr.materials[windowMaterialIndex].color = isOn ? lightColor : defaultColor;
    }
}

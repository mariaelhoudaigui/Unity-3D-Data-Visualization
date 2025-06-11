using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingScanner : MonoBehaviour
{
    public float scanDistance = 10f;
    public Camera playerCamera; // <- À assigner depuis l’inspecteur

    void Update()

    {
            if (Input.GetKeyDown(KeyCode.M))
            {
                Debug.Log("Touche M détectée !");

                RaycastHit hit;
                Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

                if (Physics.Raycast(ray, out hit, scanDistance))
            {
                ScannableBuilding sb = hit.collider.GetComponent<ScannableBuilding>();
                if (sb != null && !sb.isScanned)
                {
                    Debug.Log($"Bâtiment scanné ! Population : {sb.population}");
                    sb.isScanned = true;
                }
                else if (sb != null)
                {
                    Debug.Log("Déjà scanné !");
                }
                else
                {
                    Debug.Log("Aucun bâtiment détecté !");
                }
            }
        }
    }
}



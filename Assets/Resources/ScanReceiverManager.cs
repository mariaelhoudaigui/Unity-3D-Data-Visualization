using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Photon.Realtime;





public class ScanReceiverManager : MonoBehaviourPunCallbacks
{
    private ScannedDataList scannedDataList = new ScannedDataList();
    private string filePath;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "scanned_buildings.json");
        Debug.Log("[ScanReceiver] JSON sera sauvé à : " + filePath);
    }

    private void SaveToFile()
    {
        string json = JsonUtility.ToJson(scannedDataList, true);
        File.WriteAllText(filePath, json);

        Debug.Log("[ScanReceiver] JSON saved to: " + filePath);
    }

    // Cette méthode est appelée par le joueur qui scanne un bâtiment
    public void SendScan(int id, string buildingName, int population, float energyConsumption)
    {
        photonView.RPC("RPC_ReceiveScan", RpcTarget.All, id, buildingName, population, energyConsumption);
    }

    // Cette méthode est appelée sur tous les clients via RPC
    [PunRPC]
    public void RPC_ReceiveScan(int id, string buildingName, int population, float energyConsumption)
    {
        Debug.Log($"[ScanReceiver] Scan reçu pour bâtiment {id}");

        // Vérifie si ce bâtiment a déjà été scanné
        bool alreadyScanned = scannedDataList.scannedBuildings.Exists(b => b.id == id);
        if (alreadyScanned)
        {
            Debug.Log($"[ScanReceiver] Le bâtiment ID {id} a déjà été scanné. Scan ignoré.");
            return;
        }

        // Sinon, l’ajoute à la liste
        ScannedBuildingData data = new ScannedBuildingData()
        {
            id = id,
            buildingName = buildingName,
            population = population,
            energyConsumption = energyConsumption,
        };

        scannedDataList.scannedBuildings.Add(data);
        SaveToFile();
    }
}

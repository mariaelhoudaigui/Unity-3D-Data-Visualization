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
        Debug.Log("[ScanReceiver] JSON sera sauv� � : " + filePath);
    }

    private void SaveToFile()
    {
        string json = JsonUtility.ToJson(scannedDataList, true);
        File.WriteAllText(filePath, json);

        Debug.Log("[ScanReceiver] JSON saved to: " + filePath);
    }

    // Cette m�thode est appel�e par le joueur qui scanne un b�timent
    public void SendScan(int id, string buildingName, int population, float energyConsumption)
    {
        photonView.RPC("RPC_ReceiveScan", RpcTarget.All, id, buildingName, population, energyConsumption);
    }

    // Cette m�thode est appel�e sur tous les clients via RPC
    [PunRPC]
    public void RPC_ReceiveScan(int id, string buildingName, int population, float energyConsumption)
    {
        Debug.Log($"[ScanReceiver] Scan re�u pour b�timent {id}");

        // V�rifie si ce b�timent a d�j� �t� scann�
        bool alreadyScanned = scannedDataList.scannedBuildings.Exists(b => b.id == id);
        if (alreadyScanned)
        {
            Debug.Log($"[ScanReceiver] Le b�timent ID {id} a d�j� �t� scann�. Scan ignor�.");
            return;
        }

        // Sinon, l�ajoute � la liste
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

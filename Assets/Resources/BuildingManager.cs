using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

public class BuildingManager : MonoBehaviourPunCallbacks
{
    [Header("Assign in Inspector")]
    public GameObject[] buildings;
    public GameObject iconPrefab;

    private List<GameObject> spawnedIcons = new List<GameObject>();

    #region PUN Callbacks

    // appelé quand ce client a rejoint une room
    public override void OnJoinedRoom()
    {
        Debug.Log($"[BM] OnJoinedRoom() appelé. IsMasterClient = {PhotonNetwork.IsMasterClient}");
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[BM] Je suis MasterClient, je charge et j'envoie les données.");
            StartCoroutine(LoadAndDistributeBuildings());
        }
    }

    // appelé sur le MasterClient quand un nouvel autre client arrive
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"[BM] Nouveau joueur {newPlayer.NickName} rejoint, je renvoie les données.");
            // Re-utilise la même coroutine pour recharger et renvoyer
            StartCoroutine(LoadAndDistributeBuildings());
        }
    }

    #endregion

    IEnumerator LoadAndDistributeBuildings()
    {
        yield return StartCoroutine(BuildingDataLoader.LoadDataAsync(data =>
        {
            if (data == null)
            {
                Debug.LogError("[BM] Échec du chargement des données JSON !");
                return;
            }

            string serialized = SerializeBuildingData(data);
            Debug.Log($"[BM] Envoi RPC serializedData = {serialized}");
            photonView.RPC(nameof(RPC_ReceiveBuildingData), RpcTarget.AllBuffered, serialized);
        }));
    }

    private string SerializeBuildingData(BuildingDataset data)
    {
        var list = new List<string>();
        foreach (var b in data.buildings)
            list.Add($"{b.id}:{b.population}");
        return string.Join(";", list);
    }

    [PunRPC]
    void RPC_ReceiveBuildingData(string serializedData)
    {
        Debug.Log($"[BM] RPC_ReceiveBuildingData() avec = {serializedData}");

        // Cleanup
        foreach (var ico in spawnedIcons)
            Destroy(ico);
        spawnedIcons.Clear();

        // Parse & instantiate
        var entries = serializedData.Split(';');
        foreach (var e in entries)
        {
            if (string.IsNullOrEmpty(e)) continue;
            var parts = e.Split(':');
            int id = int.Parse(parts[0]);
            int pop = int.Parse(parts[1]);

            if (id - 1 >= buildings.Length) continue;
            var target = buildings[id - 1];

            var icon = Instantiate(iconPrefab, target.transform);
            icon.transform.localPosition = new Vector3(0, 7, 0);       // pour tester
            icon.transform.localScale = Vector3.one * 0.2f;

            // Populate text
            var text = icon.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.text = $"ID: {id}\nPop: {pop}";
            spawnedIcons.Add(icon);
        }
    }
}

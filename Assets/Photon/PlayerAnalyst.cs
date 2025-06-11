using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerAnalyst : MonoBehaviourPun
{
    [Header("Paramètres de scan")]
    public float scanRange = 10f;
    public LayerMask scanLayer;

    [Header("Références UI")]
    private TextMeshProUGUI feedbackText;

    private HashSet<int> scannedBuildingIds = new HashSet<int>();
    private int totalBuildings;
    private BuildingManager buildingManager;
   

    void Start()
    {
        Debug.Log("[Analyst] Initialisation de PlayerAnalyst...");

        feedbackText = GameObject.Find("ScanFeedbackText")?.GetComponent<TextMeshProUGUI>();
        if (feedbackText == null)
        {
            Debug.LogWarning("[Analyst] Aucun ScanFeedbackText trouvé dans la scène !");
        }

        if (buildingManager == null)
        {
            GameObject root = GameObject.Find("GameObject"); // Remplace avec le vrai nom du parent
            if (root != null)
            {
                buildingManager = root.GetComponentInChildren<BuildingManager>();
            }
        }

        if (buildingManager != null)
        {
            totalBuildings = buildingManager.buildings.Length;
            Debug.Log("[Analyst] Nombre total de bâtiments : " + totalBuildings);
        }
        else
        {
            Debug.LogError("[Analyst] Référence BuildingManager non trouvée !");
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Scan automatique déclenché avec la touche P");
            ScanForData();
            StartCoroutine(ShowScanRadius());
        }

        if (Input.GetMouseButtonDown(0)) // clic gauche souris
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, scanLayer))
            {
                Debug.Log($"[ScanClic] Raycast hit: {hit.collider.gameObject.name}");

                BuildingData buildingData = hit.collider.GetComponent<BuildingData>();
                if (buildingData != null)
                {
                    Debug.Log($"[ScanClic] Building scanné ID: {buildingData.id}, Pop: {buildingData.population}");
                    SendScanToReceiver(buildingData.id, buildingData.buildingName, buildingData.population, buildingData.energyConsumption);
                }
                else
                {
                    Debug.Log("[ScanClic] Objet cliqué n'a pas de BuildingData");
                }
            }
        }
    }

    void ScanForData()
    {
        Collider[] buildings = Physics.OverlapSphere(transform.position, scanRange, scanLayer);
        Debug.Log($"[ScanAuto] Nombre de bâtiments détectés : {buildings.Length}");

        foreach (var b in buildings)
        {
            BuildingData data = b.GetComponent<BuildingData>();
            if (data != null)
            {
                Debug.Log($"[ScanAuto] Bâtiment détecté ID: {data.id}, Pop: {data.population}");
                SendScanToReceiver(data.id, data.buildingName, data.population, data.energyConsumption);
                HandleScan(data);
            }
            else
            {
                Debug.LogWarning($"[ScanAuto] Pas de BuildingData sur {b.gameObject.name}");
            }
        }
    }

   

    void HandleScan(BuildingData building)
    {
        if (scannedBuildingIds.Contains(building.id))
        {
            ShowFeedback($"Le bâtiment avec l’ID {building.id} a déjà été scanné !");
            return;
        }
        scannedBuildingIds.Add(building.id);
        

        int remaining = totalBuildings - scannedBuildingIds.Count;
        ShowFeedback($"Bravo ! Tu as trouvé le building {building.id}. Il t’en reste {remaining}.");
    }

    void SendScanToReceiver(int id, string name, int population, float energy)
    {
        GameObject receiverObj = GameObject.Find("ScanReceiver");

        if (receiverObj != null)
        {
            PhotonView receiverView = receiverObj.GetComponent<PhotonView>();
            if (receiverView != null)
            {
                receiverView.RPC("RPC_ReceiveScan", RpcTarget.MasterClient, id, name, population, energy);
            }
            else
            {
                Debug.LogWarning("[Analyst] PhotonView manquant sur ScanReceiver !");
            }
        }
        else
        {
            Debug.LogWarning("[Analyst] Objet ScanReceiver introuvable !");
        }
    }

    IEnumerator ShowScanRadius()
    {
        float duration = 1.5f;
        float timer = 0f;

        while (timer < duration)
        {
            DrawScanCircle();
            timer += Time.deltaTime;
            yield return null;
        }
    }

    void DrawScanCircle()
    {
        int segments = 40;
        float angleStep = 360f / segments;
        Vector3 center = transform.position;
        Vector3 prevPoint = center + new Vector3(scanRange, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * scanRange, 0, Mathf.Sin(angle) * scanRange);
            Debug.DrawLine(prevPoint, newPoint, Color.green);
            prevPoint = newPoint;
        }
    }

    void ShowFeedback(string message)
    {
        Debug.Log("[UI Feedback] Message à afficher : " + message);

        if (feedbackText != null)
        {
            feedbackText.text = message;
            StopAllCoroutines();
            StartCoroutine(ClearFeedbackAfterDelay(3f));
        }
        else
        {
            Debug.LogWarning("[UI Feedback] feedbackText est null !");
        }
    }

    IEnumerator ClearFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
    }
}

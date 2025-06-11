using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System.IO;
using UnityEngine.UI;



public class PlayerVisualizer : MonoBehaviourPun
{
    private TextMeshProUGUI textDisplay;
    private Transform pieChartContainer;
    private Transform barChartContainer;
    public GameObject pieSlicePrefab;
    public GameObject barPrefab;
    [SerializeField] private TMP_InputField inputFilterIDs;

    public Button filterButton;


    private List<ScannedBuildingData> buildingData = new List<ScannedBuildingData>();
    private string filePath;

    void Start()
    {
        if (!photonView.IsMine)
        {

            enabled = false;
            return;
        }

        if (filterButton == null)
        {
            filterButton = GameObject.Find("Filter")?.GetComponent<Button>();
        }

        if (filterButton != null)
        {
            filterButton.onClick.AddListener(ApplyFilter);
        }
        else
        {
            Debug.LogWarning("FilterButton non trouv� !");
        }

        textDisplay = GameObject.Find("textDisplay")?.GetComponent<TextMeshProUGUI>();
        pieChartContainer = GameObject.Find("camembert")?.transform;
        barChartContainer = GameObject.Find("barres")?.transform;
        
      //  if (inputFilterIDs == null)
        //{
          //  Debug.LogWarning("Le champ de saisie 'InputFilterIDs' n'a pas �t� trouv� dans la sc�ne.");
        //}
        //else
        //{
          //  Debug.Log("Champ de saisie trouv� avec succ�s.");
            
        //}

        filePath = Path.Combine(Application.persistentDataPath, "scanned_buildings.json");
        Debug.Log("[Visualizer] Lecture du fichier JSON � : " + filePath);

        LoadDataFromFile();
        DisplayTextData();
        CreatePieChart();
        CreateBarChart();
        PositionDashboardInFront();
    }

    public void ApplyFilter()
    {
        textDisplay = GameObject.Find("textDisplay")?.GetComponent<TextMeshProUGUI>();
        pieChartContainer = GameObject.Find("camembert")?.transform;
        barChartContainer = GameObject.Find("barres")?.transform;
        inputFilterIDs = GameObject.Find("InputFilterIDs")?.GetComponent<TMP_InputField>();

        filePath = Path.Combine(Application.persistentDataPath, "scanned_buildings.json");
        Debug.Log("[Visualizer] Lecture du fichier JSON � : " + filePath);

        LoadDataFromFile();
        

        if (inputFilterIDs == null || string.IsNullOrWhiteSpace(inputFilterIDs.text))
        {
            Debug.LogWarning("Champ de filtre vide ou non assign�.");
            return;
        }
        Debug.Log("Texte saisi dans le champ de filtre : " + inputFilterIDs.text);

        string[] idStrings = inputFilterIDs.text.Split(',');
        List<int> targetIds = new List<int>();

        foreach (string idStr in idStrings)
        {
            if (int.TryParse(idStr.Trim(), out int id))
                targetIds.Add(id);
        }
        foreach (var building in buildingData)
        {
            Debug.Log("B�timent ID dans la liste: " + building.id);
        }
        foreach (var id in targetIds)
        {
            Debug.Log("ID filtr� : " + id);
        }


        // Filtrer les donn�es selon les IDs
        List<ScannedBuildingData> filteredData = buildingData.FindAll(b => targetIds.Contains(b.id));
        Debug.Log("Nombre de b�timents apr�s filtrage : " + filteredData.Count);

        // Mettre � jour les graphes avec les donn�es filtr�es
        DisplayTextData();
        CreatePieChart(filteredData);
        CreateBarChart(filteredData);
        PositionDashboardInFront();
    }

    void PositionDashboardInFront()
    {
        Canvas worldCanvas = GetComponentInChildren<Canvas>();
        if (worldCanvas != null)
        {
            Transform cam = Camera.main.transform;
            worldCanvas.transform.position = cam.position + cam.forward * 5.0f;
            worldCanvas.transform.rotation = Quaternion.LookRotation(worldCanvas.transform.position - cam.position);
        }
    }

    void LoadDataFromFile()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            ScannedDataList dataList = JsonUtility.FromJson<ScannedDataList>(json);
            buildingData = dataList.scannedBuildings;
            Debug.Log("dataloadeeeed");
        }
        else
        {
            Debug.LogWarning("Fichier JSON introuvable.");
        }
    }

    void DisplayTextData()
    {
        // � impl�menter si n�cessaire
    }

    void CreatePieChart(List<ScannedBuildingData> dataToUse = null)
    {
        
        dataToUse ??= buildingData;

        foreach (Transform child in pieChartContainer)
            Destroy(child.gameObject);

        float totalEnergy = 0f;
        foreach (var data in dataToUse)
            totalEnergy += data.energyConsumption;

        if (totalEnergy == 0f) return;

        float zRotation = 0f;
        foreach (var data in dataToUse)
        {
            float slicePercent = data.energyConsumption / totalEnergy;

            GameObject slice = Instantiate(pieSlicePrefab, pieChartContainer);
            slice.transform.localRotation = Quaternion.Euler(0, 0, -zRotation);

            Image sliceImage = slice.GetComponent<Image>();
            sliceImage.fillAmount = slicePercent;
            sliceImage.color = Random.ColorHSV();

            zRotation += slicePercent * 360f;
        }
    }

    void CreateBarChart(List<ScannedBuildingData> dataToUse = null)
    {
        
        
        dataToUse ??= buildingData;

        foreach (Transform child in barChartContainer)
            Destroy(child.gameObject);

        float barWidth = 50f;
        float spacing = 60f;

        for (int index = 0; index < dataToUse.Count; index++)
        {
            var data = dataToUse[index];
            GameObject bar = Instantiate(barPrefab, barChartContainer);
            RectTransform barRect = bar.GetComponent<RectTransform>();

            barRect.anchoredPosition = new Vector2(index * spacing, 0);
            barRect.sizeDelta = new Vector2(barWidth, data.population * 5);

            TextMeshProUGUI label = bar.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = $"ID {data.id}";
        }
    }

    [PunRPC]
    public void ReceiveData(string jsonData)
    {
        Debug.Log("Donn�es re�ues par le Visualiseur via RPC : " + jsonData);

        ScannedDataList received = JsonUtility.FromJson<ScannedDataList>(jsonData);
        buildingData = received.scannedBuildings;

        File.WriteAllText(filePath, jsonData);

        DisplayTextData();
        CreatePieChart();
        CreateBarChart();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;

[System.Serializable]
public class Building
{
    public int id;
    public int population;
}

[System.Serializable]
public class BuildingDataset
{
    public List<Building> buildings;
}

public class BuildingDataLoader : MonoBehaviour
{
    public static IEnumerator LoadDataAsync(Action<BuildingDataset> callback)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "buildingsData.json");
        Debug.Log($"[DEBUG] Chemin complet du fichier JSON : {path}");

        string jsonText = "";

        // Gestion des plateformes (PC/Mac & Android)
#if UNITY_ANDROID
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            jsonText = request.downloadHandler.text;
        }
        else
        {
            Debug.LogError("Erreur lors du chargement du fichier JSON sur Android !");
            callback?.Invoke(null);
            yield break;
        }
#else
        if (File.Exists(path))
        {
            jsonText = File.ReadAllText(path);
        }
        else
        {
            Debug.LogError($"Fichier JSON non trouv� : {path}");
            callback?.Invoke(null);
            yield break;
        }
#endif

        // V�rifier que les donn�es ne sont pas vides
        if (string.IsNullOrEmpty(jsonText))
        {
            Debug.LogError("Fichier JSON vide ou non lisible !");
            callback?.Invoke(null);
            yield break;
        }

        // Afficher le JSON pour v�rifier qu'il est bien lu
        Debug.Log($"Donn�es JSON charg�es : {jsonText}");

        // D�s�rialiser les donn�es JSON
        BuildingDataset data = JsonUtility.FromJson<BuildingDataset>(jsonText);

        if (data == null || data.buildings == null)
        {
            Debug.LogError($"[DEBUG] D�s�rialisation �chou�e. data = {data}, buildings = {data?.buildings}");
            callback?.Invoke(null);
        }
        else
        {
            callback?.Invoke(data);
        }

    }
}

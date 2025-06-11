using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public enum PlayerRole
{
    Analyst,
    Visualizer
}

public class PlayerRoleManager : MonoBehaviourPun
{
    public PlayerRole role;

    public GameObject filterUI;
    public Button filterButton;
    

    

    private void Start()
    {
        if (filterButton == null)
        {
            filterButton = GameObject.Find("Filter")?.GetComponent<Button>();
        }

    }

    /// <summary>
    /// Appelée depuis l'UI quand le joueur sélectionne un rôle (Analyst ou Visualizer).
    /// </summary>
    public void SetPlayerRole(PlayerRole chosenRole)
    {
        if (!photonView.IsMine) return;

        role = chosenRole;

        Debug.Log($"[ROLE MANAGER] {PhotonNetwork.NickName} a choisi le rôle : {role}");

        // Synchronise le rôle sur tous les clients avec RPC
        photonView.RPC("SetRole", RpcTarget.AllBuffered, (int)role);
    }

    [PunRPC]
    void SetRole(int roleIndex)
    {

        role = (PlayerRole)roleIndex;

        bool isVisualizer = (role == PlayerRole.Visualizer);

        if (filterUI != null)
            filterUI.SetActive(isVisualizer);

        if (filterButton != null)
            filterButton.interactable = isVisualizer;

        

        role = (PlayerRole)roleIndex;
        Debug.Log($"[ROLE MANAGER] Rôle appliqué pour {gameObject.name} : {role}");

        var analystScript = GetComponent<PlayerAnalyst>();
        var visualizerScript = GetComponent<PlayerVisualizer>();

        if (analystScript != null)
            analystScript.enabled = (role == PlayerRole.Analyst);

        if (visualizerScript != null)
            visualizerScript.enabled = (role == PlayerRole.Visualizer);

        // Important : seule l'instance locale gère l'affichage du filtre
        if (photonView.IsMine && role == PlayerRole.Analyst)
        {
            
            filterUI.SetActive(false);

            Debug.Log( "FilterUI is OFF (Analyst)");
        }

        if (photonView.IsMine && role == PlayerRole.Visualizer)
        {
          
            filterUI.SetActive(true);

            Debug.Log("FilterUI is ON (Visualizer)");
        }
    }

}

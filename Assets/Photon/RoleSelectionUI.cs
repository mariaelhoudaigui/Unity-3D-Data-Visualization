using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoleSelectionUI : MonoBehaviour
{
    public GameObject rolePanel;
    public Button analystButton;
    public Button visualizerButton;
    public PhotonView playerPrefab;
    public Transform spawnPoint; // facultatif

    void Start()
    {
        rolePanel.SetActive(true);
        analystButton.onClick.AddListener(() => OnRoleSelected(PlayerRole.Analyst));
        visualizerButton.onClick.AddListener(() => OnRoleSelected(PlayerRole.Visualizer));
    }

    void OnRoleSelected(PlayerRole selectedRole)
    {
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        GameObject playerGO = PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);

        PlayerRoleManager prm = playerGO.GetComponent<PlayerRoleManager>();
        prm.SetPlayerRole(selectedRole); // public method to set role and sync
        rolePanel.SetActive(false);
    }
}

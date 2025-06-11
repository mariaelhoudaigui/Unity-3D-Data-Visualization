using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviourPun
{
    private Rigidbody rb;
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Si ce n'est pas le joueur local, désactiver la caméra
        if (!photonView.IsMine)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cam.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        // 🔴 Le joueur local contrôle uniquement SON personnage
        if (!photonView.IsMine) return; // Les autres joueurs ne contrôlent rien ici
      

        // Input pour le joueur local
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        float rotateInput = 0f;

        if (Input.GetKey(KeyCode.Q)) rotateInput = -1f;
        if (Input.GetKey(KeyCode.E)) rotateInput = 1f;

        // Mouvement
        Vector3 move = transform.forward * moveVertical + transform.right * moveHorizontal;
        rb.MovePosition(rb.position + move * moveSpeed * Time.deltaTime);

        // Rotation
        float rotation = rotateInput * rotationSpeed * Time.deltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, rotation, 0f));
    }
}

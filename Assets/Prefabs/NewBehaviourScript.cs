using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;


public class RectTransformPhotonSync : MonoBehaviour, IPunObservable
{
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Sending data to other clients
            stream.SendNext(rectTransform.anchoredPosition);
            stream.SendNext(rectTransform.sizeDelta);
            stream.SendNext(rectTransform.localRotation);
            stream.SendNext(rectTransform.localScale);
        }
        else
        {
            // Receiving data from other clients
            rectTransform.anchoredPosition = (Vector2)stream.ReceiveNext();
            rectTransform.sizeDelta = (Vector2)stream.ReceiveNext();
            rectTransform.localRotation = (Quaternion)stream.ReceiveNext();
            rectTransform.localScale = (Vector3)stream.ReceiveNext();
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Coin : MonoBehaviourPun,Iitem
{
    public int Score = 200;
    public void Use(GameObject target)
    {
        GameManger._Instance.AddScore(Score);

        PhotonNetwork.Destroy(gameObject);
    }
}

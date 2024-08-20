using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AmmoPack : MonoBehaviourPun ,Iitem
{
    public int ammo = 30;

    public void Use(GameObject target)
    {
        PlayerShoter  shooter = target.GetComponent<PlayerShoter>();
        if(shooter != null && shooter.gun!=null)
        {
            shooter.gun.photonView.RPC("AddAmmo", RpcTarget.All, ammo);
        }
        Debug.Log("탄알이 증가함" + ammo);
        Destroy(gameObject);

        // 그냥 Destory하면 호스트시점으로마 삭제되므로 PhotonNetwork.Destory하면 나머지도 다 삭제됌
        PhotonNetwork.Destroy(gameObject);
        
    }
}

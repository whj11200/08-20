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
        Debug.Log("ź���� ������" + ammo);
        Destroy(gameObject);

        // �׳� Destory�ϸ� ȣ��Ʈ�������θ� �����ǹǷ� PhotonNetwork.Destory�ϸ� �������� �� ������
        PhotonNetwork.Destroy(gameObject);
        
    }
}

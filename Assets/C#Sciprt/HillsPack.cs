using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HillsPack : MonoBehaviourPun, Iitem
{
    public float health = 50f;
    public void Use(GameObject target)
    {
        // 
        LivingEntity life = target.GetComponent<LivingEntity>();
        if(life != null)
        {
            life.RestoreHealth(health);
        }
        PhotonNetwork.Destroy(gameObject);
        
    }
}

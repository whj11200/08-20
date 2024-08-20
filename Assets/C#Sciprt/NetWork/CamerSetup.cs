using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class CamerSetup : MonoBehaviourPun
{                        //MonoBehaviourPun 기능에서 포톤뷰를 추가한 프로퍼티이다.
    // 포톤뷰랑 시네머신에 가상 카메라랑 동기화하기위해서 
    void Start()
    {
        // 만약 자신이 로컬 플레이어라면
        if (photonView.IsMine)
        {
            // 씬에 있는 시네머신 가상 카메라를 찾으며 
            CinemachineVirtualCamera followCamer = FindObjectOfType<CinemachineVirtualCamera>();
            //가상 카메라의 추적대상을 자신의 트랜스폼으로 변경
            followCamer.Follow = transform;
            followCamer.LookAt = transform;
        }
     
        
    }
}

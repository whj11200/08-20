using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class CamerSetup : MonoBehaviourPun
{                        //MonoBehaviourPun ��ɿ��� ����並 �߰��� ������Ƽ�̴�.
    // ������ �ó׸ӽſ� ���� ī�޶�� ����ȭ�ϱ����ؼ� 
    void Start()
    {
        // ���� �ڽ��� ���� �÷��̾���
        if (photonView.IsMine)
        {
            // ���� �ִ� �ó׸ӽ� ���� ī�޶� ã���� 
            CinemachineVirtualCamera followCamer = FindObjectOfType<CinemachineVirtualCamera>();
            //���� ī�޶��� ��������� �ڽ��� Ʈ���������� ����
            followCamer.Follow = transform;
            followCamer.LookAt = transform;
        }
     
        
    }
}

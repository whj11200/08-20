using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


// �Է°� �����Ӱ� �и��ؼ� ��ũ��Ʈ�� �����.
// �Է°� ���� ������
public class Player : MonoBehaviourPun
{
    public string moveAxisName = "Vertical";
    public string rotateAxisName = "Horizontal";
    public string fireButtonName = "Fire1"; 
    public string reloadButtonName = "Relord";
    //Ű���� ������Ƽ �����
    public float move { get; private set; }
    public float rotate { get; private set; }
    public bool fire { get; private set; }
    public bool reload { get; private set; }
    void Start()
    {
     
    }


    void Update()
    {
        
            if (!photonView.IsMine) return;
            //���� �÷��̾ �ƴϸ� �Է��� ���� ����
        
        
        if (GameManger.Instance != null && GameManger.Instance.isGameOver)
        {
            move = 0f;
            rotate = 0; 
            fire = false;
            reload = false;
            return;
        }
        move = Input.GetAxis(moveAxisName);
        rotate = Input.GetAxis(rotateAxisName);
        fire = Input.GetButton(fireButtonName);
        reload = Input.GetButtonDown(reloadButtonName);
    }
}

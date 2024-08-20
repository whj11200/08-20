using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviourPun
{
    public float movespeed = 1f;
    public float rotatespeed = 180f;
    private Player playerinput;
    private Rigidbody playerrigidbody;
    private Animator playeranimator;
    private readonly int hashmove = Animator.StringToHash("Move");
 
    void Start()
    {
        playerrigidbody = GetComponent<Rigidbody>();
        playeranimator = GetComponent<Animator>();    
        playerinput = GetComponent<Player>();
        
    }

    void FixedUpdate()
    {
        Rotate();
        Move();
        // �ִϸ����Ϳ����� x,y���� �޾� �����̰� �Ѵ�.
        playeranimator.SetFloat(hashmove, playerinput.move);
    }
    private void Move()
    {
        Vector3 moveDistance = 
            playerinput.move *transform.forward * movespeed*Time.deltaTime;
                      // �÷��̾��� �������� addforceó�� ������ �� �ְ��ϴ� ������ٵ� �����ϴ� �Լ�
        playerrigidbody.MovePosition(playerrigidbody.position+ moveDistance);
    }
    private void Rotate()
    {
        float trun = playerinput.rotate * rotatespeed *Time.deltaTime;
                                                               // vector3 ���� �޾Ƽ� y������ ȸ����
        playerrigidbody.rotation = playerrigidbody.rotation * Quaternion.Euler(0,trun, 0);
    }
}

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
        // 애니메이터에서도 x,y값을 받아 움직이게 한다.
        playeranimator.SetFloat(hashmove, playerinput.move);
    }
    private void Move()
    {
        Vector3 moveDistance = 
            playerinput.move *transform.forward * movespeed*Time.deltaTime;
                      // 플레이어의 움직임을 addforce처럼 움직일 수 있게하는 리지드바디를 지원하는 함수
        playerrigidbody.MovePosition(playerrigidbody.position+ moveDistance);
    }
    private void Rotate()
    {
        float trun = playerinput.rotate * rotatespeed *Time.deltaTime;
                                                               // vector3 값을 받아서 y축으로 회전함
        playerrigidbody.rotation = playerrigidbody.rotation * Quaternion.Euler(0,trun, 0);
    }
}

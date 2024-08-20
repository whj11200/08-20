using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


//IK : 왼손 오른손이 무기에 정확하게 부착 되게
//총알 발사 
public class PlayerShoter : MonoBehaviourPun
{
    public Gun gun;
    public Transform gunPivot; // 총배치 위치
    public Transform leftHandMound; // 왼손 손잡이
    public Transform rightHandMound; //  오른쪽 손잡이
    private Player playerinput;
    private Animator playerAnimator;
    private readonly int Hashrelord = Animator.StringToHash("Relord");
    

    private void OnEnable()
    {
        gun.gameObject.SetActive(true);
    }
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerinput = GetComponent<Player>();
    }


    void Update()
    {
        if (!photonView.IsMine) return;
        if(playerinput.fire)
        {
            gun.Fire();
        }
        else if (playerinput.reload && gun.Relord())
        {
            playerAnimator.SetTrigger(Hashrelord);
        }
        UpdateuI();
    }
    void UpdateuI() // 탄알 Ui갱신
    {
        // 총 스크립트가 있거나 uimanger 가 있을 시 
        if(gun != null && UiManger.ui_instance != null)
        {
            UiManger.ui_instance.AmmoTextUpdate(gun.magAmmo,gun.ammoRemain);
        }
        
    }
    private void OnAnimatorIK(int layerIndex)
    {
        // 총이 기준점 gunPivot을 3d모델의 오른쪽 팔꿈치 위치로 이동
        gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        //IK 를 사용하여 왼속의 위치와 회전을 총의 안쪽 손잡이 맞춤
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);// 가중치
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);


        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMound.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMound.rotation);

        //IK 를 사용하여 오른속의 위치와 회전을 총의 안쪽 손잡이 맞춤
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);


        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, leftHandMound.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, leftHandMound.rotation);

    }
    private void OnDisable()
    {
        //슈터가 비활성화 될때 총도 함께 비활성화
        gun.gameObject.SetActive(false);
    }
}

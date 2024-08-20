using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


//IK : �޼� �������� ���⿡ ��Ȯ�ϰ� ���� �ǰ�
//�Ѿ� �߻� 
public class PlayerShoter : MonoBehaviourPun
{
    public Gun gun;
    public Transform gunPivot; // �ѹ�ġ ��ġ
    public Transform leftHandMound; // �޼� ������
    public Transform rightHandMound; //  ������ ������
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
    void UpdateuI() // ź�� Ui����
    {
        // �� ��ũ��Ʈ�� �ְų� uimanger �� ���� �� 
        if(gun != null && UiManger.ui_instance != null)
        {
            UiManger.ui_instance.AmmoTextUpdate(gun.magAmmo,gun.ammoRemain);
        }
        
    }
    private void OnAnimatorIK(int layerIndex)
    {
        // ���� ������ gunPivot�� 3d���� ������ �Ȳ�ġ ��ġ�� �̵�
        gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        //IK �� ����Ͽ� �޼��� ��ġ�� ȸ���� ���� ���� ������ ����
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);// ����ġ
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);


        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMound.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMound.rotation);

        //IK �� ����Ͽ� �������� ��ġ�� ȸ���� ���� ���� ������ ����
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);


        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, leftHandMound.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, leftHandMound.rotation);

    }
    private void OnDisable()
    {
        //���Ͱ� ��Ȱ��ȭ �ɶ� �ѵ� �Բ� ��Ȱ��ȭ
        gun.gameObject.SetActive(false);
    }
}

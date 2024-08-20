using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// LivingEntity Ŭ������ MonoBehaviourPun�� ����Ͽ� Photon ��Ʈ��ŷ ����� ����� �� ������, IDeamage �������̽��� �����մϴ�.
public class LivingEntity : MonoBehaviourPun, IDeamage
{
    public float startingHealth = 100f; // ���� ü��
    public float health { get; protected set; } // ���� ü��

    public bool dead { get; protected set; } // ��� ����
    public event Action onDeath; // ��� �� �ߵ��Ǵ� �̺�Ʈ

    // ȣ��Ʈ���� ��� Ŭ���̾�Ʈ�� ��� ���¸� ����ȭ�ϴ� �޼���
    [PunRPC]
    public void ApplyUpdateHealth(float newhealth, bool newDead)
    {
        health = newhealth; // ���ο� ü�� ������ ������Ʈ
        dead = newDead; // ���ο� ��� ���·� ������Ʈ
    }

    // ����ü�� Ȱ��ȭ�� �� ���¸� ����
    protected virtual void OnEnable()
    {
        // ���� ���� ���� �Լ�
        dead = false; // ��� ���¸� false�� �ʱ�ȭ
        health = startingHealth; // ü���� ���� ü������ �ʱ�ȭ
    }

    // ȣ��Ʈ���� �ܵ� ����ǰ�, ȣ��Ʈ�� ���� �ٸ� Ŭ���̾�Ʈ���� �ϰ� ����Ǵ� �޼���
    [PunRPC]
    public virtual void OnDeamge(float damge, Vector3 hitPoint, Vector3 hitNormal)
    {
        // �ڽ��� ȣ��Ʈ���� Ȯ��
        if (PhotonNetwork.IsMasterClient)
        {
            health -= damge; // ȣ��Ʈ�̸� ��������ŭ ü���� ����
            // ȣ��Ʈ���� Ŭ���̾�Ʈ�� ü���� ����ȭ
            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health, dead);
            // ������ ������ Ŭ���̾�Ʈ�� ����ȭ
            photonView.RPC("OnDeamge", RpcTarget.Others, hitPoint, hitNormal);
        }

        // ü���� 0 �����̰�, ���� ���� �ʾҴٸ�
        if (health <= 0 && !dead)
        {
            Die(); // ��� ó��
        }
    }

    // ü���� ȸ���ϴ� ���
    [PunRPC]
    public virtual void RestoreHealth(float newHealth)
    {
        if (dead)
        {
            // �̹� �׾��ٸ� ü���� ȸ���� �� ����
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            // ȣ��Ʈ�� ��쿡�� ü���� �߰���
            health += newHealth; // ü�� ȸ��
            // �������� Ŭ���̾�Ʈ�� ü���� ����ȭ (ȣ��Ʈ ������ �������� ������ ������ �����鿡�� ����� ui�� Ŭ���̾�Ʈ������ �Ѱ� ���� �� �ֵ�����)
            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health, dead);
            // �ٸ� Ŭ���̾�Ʈ�� RestoreHealth�� ������
            photonView.RPC("RestoreHealth", RpcTarget.Others, newHealth);
        }
    }

    // ��� ó�� �޼���
    public virtual void Die()
    {
        if (onDeath != null)
        {
            onDeath(); // ��� �̺�Ʈ �ߵ�
        }
        dead = true; // ��� ���¸� true�� ����
    }
}

// LivingEntity Ŭ������ IDeamage �������̽��� ����ϹǷ� OnDamage() �޼��带 �ݵ�� �����ؾ� �մϴ�.
// LivingEntity ���� RestoreHealth()�� OnDamage() �޼���� [RunRPC] �Ӽ����� ���� �Ǿ� �־���. �������̵� ���鿡����
// ���� �޼��� �� ���� [PunPRC] �Ӽ��� �����ؾ� ���������� RPC�� ���� ���� ������ �� �� �ִ�.
// ���� PlayerHealth ��ũ��Ʈ�� RestoreHealth() ��  OnDamage() ���� ������ RPC�� ������
// � Ŭ���̾�Ʈ���� PlayerHealth ��ũ��Ʈ�� OnDamege()�� ���� �Ǿ��ٰ� �������� �� Ŭ���̾�Ʈ�� ȣ��Ʈ�� �µ� �ƴϵ� ȿ������ �����ϰ� 
// ü�� �����̵带 �����ϴ� �κ��� ��� ����� ���� �ȴ�.
// �� ��� Ŭ���̾�Ʈ���� PlayerHealth ��ũ��Ʈ�� OnDamage()�� ���ÿ� ���� �ȴٰ� ���� �� ���� ������ ������ 
// ȣ��Ʈ������ ������ �ǰ� ������ Ŭ���̾�Ʈ�� ������ ���̴� ���峪 ui ȿ���� �� �� �ֵ��� �Ѵ�. PlayerHealth��  RestoreHealth�� ����������
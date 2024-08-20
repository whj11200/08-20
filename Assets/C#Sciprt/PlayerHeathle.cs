using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// PlayerHeathle Ŭ������ LivingEntity�� ��ӹ޾� �÷��̾��� ����� ���õ� ����� �����մϴ�.
public class PlayerHeathle : LivingEntity
{
    public Slider healthSliber; // �÷��̾��� ü���� ǥ���ϴ� �����̴�
    public AudioClip hitClip; // ���ظ� ���� �� ����� ����� Ŭ��
    public AudioClip itemPickupCilp; // �������� ȹ���� �� ����� ����� Ŭ��
    public AudioClip Deadclip; // ����� �� ����� ����� Ŭ��
    private AudioSource playerAudioPlayer; // ����� �ҽ��� �����ϴ� ������Ʈ
    private Animator playerAnimator; // �ִϸ��̼��� �����ϴ� ������Ʈ
    private PlayerMovement playerMovement; // �÷��̾��� �̵��� �����ϴ� ������Ʈ
    private PlayerShoter playerShoter; // �÷��̾��� ����� �����ϴ� ������Ʈ
    private readonly int HashDie = Animator.StringToHash("Die"); // "Die" �ִϸ��̼� �ؽ�

    private void Awake()
    {
        // ������Ʈ �ʱ�ȭ
        playerShoter = GetComponent<PlayerShoter>();
        playerAudioPlayer = GetComponent<AudioSource>();
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    protected override void OnEnable()
    {
        // Ȱ��ȭ�� �� ȣ��Ǵ� �޼ҵ�
        base.OnEnable(); // �θ� Ŭ������ OnEnable ȣ��
        healthSliber.gameObject.SetActive(true); // �����̴� Ȱ��ȭ
        healthSliber.maxValue = startingHealth; // �����̴��� �ִ밪�� ���� ü������ ����
        healthSliber.value = health; // ���� ü������ �����̴� �� ����
        playerMovement.enabled = true; // �̵� ������Ʈ Ȱ��ȭ
        playerShoter.enabled = true; // ��� ������Ʈ Ȱ��ȭ
    }

    [PunRPC]
    public override void RestoreHealth(float newHealth)
    {
        // ü���� ȸ���ϴ� �޼ҵ�
        base.RestoreHealth(newHealth); // �θ� Ŭ������ RestoreHealth ȣ��
        healthSliber.value = health; // �����̴� �� ������Ʈ
    }

    [PunRPC]
    public override void OnDeamge(float damge, Vector3 hitPoint, Vector3 hitDirection)
    {
        // ���ظ� �޾��� �� ȣ��Ǵ� �޼ҵ�
        if (!dead) // �÷��̾ ������� �ʾ��� ���
        {
            playerAudioPlayer.PlayOneShot(hitClip); // ���� �Ҹ� ���
        }
        base.OnDeamge(damge, hitPoint, hitDirection); // �θ� Ŭ������ OnDamage ȣ��
        healthSliber.value = health; // �����̴� �� ������Ʈ
    }

    public override void Die()
    {
        // ��� ó�� �޼ҵ�
        base.Die(); // �θ� Ŭ������ Die ȣ��
        healthSliber.gameObject.SetActive(false); // �����̴� ��Ȱ��ȭ

        playerAudioPlayer.PlayOneShot(Deadclip); // ��� �Ҹ� ���
        playerAnimator.SetTrigger(HashDie); // ��� �ִϸ��̼� Ʈ����
        playerMovement.enabled = false; // �̵� ������Ʈ ��Ȱ��ȭ
        playerShoter.enabled = false; // ��� ������Ʈ ��Ȱ��ȭ
        Invoke("Respawn", 5.0f); // 5�� �� Respawn �޼ҵ� ȣ��
    }

    public void Respawn() // �÷��̾ ��� �� 5�� �Ŀ� ��Ȱ�ϴ� �޼ҵ�
    {
        if (photonView.IsMine) // ���� �÷��̾ ��ġ ����
        {
            // �������� �ݰ� 5���� ������ ���� ��ġ ���
            Vector3 randomSpawnPos = Random.insideUnitSphere * 5f;
            randomSpawnPos.y = 0f; // y�� �� 0���� ����
            transform.position = randomSpawnPos; // ������ ��ġ�� �̵�
        }
        // OnDisable ȣ���� ����
        gameObject.SetActive(false);
        // OnEnable ȣ���� ����
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        // �ٸ� �ݶ��̴��� �浹���� �� ȣ��Ǵ� �޼ҵ�
        if (!dead) // �÷��̾ ������� �ʾ��� ���
        {
            // �浹�� ������Ʈ���� Iitem ������Ʈ�� ������
            Iitem item = other.GetComponent<Iitem>();
            if (item != null) // �������� ������ ���
            {
                // ȣ��Ʈ�� ������ ���
                 //ȣ��Ʈ���� �������� ��� �� ���� ȿ���� ��� Ŭ���̾�Ʈ�� ����ȭ ��Ŵ
                if (PhotonNetwork.IsMasterClient)
                {
                    item.Use(gameObject); // ������ ���
                }
                
                playerAudioPlayer.PlayOneShot(itemPickupCilp, 1.0f); // ������ ���� �Ҹ� ���
            }
        }
    }
}

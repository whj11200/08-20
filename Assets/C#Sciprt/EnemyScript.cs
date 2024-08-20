using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

// MonoBehaviour�� ��� ��ŸƮ �ڸ�ƾ��
public class EnemyScript : LivingEntity 
{
    public LayerMask whatistarget; //������� ���̾�
    private LivingEntity targetEntity;//���� ���
    private NavMeshAgent pathFinder; // ��� ��� ai ������Ʈ
    public ParticleSystem hitEffect;
    public AudioClip deathSound;
    public AudioClip hitSound;
    private Animator enemyAnimator;
    private AudioSource enemyAudioSource;
    private Renderer enemyRenderer;
    private Transform enemyTransform;
    public float damage = 20f;
    public float timeBetAttack = 0.5f;//���� ����
    private float lastAttackTime; //������ ���� ����

    private bool hasTarget
    {
        get
        { // ������ ����� �����ϰ� ����� ��� ���� �ʾҴٸ�
            if (targetEntity != null && !targetEntity.dead)
            {
                return true;
            }
            return false;

        }
    }
    private readonly int hashTagetss = Animator.StringToHash("HasTarget");
    private readonly int hashdie = Animator.StringToHash("Die");
    private void Awake()
    {
        enemyTransform = transform;
        pathFinder = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
        enemyAudioSource = GetComponent<AudioSource>();
        enemyRenderer = GetComponentInChildren<Renderer>();
        //whatistarget = LayerMask.NameToLayer("Player");
    }
    [PunRPC]
    public void SetUp(float newHealth, float newDamage, float newSpeed, Color skinColor)
    {


        startingHealth = newHealth;
        health = newHealth;
        damage = newDamage;
        pathFinder.speed = newSpeed;
        enemyRenderer.material.color = skinColor;

    }

    private void Start()
    {

        //ȣ��Ʈ�� �ƴϸ� ��������
        if (!PhotonNetwork.IsMasterClient) return;
        //StartCoroutine("UpdatePath");
        // ���ӿ�����Ʈ�� Ȱ��ȭ�� ���ÿ� AI ���� ��ƾ�� ������
        // �ڷ�ƾ�� �ѹ� ȣ��ǹǷ�  ���� while������ ��� ȣ���ϵ��� ������
        // InvokeRepeating��  �ʱ�ð��� �Է��ϰ� 0.25�� ���� �ݺ��ǹǷ� �����ϰ� �� �� �ִ�.
        InvokeRepeating("UpdatePath", 0.01f, 0.25f);
    }

    void Update()
    {
        // ȣ��Ʈ�� �ƴϸ� �������Ͷ�
        if (!PhotonNetwork.IsMasterClient) return;
        enemyAnimator.SetBool(hashTagetss, hasTarget);
    }
    void UpdatePath()
    {
      
        if(!dead)
        {
            if (hasTarget)// ��������� �����ʾҴٸ�
            {
                
                pathFinder.isStopped = false;
                pathFinder.SetDestination(targetEntity.transform.position);
            }
            else
            {
                pathFinder.isStopped = true;
                Collider[] colliders = Physics.OverlapSphere(transform.position,20f,whatistarget);
                for( int i = 0; i < colliders.Length; i++)
                {

                    LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();
                    if(livingEntity != null && !livingEntity.dead)
                    {
                        targetEntity = livingEntity;
                        break;
                    }
                }
            }
         
        }
     
        //while (true)
        //{
        //    yield return new WaitForSeconds(0.25f);
        //}
    }
    [PunRPC]
    public override void OnDeamge(float damge, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDeamge(damge, hitPoint, hitNormal);
        if (!dead)
        {
            // ���ݹ��� ������ �������� ��ƼŬ ȿ�� ������Ѷ�
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();
            enemyAudioSource.PlayOneShot(hitSound);
        }

    }
    public override void Die()
    {
        base.Die();//�⺻���� ���ó�� �ϰ� 
        Collider[] enemyColliders = GetComponents<Collider>();
        for(int i = 0;i < enemyColliders.Length;i++)
        {
            // �ٸ� AI�� ���� ���� �ʵ��� �ڽ��� ��� �ݶ��̴� ��Ȱ��ȭ
            enemyColliders[i].enabled = false;
        }
        pathFinder.isStopped= true;
        pathFinder.enabled = false;
        enemyAudioSource.PlayOneShot(deathSound);
        enemyAnimator.SetTrigger(hashdie);
    }
    private void OnTriggerStay(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        //Ʈ���� �浹�� ���� ���ӿ�����Ʈ�� ���� ����̶�� ���� �����Ҷ�
        if (!dead && Time.time >= lastAttackTime + timeBetAttack)
        { // ������ LivingEntity Ÿ���� �������� 
            LivingEntity attackTaget = other.GetComponent<LivingEntity>();
            if (attackTaget != null&& attackTaget == targetEntity)
            {
                // ������ LivingEntity �� �ڽ��� ���� ����̶�� ���� ����
                lastAttackTime = Time.time;
                Vector3 hitpos = other.ClosestPoint(transform.position);
                // ��������� �ǰ���ġ�� �ǰݹ����� �ٻ簪���� ��� 
                Vector3 hitNormal = transform.position - other.transform.position;
                attackTaget.OnDeamge(damage, hitpos, hitNormal);
            }

           
        }

    }

}
       
   

    

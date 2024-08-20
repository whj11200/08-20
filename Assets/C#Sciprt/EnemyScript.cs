using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

// MonoBehaviour가 없어도 스타트 코르틴이
public class EnemyScript : LivingEntity 
{
    public LayerMask whatistarget; //추적대상 레이어
    private LivingEntity targetEntity;//추적 대상
    private NavMeshAgent pathFinder; // 경로 계산 ai 에이전트
    public ParticleSystem hitEffect;
    public AudioClip deathSound;
    public AudioClip hitSound;
    private Animator enemyAnimator;
    private AudioSource enemyAudioSource;
    private Renderer enemyRenderer;
    private Transform enemyTransform;
    public float damage = 20f;
    public float timeBetAttack = 0.5f;//공격 간격
    private float lastAttackTime; //마지막 공격 시점

    private bool hasTarget
    {
        get
        { // 추적할 대상이 존재하고 대상이 사망 하지 않았다면
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

        //호스트가 아니면 하지마라
        if (!PhotonNetwork.IsMasterClient) return;
        //StartCoroutine("UpdatePath");
        // 게임오브젝트가 활성화에 동시에 AI 추적 루틴이 시작함
        // 코루틴은 한번 호출되므로  따로 while문으로 계속 호출하도록 하지만
        // InvokeRepeating는  초기시간을 입력하고 0.25초 마다 반복되므로 간편하게 쓸 수 있다.
        InvokeRepeating("UpdatePath", 0.01f, 0.25f);
    }

    void Update()
    {
        // 호스트가 아니면 빠져나와라
        if (!PhotonNetwork.IsMasterClient) return;
        enemyAnimator.SetBool(hashTagetss, hasTarget);
    }
    void UpdatePath()
    {
      
        if(!dead)
        {
            if (hasTarget)// 추적대상이 죽지않았다면
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
            // 공격받은 지점에 방향으로 파티클 효과 재생시켜라
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();
            enemyAudioSource.PlayOneShot(hitSound);
        }

    }
    public override void Die()
    {
        base.Die();//기본으로 사망처리 하고 
        Collider[] enemyColliders = GetComponents<Collider>();
        for(int i = 0;i < enemyColliders.Length;i++)
        {
            // 다른 AI는 방해 받지 않도록 자신의 모든 콜라이더 비활성화
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
        //트리거 충돌한 상대방 게임오브젝트가 추적 대상이라면 공격 실행할때
        if (!dead && Time.time >= lastAttackTime + timeBetAttack)
        { // 상대방의 LivingEntity 타입을 가져오기 
            LivingEntity attackTaget = other.GetComponent<LivingEntity>();
            if (attackTaget != null&& attackTaget == targetEntity)
            {
                // 상대방의 LivingEntity 가 자신의 추적 대상이라면 공격 실행
                lastAttackTime = Time.time;
                Vector3 hitpos = other.ClosestPoint(transform.position);
                // 맞은대상의 피격위치와 피격방향을 근사값으로 계산 
                Vector3 hitNormal = transform.position - other.transform.position;
                attackTaget.OnDeamge(damage, hitpos, hitNormal);
            }

           
        }

    }

}
       
   

    

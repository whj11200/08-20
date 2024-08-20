using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// PlayerHeathle 클래스는 LivingEntity를 상속받아 플레이어의 생명과 관련된 기능을 구현합니다.
public class PlayerHeathle : LivingEntity
{
    public Slider healthSliber; // 플레이어의 체력을 표시하는 슬라이더
    public AudioClip hitClip; // 피해를 받을 때 재생할 오디오 클립
    public AudioClip itemPickupCilp; // 아이템을 획득할 때 재생할 오디오 클립
    public AudioClip Deadclip; // 사망할 때 재생할 오디오 클립
    private AudioSource playerAudioPlayer; // 오디오 소스를 관리하는 컴포넌트
    private Animator playerAnimator; // 애니메이션을 관리하는 컴포넌트
    private PlayerMovement playerMovement; // 플레이어의 이동을 관리하는 컴포넌트
    private PlayerShoter playerShoter; // 플레이어의 사격을 관리하는 컴포넌트
    private readonly int HashDie = Animator.StringToHash("Die"); // "Die" 애니메이션 해시

    private void Awake()
    {
        // 컴포넌트 초기화
        playerShoter = GetComponent<PlayerShoter>();
        playerAudioPlayer = GetComponent<AudioSource>();
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    protected override void OnEnable()
    {
        // 활성화될 때 호출되는 메소드
        base.OnEnable(); // 부모 클래스의 OnEnable 호출
        healthSliber.gameObject.SetActive(true); // 슬라이더 활성화
        healthSliber.maxValue = startingHealth; // 슬라이더의 최대값을 시작 체력으로 설정
        healthSliber.value = health; // 현재 체력으로 슬라이더 값 설정
        playerMovement.enabled = true; // 이동 컴포넌트 활성화
        playerShoter.enabled = true; // 사격 컴포넌트 활성화
    }

    [PunRPC]
    public override void RestoreHealth(float newHealth)
    {
        // 체력을 회복하는 메소드
        base.RestoreHealth(newHealth); // 부모 클래스의 RestoreHealth 호출
        healthSliber.value = health; // 슬라이더 값 업데이트
    }

    [PunRPC]
    public override void OnDeamge(float damge, Vector3 hitPoint, Vector3 hitDirection)
    {
        // 피해를 받았을 때 호출되는 메소드
        if (!dead) // 플레이어가 사망하지 않았을 경우
        {
            playerAudioPlayer.PlayOneShot(hitClip); // 피해 소리 재생
        }
        base.OnDeamge(damge, hitPoint, hitDirection); // 부모 클래스의 OnDamage 호출
        healthSliber.value = health; // 슬라이더 값 업데이트
    }

    public override void Die()
    {
        // 사망 처리 메소드
        base.Die(); // 부모 클래스의 Die 호출
        healthSliber.gameObject.SetActive(false); // 슬라이더 비활성화

        playerAudioPlayer.PlayOneShot(Deadclip); // 사망 소리 재생
        playerAnimator.SetTrigger(HashDie); // 사망 애니메이션 트리거
        playerMovement.enabled = false; // 이동 컴포넌트 비활성화
        playerShoter.enabled = false; // 사격 컴포넌트 비활성화
        Invoke("Respawn", 5.0f); // 5초 후 Respawn 메소드 호출
    }

    public void Respawn() // 플레이어가 사망 후 5초 후에 부활하는 메소드
    {
        if (photonView.IsMine) // 로컬 플레이어만 위치 변경
        {
            // 원점에서 반경 5유닛 내부의 랜덤 위치 계산
            Vector3 randomSpawnPos = Random.insideUnitSphere * 5f;
            randomSpawnPos.y = 0f; // y축 값 0으로 설정
            transform.position = randomSpawnPos; // 지정된 위치로 이동
        }
        // OnDisable 호출을 위해
        gameObject.SetActive(false);
        // OnEnable 호출을 위해
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 다른 콜라이더와 충돌했을 때 호출되는 메소드
        if (!dead) // 플레이어가 사망하지 않았을 경우
        {
            // 충돌한 오브젝트에서 Iitem 컴포넌트를 가져옴
            Iitem item = other.GetComponent<Iitem>();
            if (item != null) // 아이템이 존재할 경우
            {
                // 호스트만 아이템 사용
                 //호스트에서 아이템을 사용 후 사용된 효과를 모든 클라이언트에 동기화 시킴
                if (PhotonNetwork.IsMasterClient)
                {
                    item.Use(gameObject); // 아이템 사용
                }
                
                playerAudioPlayer.PlayOneShot(itemPickupCilp, 1.0f); // 아이템 습득 소리 재생
            }
        }
    }
}

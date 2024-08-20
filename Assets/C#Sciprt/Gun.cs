using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// Gun 클래스는 총의 기능을 관리하며, Photon 네트워크를 통해 멀티플레이어 환경에서도 작동하도록 설계되었습니다.
public class Gun : MonoBehaviourPun, IPunObservable
{
    // 총의 상태를 정의하는 열거형
    public enum State { Ready, Empty, Reloading }
    public State state { get; private set; } // 총의 현재 상태

    public Transform fireTransform; // 발사 위치
    public ParticleSystem muzzleFlashEffect; // 총구 불꽃 효과
    public ParticleSystem shellEjectEffect; // 탄피 배출 효과
    public LineRenderer lineRenderer; // 총알 궤적을 나타내는 선
    private AudioSource gunAudioPlayer; // 총의 오디오 소스
    public AudioClip shotClip; // 총 발사 소리
    public AudioClip relordcilp; // 재장전 소리
    public float damage = 25f; // 총의 데미지
    public float firedistance = 50f; // 총의 사정거리
    internal int ammoRemain = 50; // 남은 전체 탄약
    internal int magCapacity = 25; // 총의 최대 탄약 용량
    internal int magAmmo; // 현재 남은 탄약 수
    public float timeBetfire = 0.12f; // 발사 간격
    public float reloadTime = 1.0f; // 재장전 시간
    private float lastFiretime; // 마지막 발사 시간

    void Awake()
    {
        // 필요한 컴포넌트를 초기화
        gunAudioPlayer = GetComponent<AudioSource>(); // 오디오 소스 컴포넌트 가져오기
        lineRenderer = GetComponent<LineRenderer>(); // 선 렌더러 컴포넌트 가져오기

        lineRenderer.positionCount = 2; // 선의 점 개수 설정
        lineRenderer.enabled = false; // 초기에는 선 비활성화
    }

    private void OnEnable()
    {
        magAmmo = magCapacity; // 재장전 시 탄창을 가득 채움
        state = State.Ready; // 상태를 준비 완료로 설정
        lastFiretime = 0f; // 마지막 발사 시간 초기화
    }

    // 총 발사 시도 메소드
    public void Fire()
    {
        // 현재 상태가 준비 상태이고, 발사 가능한 시간인지 확인
        if (state == State.Ready && Time.time >= lastFiretime + timeBetfire)
        {
            lastFiretime = Time.time; // 마지막 발사 시간 갱신
            Shot(); // 실제 발사 메소드 호출
        }
    }

    // 실제 발사 로직
    private void Shot()
    {
        RaycastHit hit; // 충돌 정보를 저장할 변수
        Vector3 hitposition = Vector3.zero; // 충돌 위치 초기화

        // 발사 위치에서 전방으로 레이캐스트
        if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, firedistance))
        {
            // 충돌한 오브젝트에서 IDeamage 인터페이스를 구현한 객체를 가져옴
            IDeamage target = hit.collider.GetComponent<IDeamage>();
            if (target != null)
            {
                // 상대방에게 데미지를 입힘
                target.OnDeamge(damage, hit.point, hit.normal);
            }
            // 레이가 충돌한 위치 저장
            hitposition = hit.point;
        }
        else
        {
            // 레이가 충돌하지 않았을 경우 최대 사정 거리까지 위치 계산
            hitposition = fireTransform.position + fireTransform.forward * firedistance;
        }

        // 발사 효과 실행
        StartCoroutine(ShotEffect(hitposition));
        // 호스트만 발사 나머진 대기
        photonView.RPC("ShotProcessOnSever", RpcTarget.MasterClient);
        // 실제 발사처리는 호스트에 대리 
        magAmmo--; // 남은 탄약 수 감소

        // 탄약이 0 이하일 경우 상태를 Empty로 변경
        if (magAmmo <= 0)
        {
            state = State.Empty;
        }
    }

    [PunRPC] // 호스트에 실제로 발사 처리
    private void ShotProcessOnSever()
    {
        RaycastHit hit;
        Vector3 hitPos = Vector3.zero;
        if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, firedistance))
        {
            IDeamage target = hit.collider.GetComponent<IDeamage>();
            if (target != null)
            {
                target.OnDeamge(damage, hit.point, hit.normal);
            }
            hitPos = hit.point;

        }
        else
        {
            hitPos = fireTransform.position + fireTransform.forward * firedistance;
        }

        photonView.RPC("ShotEffectProcessOnClient", RpcTarget.All, hitPos);
    }

    [PunRPC]
    void ShotEffectProcessOnClient(Vector3 hitpos)
    {
        StartCoroutine(ShotEffect(hitpos));
    }

    // 발사 효과를 처리하는 코루틴
    IEnumerator ShotEffect(Vector3 hitposition)
    {
        lineRenderer.enabled = true; // 선 활성화
        muzzleFlashEffect.Play(); // 총구 불꽃 효과 재생
        shellEjectEffect.Play(); // 탄피 배출 효과 재생
        gunAudioPlayer.PlayOneShot(shotClip); // 총 발사 소리 재생

        // 선의 시작점과 끝점 설정
        lineRenderer.SetPosition(0, fireTransform.position);
        lineRenderer.SetPosition(1, hitposition);
        yield return new WaitForSeconds(0.03f); // 잠시 대기

        lineRenderer.enabled = false; // 선 비활성화
    }

    // 재장전 시도 메소드
    public bool Relord()
    {
        // 현재 상태가 재장전 중이거나 남은 탄약이 없거나 탄창이 가득 차있으면 false 반환
        if (state == State.Reloading || ammoRemain <= 0 || magAmmo >= magCapacity)
        {
            return false;
        }

        StartCoroutine(RelordRoutine()); // 재장전 루틴 시작
        return true; // 재장전 시도 성공
    }

    // 재장전 루틴 코루틴
    IEnumerator RelordRoutine()
    {
        state = State.Reloading; // 상태를 재장전 중으로 변경
        gunAudioPlayer.PlayOneShot(relordcilp); // 재장전 소리 재생

        yield return new WaitForSeconds(reloadTime); // 재장전 시간 대기

        // 채울 탄약 수 계산
        int ammoToFill = magCapacity - magAmmo;
        // 남은 탄약이 부족할 경우 채워야 할 탄약 수 조정
        if (ammoRemain < ammoToFill)
        {
            ammoToFill = ammoRemain;
        }
        // 탄창에 탄약 채우기
        magAmmo += ammoToFill;
        // 남은 탄약에서 채운 만큼 줄임
        ammoRemain -= ammoToFill;

        state = State.Ready; // 상태를 준비 완료로 변경
    }

    [PunRPC]
    public void AddAmmo(int ammo)
    {
        ammoRemain += ammo; // 남은 탄약 수 증가
    }

    // Photon 네트워크 상태 동기화 메소드
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // 로컬에서 송신하는 경우
        {
            stream.SendNext(ammoRemain); // 남은 탄약 수 송신
            stream.SendNext(magAmmo); // 현재 탄약 수 송신
            stream.SendNext(state); // 현재 총의 상태 송신
        }
        else if (stream.IsReading) // 다른 유저의 상태를 수신하는 경우
        {
            ammoRemain = (int)stream.ReceiveNext(); // 남은 탄약 수 수신
            magAmmo = (int)stream.ReceiveNext(); // 현재 탄약 수 수신
            state = (State)stream.ReceiveNext(); // 총의 상태 수신
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// LivingEntity 클래스는 MonoBehaviourPun을 상속하여 Photon 네트워킹 기능을 사용할 수 있으며, IDeamage 인터페이스를 구현합니다.
public class LivingEntity : MonoBehaviourPun, IDeamage
{
    public float startingHealth = 100f; // 시작 체력
    public float health { get; protected set; } // 현재 체력

    public bool dead { get; protected set; } // 사망 여부
    public event Action onDeath; // 사망 시 발동되는 이벤트

    // 호스트에서 모든 클라이언트로 사망 상태를 동기화하는 메서드
    [PunRPC]
    public void ApplyUpdateHealth(float newhealth, bool newDead)
    {
        health = newhealth; // 새로운 체력 값으로 업데이트
        dead = newDead; // 새로운 사망 상태로 업데이트
    }

    // 생명체가 활성화될 때 상태를 리셋
    protected virtual void OnEnable()
    {
        // 물려 받을 가상 함수
        dead = false; // 사망 상태를 false로 초기화
        health = startingHealth; // 체력을 시작 체력으로 초기화
    }

    // 호스트에서 단독 실행되고, 호스트를 통해 다른 클라이언트에서 일괄 실행되는 메서드
    [PunRPC]
    public virtual void OnDeamge(float damge, Vector3 hitPoint, Vector3 hitNormal)
    {
        // 자신이 호스트인지 확인
        if (PhotonNetwork.IsMasterClient)
        {
            health -= damge; // 호스트이면 데미지만큼 체력을 줄임
            // 호스트에서 클라이언트로 체력을 동기화
            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health, dead);
            // 데미지 정보도 클라이언트로 동기화
            photonView.RPC("OnDeamge", RpcTarget.Others, hitPoint, hitNormal);
        }

        // 체력이 0 이하이고, 아직 죽지 않았다면
        if (health <= 0 && !dead)
        {
            Die(); // 사망 처리
        }
    }

    // 체력을 회복하는 기능
    [PunRPC]
    public virtual void RestoreHealth(float newHealth)
    {
        if (dead)
        {
            // 이미 죽었다면 체력을 회복할 수 없음
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            // 호스트인 경우에만 체력이 추가됨
            health += newHealth; // 체력 회복
            // 서버에서 클라이언트로 체력을 동기화 (호스트 유저는 데미지만 받지만 나머지 유저들에겐 사운드와 ui를 클라이언트쪽으로 넘겨 보일 수 있도록함)
            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health, dead);
            // 다른 클라이언트도 RestoreHealth를 실행함
            photonView.RPC("RestoreHealth", RpcTarget.Others, newHealth);
        }
    }

    // 사망 처리 메서드
    public virtual void Die()
    {
        if (onDeath != null)
        {
            onDeath(); // 사망 이벤트 발동
        }
        dead = true; // 사망 상태를 true로 설정
    }
}

// LivingEntity 클래스는 IDeamage 인터페이스를 상속하므로 OnDamage() 메서드를 반드시 구현해야 합니다.
// LivingEntity 에서 RestoreHealth()와 OnDamage() 메서드는 [RunRPC] 속성으로 선언 되어 있었다. 오버라이드 측면에서도
// 원본 메서드 와 같이 [PunPRC] 속성을 선언해야 정상적으로 RPC를 통해 원격 실행을 할 수 있다.
// 따라서 PlayerHealth 스크립트와 RestoreHealth() 와  OnDamage() 에도 동일한 RPC를 선언함
// 어떤 클라이언트에서 PlayerHealth 스크립트의 OnDamege()를 실행 되었다고 가정했을 때 클라이언트가 호스트가 맞든 아니든 효과음을 제생하고 
// 체력 슬라이드를 갱신하는 부분은 모두 제대로 실행 된다.
// 즉 모든 클라이언트에서 PlayerHealth 스크립트의 OnDamage()를 동시에 실행 된다고 했을 때 실제 데미지 적용은 
// 호스트에서만 실행이 되고 나머지 클라이언트는 겉으로 보이는 사운드나 ui 효과만 볼 수 있도록 한다. PlayerHealth에  RestoreHealth도 마찬가지다
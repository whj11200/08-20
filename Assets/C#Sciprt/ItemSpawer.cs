using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Photon.Pun;


public class ItemSpawer : MonoBehaviourPun
{
    public GameObject[] items; // 생성할 아이템들(탄약,체력회복)
    public Transform playerTransfrom; // 플레이어 위치
    public float maxDist = 5f; // 플레이어 위치에서 아이템이 배치될 최대반전
    public float timeBetSpawnMax = 7f; // 최대 시간 간격
    public float timeBetSpawnMin = 2f; // 최소 시간 간격
    private float timeBetSpwan; // 생성 간격
    private float lastSpawnTime; //마지막 생성 시점
    
 
    void Start()
    {
         // 아이템이 2~7초사이를 정함
        timeBetSpwan = Random.Range(timeBetSpwan, timeBetSpawnMax);
        lastSpawnTime = 0f;
        //playerTransfrom = GameObject.FindWithTag("Player").transform;
    }

    
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // 현재시간이 마지막시간과 생성 간격이 보다 크면과 플레이어 위치가 null이 아닐때
        if (Time.time >= lastSpawnTime + timeBetSpwan && playerTransfrom != null)
        {
            //마지막시간에 현재시간을 대입
            lastSpawnTime = Time.time;
            // 아이템이 2~7초사이를 정함
            timeBetSpwan = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
            //소환하는 함수 
            Spawn();
        }
    }

    void Spawn()
    {
        //벡터 변수에 네비메쉬 거리 함수를 대입
        Vector3 spawnpos = GetRandomPointOnNavmesh(Vector3.zero,maxDist);
        // 바닥에서 0.5 만큼 올리기
        spawnpos += Vector3.up * 0.5f;
        // 아이템 중 하나를 무작위로 골라서  랜덤으로 설정
        GameObject itemTocreate = items[Random.Range(0,items.Length)];
        #region 자기자신만 생성되고 소멸 된다.



        // 아이템을 무작위 위치값에 무작위 오브젝트생성
        //GameObject item = Instantiate(itemTocreate,spawnpos,Quaternion.identity);
        //// 생성된 아이템이 5초 후에 파괴되도록 설정
        //Destroy(item, 5f);
        #endregion
        // 네트워크에서 모든 클라이언트에서 해당 아이템 생성
        GameObject item = PhotonNetwork.Instantiate(itemTocreate.name, spawnpos,Quaternion.identity);
        StartCoroutine(DestroyAfter(item,5f));
    }

    IEnumerator DestroyAfter(GameObject target , float delay)
    {
        yield return new WaitForSeconds(delay);
        // 오브젝트가 있으면
        if(target != null)
        {
            //파괴
            PhotonNetwork.Destroy(target);
        }
        
    }

    //네비 메쉬위에 랜덤한 위치를 반환 하는 메서드
    // center를 중심으로 거리 반경에서 랜덤한 위치를 찾음
    private Vector3 GetRandomPointOnNavmesh(Vector3 center, float distance)
    {
       // 벡터의 변수에  반지름이 1인 원 안에서 랜덤한 점을 반환하는 프로퍼티
       Vector3 randomPos = Random.insideUnitSphere * distance+center;
        // 네비메쉬 샘플링의 결과 정보를 저장 하는 변수
        NavMeshHit hit;
        //네비메쉬의 반경안에 샘플링 되는 월드 좌표와  포지션 정보를 담을 NavMeshHit 객체,샘플링할 최대 거리,검색할 네비메시 영역
        NavMesh.SamplePosition(randomPos,out hit,distance,NavMesh.AllAreas);
        return hit.position;
    }
}

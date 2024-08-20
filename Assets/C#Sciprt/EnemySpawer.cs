using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class EnemySpawer : MonoBehaviourPun,IPunObservable
{
    [SerializeField] public EnemyScript enemyPrefab; // 적 오브젝트 스크립트 (오타 수정: enemyperfad -> enemyPrefab)
    [SerializeField] public Transform[] spawnPoints; // 적이 생성될 위치 배열 (오타 수정: spwanPoints -> spawnPoints)
    [SerializeField] public float damageMax = 40f; // 최대 피해량
    [SerializeField] public float damageMin = 20f; // 최소 피해량
    [SerializeField] public float healthMax = 200f; // 최대 체력
    [SerializeField] public float healthMin = 100f; // 최소 체력
    [SerializeField] public float speedMax = 3f; // 최대 속도
    [SerializeField] public float speedMin = 1.0f; // 최소 속도
    [SerializeField] public Color strongEnemyColor = Color.red; // 강한 적 AI가 가지게 될 피부색
    private List<EnemyScript> enemies = new List<EnemyScript>(); // 현재 게임에 존재하는 적 리스트
    private int wave; // 현재 웨이브 수
    private int enemyCount = 0;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            //남은 적수를 네트워크로 보내기
            stream.SendNext(enemies.Count); 
            stream.SendNext(wave);//현재 웨이브를 네트워크로 보내기
        }
        else
        {
            //리모트 오브젝트 읽기 부분이 실행됌
            //남은 적수 네트워크 받기
            enemyCount=(int)stream.ReceiveNext();
            // 현재 웨이브를 네트워크로 통해 받기
            wave = (int)stream.ReceiveNext();

        }
    }
    void Awake()
    {
        // 좀비 색상이 직렬화 되어서 서버가 갔다가 다시 역직렬화 되어서 color 값으로 되돌아온다.
        PhotonPeer.RegisterType(typeof(Color), 120, ColorSerialization.SerializeColor,ColorSerialization.DeserializeColor);

    }
    void Update()
    {
        // 호스트만 적을 직접 생성 할 수 있다.
        // 다른 클라이언트는 호스트가 생성한 적을 동기화 해서 받아온다.
        if (PhotonNetwork.IsMasterClient)
        {  // 게임이 종료된 경우, 적 스폰을 중지
            if (GameManger.Instance != null && GameManger.Instance.isGameOver)
            {
                return;
            }

            // 적을 모두 물리친 경우 다음 웨이브로 넘어감
            if (enemies.Count <= 0)
            {
                SpawnWave(); // 새로운 웨이브 스폰
            }

            UpdateUi(); // UI 업데이트
        }
      
      
    }

    void UpdateUi()
    {
       if(PhotonNetwork.IsMasterClient)
        {
            // 현재 웨이브와 적의 수를 UI에 업데이트
            //호스트가 직접 갱신 한 적리스트를 이용해 남은 적수 표시
            UiManger.ui_instance.UpdateWaveText(wave, enemies.Count);
        }
        else
        {
            // 클라이언트는 적 리스트 갱신 할 수 없으므로
            // 호스트가 보내준 enemyCount를 이용해 적 수 표시
            UiManger.ui_instance.UpdateWaveText(wave,enemyCount);
        }
       
    }

    void SpawnWave() // 현재 웨이브에 맞춰서 적 생성
    {
        wave++; // 현재 웨이브 수 증가
        int spawnCount = Mathf.RoundToInt(wave * 1.5f); // 웨이브 수에 따라 적 생성 수 계산
        for (int i = 0; i < spawnCount; i++)
        {
            // 적의 강도 (세기)를 0에서 1 사이에서 랜덤으로 결정
            float enemyIntensity = Random.Range(0f, 1f);
            CreateEnemy(enemyIntensity); // 적 생성
        }
    }

    void CreateEnemy(float intensity) // 적 오브젝트 생성하고 추적할 대상을 할당
    {
        // intensity 를 기반으로 적의 능력치 결정
        float health = Mathf.Lerp(healthMin, healthMax, intensity); // 체력
        float damage = Mathf.Lerp(damageMin, damageMax, intensity); // 피해량
        float speed = Mathf.Lerp(speedMin, speedMax, intensity); // 속도
        // intensity 를 기반으로 하얀색과 강한 적 색상 사이에서 피부색 결정
        Color skinColor = Color.Lerp(Color.white, strongEnemyColor, intensity);
        // 생성할 위치를 랜덤으로 결정
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // 적 오브젝트 생성 및 초기화
        //EnemyScript enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        GameObject createEnemy = PhotonNetwork.Instantiate(enemyPrefab.gameObject.name, spawnPoint.position, spawnPoint.rotation);
        EnemyScript enemy = createEnemy.GetComponent<EnemyScript>();
        //적 생성 시 네트워크로 적 생성        //enemy.SetUp(health, damage, speed, skinColor); // 적의 능력치 설정
        enemy.photonView.RPC("SetUp", RpcTarget.All,health,damage,speed,skinColor);    

        enemies.Add(enemy); // 적을 리스트에 추가
        // 적이 죽을 시 이벤트 등록
        enemy.onDeath += () => enemies.Remove(enemy); // 리스트에서 제거
        enemy.onDeath += () => StartCoroutine(DestoryAfter(enemy.gameObject,10f));
        enemy.onDeath += () => GameManger._Instance.AddScore(100); // 점수 추가
    }
    IEnumerator DestoryAfter(GameObject target , float delay)
    {
        yield return new WaitForSeconds(delay);
        if(target == null)
        {
            PhotonNetwork.Destroy(target);
        }
    }
   
}

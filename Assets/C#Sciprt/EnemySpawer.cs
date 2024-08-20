using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class EnemySpawer : MonoBehaviourPun,IPunObservable
{
    [SerializeField] public EnemyScript enemyPrefab; // �� ������Ʈ ��ũ��Ʈ (��Ÿ ����: enemyperfad -> enemyPrefab)
    [SerializeField] public Transform[] spawnPoints; // ���� ������ ��ġ �迭 (��Ÿ ����: spwanPoints -> spawnPoints)
    [SerializeField] public float damageMax = 40f; // �ִ� ���ط�
    [SerializeField] public float damageMin = 20f; // �ּ� ���ط�
    [SerializeField] public float healthMax = 200f; // �ִ� ü��
    [SerializeField] public float healthMin = 100f; // �ּ� ü��
    [SerializeField] public float speedMax = 3f; // �ִ� �ӵ�
    [SerializeField] public float speedMin = 1.0f; // �ּ� �ӵ�
    [SerializeField] public Color strongEnemyColor = Color.red; // ���� �� AI�� ������ �� �Ǻλ�
    private List<EnemyScript> enemies = new List<EnemyScript>(); // ���� ���ӿ� �����ϴ� �� ����Ʈ
    private int wave; // ���� ���̺� ��
    private int enemyCount = 0;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            //���� ������ ��Ʈ��ũ�� ������
            stream.SendNext(enemies.Count); 
            stream.SendNext(wave);//���� ���̺긦 ��Ʈ��ũ�� ������
        }
        else
        {
            //����Ʈ ������Ʈ �б� �κ��� ������
            //���� ���� ��Ʈ��ũ �ޱ�
            enemyCount=(int)stream.ReceiveNext();
            // ���� ���̺긦 ��Ʈ��ũ�� ���� �ޱ�
            wave = (int)stream.ReceiveNext();

        }
    }
    void Awake()
    {
        // ���� ������ ����ȭ �Ǿ ������ ���ٰ� �ٽ� ������ȭ �Ǿ color ������ �ǵ��ƿ´�.
        PhotonPeer.RegisterType(typeof(Color), 120, ColorSerialization.SerializeColor,ColorSerialization.DeserializeColor);

    }
    void Update()
    {
        // ȣ��Ʈ�� ���� ���� ���� �� �� �ִ�.
        // �ٸ� Ŭ���̾�Ʈ�� ȣ��Ʈ�� ������ ���� ����ȭ �ؼ� �޾ƿ´�.
        if (PhotonNetwork.IsMasterClient)
        {  // ������ ����� ���, �� ������ ����
            if (GameManger.Instance != null && GameManger.Instance.isGameOver)
            {
                return;
            }

            // ���� ��� ����ģ ��� ���� ���̺�� �Ѿ
            if (enemies.Count <= 0)
            {
                SpawnWave(); // ���ο� ���̺� ����
            }

            UpdateUi(); // UI ������Ʈ
        }
      
      
    }

    void UpdateUi()
    {
       if(PhotonNetwork.IsMasterClient)
        {
            // ���� ���̺�� ���� ���� UI�� ������Ʈ
            //ȣ��Ʈ�� ���� ���� �� ������Ʈ�� �̿��� ���� ���� ǥ��
            UiManger.ui_instance.UpdateWaveText(wave, enemies.Count);
        }
        else
        {
            // Ŭ���̾�Ʈ�� �� ����Ʈ ���� �� �� �����Ƿ�
            // ȣ��Ʈ�� ������ enemyCount�� �̿��� �� �� ǥ��
            UiManger.ui_instance.UpdateWaveText(wave,enemyCount);
        }
       
    }

    void SpawnWave() // ���� ���̺꿡 ���缭 �� ����
    {
        wave++; // ���� ���̺� �� ����
        int spawnCount = Mathf.RoundToInt(wave * 1.5f); // ���̺� ���� ���� �� ���� �� ���
        for (int i = 0; i < spawnCount; i++)
        {
            // ���� ���� (����)�� 0���� 1 ���̿��� �������� ����
            float enemyIntensity = Random.Range(0f, 1f);
            CreateEnemy(enemyIntensity); // �� ����
        }
    }

    void CreateEnemy(float intensity) // �� ������Ʈ �����ϰ� ������ ����� �Ҵ�
    {
        // intensity �� ������� ���� �ɷ�ġ ����
        float health = Mathf.Lerp(healthMin, healthMax, intensity); // ü��
        float damage = Mathf.Lerp(damageMin, damageMax, intensity); // ���ط�
        float speed = Mathf.Lerp(speedMin, speedMax, intensity); // �ӵ�
        // intensity �� ������� �Ͼ���� ���� �� ���� ���̿��� �Ǻλ� ����
        Color skinColor = Color.Lerp(Color.white, strongEnemyColor, intensity);
        // ������ ��ġ�� �������� ����
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // �� ������Ʈ ���� �� �ʱ�ȭ
        //EnemyScript enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        GameObject createEnemy = PhotonNetwork.Instantiate(enemyPrefab.gameObject.name, spawnPoint.position, spawnPoint.rotation);
        EnemyScript enemy = createEnemy.GetComponent<EnemyScript>();
        //�� ���� �� ��Ʈ��ũ�� �� ����        //enemy.SetUp(health, damage, speed, skinColor); // ���� �ɷ�ġ ����
        enemy.photonView.RPC("SetUp", RpcTarget.All,health,damage,speed,skinColor);    

        enemies.Add(enemy); // ���� ����Ʈ�� �߰�
        // ���� ���� �� �̺�Ʈ ���
        enemy.onDeath += () => enemies.Remove(enemy); // ����Ʈ���� ����
        enemy.onDeath += () => StartCoroutine(DestoryAfter(enemy.gameObject,10f));
        enemy.onDeath += () => GameManger._Instance.AddScore(100); // ���� �߰�
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

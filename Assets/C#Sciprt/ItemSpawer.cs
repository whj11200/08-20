using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Photon.Pun;


public class ItemSpawer : MonoBehaviourPun
{
    public GameObject[] items; // ������ �����۵�(ź��,ü��ȸ��)
    public Transform playerTransfrom; // �÷��̾� ��ġ
    public float maxDist = 5f; // �÷��̾� ��ġ���� �������� ��ġ�� �ִ����
    public float timeBetSpawnMax = 7f; // �ִ� �ð� ����
    public float timeBetSpawnMin = 2f; // �ּ� �ð� ����
    private float timeBetSpwan; // ���� ����
    private float lastSpawnTime; //������ ���� ����
    
 
    void Start()
    {
         // �������� 2~7�ʻ��̸� ����
        timeBetSpwan = Random.Range(timeBetSpwan, timeBetSpawnMax);
        lastSpawnTime = 0f;
        //playerTransfrom = GameObject.FindWithTag("Player").transform;
    }

    
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // ����ð��� �������ð��� ���� ������ ���� ũ��� �÷��̾� ��ġ�� null�� �ƴҶ�
        if (Time.time >= lastSpawnTime + timeBetSpwan && playerTransfrom != null)
        {
            //�������ð��� ����ð��� ����
            lastSpawnTime = Time.time;
            // �������� 2~7�ʻ��̸� ����
            timeBetSpwan = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
            //��ȯ�ϴ� �Լ� 
            Spawn();
        }
    }

    void Spawn()
    {
        //���� ������ �׺�޽� �Ÿ� �Լ��� ����
        Vector3 spawnpos = GetRandomPointOnNavmesh(Vector3.zero,maxDist);
        // �ٴڿ��� 0.5 ��ŭ �ø���
        spawnpos += Vector3.up * 0.5f;
        // ������ �� �ϳ��� �������� ���  �������� ����
        GameObject itemTocreate = items[Random.Range(0,items.Length)];
        #region �ڱ��ڽŸ� �����ǰ� �Ҹ� �ȴ�.



        // �������� ������ ��ġ���� ������ ������Ʈ����
        //GameObject item = Instantiate(itemTocreate,spawnpos,Quaternion.identity);
        //// ������ �������� 5�� �Ŀ� �ı��ǵ��� ����
        //Destroy(item, 5f);
        #endregion
        // ��Ʈ��ũ���� ��� Ŭ���̾�Ʈ���� �ش� ������ ����
        GameObject item = PhotonNetwork.Instantiate(itemTocreate.name, spawnpos,Quaternion.identity);
        StartCoroutine(DestroyAfter(item,5f));
    }

    IEnumerator DestroyAfter(GameObject target , float delay)
    {
        yield return new WaitForSeconds(delay);
        // ������Ʈ�� ������
        if(target != null)
        {
            //�ı�
            PhotonNetwork.Destroy(target);
        }
        
    }

    //�׺� �޽����� ������ ��ġ�� ��ȯ �ϴ� �޼���
    // center�� �߽����� �Ÿ� �ݰ濡�� ������ ��ġ�� ã��
    private Vector3 GetRandomPointOnNavmesh(Vector3 center, float distance)
    {
       // ������ ������  �������� 1�� �� �ȿ��� ������ ���� ��ȯ�ϴ� ������Ƽ
       Vector3 randomPos = Random.insideUnitSphere * distance+center;
        // �׺�޽� ���ø��� ��� ������ ���� �ϴ� ����
        NavMeshHit hit;
        //�׺�޽��� �ݰ�ȿ� ���ø� �Ǵ� ���� ��ǥ��  ������ ������ ���� NavMeshHit ��ü,���ø��� �ִ� �Ÿ�,�˻��� �׺�޽� ����
        NavMesh.SamplePosition(randomPos,out hit,distance,NavMesh.AllAreas);
        return hit.position;
    }
}

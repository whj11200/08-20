using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;


public class GameManger : MonoBehaviourPunCallbacks,IPunObservable
{
    public static GameManger Instance;
    
    public static GameManger _Instance
    {
        get
        {
            if(Instance == null)
                Instance = FindObjectOfType<GameManger>();
            return Instance;
        }
    }
    public GameObject PlayerPrefad;//������ �÷��̾� ĳ���� ������

    public bool isGameOver
    {
        get; private set;
    }
    void Awake()
    {
        // ���� �̱��� ������Ʈ�� �� �ٸ� GameManger ������Ʈ�� �ִٸ�
       if(Instance != null)
       {
         Destroy(gameObject);
       }
       
    }

    private void Start()
    {
        //�÷��̾� ĳ������ ��� �̺�Ʈ �߻� �� ���ӿ���
        //FindObjectOfType<PlayerHeathle>().onDeath += EndGame;
        //�÷��̾ ������ ��ġ
        Vector3 randomSpwanPos = Random.insideUnitSphere * 5f;
        randomSpwanPos.y = 1;
        PhotonNetwork.Instantiate(PlayerPrefad.name,randomSpwanPos,Quaternion.identity);


    }
    private int score = 0;
    public void AddScore(int newScore)
    {
        if (!isGameOver)
        {
            score += newScore;
            UiManger.ui_instance.UpdateScoreText(score);
        }
    }
    public void EndGame()
    {
        isGameOver = true;
        UiManger.ui_instance.SetActiveGameOverUI(true);
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // ������ �������� �۽�
        {
            stream.SendNext(score);
        }
        else // �ٸ� ��Ʈ��ũ ������ �������� ����
        {
            score = (int)stream.ReceiveNext();
            // ��Ʈ��ũ�� ���� score �� ���
            UiManger.ui_instance .UpdateScoreText(score);
            //����ȭ �ؼ� ���� ���� uiǥ��
        }
    }
}

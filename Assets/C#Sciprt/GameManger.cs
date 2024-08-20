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
    public GameObject PlayerPrefad;//생성할 플레이어 캐릭터 프리팹

    public bool isGameOver
    {
        get; private set;
    }
    void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManger 오브젝트가 있다면
       if(Instance != null)
       {
         Destroy(gameObject);
       }
       
    }

    private void Start()
    {
        //플레이어 캐릭터의 사망 이벤트 발생 시 게임오버
        //FindObjectOfType<PlayerHeathle>().onDeath += EndGame;
        //플레이어가 생성할 위치
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
        if (stream.IsWriting) // 로컬의 움직임을 송신
        {
            stream.SendNext(score);
        }
        else // 다른 네트워크 유저의 움직임을 수신
        {
            score = (int)stream.ReceiveNext();
            // 네트워크를 통해 score 값 평균
            UiManger.ui_instance .UpdateScoreText(score);
            //동기화 해서 받은 점수 ui표시
        }
    }
}

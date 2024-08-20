using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; //  포톤 유니티 네트워크
using Photon.Realtime; // 유니티 포톤 서비스
using UnityEngine.UI;
// 마스터 서버 와 (리슨서버) Mach Making 룸 접속 담당
public class LobbyManger : MonoBehaviourPunCallbacks
{                         //monobehavior를 쓰면서 포톤의 콜벳 함수도 공동으로 쓴다.
    // 게임 버전
    private string gameVersion = "1";
    // 네트워크 정보표시
    public Text connectionInfoText;
    // 룸 접속 버튼 방만들기 버튼
    // 버튼네트워크를 받을라면 ui이벤트인 Interactable를 활성화 해야만 한다.
    public Button joinButton;
    
    void Start()
    {
        
        // 접속에 필요한 정보 (게임버전) 설정
       PhotonNetwork.GameVersion = gameVersion;
        // 설정한 정보로 마스터 서버 접속 시도
       PhotonNetwork.ConnectUsingSettings();
        // 룸 접속 버튼 잠시 비활성화
        joinButton.interactable = false;
        connectionInfoText.text = "마스터 서버에 접속중.....";
        // 만약 버전이 달라지면 접속해버릴 시 이전 버전에 접속 시켜버린다.
    }
    // 마스터 서버 접속 성공시 자동 실행
    public override void OnConnectedToMaster()
    {
       joinButton.interactable = true;
        connectionInfoText.text = "온라인 : 마스터 서버와 연결완료";
    }
    // 마스터 서버 접속 실패시 자동 실행
    public override void OnDisconnected(DisconnectCause cause)
    {
        joinButton.interactable = false;
        connectionInfoText.text = "오프라인 ";
    }
    //룸접속 시도 joying 버튼을 눌렀을 때 호출되는 함수
    public void Connet()
    {
        // 중복 접속을 막기 위해 false로 설정
        joinButton.interactable = false;
        // 마스터 서버에 접속 중이라면
        if(PhotonNetwork.IsConnected)
        {
            connectionInfoText.text = "룸에 접속";
            PhotonNetwork.JoinRandomRoom(); // 아무방에나 접속 랜덤 매치 메이킹
        }
        else
        {
            connectionInfoText.text = "오프라인 ";
            PhotonNetwork.ConnectUsingSettings();
            //마스터 서버로 재접속 시도
        }
    }
    // (빈룸이 없어서) 랜덤 를 참가에 실패한 경우 실행 자동 매칭함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "빈 방이 없음, 새로운 방 생성...";
        // 룸 생성함수안에 null이면 새로운 룸옵션생하며 플레이어의 수는 4명으로 제안
        Debug.Log("방이 없음");
        PhotonNetwork.CreateRoom(null, new RoomOptions() {MaxPlayers = 4});

        // 생성된 룸 목록을 확인하는 기능을 만들지 않으므로 룸의 이름은
        // 입력 하지 않고 null로 입력 했다. 수용 가능한 인원은 4명으로 제한
        // 참고로 생성된 룸은 리슨 서버방식으로 동작 하며
        // 룸을 생성한 클라이언트 호스트 역활을 맡는다.
    }
    // 룸에 참가 완료된 자동 실행
    public override void OnJoinedRoom()
    {
        connectionInfoText.text = "방 참가 성공";
        PhotonNetwork.LoadLevel("Main");
        // 모든 룸 참가자가 메인씬으로 이동함
    }
 
}

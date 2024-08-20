using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; //  ���� ����Ƽ ��Ʈ��ũ
using Photon.Realtime; // ����Ƽ ���� ����
using UnityEngine.UI;
// ������ ���� �� (��������) Mach Making �� ���� ���
public class LobbyManger : MonoBehaviourPunCallbacks
{                         //monobehavior�� ���鼭 ������ �ݺ� �Լ��� �������� ����.
    // ���� ����
    private string gameVersion = "1";
    // ��Ʈ��ũ ����ǥ��
    public Text connectionInfoText;
    // �� ���� ��ư �游��� ��ư
    // ��ư��Ʈ��ũ�� ������� ui�̺�Ʈ�� Interactable�� Ȱ��ȭ �ؾ߸� �Ѵ�.
    public Button joinButton;
    
    void Start()
    {
        
        // ���ӿ� �ʿ��� ���� (���ӹ���) ����
       PhotonNetwork.GameVersion = gameVersion;
        // ������ ������ ������ ���� ���� �õ�
       PhotonNetwork.ConnectUsingSettings();
        // �� ���� ��ư ��� ��Ȱ��ȭ
        joinButton.interactable = false;
        connectionInfoText.text = "������ ������ ������.....";
        // ���� ������ �޶����� �����ع��� �� ���� ������ ���� ���ѹ�����.
    }
    // ������ ���� ���� ������ �ڵ� ����
    public override void OnConnectedToMaster()
    {
       joinButton.interactable = true;
        connectionInfoText.text = "�¶��� : ������ ������ ����Ϸ�";
    }
    // ������ ���� ���� ���н� �ڵ� ����
    public override void OnDisconnected(DisconnectCause cause)
    {
        joinButton.interactable = false;
        connectionInfoText.text = "�������� ";
    }
    //������ �õ� joying ��ư�� ������ �� ȣ��Ǵ� �Լ�
    public void Connet()
    {
        // �ߺ� ������ ���� ���� false�� ����
        joinButton.interactable = false;
        // ������ ������ ���� ���̶��
        if(PhotonNetwork.IsConnected)
        {
            connectionInfoText.text = "�뿡 ����";
            PhotonNetwork.JoinRandomRoom(); // �ƹ��濡�� ���� ���� ��ġ ����ŷ
        }
        else
        {
            connectionInfoText.text = "�������� ";
            PhotonNetwork.ConnectUsingSettings();
            //������ ������ ������ �õ�
        }
    }
    // (����� ���) ���� �� ������ ������ ��� ���� �ڵ� ��Ī�Լ�
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "�� ���� ����, ���ο� �� ����...";
        // �� �����Լ��ȿ� null�̸� ���ο� ��ɼǻ��ϸ� �÷��̾��� ���� 4������ ����
        Debug.Log("���� ����");
        PhotonNetwork.CreateRoom(null, new RoomOptions() {MaxPlayers = 4});

        // ������ �� ����� Ȯ���ϴ� ����� ������ �����Ƿ� ���� �̸���
        // �Է� ���� �ʰ� null�� �Է� �ߴ�. ���� ������ �ο��� 4������ ����
        // ����� ������ ���� ���� ����������� ���� �ϸ�
        // ���� ������ Ŭ���̾�Ʈ ȣ��Ʈ ��Ȱ�� �ô´�.
    }
    // �뿡 ���� �Ϸ�� �ڵ� ����
    public override void OnJoinedRoom()
    {
        connectionInfoText.text = "�� ���� ����";
        PhotonNetwork.LoadLevel("Main");
        // ��� �� �����ڰ� ���ξ����� �̵���
    }
 
}

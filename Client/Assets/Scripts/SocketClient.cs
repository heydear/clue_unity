using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using System.Collections.Generic;

public class SocketClient : MonoBehaviour
{

    public PlayManager PM;
    string ipAdress = "127.0.0.1";
    int port = 4567;


    byte[] sendByte;
    byte[] receiveBuffer;
    NetworkStream NS;
    StreamReader SR = null;
    StreamWriter SW = null;
    TcpClient client = null;
    TcpListener Listener = null;



    void Start()
    {
        client = new TcpClient(ipAdress,port);
        Thread echo_thread = new Thread(ReceiveThread);
        echo_thread.Start();
    }

    public void ReceiveThread()
    {
        while (true)
        {
            if (PM.currentServerState != SERVER_STATE.NOTHING)
                continue;

            NS = client.GetStream();
            receiveBuffer = new byte[1024];
            NS.Read(receiveBuffer, 0, receiveBuffer.Length);
            string msg = Encoding.UTF8.GetString(receiveBuffer, 0, receiveBuffer.Length); // byte[] to string
            Debug.Log(msg);
            string[] split_text;

            split_text = msg.Split('+');
            switch (split_text[0])
            {
                case "START":
                    CallStart();
                    SetCard(int.Parse(split_text[1]), int.Parse(split_text[2]), int.Parse(split_text[3]), int.Parse(split_text[4]), int.Parse(split_text[5]));
                    if (PM)
                    {
                        PM.currentServerState = SERVER_STATE.START;
                    }
                    break;
                case "PLAY":
                    if (PM)
                    {
                        PM.currentServerState = SERVER_STATE.MYTURN;
                    }
                    break;
                case "COMMON":
                    if (split_text.Length >= 6)
                        SetCommonCard(int.Parse(split_text[1]), int.Parse(split_text[2]), int.Parse(split_text[3]), int.Parse(split_text[4]));
                    else
                        SetCommonCard(int.Parse(split_text[1]), int.Parse(split_text[2]));

                    if (PM)
                    {
                        PM.currentServerState = SERVER_STATE.COMMON;
                    }
                    break;
                case "SELECT":
                    SelectOpenCard(int.Parse(split_text[1]), int.Parse(split_text[2]), int.Parse(split_text[3]), int.Parse(split_text[4]));
                    if (PM)
                    {
                        PM.currentServerState = SERVER_STATE.SELECT;
                    }
                    break;
                case "MOVE":
                    if (PM.currentServerState == SERVER_STATE.MYTURN_END)
                        break;

                    SetTokenMove(int.Parse(split_text[1]), float.Parse(split_text[2]), float.Parse(split_text[3]));
                    if (PM)
                    {
                        PM.currentServerState = SERVER_STATE.MOVING_OTHER;
                    }
                    break;
                case "REASON":
                    if (PM)
                    {
                        PM.currentServerState = SERVER_STATE.SELECT;
                    }
                    break;
                case "SHOW":
                    ShowSelectCard(int.Parse(split_text[1]));
                    if (PM)
                    {
                        PM.currentServerState = SERVER_STATE.RECEIVE_OPENCARD;
                    }
                    break;
                case "FINAL":
                    FinalReasonResult(int.Parse(split_text[1]), int.Parse(split_text[2]));
                    if (PM)
                    {
                        PM.currentServerState = SERVER_STATE.FINAL_REASON;
                    }
                    break;
            }
        }
    }
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        try
    //        {
    //            NS = client.GetStream();
    //            string msg ="Do you hear me"; // test Text
    //            int byteCount = Encoding.UTF8.GetByteCount(msg);
    //            sendByte = new byte[byteCount];
    //            sendByte = Encoding.UTF8.GetBytes(msg);
    //            NS.Write(sendByte, 0, sendByte.Length);
    //        }
    //        catch (SocketException e)
    //        {
    //            Debug.Log("Socket send or receive error ! : " + e.ToString());
    //        }
    //    }
    //    if (Input.GetKeyDown(KeyCode.B))
    //    {
    //        //socket.Disconnect(true);
    //        client.Close();
    //    }
    //}

    public void StartMessage()              //시작 메시지 보낼 시 서버에서 카드세팅 후 전부 배분 후에 턴 부여
    {
        NS = client.GetStream();
        string msg = "START"; // test Text
        int byteCount = Encoding.UTF8.GetByteCount(msg);
        sendByte = new byte[byteCount];
        sendByte = Encoding.UTF8.GetBytes(msg);
        NS.Write(sendByte, 0, sendByte.Length);
    }

    public void MoveMessage(string result)               //매개변수로 뚫을 지 대화 필요
    {
        NS = client.GetStream();
        string msg = "MOVE+"+ result;// MOVE + 타겟 토큰 X좌표 Y좌표 입력
        int byteCount = Encoding.UTF8.GetByteCount(msg);
        sendByte = new byte[byteCount];
        sendByte = Encoding.UTF8.GetBytes(msg);
        NS.Write(sendByte, 0, sendByte.Length);
    }

    public void ReasonMessage(string result)
    {
        NS = client.GetStream();
        string msg = "REASON+" + result; // REASON + 캐릭터 카드 번호 + 장소 번호 + 무기 번호
        int byteCount = Encoding.UTF8.GetByteCount(msg);
        sendByte = new byte[byteCount];
        sendByte = Encoding.UTF8.GetBytes(msg);
        NS.Write(sendByte, 0, sendByte.Length);
    }

    public void FinalReasonMessage(string result)
    {
        NS = client.GetStream();
        string msg = "FINAL+" + result; // REASON + 캐릭터 카드 번호 + 장소 번호 + 무기 번호
        int byteCount = Encoding.UTF8.GetByteCount(msg);
        sendByte = new byte[byteCount];
        sendByte = Encoding.UTF8.GetBytes(msg);
        NS.Write(sendByte, 0, sendByte.Length);
    }

    public void OpenCardMessage(string result)
    {
        //////////Card값이 -1이면 보유하지 않은 카드
        /////////////////////////////////카드 세팅해주고 서버한테 카드값 쏴주면 됨 
        NS = client.GetStream();
        string msg = "SELECT+" + result; // SELECT+ 선택한 카드 고유값 인데 일단 테스트로 이거보냄
        int byteCount = Encoding.UTF8.GetByteCount(msg);
        sendByte = new byte[byteCount];
        sendByte = Encoding.UTF8.GetBytes(msg);
        NS.Write(sendByte, 0, sendByte.Length);
    }

    public void EndMessage()  //턴 종료 시 호출 다음 플레이어에게 플레이 권한 부여
    {
        Debug.Log("end");
        NS = client.GetStream();
        string msg = "END+";
        int byteCount = Encoding.UTF8.GetByteCount(msg);
        sendByte = new byte[byteCount];
        sendByte = Encoding.UTF8.GetBytes(msg);
        NS.Write(sendByte, 0, sendByte.Length);
    }

    private void CallStart()
    {
        Debug.Log("Start");
    }

    private void SetCard(int PlayerIndex, int FirstCard=0, int SecondCard=0, int ThirdCard=0, int FourthCard=0)
    {
        PM.currentPlayerIndex = PlayerIndex;
        PM.SetupMyCardList(FirstCard, SecondCard, ThirdCard, FourthCard);
    }

    private void SetCommonCard(int FirstCommonCard = -1, int SecondCommonCard = -1, int ThirdCommonCard = -1, int FourthCommonCard = -1)
    {
        PM.SetupShareCardList(FirstCommonCard, SecondCommonCard, ThirdCommonCard, FourthCommonCard);
    }

    private void SelectOpenCard(int Card1, int Card2, int Card3, int CallPlayerNum)
    {
        PM.SetupOpenCardChoice(new int[3] { Card1, Card2, Card3 }, CallPlayerNum);
    }

    private void ShowSelectCard(int Card)
    {
        PM.SetupReceiveOpenCard(Card);
        Debug.Log(Card + "받음");
    }

    private void SetTokenMove(int targetToken, float x, float y)
    {
        PM.SetupMoveTokenInfo(targetToken, new Vector2(x, y));
    }

    private void FinalReasonResult(int isCorrect, int playerIndex)
    {
        PM.SetupFinalReasonResult(isCorrect == 0 ? false : true, playerIndex);
    }
}
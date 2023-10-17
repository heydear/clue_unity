using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static SERVER_STATE;

public enum ROOM
{
    MOVING = -1,
    LOUNGE = 0,     // 주차장
    DININGROOM,     // 거실
    KITCHEN,        // 부엌
    BALLROOM,       // 무도회장
    LIBRARY,        // 서재
    STUDY,          // 공부방
    HALL,           // 현관
    CONSERVATORY,   // 온실
    BILLIARDROOM = 8,   // 당구장
    FINALCLUE = -2,

    COUNT = 9
}
public enum WEAPON
{
    KNIFE = 0,
    ROPE,
    LEADPIPE,
    WRENCH,
    CANDLESTICK,
    REVOLVER,

    COUNT = 6
}
public enum SUSPECT
{
    RED = 0,    // Miss Scarlet
    YELLOW,     // Colonel Mustard
    GREEN,      // Mr. Green
    BLUE,       // Mrs. Peacock
    PURPLE,     // Professor Plum
    WHITE,      // Mrs. White

    COUNT = 6
}

public enum SERVER_STATE : int
{
    NOTHING = -1,
    START = 0,
    COMMON,
    MYTURN,
    MYTURN_END,
    REASONING_OTHER,
    MOVING_OTHER,
    SELECT,
    RECEIVE_OPENCARD,
    FINAL_REASON,
}

public class PlayManager : MonoBehaviour
{
    public CardInfo cardInfoList;
    public SocketClient sockManager;
    public RoomManager roomManager;
    public SERVER_STATE currentServerState;

    [HideInInspector]
    public int currentPlayerIndex;
    public bool isFinal = false;
    public int finalReasonPlayerIndex;
    private bool isCorrect = false;

    [Header("My Card List Setting")]
    public GameObject cardPrefab;
    public Transform myCardParent;
    public Transform shareCardParent;
    public Image bigCardImage;
    private List<Card> myCardList;
    private List<Card> shareCardList;

    [Header("Token List Setting")]
    public List<TokenInfo> tokensList;
    private TokenInfo myToken;

    [Header("UI Setting")]
    public GameObject startButton;
    public Button rollButton;
    public Text rollingDiceText;
    public Button reasonButton;
    public Button finalReasonButton;
    public Text infoText;
    private bool isInfomation;

    public Dropdown openCardDropdown;
    private int[] openCards;
    private int callPlayerNum = 0;
    private int receiveOpenCardNum = 0;

    private int chosenSuspectIndex;
    private int chosenRoomIndex;
    private int chosenWeaponIndex;

    private int moveTargetTokenIndex = 0;
    private Vector2 moveTargetPoint = Vector2.zero;

    private Dictionary<string, string> CardNameEnToKr;



    private void Awake()
    {
        isInfomation = false;
        currentServerState = NOTHING;

        chosenSuspectIndex = 0;
        chosenRoomIndex = 0;
        chosenWeaponIndex = 0;

        openCards = new int[3];
        myCardList = new List<Card>();
        CardNameEnToKr = new Dictionary<string, string>();

        InitCardNameEnToKr();
    }

    private void Start()
    {
        //SetupOpenCardChoice(new int[3] { -1, 1, 10 });
    }

    private void Update()
    {
        switch(currentServerState)
        {
            case START:
                startButton.SetActive(false);

                myToken = tokensList?.Find(d => d.suspect == (SUSPECT)currentPlayerIndex);
                myToken.isMine = true;

                InstantiateCardList(myCardList, myCardParent);

                currentServerState = NOTHING;
                break;
            case COMMON:
                InstantiateCardList(shareCardList, shareCardParent);

                currentServerState = NOTHING;
                break;
            case MYTURN:
                rollButton.interactable = true;

                currentServerState = NOTHING;
                break;
            case MYTURN_END:
                rollButton.interactable = false;
                reasonButton.interactable = false;
                sockManager.EndMessage();

                currentServerState = NOTHING;
                break;
            case MOVING_OTHER:
                if (isFinal)
                {
                    reasonButton.gameObject.SetActive(false);
                    finalReasonButton.gameObject.SetActive(true);
                }

                tokensList?.Find(d => d.suspect.ToString().Equals(cardInfoList?.FindCard(moveTargetTokenIndex)?.cardName))?.UpdateLocation(moveTargetPoint.x, moveTargetPoint.y);

                currentServerState = NOTHING;
                break;
            case REASONING_OTHER:

                currentServerState = NOTHING;
                break;
            case SELECT:
                InstantiateOpenCardChoice();

                currentServerState = NOTHING;
                break;
            case RECEIVE_OPENCARD:
                ReceiveOpenCard();

                break;
            case FINAL_REASON:
                FinalReasonResult();

                break;
        }


        //{
        //    rollButton.interactable = false;
        //    reasonButton.interactable = false;
        //}
    }

    public void InitCardNameEnToKr()
    {
        CardNameEnToKr.Add("LOUNGE", "주차장");
        CardNameEnToKr.Add("DININGROOM", "거실");
        CardNameEnToKr.Add("KITCHEN", "부엌");
        CardNameEnToKr.Add("BALLROOM", "무도회장");
        CardNameEnToKr.Add("LIBRARY", "서재");
        CardNameEnToKr.Add("STUDY", "공부방");
        CardNameEnToKr.Add("HALL", "현관");
        CardNameEnToKr.Add("CONSERVATORY", "온실");
        CardNameEnToKr.Add("BILLIARDROOM", "당구장");

        CardNameEnToKr.Add("KNIFE", "칼");
        CardNameEnToKr.Add("ROPE", "밧줄");
        CardNameEnToKr.Add("LEADPIPE", "파이프");
        CardNameEnToKr.Add("WRENCH", "랜치");
        CardNameEnToKr.Add("CANDLESTICK", "촛대");
        CardNameEnToKr.Add("REVOLVER", "리볼버");

        CardNameEnToKr.Add("RED", "스칼렛");
        CardNameEnToKr.Add("YELLOW", "머스타드");
        CardNameEnToKr.Add("GREEN", "그린");
        CardNameEnToKr.Add("BLUE", "피콕");
        CardNameEnToKr.Add("PURPLE", "플럼");
        CardNameEnToKr.Add("WHITE", "화이트");
    }

    /// <summary>
    /// 배분받은 개인 카드 세팅 함수
    /// </summary>
    public void SetupMyCardList(int FirstCard = 0, int SecondCard = 0, int ThirdCard = 0, int FourthCard = 0)
    {
        myCardList = new List<Card>
        {
            cardInfoList.FindCard(FirstCard),
            cardInfoList.FindCard(SecondCard),
            cardInfoList.FindCard(ThirdCard),
            cardInfoList.FindCard(FourthCard),
        };
    }

    /// <summary>
    /// 배분받은 공용 카드 세팅 함수
    /// </summary>
    public void SetupShareCardList(int FirstCard = -1, int SecondCard = -1, int ThirdCard = -1, int FourthCard = -1)
    {
        shareCardList = new List<Card>();

        if(FirstCard != -1)
            shareCardList.Add(cardInfoList.FindCard(FirstCard));
        if (SecondCard != -1)
            shareCardList.Add(cardInfoList.FindCard(SecondCard));
        if (ThirdCard != -1)
            shareCardList.Add(cardInfoList.FindCard(ThirdCard));
        if (FourthCard != -1)
            shareCardList.Add(cardInfoList.FindCard(FourthCard));
    }

    public void InstantiateCardList(List<Card> cards, Transform cardParent)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject cardObject = Instantiate(cardPrefab, cardParent);
            Image cardImage = cardObject.GetComponent<Image>();

            cardImage.sprite = cardInfoList.FindCard(cards[i].cardNumber).cardImage;

            EventTrigger eventTrigger = cardObject.GetComponent<EventTrigger>();
            EventTrigger.Entry entry_PointerEnter = eventTrigger.triggers.Find(d => d.eventID == EventTriggerType.PointerEnter);
            EventTrigger.Entry entry_PointerExit = eventTrigger.triggers.Find(d => d.eventID == EventTriggerType.PointerExit);

            entry_PointerEnter.callback.AddListener(d => bigCardImage.gameObject.SetActive(true));
            entry_PointerEnter.callback.AddListener(d => bigCardImage.GetComponent<ImageChange>().GetImage(cardImage));
            entry_PointerExit.callback.AddListener(d => bigCardImage.gameObject.SetActive(false));

            eventTrigger.triggers.Add(entry_PointerEnter);
            eventTrigger.triggers.Add(entry_PointerExit);
        }
    }

    /// <summary>
    /// 주사위 굴리기 함수
    /// </summary>
    public void Rolling()
    {
        int dice1 = Random.Range(1, 7);
        int dice2 = Random.Range(1, 7);

        myToken.MoveToDice(dice1 + dice2);
        rollingDiceText.text = (dice1 + dice2).ToString();
    }

    public void SetupMoveTokenInfo(int targetToken, Vector2 targetPos)
    {
        moveTargetTokenIndex = targetToken;
        moveTargetPoint = targetPos;
    }

    public void UpdateStepCount(int count)
    {
        rollingDiceText.text = count.ToString();
        
        if (count <= 0)
        {
            Vector2 targetPos = myToken.tokenMove.currentPoint;
            int suspectIndex = cardInfoList.FindCard(myToken.suspect.ToString()).cardNumber;
            sockManager.MoveMessage(suspectIndex.ToString() + "+" + targetPos.x.ToString() + "+" + targetPos.y.ToString() + "+");
            Debug.Log(suspectIndex.ToString() + "+" + targetPos.x.ToString() + "+" + targetPos.y.ToString() + "+");

            if(myToken.currentState != ROOM.MOVING)
            {
                reasonButton.interactable = true;

                SetupChosenSuspect(chosenSuspectIndex);
                SetupChosenRoom(chosenRoomIndex);
                SetupChosenWeapon(chosenWeaponIndex);
            }
            else
            {
                currentServerState = MYTURN_END;
            }
        }
    }

    /// <summary>
    /// 추리 함수
    /// </summary>
    public void Reasoning()
    {
        PlayerPlacedCheck ppc = roomManager?.FindRoom(cardInfoList?.FindCard(chosenRoomIndex)?.cardName);
        sockManager.MoveMessage(chosenSuspectIndex.ToString() + "+" + ppc.originPos.x.ToString() + "+" + ppc.originPos.y.ToString() + "+");

        if (!isFinal)
        {
            sockManager.ReasonMessage(chosenSuspectIndex.ToString() + "+" + chosenRoomIndex.ToString() + "+" + chosenWeaponIndex + "+");
        }
        else
        {
            sockManager.FinalReasonMessage(chosenSuspectIndex.ToString() + "+" + chosenRoomIndex.ToString() + "+" + chosenWeaponIndex + "+");
        }

        Debug.Log(chosenSuspectIndex + ", " + chosenRoomIndex + ", " + chosenWeaponIndex);
    }

    public void SetupFinalReasonResult(bool _isCorrect, int targetPlayerIndex)
    {
        isCorrect = _isCorrect;
        finalReasonPlayerIndex = targetPlayerIndex;
    }

    public bool CheckFinalReasonMyToken()
    {
        if (myToken.suspect == (SUSPECT)finalReasonPlayerIndex)
            return true;
        else
            return false;
    }

    public void FinalReasonResult()
    {
        if(isCorrect)
        {
            StartCoroutine(CoProgramExit("플레이어 " + (finalReasonPlayerIndex + 1) + "이(가) 최종 추리에 성공했습니다!"));

            currentServerState = NOTHING;
        }
        else if(!isCorrect)
        {
            StartCoroutine(CoInfoMessage("플레이어 " + (finalReasonPlayerIndex + 1) + "이(가) 최종 추리에 실패했습니다."));

            if(CheckFinalReasonMyToken())
                currentServerState = MYTURN_END;
            else
                currentServerState = NOTHING;
        }
    }


    public void SetupOpenCardChoice(int[] Cards, int _callPlayerNum)
    {
        callPlayerNum = _callPlayerNum;
        openCards = Cards;
    }

    public void InstantiateOpenCardChoice()
    {
        openCardDropdown.ClearOptions();

        foreach (int card in openCards)
        {
            if (card != -1)
            {
                openCardDropdown.AddOptions(new List<Dropdown.OptionData>()
                {
                    new Dropdown.OptionData(CardNameEnToKr[cardInfoList.FindCard(card).cardName])
                });
            }
        }

        if (openCardDropdown.options.Count > 0)
        {
            openCardDropdown.transform.parent.gameObject.SetActive(true);
        }
        //else
        //{
        //    sockManager.OpenCardMessage("nothing+");
        //}
    }

    public void SelectOpenCard(Dropdown dropdown)
    {
        sockManager.OpenCardMessage(openCards[dropdown.value].ToString() + "+" + callPlayerNum + "+");
        openCardDropdown.transform.parent.gameObject.SetActive(false);
    }

    public void SetupChosenSuspect(int value)
    {
        chosenSuspectIndex = cardInfoList.FindCard(((SUSPECT)value).ToString()).cardNumber;
    }
    public void SetupChosenRoom(int value)
    {
        chosenRoomIndex = cardInfoList.FindCard(((ROOM)value).ToString()).cardNumber;
    }
    public void SetupChosenWeapon(int value)
    {
        chosenWeaponIndex = cardInfoList.FindCard(((WEAPON)value).ToString()).cardNumber;
    }

    public void SetupReceiveOpenCard(int card)
    {
        receiveOpenCardNum = card;
    }

    public void ReceiveOpenCard()
    {
        string msg;

        if (receiveOpenCardNum == -1)
        {
            msg = "해당 카드를 가지고 있는 플레이어가 없습니다.";
        }
        else
        {
            msg = CardNameEnToKr[cardInfoList.FindCard(receiveOpenCardNum).cardName] + " 카드를 받았습니다.";
        }

        StartCoroutine(CoInfoMessage(msg));

        currentServerState = MYTURN_END;
    }

    private IEnumerator CoInfoMessage(string msg)
    {
        if (isInfomation)
        {
            yield return new WaitForSeconds(0.1f);
        }
        isInfomation = true;

        infoText.text = msg;
        infoText.transform.parent.gameObject.SetActive(true);

        yield return new WaitForSeconds(3.0f);

        infoText.transform.parent.gameObject.SetActive(false);
        isInfomation = false;
    }

    private IEnumerator CoProgramExit(string msg)
    {
        if (isInfomation)
        {
            yield return new WaitForSeconds(0.1f);
        }
        isInfomation = true;

        infoText.text = msg;
        infoText.transform.parent.gameObject.SetActive(true);

        yield return new WaitForSeconds(3.0f);

        infoText.text = "3초 뒤 프로그램이 종료됩니다.";
        
        yield return new WaitForSeconds(3.0f);

        Application.Quit();
    }
}

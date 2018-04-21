using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour
{

    public GameObject ThinkBubble;
    public Text thinkBubbleText;

    public Button resetButton;

    public AudioSource audioSource;

    public AudioClip wrongButtonClickAudio;

    private StreamWriter writer = null;

    private NetworkStream stream;

    private bool isConnected = false;

    public Text playerNameText;
    public Text mainMessage;
    public Text resultText;
    public Button connectButton;


    public Button diamondButton;
    public Button clubButton;
    public Button heartsButton;
    public Button spadeButton;

    private string CurrentPlayerName;

    public RectTransform trumpSelectedPanel;

    public GameObject playerCardPanel;
    // public RectTransform playerCardPanel;

    public RectTransform trumphSelectingPanel;
    public RectTransform playerWonCardPanel;

    public RectTransform opponentCardPanel;
    public RectTransform opponentSelectedCardPanel;
    public RectTransform opponentWonCardPanel;

    public GameObject avatar;


    private List<String> currentCardList;

    public GameObject smallPrefabButton;
    public GameObject middlePrefabButton;
    public GameObject tooSmallPrefabButton;

    public RectTransform Cardinitialposition;

    public GameObject selectedCardPanel;

    private string lastSelectedCard;

    private int playerWonTrickCount;
    private int opponentWonTrickCount;

    public Text playerWonTrickCountText;
    public Text opponentWonTrickCountText;

    private const string player1name = "player_1";

    public GameObject[] wayPoints;

    public GameObject[] opponenetWayPoints;

    private GameObject wayPoint;

    private GameObject opponenetWayPoint;

    bool isRotate = false;

    private GameObject currentQuadObject;

    private GameObject currentOpnontQuadObject;

    private float lerpTime = 10;

    private float currentLerpTime = 0;

    private int num = 0;

    private bool isPlayerMoveCard = false;

    private bool isOppenonetPlayerMoveCard = false;
        
    private void Awake()
    {
        Button btn = connectButton.GetComponent<Button>();
        btn.onClick.AddListener(Button_Connect);

        Button btnDiamond = diamondButton.GetComponent<Button>();
        btnDiamond.onClick.AddListener(delegate { Trumph_Selction("d"); });

        Button btnClubButton = clubButton.GetComponent<Button>();
        btnClubButton.onClick.AddListener(delegate { Trumph_Selction("c"); });

        Button btnHeartsButton = heartsButton.GetComponent<Button>();
        btnHeartsButton.onClick.AddListener(delegate { Trumph_Selction("h"); });

        Button btnSpadeButton = spadeButton.GetComponent<Button>();
        btnSpadeButton.onClick.AddListener(delegate { Trumph_Selction("s"); });

        Button btnReset = resetButton.GetComponent<Button>();
        btnReset.onClick.AddListener(ClickReset);

    }

    void Start()
    {
        //List<string> list = new List<string>();

        //list.Add("c9");
        //list.Add("c7");
        //list.Add("c14");

        //list.Add("c13");
        //list.Add("d12");
        //list.Add("d8");

        //list.Add("d13");
        //list.Add("d10");


        //loadPanelCardButtons(list);

        //loadWonCard(opponentWonCardPanel);
        //loadWonCard(playerWonCardPanel);

        //loadOpponentPanelCardButtons(3);

        ////loadselectedTrumph("s");
        //this.CurrentPlayerName = "Player_1";
        //playerNameText.text = "Player 1";

        //panelShow(trumphSelectingPanel);

        //loadselectedTrumph(new BiddingMessage("Player_1", 0, "h"));



        //displayMessage("Waiting for the other player choose the trump!");
        //gameObjectShow(avatar);
        //displayMessage("Choose a trump!");


        //StartCoroutine(Move(cube1, cube2.transform.localPosition));
        ResetGameObjectsWithTag(false);
    }

    void Update()
    {
        if (isConnected && stream != null && stream.CanRead)
        {
            readMessage();
        }

        if (isPlayerMoveCard)
        {
            currentLerpTime += Time.deltaTime;

            if (currentLerpTime >= lerpTime)
            {
                currentLerpTime = lerpTime;
            }

            float prec = currentLerpTime / lerpTime;

            if (isRotate)
            {
                currentQuadObject.transform.Rotate(new Vector3(Time.deltaTime * 45, 0, 0));
            }

            currentQuadObject.transform.position = Vector3.Lerp(currentQuadObject.transform.localPosition, wayPoint.transform.localPosition, prec);

            if (currentQuadObject.transform.position == wayPoints[wayPoints.Length - 1].transform.position)
            {
                print("done 1");
                isRotate = false;

                currentLerpTime = 0;
                num = 0;
                isPlayerMoveCard = false;
            }
            else if (currentQuadObject.transform.position == wayPoints[num].transform.position)
            {
                print("Move to " + num);

                num++;

                currentLerpTime = 0;
                wayPoint = wayPoints[num];
                isRotate = true;

            }

        }


        if (isOppenonetPlayerMoveCard)
        {
            currentLerpTime += Time.deltaTime;

            if (currentLerpTime >= lerpTime)
            {
                currentLerpTime = lerpTime;
            }

            float prec = currentLerpTime / lerpTime;

            if (isRotate)
            {
                currentOpnontQuadObject.transform.Rotate(new Vector3(Time.deltaTime * 43, 0, 0));
            }

            currentOpnontQuadObject.transform.position = Vector3.Lerp(currentOpnontQuadObject.transform.localPosition,
                opponenetWayPoint.transform.localPosition, prec);

            if (currentOpnontQuadObject.transform.position == opponenetWayPoints[opponenetWayPoints.Length - 1].transform.position)
            {
                print("done 1");
                isRotate = false;

                currentLerpTime = 0;
                num = 0;
                isOppenonetPlayerMoveCard = false;

                StartCoroutine(RemoveAfterSeconds(3));

                loadPanelCardButtonsSetActive(currentCardList, true);
                
            }
            else if (currentOpnontQuadObject.transform.position == opponenetWayPoints[num].transform.position)
            {
                print("Move to " + num);

                num++;

                currentLerpTime = 0;
                opponenetWayPoint = opponenetWayPoints[num];
                isRotate = true;

            }
        }
        
    }

    private void ResetQuad(GameObject quadObject, bool isOpponent)
    {
        quadObject.GetComponent<Renderer>().material = null;

        if (isOpponent)
        {

            Material yourMaterial = (Material)Resources.Load("images/Materials/back", typeof(Material));

            quadObject.GetComponent<Renderer>().material = yourMaterial;
        }

        String quadName = quadObject.name + "_Intial";
        GameObject initialQuad = GameObject.Find(quadName);

        quadObject.transform.position = initialQuad.transform.position;
        quadObject.transform.rotation = initialQuad.transform.rotation;

        quadObject.gameObject.SetActive(false);
    }


    private void ResetGameObjectsWithTag(bool isActive)
    {
        var fooGroup = Resources.FindObjectsOfTypeAll<GameObject>();// .FindObjectsOfTypeAll<PrimitiveType.Quad>();

        foreach (GameObject gameObject in fooGroup)
        {
            if (gameObject.tag == "myTag")
            {
                gameObject.SetActive(isActive);
            }
        }

    }
    private void ConnectToServer()
    {
        TcpClient client = new TcpClient("10.203.72.10", 1500);

        stream = client.GetStream();

        stream.ReadTimeout = 10;

        writer = new StreamWriter(stream);

        isConnected = true;

        print("Connectioned");

    }

    public void Button_Connect()
    {
        ConnectToServer();

        print("Connect Clicked");

        Message message = new Message((int)MessageType.CONNECTIONESTABLISH_CLIENTREQUEST, true, "usernasme send dddd", false, 0);

        message.ConnectionMessage = new ConnectionMessage("Player ", "Player ");

        writeMessage(message);
    }

    public void ClickReset()
    {
        print("Reset");

        Application.LoadLevel(Application.loadedLevel);

    }

    public void Trumph_Selction(string type)
    {
        print("trumph selected");

        Message message = new Message((int)MessageType.BIDDING_CLIENTRESPONSE, true, "usernasme send dddd", false, 0);

        message.BiddingMessage = new BiddingMessage(this.CurrentPlayerName, 0, type);

        writeMessage(message);
    }

    void readMessage()
    {

        try
        {
            byte[] bLen = new Byte[4];
            int data = stream.Read(bLen, 0, 4);
            if (data > 0)
            {
                int len = BitConverter.ToInt32(bLen, 0);

                //print("len = " + len);

                Byte[] buff = new byte[1024];

                data = stream.Read(buff, 0, len);

                if (data > 0)
                {
                    string result = Encoding.ASCII.GetString(buff, 0, data);

                    stream.Flush();

                    print(result);

                    Message message = JsonUtility.FromJson<Message>(result);

                    readOperation(message);
                }
            }
        }
        catch (Exception ex)
        {

        }

    }

    void writeMessage(Message message)
    {
        string json = JsonUtility.ToJson(message);

        writer.Write(json);

        writer.Flush();
    }

    void readOperation(Message message)
    {
        try
        {
            switch (message.getMessageType())
            {
                case MessageType.NONE:
                    print("None message type send form serer");
                    break;
                case MessageType.CONNECTIONESTABLISH_SERVERESPONSE:

                    if (message.IsError)
                    {
                        displayMessage(message.message);
                    }
                    else
                    {
                        setClient(message.ConnectionMessage);
                    }

                    break;
                case MessageType.CONNECTIONESTABLISH_SERVERESPONSE_OTHERPLAYERS:
                    setOtherPlayers(message.ConnectionMessage);
                    break;
                case MessageType.CONNECTIONESTABLISH_SERVERESPONSE_PREVIOUSPLAYERS:
                    setOtherPlayers(message.ConnectionMessage);
                    break;
                case MessageType.DEAL_CARDS_TO_CLIENT:
                    gameObjectShow(avatar);
                    loadCardsMessage(message.CardMessage);

                    ShowThinkBubble("wait a second I'll choose a trump!", false);

                    //displayMessage("Waiting for the other player choose the trump!");
                    break;
                case MessageType.BIDDING_SERVERREQUEST:
                    biddingMessage(message.BiddingMessage);
                    break;
                case MessageType.TRUMPH_SELECTION_RESPONSE:
                    loadselectedTrumph(message.BiddingMessage);
                    break;

                case MessageType.PLAYGAME_SERVERREQUEST:
                    playGameMessage(message.PlayGameMessage);
                    break;
                case MessageType.PLAYGAME_SERVERRESPONSE:
                    if (message.IsError)
                    {
                        playWrongCardPopup(message.PlayGameMessage);
                    }
                    else
                    {
                        setOtherPlayerCard(message.PlayGameMessage);
                    }
                    break;
                case MessageType.PLAYGAME_SERVERRESPONSE_PLAYER_WON_TRICK:
                    setTrickWonMessage(message.PlayGameMessage);
                    break;
                case MessageType.PLAYGAME_SERVERRESPONSE_TEAM_SCORE:
                    setTeamScoreMessage(message.GameStatMessage);
                    setMatchStatMessage(message.MatchStatMessage);
                    break;
                case MessageType.PLAYGAME_SERVERRESPONSE_TEAM_WON_GAME_WITH_DEAL_CARDS:
                    setMatchStatMessage(message.MatchStatMessage);
                    setDealAgainMessage();
                    clearMyBid();
                    break;
                case MessageType.PLAYGAME_SERVERRESPONSE_TEAM_WON_MATCH:
                    setMatchStatMessage(message.MatchStatMessage);
                    setMatchWonMessage(message.MatchWonMessage);
                    clearMyBid();
                    break;

            }

        }
        catch (IOException e)
        {
            print(e.Message);
        }
    }

    private void setMatchWonMessage(MatchWonMessage matchWonMessage)
    {
        throw new NotImplementedException();
    }

    private void clearMyBid()
    {
        destroyChilds(trumpSelectedPanel);
    }

    private void setDealAgainMessage()
    {
        destroyChilds(playerWonCardPanel);
        destroyChilds(opponentWonCardPanel);

        playerWonTrickCount = 0;
        playerWonTrickCountText.text = string.Empty;

        opponentWonTrickCount = 0;
        opponentWonTrickCountText.text = string.Empty;

    }

    private void setMatchStatMessage(string matchStatMessage)
    {
        resultText.text = matchStatMessage;

        ShowThinkBubble(matchStatMessage, true);
    }

    private void setTeamScoreMessage(GameStatMessage gameStatMessage)
    {
        if (gameStatMessage.playerName == CurrentPlayerName)
        {
            loadPanelCardButtonsSetActive(currentCardList, true);
        }
        else
        {
            loadPanelCardButtonsSetActive(currentCardList, false);
        }

    }

    private void loadselectedTrumph(BiddingMessage message)
    {
        HideThinkBubble();

        displayMessage("");

        string trumph = message.trumph;

        print("selected Trumph" + trumph);

        GameObject goButton = (GameObject)Instantiate(middlePrefabButton);

        goButton.transform.SetParent(trumpSelectedPanel, false);
        goButton.transform.localScale = new Vector3(.50f, .50f, .50f);
        goButton.transform.localPosition = new Vector3(0, 0, 0);

        Sprite sprite = getSprite("clubs");

        if (trumph.Equals("s"))
        {
            sprite = getSprite("spade");
        }
        else if (trumph.Equals("h"))
        {
            sprite = getSprite("hearts");
        }
        else if (trumph.Equals("d"))
        {
            sprite = getSprite("diamond");
        }

        // set image
        Image image = goButton.GetComponentInChildren<Image>();
        image.sprite = sprite;

        gameObjectShow(goButton);

        panelHide(trumphSelectingPanel);

        if (message.playerName == this.CurrentPlayerName)
        {
            loadPanelCardButtonsSetActive(currentCardList, true);

        }

    }

    int currentPlayerMessageCount = 0;
    int nextPlayerMessageCount = 0;

    private void setTrickWonMessage(PlayGameMessage playGameMessage)
    {

        if (playGameMessage.wonPlayerName == CurrentPlayerName)
        {
            if (currentPlayerMessageCount % 3 == 0)
                ShowThinkBubble("You won the trick!", true);
            else if (currentPlayerMessageCount % 3 == 1)
                ShowThinkBubble("You are lucky on this time!", true);
            else if (currentPlayerMessageCount % 3 == 2)
                ShowThinkBubble("Hey hey you get the advantage!", true);

            currentPlayerMessageCount++;

            loadWonCard(playerWonCardPanel);
            playerWonTrickCount++;
            playerWonTrickCountText.text = playerWonTrickCount.ToString();

            StartCoroutine(RemoveAfterSeconds(3));

            loadPanelCardButtonsSetActive(currentCardList, true);
        }
        else
        {
            if (nextPlayerMessageCount % 3 == 0)
                ShowThinkBubble("I'm lucky in this trick!", true);
            else if (nextPlayerMessageCount % 3 == 1)
                ShowThinkBubble("I am moving forward!", true);
            else if (nextPlayerMessageCount % 3 == 2)
                ShowThinkBubble("I am making an advantage!", true);

            nextPlayerMessageCount++;

            loadWonCard(opponentWonCardPanel);
            opponentWonTrickCount++;
            opponentWonTrickCountText.text = opponentWonTrickCount.ToString();


            loadPanelCardButtonsSetActive(currentCardList, false);
        }

        ResetQuad(currentQuadObject, false);
        ResetQuad(currentOpnontQuadObject, true);
    }

    int oponentCardCount = 1;

    private void setOtherPlayerCard(PlayGameMessage playGameMessage)
    {
        if (playGameMessage.playerName != CurrentPlayerName)
        {
            int cardCount = currentCardList.Count - 1;

            String quadName = "OQuad_" + oponentCardCount;

            GameObject myQuad = GameObject.Find(quadName);

            Material yourMaterial = (Material)Resources.Load("images/Materials/" + playGameMessage.card, typeof(Material));

            myQuad.GetComponent<Renderer>().material = yourMaterial;

            currentOpnontQuadObject = myQuad;

            isOppenonetPlayerMoveCard = true;

            opponenetWayPoint = opponenetWayPoints[0];

            oponentCardCount++;


        }

    }



    private void playGameMessage(PlayGameMessage playGameMessage)
    {
    }

    private void loadCardsMessage(CardMessage cardMessage)
    {
        oponentCardCount = 1;

        ResetGameObjectsWithTag(true);

        loadPanelCardButtons(cardMessage.initialSetOfCards);

        loadOpponentPanelCardButtons(currentCardList.Count);
    }

    private void biddingMessage(BiddingMessage biddingMessage)
    {
        print("bidding request received");

        if (biddingMessage.playerName == CurrentPlayerName)
        {
            ShowThinkBubble("Hey you need to select a trump!", false);

            //displayMessage("Choose a trump!");

            panelShow(trumphSelectingPanel);
        }
    }

    private void setOtherPlayers(ConnectionMessage connectionMessage)
    {
        displayMessage("");
    }

    private void setClient(ConnectionMessage connectionMessage)
    {
        print("set client called");

        connectButton.enabled = false;
        connectButton.gameObject.SetActive(false);

        this.CurrentPlayerName = connectionMessage.playerName;

        playerNameText.text = connectionMessage.playerName;

        print(connectionMessage.username + " player " + connectionMessage.playerName);

    }

    private void displayMessage(string message)
    {
        mainMessage.text = message;
    }

    void loadPanelCardButtons(List<String> cards)
    {

        currentCardList = new List<string>();
        currentCardList = cards;

        int i = 1;

        foreach (String card in cards)
        {
            String buttonName = "Button_" + i;

            Button button = GameObject.Find(buttonName).GetComponent<UnityEngine.UI.Button>();

            button.interactable = false;

            button.GetComponent<Image>().sprite = getSprite(card);

            String quadName = "Quad_" + i;

            GameObject myQuad = GameObject.Find(quadName);

            Material yourMaterial = (Material)Resources.Load("images/Materials/" + card, typeof(Material));

            myQuad.GetComponent<Renderer>().material = yourMaterial;

            i++;
        }

        playerCardPanel.gameObject.SetActive(true);
    }

    void loadPanelCardButtonsSetActive(List<String> cards, bool isInteractable)
    {
        for (int i = 1; i <= 8; i++)
        {
            String buttonName = "Button_" + i;

            if (GameObject.Find(buttonName) != null)
            {
                Button button = GameObject.Find(buttonName).GetComponent<UnityEngine.UI.Button>();

                button.interactable = isInteractable;
            }

        }


        playerCardPanel.gameObject.SetActive(true);
    }

    private void loadOpponentPanelCardButtons(int cardCount)
    {
        for (int i = 1; i <= 8; i++)
        {
            String quadName = "OQuad_" + i;

            GameObject myQuad = GameObject.Find(quadName);

            if (i > cardCount)
            {
                myQuad.gameObject.SetActive(false);

            }

        }

    }

    private void removeSelectedCardFromList(string card)
    {
        currentCardList.Remove(card);

        loadPanelCardButtonsSetActive(currentCardList, false);
    }

    void loadSelectedCardToPanel(string card, GameObject panel)
    {
        destroyChilds(panel);

        GameObject goButton = new GameObject(card);

        goButton.AddComponent<Image>();

        Image image = goButton.GetComponentInChildren<Image>();

        image.sprite = getSprite(card);

        image.rectTransform.sizeDelta = new Vector2(0.02f, .03f);

        gameObjectShow(goButton);

        StartCoroutine(Move(goButton, panel.transform.position));

        goButton.transform.parent = panel.transform;
        goButton.transform.position = panel.transform.position;
    }

    private void loadWonCard(RectTransform panel)
    {
        GameObject goButton = (GameObject)Instantiate(smallPrefabButton);

        goButton.transform.SetParent(panel, false);
        goButton.transform.localScale = new Vector3(.25f, .25f, 1);
        goButton.transform.localPosition = new Vector3(0, 0, 0);

        Image image = goButton.GetComponentInChildren<Image>();

        image.sprite = getSprite("back");

        gameObjectShow(goButton);
    }

    private Sprite getSprite(string card)
    {
        return (Sprite)Resources.Load<Sprite>("sprites/" + card) as Sprite;
    }

    private void gameObjectShow(GameObject gameobjet)
    {
        gameobjet.SetActive(true);
    }

    private void gameObjectHide(GameObject gameobjet)
    {
        gameobjet.SetActive(false);

    }

    private void panelHide(RectTransform panel)
    {
        panel.gameObject.SetActive(false);

    }

    private void panelShow(RectTransform panel)
    {
        panel.gameObject.SetActive(true);
    }

    private void destroyChilds(GameObject panel)
    {
        foreach (Transform child in panel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void destroyChilds(RectTransform panel)
    {
        foreach (Transform child in panel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private IEnumerator Move(GameObject obj, Vector3 position)
    {
        StartCoroutine(Move(obj, obj.gameObject.transform.localPosition, position, getSpeed(6)));

        waitForSeconds(6f);

        yield return new WaitForSeconds(getSpeed(10));
    }

    /// <summary>
    /// Moves the specified object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="sPos">The s position.</param>
    /// <param name="ePos">The e position.</param>
    /// <param name="time">The time.</param>
    /// <returns></returns>
    private IEnumerator Move(GameObject obj, Vector3 sPos, Vector3 ePos, float time)
    {
        float i = 0.0f;

        float rate = (float)1.0 / time;

        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;

            if (obj != null)
            {
                obj.transform.localPosition = Vector3.Lerp(sPos, ePos, i);
            }

            yield return true;
        }
    }

    private float speedFactor = 0.2f;

    private float getSpeed(int i)
    {
        return i * speedFactor;
    }

    private IEnumerator waitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    private IEnumerator waitForSeconds(float seconds, Message message)
    {
        print("Wait");

        yield return new WaitForSeconds(seconds);

        writeMessage(message);

        print("Finish");
    }

    private void ShowThinkBubble(String message, bool isWaitForSeconds)
    {
        ThinkBubble.SetActive(true);

        thinkBubbleText.text = message;

        if (isWaitForSeconds)
        {
            StartCoroutine(RemoveAfterSeconds(2));
        }
    }

    IEnumerator RemoveAfterSeconds(int seconds)
    {
        yield return new WaitForSeconds(seconds);

        HideThinkBubble();
    }

    private void HideThinkBubble()
    {
        ThinkBubble.SetActive(false);
    }

    private Button currentButton;

    public void CardSelectButton2_OnClick(int id)
    {
        String buttonName = "Button_" + id;
        Button button = GameObject.Find(buttonName).GetComponent<UnityEngine.UI.Button>();

        string spriteName = button.GetComponent<Image>().sprite.name;

        string card = spriteName;

        if (spriteName.Contains("_"))
        {
            card = spriteName.Substring(0, spriteName.Length - 2);
        }

        currentButton = button;

        button.gameObject.SetActive(false);

        String quadName = "Quad_" + id;
        GameObject myQuad = GameObject.Find(quadName);
        currentQuadObject = myQuad;

        wayPoint = wayPoints[num];

        isPlayerMoveCard = true;

        removeSelectedCardFromList(card);

        Message message = new Message((int)MessageType.PLAYGAME_CLIENTRESPONSE, true, this.CurrentPlayerName + " selected card : " + card, false, 0);

        message.PlayGameMessage = (new PlayGameMessage(this.CurrentPlayerName, card));

        writeMessage(message);
    }

    private void playWrongCardPopup(PlayGameMessage playGameMessage)
    {
        if (playGameMessage.playerName == CurrentPlayerName)
        {
            audioSource.clip = wrongButtonClickAudio;
            audioSource.Play();

            currentCardList.Add(lastSelectedCard);

            currentButton.gameObject.SetActive(true);

            wayPoint = null;

            isPlayerMoveCard = false;

            ResetQuad(currentQuadObject, false);

            currentQuadObject = null;

            ShowThinkBubble("Dont play wrong card!", true);

            loadPanelCardButtonsSetActive(currentCardList, true);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Client2 : MonoBehaviour
{
    public GameObject quad1;

    public Canvas tableCanvas;
    public GameObject ThinkBubble;
    public Text thinkBubbleText;

    public GameObject Player1TopPosition;
    public GameObject Player2TopPosition;

    public GameObject Player1BottomPosition;
    public GameObject Player2BottomPosition;

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

    public GameObject prefabButton;
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

    float starttime;
    float totalDistance;

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
        starttime = Time.time;
        totalDistance = Vector3.Distance(Player1TopPosition.transform.position, Player1BottomPosition.transform.position);

        List<string> list = new List<string>();

        list.Add("c9");
        list.Add("c7");
        list.Add("c14");

        list.Add("c13");
        list.Add("d12");
        list.Add("d8");

        list.Add("d13");
        list.Add("d10");


        loadPanelCardButtons(list, true);

        //loadCardToPanel("c8", opponentSelectedCardPanel);

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
    }

    public GameObject[] wayPoints;

    private GameObject wayPoint;

    bool isRotate = false;

    private GameObject currentQuadObject;

    private float lerpTime = 10;

    private float currentLerpTime = 0;
    
    private int num = 0;

    private bool go = false;

    void Update()
    {
        if (isConnected && stream.CanRead)
        {
            readMessage();
        }


        if (go)
        {
            currentLerpTime += Time.deltaTime;

            if (currentLerpTime >= lerpTime)
            {
                currentLerpTime = lerpTime;
            }

            float prec = currentLerpTime / lerpTime;

            if (isRotate)
            {
                currentQuadObject.transform.Rotate(new Vector3(Time.deltaTime * 52, 0, 0));
            }

            currentQuadObject.transform.position = Vector3.Lerp(currentQuadObject.transform.localPosition, wayPoint.transform.localPosition, prec);

            if (currentQuadObject.transform.position == wayPoints[wayPoints.Length - 1].transform.position)
            {
                print("done 1");
                isRotate = false;

                currentLerpTime = 0;
                num = 0;
                go = false;
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

        //GameObject firstPart = GameObject.Find("c8");
        //GameObject secondPart = GameObject.Find("d13");
        //////firstPart.transform.parent = null;
        //////secondPart.transform.parent = null;

        ////firstPart.transform.position = new Vector3(2, 0, 0);

        //StartCoroutine(Move(firstPart, new Vector3(2, firstPart.transform.position.y, firstPart.transform.position.z)));

        ////firstPart.transform.parent = selectedCardPanel.transform;

        //firstPart.transform.position = Vector3.Lerp(firstPart.transform.position, selectedCardPanel.transform.position, speed * Time.deltaTime);

        // secondPart.transform.position = Vector3.Lerp(secondPart.transform.position, selectedCardPanel.transform.position, speed * Time.deltaTime);

        //firstPart.transform.position = selectedCardPanel.transform.localPosition;// new Vector3(100, 100, 100);

        //if (firstPart.transform.position != selectedCardPanel.transform.position)
        //{
        //    Vector3 pos = Vector3.MoveTowards(firstPart.transform.position, selectedCardPanel.transform.position, speed * Time.deltaTime);

        //    firstPart.GetComponent<Rigidbody>().MovePosition(pos);

        //}

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
                    loadCardsMessage(message.CardMessage);
                    displayMessage("Waiting for the other player choose the trump!");
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
    }

    private void setTeamScoreMessage(GameStatMessage gameStatMessage)
    {
        if (gameStatMessage.playerName == CurrentPlayerName)
        {
            loadPanelCardButtons(currentCardList, true);
        }
        else
        {
            loadPanelCardButtons(currentCardList, false);
        }

    }

    private void loadselectedTrumph(BiddingMessage message)
    {
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
            loadPanelCardButtons(currentCardList, true);

        }

    }

    private void setTrickWonMessage(PlayGameMessage playGameMessage)
    {
        destroyChilds(Player1TopPosition);

        destroyChilds(Player2TopPosition);

        destroyChilds(Player1BottomPosition);

        destroyChilds(Player2BottomPosition);

        if (playGameMessage.wonPlayerName == CurrentPlayerName)
        {
            ShowThinkBubble("I m lucky in this trick!");

            loadWonCard(playerWonCardPanel);
            playerWonTrickCount++;
            playerWonTrickCountText.text = playerWonTrickCount.ToString();

            loadPanelCardButtons(currentCardList, true);
        }
        else
        {
            ShowThinkBubble("You won the trick!");

            loadWonCard(opponentWonCardPanel);
            opponentWonTrickCount++;
            opponentWonTrickCountText.text = opponentWonTrickCount.ToString();

            loadPanelCardButtons(currentCardList, false);
        }


    }

    private void setOtherPlayerCard(PlayGameMessage playGameMessage)
    {
        if (playGameMessage.playerName != CurrentPlayerName)
        {
            loadOpponentPanelCardButtons(currentCardList.Count - 1);

            if (playGameMessage.playerName.ToLower() == player1name)
            {
                loadSelectedCardToPanel(playGameMessage.card, Player1BottomPosition);
            }
            else
            {
                loadSelectedCardToPanel(playGameMessage.card, Player2BottomPosition);
            }

            loadPanelCardButtons(currentCardList, true);
        }

    }

    private void playWrongCardPopup(PlayGameMessage playGameMessage)
    {
        if (playGameMessage.playerName == CurrentPlayerName)
        {
            audioSource.clip = wrongButtonClickAudio;
            audioSource.Play();

            currentCardList.Add(lastSelectedCard);

            if (this.CurrentPlayerName.ToLower() == player1name)
            {
                destroyChilds(Player1TopPosition);
            }
            else
            {
                destroyChilds(Player2TopPosition);
            }

            loadPanelCardButtons(currentCardList, true);
        }
    }

    private void playGameMessage(PlayGameMessage playGameMessage)
    {

        //if (playGameMessage.playerName == CurrentPlayerName)
        //{
        //    playerCardPanel.gameObject.SetActive(true);
        //}
        //else
        //{
        //    playerCardPanel.gameObject.SetActive(false);
        //}
    }

    private void loadCardsMessage(CardMessage cardMessage)
    {
        loadPanelCardButtons(cardMessage.initialSetOfCards, false);

        loadOpponentPanelCardButtons(currentCardList.Count);
    }

    private void biddingMessage(BiddingMessage biddingMessage)
    {
        print("bidding request received");

        if (biddingMessage.playerName == CurrentPlayerName)
        {
            displayMessage("Choose a trump!");

            panelShow(trumphSelectingPanel);
        }
    }

    private void setOtherPlayers(ConnectionMessage connectionMessage)
    {
        gameObjectShow(avatar);

        displayMessage("");
    }

    private void setClient(ConnectionMessage connectionMessage)
    {
        print("set client called");

        connectButton.enabled = false;
        connectButton.gameObject.SetActive(false);

        // usernameText.text = connectionMessage.playerName;

        //this.CurrentUsername = connectionMessage.username;

        this.CurrentPlayerName = connectionMessage.playerName;

        playerNameText.text = connectionMessage.playerName;

        print(connectionMessage.username + " player " + connectionMessage.playerName);

        //if (connectionMessage.isPreviousUser)
        // {
        gameObjectShow(avatar);
        // }
    }

    private void displayMessage(string message)
    {
        mainMessage.text = message;
    }

    void loadPanelCardButtons(List<String> cards, bool isInteractable)
    {
        currentCardList = new List<string>();
        currentCardList = cards;

        int i = 1;

        foreach (String card in cards)
        {
            String buttonName = "Button_" + i;

            Button button = GameObject.Find(buttonName).GetComponent<UnityEngine.UI.Button>();

            button.GetComponent<Image>().sprite = getSprite(card);

            button.tag = card;

            String quadName = "Quad_" + i;

            GameObject myQuad = GameObject.Find(quadName);

            Material yourMaterial = (Material)Resources.Load("images/Materials/" + card, typeof(Material));

            myQuad.GetComponent<Renderer>().material = yourMaterial;

            i++;
        }

        //destroyChilds(playerCardPanel);

        // panelHide(cardPanel);

        //int i = 0;

        //foreach (String card in cards)
        //{

        //    float corrdicate = i * 3;

        //    GameObject goButton = (GameObject)Instantiate(tooSmallPrefabButton);

        //    goButton.name = card;

        //    //goButton.transform.SetParent(playerCardPanel, isInteractable);

        //    goButton.transform.SetParent(playerCardPanel.transform);

        //    //goButton.transform.localScale = new Vector3(.50f, .50f, .50f);
        //    goButton.transform.localPosition = new Vector3(corrdicate, 0, 0);


        //    // set button click
        //    Button tempButton = goButton.GetComponentInChildren<Button>();
        //    tempButton.onClick.AddListener(() => CardSelectButton_OnClick(card));
        //    tempButton.interactable = isInteractable;
        //    //tempButton.interactable = isInteractable;

        //    Image image = goButton.GetComponentInChildren<Image>();

        //    image.sprite = getSprite(card);

        //    gameObjectShow(goButton);

        //    i++;
        //}

        playerCardPanel.gameObject.SetActive(true);
    }

    private void loadOpponentPanelCardButtons(int cardCount)
    {
        destroyChilds(opponentCardPanel);

        opponentCardPanel.gameObject.SetActive(false);

        for (int i = 0; i < cardCount; i++)
        {
            int corrdinate = i * 3;

            GameObject goButton = (GameObject)Instantiate(middlePrefabButton);

            goButton.transform.SetParent(opponentCardPanel, false);
            //goButton.transform.localScale = new Vector3(1, .50f, .50f);
            goButton.transform.localPosition = new Vector3(corrdinate, 0, 0);

            // set image
            Image image = goButton.GetComponentInChildren<Image>();

            image.sprite = getSprite("back");

            gameObjectShow(goButton);

        }

        opponentCardPanel.gameObject.SetActive(true);
    }

    void CardSelectButton_OnClick(string card)
    {
        Debug.Log("Button clicked = " + card);

        lastSelectedCard = card;

        //loadCardToPanel(card, playerSelectedCardPanel);

        //GameObject goButton = (GameObject)Instantiate(tooSmallPrefabButton);

        GameObject goButton = new GameObject(card);

        goButton.AddComponent<Image>();

        Image image = goButton.GetComponentInChildren<Image>();

        image.sprite = getSprite(card);

        image.rectTransform.sizeDelta = new Vector2(0.02f, .03f);

        gameObjectShow(goButton);

        //       goButton.transform.position = Vector3.Lerp(goButton.transform.position, Player1TopPosition.transform.position, .5f);
        //       goButton.transform.parent = Player1TopPosition.transform;
        //goButton.transform.position = Player1TopPosition.transform.position;
        //       StartCoroutine(waitForSeconds(5f));
        //       goButton.transform.position = Vector3.Lerp(Player1TopPosition.transform.position, Player1BottomPosition.transform.position, .5f);

        //       goButton.transform.parent = Player1BottomPosition.transform;
        //        goButton.transform.position = Player1BottomPosition.transform.position;


        if (this.CurrentPlayerName.ToLower() == player1name)
        {
            StartCoroutine(Move(goButton, Player1TopPosition.transform.position));
            goButton.transform.parent = Player1TopPosition.transform;


            goButton.transform.position = Player1TopPosition.transform.position;

            //StartCoroutine(waitForSeconds(5f));

            //goButton.transform.parent = null;

            //goButton.transform.Rotate(-90f, 0f, 0f);

            //StartCoroutine(Move(goButton, Player1BottomPosition.transform.position));

            //goButton.transform.parent = Player1BottomPosition.transform;
            //goButton.transform.position = Player1BottomPosition.transform.position;

        }
        //else
        //{
        //    StartCoroutine(Move(goButton, Player2TopPosition.transform.position));

        //    goButton.transform.parent = Player2TopPosition.transform;
        //    goButton.transform.position = Player2TopPosition.transform.position;

        //    goButton.transform.parent = null;

        //    goButton.transform.Rotate(-90f, 0f, 0f);

        //    StartCoroutine(Move(goButton, Player2BottomPosition.transform.position));

        //    goButton.transform.parent = Player2BottomPosition.transform;
        //    goButton.transform.position = Player2BottomPosition.transform.position;
        //}

        removeSelectedCardFromList(card);

        Message message = new Message((int)MessageType.PLAYGAME_CLIENTRESPONSE, true, this.CurrentPlayerName + " selected card : " + card, false, 0);

        message.PlayGameMessage = (new PlayGameMessage(this.CurrentPlayerName, card));

        //StartCoroutine(waitForSeconds(2f, message));
    }

    private void removeSelectedCardFromList(string card)
    {
        currentCardList.Remove(card);

        loadPanelCardButtons(currentCardList, true);
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

    void loadCardToPanel(string card, RectTransform panel)
    {
        destroyChilds(panel);

        GameObject goButton = (GameObject)Instantiate(smallPrefabButton);

        goButton.name = card;
        goButton.transform.SetParent(panel, false);
        goButton.transform.localScale = new Vector3(.25f, .25f, 1);
        goButton.transform.localPosition = new Vector3(0, 0, 0);

        Image image = goButton.GetComponentInChildren<Image>();

        image.sprite = getSprite(card);

        gameObjectShow(goButton);
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
        StartCoroutine(Move(obj, obj.gameObject.transform.localPosition, position, getSpeed(10)));

        waitForSeconds(10f);

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

            obj.transform.localPosition = Vector3.Lerp(sPos, ePos, i);


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
        print("waitForSeconds : Wait");
        yield return new WaitForSeconds(seconds);

        print("waitForSeconds :  Finish");
    }

    private IEnumerator waitForSeconds(float seconds, Message message)
    {
        print("Wait");

        yield return new WaitForSeconds(seconds);

        writeMessage(message);

        print("Finish");
    }

    private void ShowThinkBubble(String message)
    {
        ThinkBubble.SetActive(true);

        thinkBubbleText.text = message;

        StartCoroutine(RemoveAfterSeconds(2));

    }

    IEnumerator RemoveAfterSeconds(int seconds)
    {
        yield return new WaitForSeconds(seconds);

        ThinkBubble.SetActive(false);
    }
    
    public void CardSelectButton2_OnClick(int id)
    {
        String buttonName = "Button_" + id;
        Button button = GameObject.Find(buttonName).GetComponent<UnityEngine.UI.Button>();
        button.gameObject.SetActive(false);
        print(button.tag);

        String quadName = "Quad_" + id;
        GameObject myQuad = GameObject.Find(quadName);
        currentQuadObject = myQuad;

        num = 0;

        wayPoint = wayPoints[num];

        go = true;

        Message message = new Message((int)MessageType.PLAYGAME_CLIENTRESPONSE, true, this.CurrentPlayerName + " selected card : " + "c2", false, 0);

        message.PlayGameMessage = (new PlayGameMessage(this.CurrentPlayerName, "c2"));

        //StartCoroutine(waitForSeconds(2f, message));

    }
    




}

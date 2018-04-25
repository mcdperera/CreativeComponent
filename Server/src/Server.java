
import com.google.gson.Gson;
import java.io.*;
import java.net.*;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.CharBuffer;
import java.nio.charset.CharacterCodingException;
import java.nio.charset.Charset;
import java.nio.charset.CharsetDecoder;
import java.nio.charset.CharsetEncoder;
import java.text.SimpleDateFormat;
import java.util.*;
import java.util.concurrent.TimeUnit;
import java.util.logging.Level;
import java.util.logging.Logger;

/*
 * The server that can be run both as a console application or a GUI
 */
/**
 *
 * @author Charmal remarks
 * http://ferozedaud.blogspot.com/2009/11/howto-serialize-data-from-object-from.html
 * https://stackoverflow.com/questions/14824491/can-i-communicate-between-java-and-c-sharp-using-just-sockets
 */
public class Server {

    List<String> playerNames = Arrays.asList("Player_1, Player_2".toLowerCase().split(","));

    static int maximumNumberOfClients = 2;

    private static int uniqueId = -1;

    private static int sendBiddingRequestCount = 0;

    private static int biddingClientIndex = 0;

    private static int playClientIndex = 0;

    private static int trickPlayCount = 0;

    private static int roundTrickPlayCount = 0;

    private final ArrayList<ClientThread> Clients;

    private final ArrayList<MatchStat> MatchStatList;

    private final SimpleDateFormat simpleDateFormat;

    private final int port;

    // When stoping the server needs to change.
    private boolean keepGoing;

    private String SelectedTrickCardSuit;

    private static final String redTeamName = "Red";

    private static final String blueTeamName = "Blue";

    private static final int winScore = 250;

    private static final int maxRoundTrickCount = 8;

    private MatchStat CurrentMatchStat;

    private int maxPlayClientIndex = 1;

    private int maxNumberOfCardsPerPlayer = 8;

    private String selectedTrumph = "";

    /**
     * Desired port to connect with the client.
     *
     * @param port
     */
    public Server(int port) {

        this.port = port;

        simpleDateFormat = new SimpleDateFormat("HH:mm:ss");

        Clients = new ArrayList<>();

        MatchStatList = new ArrayList<>();
    }

    /**
     * Server going to start.
     *
     * @throws java.lang.InterruptedException
     */
    public void start() throws InterruptedException {
        keepGoing = true;
        /* create socket server and wait for connection requests */
        try {

            // the socket used by the server
            ServerSocket serverSocket = new ServerSocket(port);

            // infinite loop to wait for connections
            while (keepGoing) {

                display("Server waiting for Clients on port " + port + ".");

                Socket socket = serverSocket.accept(); // accept connection

                // If someting went wrong and keepGoing is flase 
                // Server needs to stop.
                if (!keepGoing) {
                    break;
                }

                ClientThread t = new ClientThread(socket);  // make a thread of it

                Clients.add(t);

                t.start();

                if (maximumNumberOfClients == Clients.size()) {
                    dealCards();

                    biddingClientIndex = 0;

                    sendBiddingRequestMessage();
                }

            }

            // I was asked to stop
            try {
                serverSocket.close();

                for (int i = 0; i < Clients.size(); ++i) {
                    ClientThread tc = Clients.get(i);
                    try {
                        tc.inputStream.close();
                        tc.outputStream.close();
                        tc.socket.close();
                    } catch (IOException ioE) {
                        display("Exception generated: " + ioE);
                    }
                }

            } catch (Exception e) {
                display("Exception closing the server and clients: " + e);
            }
        } catch (IOException e) {
            String msg = simpleDateFormat.format(new Date()) + " Exception on new ServerSocket: " + e + "\n";
            display(msg);
        }
    }

    private void display(String message) {
        System.out.println(
                simpleDateFormat.format(new Date()) + " : " + message);
    }

    private synchronized void boradastToAll(Message message) {

        for (int i = Clients.size(); --i >= 0;) {
            ClientThread ct = Clients.get(i);
            ct.writeMessage(message);
        }
    }

    private synchronized void boradastToOtherClients(ClientThread client, Message message) {

        if (Clients.size() > 0) {
            display("Sends other player details to '" + client.username + "' user");
        } else {
            display("No other players at this time.");
        }

        for (int i = Clients.size(); --i >= 0;) {
            ClientThread ct = Clients.get(i);
            ct.writeMessage(message);
        }
    }

    synchronized void remove(int id) {
        for (int i = 0; i < Clients.size(); ++i) {
            ClientThread ct = Clients.get(i);
            if (ct.id == id) {
                Clients.remove(i);
                return;
            }
        }
    }

    /**
     *
     * @param args
     * @throws java.lang.InterruptedException
     */
    public static void main(String[] args) throws InterruptedException {
        Server server = new Server(1500);
        server.start();
    }

    /**
     *
     * @param username
     * @return
     */
    public boolean isUsernameUnique(String username) {

        for (int i = 0; i < Clients.size(); ++i) {
            ClientThread ct = Clients.get(i);
            if (ct.username.equalsIgnoreCase(username)) {
                return false;
            }
        }
        return true;
    }

    private synchronized void dealCards() {

        PackOfCard packOfCards = new PackOfCard();

        for (int i = Clients.size(); --i >= 0;) {
            ClientThread ct = Clients.get(i);

            String msg = "Server sends set of cards to'" + ct.username;

            Message cardMessage = new Message(
                    MessageType.DEAL_CARDS_TO_CLIENT.getValue(), false, msg,
                    false, ErrorMessageType.NONE.getValue());

            ArrayList<String> dealCards = packOfCards.getSetofCards(maxNumberOfCardsPerPlayer);

            cardMessage.CardMessage = (new CardMessage(ct.playername, dealCards));
            ct.setsOfCards = dealCards;

            ct.writeMessage(cardMessage);
        }

    }

    private void sendBiddingRequestMessage() {

        ClientThread ct = Clients.get(biddingClientIndex);

        String msg = "'" + ct.username + "' added his bid";

        Message dealMessage = new Message(
                MessageType.BIDDING_SERVERREQUEST.getValue(), false, msg,
                false, ErrorMessageType.NONE.getValue());

        dealMessage.BiddingMessage = (new BiddingMessage(ct.playername, 0, ""));

        ct.writeMessage(dealMessage);

        if (biddingClientIndex == maxPlayClientIndex) {
            biddingClientIndex = 0;
        } else {
            biddingClientIndex++;
        }

        sendBiddingRequestCount++;
    }

    private void playTrick() {

        trickPlayCount++;

        ClientThread ct = Clients.get(playClientIndex);

        String msg = "'" + ct.username + "' needs to draw his/her card. ";

        Message PlayGameMessage = new Message(
                MessageType.PLAYGAME_SERVERREQUEST.getValue(), false, msg,
                false, ErrorMessageType.NONE.getValue());

        PlayGameMessage.PlayGameMessage = new PlayGameMessage(ct.playername);
        ct.writeMessage(PlayGameMessage);

        if (playClientIndex == maxPlayClientIndex) {
            playClientIndex = 0;
        } else {
            playClientIndex++;
        }
    }

    /**
     * One instance of this thread will run for each client
     */
    class ClientThread extends Thread {

        Socket socket;

        //ObjectInputStream inputStream;
        //ObjectOutputStream outputStream;
        OutputStream outputStream;
        InputStream inputStream;

        // unique id
        int id;

        // the Username of the Client
        String username;

        // the only type of message a will receive
        Message message;

        String team;

        // the date I connect
        String date;

        //
        String playername;

        Integer biddingAmount;

        String selectedCard;

        Integer wonTrickCouont = 0;

        private ArrayList<String> setsOfCards;

        // Constructore
        ClientThread(Socket socket) {

            // a unique id
            id = ++uniqueId;

            this.socket = socket;

            this.playername = playerNames.get(id);

            try {

                outputStream = socket.getOutputStream();
                inputStream = socket.getInputStream();

//                // read the username
                Message message = (Message) readMessage(inputStream);// inputStream.readObject();
                display(message.getMessage());

                String userName = message.ConnectionMessage.getUsername();

//                message = new Message(0, true, "From Java", keepGoing, 0);//
//                writeMsg(message);
                // check wether the username is unique among the players
                //if (isUsernameUnique(userName)) {
                this.team = id % 2 == 0 ? blueTeamName : redTeamName;

                String localMessage = "User '" + userName + "' just connected.";

                display(localMessage);

                sendMessageaboutPreiousUsers();

                this.username = userName;

                Message returnMessage = new Message(
                        MessageType.CONNECTIONESTABLISH_SERVERESPONSE.getValue(),
                        false, localMessage, false, ErrorMessageType.NONE.getValue());

                returnMessage.ConnectionMessage = (new ConnectionMessage(this.playername, this.username));
                writeMessage(returnMessage);

                Message returnMessageForOthers = new Message(
                        MessageType.CONNECTIONESTABLISH_SERVERESPONSE_OTHERPLAYERS.getValue(),
                        false, localMessage + "as other user",
                        false, ErrorMessageType.NONE.getValue());

                returnMessageForOthers.ConnectionMessage = (new ConnectionMessage(this.playername, this.username));
                boradastToOtherClients(this, returnMessageForOthers);

            } catch (IOException e) {
                display("Exception creating new Input/output Streams: " + e);
                return;
            }

            date = new Date().toString() + "\n";
        }

        private Message readMessage(InputStream inputStream) throws CharacterCodingException, IOException {

            Message message = null;

            byte[] bytes = new byte[1024];
            try {
                int data = inputStream.read(bytes);
                if (data != -1) {
                    String json = new String(bytes, 0, data);
                    System.out.println(json);
                    message = new Gson().fromJson(json, Message.class);

                    System.out.println("Message " + message.message);

                    return message;
                }
            } catch (IOException e) {
                e.printStackTrace();
            }

            return message;

        }

        private void writeMessage(Message msg) {
            try {
                String json = new Gson().toJson(msg);
                System.out.println(json);
                byte[] bytes = json.getBytes();
                byte[] bytesSize = Util.intToByteArray(json.length());
                outputStream.write(bytesSize, 0, 4);
                outputStream.write(bytes, 0, bytes.length);
                outputStream.flush();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }

        private boolean writeMsg(Message msg, OutputStream oStream) throws IOException {
            // if Client is still connected send the message to it

            byte[] underlyingBuffer = new byte[1024];
            ByteBuffer buffer = ByteBuffer.wrap(underlyingBuffer);
            buffer.order(ByteOrder.LITTLE_ENDIAN);
            Charset charset = Charset.forName("UTF-8");
            CharsetEncoder encoder = charset.newEncoder();
            String employeeStr = new Gson().toJson(msg);
            CharBuffer nameBuffer = CharBuffer.wrap(employeeStr.toCharArray());
            ByteBuffer nbBuffer = null;
            try {
                nbBuffer = encoder.encode(nameBuffer);
            } catch (CharacterCodingException e) {
                throw new ArithmeticException();
            }
            //System.out.println(String.format("String [%1$s] #bytes = %2$s", employeeStr, nbBuffer.limit()));
            buffer.putInt(nbBuffer.limit());
            buffer.put(nbBuffer);

            buffer.flip();

            int dataToSend = buffer.remaining();
//   System.out.println("# bytes = " + dataToSend);
//   
//   System.out.println("#Bytes in output buffer: " + written + " limit = " + buffer.limit() + " pos = " + buffer.position() + " remaining = " + buffer.remaining());
//   
            int remaining = dataToSend;
            while (remaining > 0) {
                oStream.write(buffer.get());
                --remaining;
            }

            return true;
        }

        // what will run forever
        public void run() {

            boolean keepGoing = true;

            while (keepGoing) {

                try {
                    message = (Message) readMessage(inputStream);//inputStream.readObject();

                    if (message == null) {
                        return;
                    }
                } catch (IOException e) {
                    display(username + " Exception reading Streams: " + e);
                    break;
                }

                switch (this.message.getMessageType()) {
                    case NONE:
                        message = new Message(0, true, "From Java", keepGoing, 0);//
                        writeMessage(message);
                        break;

                    case BIDDING_CLIENTRESPONSE:

                        BiddingMessage biddingMessage = this.message.BiddingMessage;

                        if (sendBiddingRequestCount == 1) {

                            sendBiddingRequestCount = 0;
                            biddingClientIndex = -1;

                            // Start the trick after the bidding approved.
                            trickPlayCount = 0;

                            selectedTrumph = biddingMessage.getTrumph();

                            sendTrumphSelectedMessage(biddingMessage.getPlayerName());

                            playTrick();

                            break;
                        } else {
                            sendBiddingRequestMessage();
                        }
                        break;

                    case PLAYGAME_CLIENTRESPONSE:

                        PlayGameMessage playMessage = this.message.PlayGameMessage;
                        String cardSelectedPlayername = playMessage.getPlayerName();
                        String card = playMessage.getCard();
                        String suit = card.substring(0, 1);

                        if (CurrentMatchStat == null) {

                            ClientThread client = getClient(cardSelectedPlayername);

                            CurrentMatchStat = new MatchStat(client.id,
                                    MatchStatList.size() + 1, 0, 0);
                        }

                        if (trickPlayCount == 1) {
                            SelectedTrickCardSuit = suit;
                        }

                        if (!SelectedTrickCardSuit.equalsIgnoreCase(suit)
                                && checkAnyIlleaglePlay(cardSelectedPlayername, SelectedTrickCardSuit)) {

                            String usernameNotUniqueMessage = "User '" + username + "' play .";

                            display(usernameNotUniqueMessage);

                            Message returnMessage = new Message(
                                    MessageType.PLAYGAME_SERVERRESPONSE.getValue(),
                                    false, usernameNotUniqueMessage, true, ErrorMessageType.PLAY_CHEATCARD.getValue());

                            returnMessage.PlayGameMessage = (new PlayGameMessage(this.playername, card));
                            writeMessage(returnMessage);

                        } else {

                            display(this.message.getMessage());

                            ClientThread ct = getClient(cardSelectedPlayername);

                            ct.selectedCard = card;

                            ct.setsOfCards.remove(card);

                            display(cardSelectedPlayername + " selected " + card);

                            String returnMsg = cardSelectedPlayername + " selected " + card;

                            Message returnMessage = new Message(
                                    MessageType.PLAYGAME_SERVERRESPONSE.getValue(),
                                    false, returnMsg, false, ErrorMessageType.NONE.getValue());

                            returnMessage.PlayGameMessage = (new PlayGameMessage(this.playername, card));
                            boradastToOtherClients(this, returnMessage);

                            if (trickPlayCount == maximumNumberOfClients) {

                                trickPlayCount = 0;
                                roundTrickPlayCount++;

                                // Display Player who won the trick.
                                String wonPlayer = checkWonTrickPlayer();

                                SelectedTrickCardSuit = "";

                                try {
                                    TimeUnit.SECONDS.sleep(4);
                                } catch (InterruptedException ex) {
                                    Logger.getLogger(Server.class.getName()).log(Level.SEVERE, null, ex);
                                }

                                returnMsg = wonPlayer + " player won the trick.";

                                returnMessage = new Message(
                                        MessageType.PLAYGAME_SERVERRESPONSE_PLAYER_WON_TRICK.getValue(),
                                        false, returnMsg, false, ErrorMessageType.NONE.getValue());

                                returnMessage.PlayGameMessage = (new PlayGameMessage(this.playername, wonPlayer,
                                        this.team, wonPlayer));

                                boradastToAll(returnMessage);

                                try {
                                    TimeUnit.SECONDS.sleep(2);
                                } catch (InterruptedException ex) {
                                    Logger.getLogger(Server.class.getName()).log(Level.SEVERE, null, ex);
                                }

                                if (roundTrickPlayCount == maxNumberOfCardsPerPlayer) {

                                    setRoundStatMessage();

                                    MatchWonMessage matchWonMessage = isMatchWon();

                                    if (matchWonMessage != null) {

                                        returnMsg = matchWonMessage.getWonTeam() + " wons the match.";

                                        returnMessage = new Message(
                                                MessageType.PLAYGAME_SERVERRESPONSE_TEAM_WON_MATCH.getValue(),
                                                false, returnMsg, false, ErrorMessageType.NONE.getValue());

                                        returnMessage.MatchStatMessage = (getMatchStat());
//
                                        returnMessage.MatchWonMessage = (matchWonMessage);

                                        boradastToAll(returnMessage);
                                    } else {

                                        try {
                                            TimeUnit.SECONDS.sleep(2);
                                        } catch (InterruptedException ex) {
                                            Logger.getLogger(Server.class.getName()).log(Level.SEVERE, null, ex);
                                        }

                                        CurrentMatchStat = null;

                                        returnMsg = "Either red team or blue team won't won the Match. So next round needs to start.";

                                        returnMessage = new Message(
                                                MessageType.PLAYGAME_SERVERRESPONSE_TEAM_WON_GAME_WITH_DEAL_CARDS.getValue(),
                                                false, returnMsg, false, ErrorMessageType.NONE.getValue());

                                        returnMessage.MatchStatMessage = (getMatchStat());
                                        boradastToAll(returnMessage);

                                        playClientIndex = nextRoundPlayerIndex();
                                        biddingClientIndex = playClientIndex;

                                        roundTrickPlayCount = 0;

                                        for (ClientThread clientThread : Clients) {
                                            clientThread.biddingAmount = 0;
                                            clientThread.selectedCard = "";
                                            clientThread.wonTrickCouont = 0;
                                        }

                                        dealCards();

                                        sendBiddingRequestMessage();
                                    }

                                } else {
                                    playTrick();
                                }

                                break;
                            } else {
                                playTrick();
                            }
                        }
                        break;
                    case DEAL_CARD_TO_SERVER:
                        break;
                    default:
                        throw new AssertionError(this.message.getMessageType().name());
                }
            }

            remove(id);
            close();
        }

        // try to close everything
        private void close() {

            // try to close the connection
            try {
                if (outputStream != null) {
                    outputStream.close();
                }
            } catch (Exception e) {
            }
            try {
                if (inputStream != null) {
                    inputStream.close();
                }
            } catch (Exception e) {
            };
            try {
                if (socket != null) {
                    socket.close();
                }
            } catch (Exception e) {
            }
        }

        /*
		 * Write a String to the Client output stream
         */
        private void sendMessageaboutPreiousUsers() {

            for (int i = Clients.size(); --i >= 0;) {
                ClientThread ct = Clients.get(i);

                String msg = "'" + ct.username + "' user preiously registered";

                Message preiousUserMessage = new Message(
                        MessageType.CONNECTIONESTABLISH_SERVERESPONSE.getValue(),
                        false, msg, false, ErrorMessageType.NONE.getValue());

                preiousUserMessage.ConnectionMessage = (new ConnectionMessage(ct.playername, ct.username));
                writeMessage(preiousUserMessage);
            }

        }

        private String checkWonTrickPlayer() {

            String trickWonPlayer = "";

            //Map<String, Boolean> selectedCardsWithTrump = new HashMap<String, Boolean>();
            boolean isFirstCardTrump = false;
            boolean isSecondCardTrump = false;

            ArrayList<String> selectedCards = new ArrayList<>();

            for (int i = Clients.size(); --i >= 0;) {
                ClientThread ct = Clients.get(i);

                String suit = ct.selectedCard.substring(0, 1);

                if (SelectedTrickCardSuit.equalsIgnoreCase(suit)
                        || suit.equalsIgnoreCase(selectedTrumph)) {

                    if (i == 1 && suit.equalsIgnoreCase(selectedTrumph)) {
                        isFirstCardTrump = true;
                    }

                    if (i == 0 && suit.equalsIgnoreCase(selectedTrumph)) {
                        isSecondCardTrump = true;
                    }

                    selectedCards.add(ct.selectedCard);

                }

            }
            ArrayList<String> newSelectedCards = new ArrayList<>();

            if ((isFirstCardTrump & isSecondCardTrump) || (!isFirstCardTrump & !isSecondCardTrump)) {
                newSelectedCards = selectedCards;
            } else if ((isFirstCardTrump & !isSecondCardTrump) || (!isFirstCardTrump & isSecondCardTrump)) {

                for (String item : selectedCards) {

                    String suit = item.substring(0, 1);

                    if (suit.equalsIgnoreCase(selectedTrumph)) {
                        newSelectedCards.add(item);
                    }
                }

            }

            display("****************");
            display("Select card count" + newSelectedCards.size());

            String highestCard = getHighestPlayedCard(newSelectedCards);

            for (int i = Clients.size(); --i >= 0;) {
                ClientThread ct = Clients.get(i);

                display("playername " + ct.playername + " selectedCard " + ct.selectedCard);
                if (ct.selectedCard.equalsIgnoreCase(highestCard)) {
                    trickWonPlayer = ct.playername;
                    ct.wonTrickCouont++;
                    playClientIndex = ct.id;
                }
            }

            display("highestCard " + highestCard);
            display("trickWonPlayer " + trickWonPlayer);
            display("****************");

            if (trickWonPlayer == "" || trickWonPlayer.isEmpty() || trickWonPlayer == null) {
                trickWonPlayer = "player_1";
            }
            return trickWonPlayer;
        }

        private String getHighestPlayedCard(ArrayList<String> selectedCards) {

            String highestCard = "";
            int highestValue = 0;

            for (String card : selectedCards) {

                Integer cardValue = Integer.parseInt(card.substring(1, card.length()));

                if (highestValue < cardValue) {
                    highestValue = cardValue;
                    highestCard = card;
                }
            }

            return highestCard;

        }

        private String getHighestPlayedCard2(ArrayList<String> selectedCards) {

            String highestCard = "";
            int highestValue = 0;

            boolean trumphCardSelected = false;

            for (String card : selectedCards) {

                Integer cardValue = Integer.parseInt(card.substring(1, card.length()));

                String suit = card.substring(0, 1);

                if (suit.equalsIgnoreCase(selectedTrumph)) {

                    highestValue = cardValue;
                    highestCard = card;

                    if (highestValue < cardValue) {
                        highestValue = cardValue;
                        highestCard = card;
                    }

                    trumphCardSelected = true;

                } else if (!trumphCardSelected) {
                    if (highestValue < cardValue) {
                        highestValue = cardValue;
                        highestCard = card;
                    }
                }

            }

            return highestCard;

        }

        private boolean checkAnyIlleaglePlay(String playerName, String suit) {

            ClientThread client = getClient(playerName);

            for (String card : client.setsOfCards) {

                if (card.substring(0, 1).equalsIgnoreCase(suit)) {
                    return true;
                }
            }

            return false;
        }

        private void sendTrumphSelectedMessage(String playerName) {

            String returnMsg = "Player " + playerName + "selected " + selectedTrumph;

            Message returnMessage = new Message(
                    MessageType.TRUMPH_SELECTION_RESPONSE.getValue(),
                    false, returnMsg, false, ErrorMessageType.NONE.getValue());

            returnMessage.BiddingMessage = new BiddingMessage(playerName, 0, selectedTrumph);

            boradastToAll(returnMessage);

        }

        private void sendTrickStatMessage() {

            String returnMsg = team + " team won the trick.";

            //String team;
            Message returnMessage = new Message(
                    MessageType.PLAYGAME_SERVERRESPONSE_TEAM_SCORE.getValue(),
                    false, returnMsg, false, ErrorMessageType.NONE.getValue());

            List<String> usernameBids = new ArrayList<>();

            LinkedHashMap<String, Integer> userBids = new LinkedHashMap<String, Integer>();

            for (int i = 0; i < Clients.size(); i++) {
                ClientThread ct = Clients.get(i);

                usernameBids.add(ct.username + "(" + ct.team + ")"
                        + "\t" + ct.getBid(ct) + "\t" + ct.getwonTrick(ct));

                userBids.put(ct.playername, ct.getBid(ct));
            }

            GameStatMessage gameStatMessage = new GameStatMessage("p1", "", usernameBids, userBids);

            returnMessage.GameStatMessage = (gameStatMessage);
            returnMessage.MatchStatMessage = getMatchStat();

            boradastToAll(returnMessage);

        }

        private void setRoundStatMessage() {

            int redTeamTricksWon = 0;
            int blueTeamTricksWon = 0;

            int redTeamScore = 0;
            int blueTeamScore = 0;

            for (int i = Clients.size(); --i >= 0;) {
                ClientThread ct = Clients.get(i);

                if (ct.team.equalsIgnoreCase(redTeamName)) {
                    redTeamTricksWon += ct.getwonTrick(ct);
                } else {
                    blueTeamTricksWon += ct.getwonTrick(ct);
                }
            }

            if (redTeamTricksWon > blueTeamTricksWon) {
                redTeamScore = 1;
                blueTeamScore = 0;
            } else if (blueTeamTricksWon > redTeamTricksWon) {
                redTeamScore = 0;
                blueTeamScore = 1;
            } else {
                redTeamScore = 0;
                blueTeamScore = 0;
            }

            // player 2 or player 4
            CurrentMatchStat.setRedTeamScore(redTeamScore);

            // player 1 or player 3
            CurrentMatchStat.setBlueTeamScore(blueTeamScore);

            MatchStatList.add(CurrentMatchStat);
        }

        private int finalScore(int bidTricks, int wonTricks) {
            return (wonTricks >= bidTricks ? bidTricks * 10 + wonTricks - bidTricks
                    : bidTricks * -10);
        }

        private Integer getBid(ClientThread ct) {

            Integer bidAmount = 0;
            if (ct.biddingAmount != null) {
                bidAmount = ct.biddingAmount;
            }

            return bidAmount;
        }

        private Integer getwonTrick(ClientThread ct) {
            Integer wonTrickCount = 0;

            if (ct.wonTrickCouont != null) {
                wonTrickCount = ct.wonTrickCouont;
            }
            return wonTrickCount;
        }

        private MatchWonMessage isMatchWon() {

            int redTeamMarks = 0, blueTeamMarks = 0;

            for (MatchStat matchStat : MatchStatList) {
                blueTeamMarks += matchStat.getBlueTeamScore();
                redTeamMarks += matchStat.getRedTeamScore();
            }

            String wonTeamName = "";

            if (redTeamMarks > winScore && redTeamMarks > blueTeamMarks) {

                wonTeamName = redTeamName;

            } else if (blueTeamMarks > winScore && blueTeamMarks > redTeamMarks) {

                wonTeamName = blueTeamName;
            } else if (blueTeamMarks > winScore && redTeamMarks > winScore) {
                {
                    if (redTeamMarks > blueTeamMarks) {
                        wonTeamName = redTeamName;
                    } else {
                        wonTeamName = blueTeamName;
                    }
                }
            }

            if (wonTeamName.isEmpty()) {
                return null;
            } else {
                return new MatchWonMessage("", wonTeamName);
            }
        }

        private ClientThread getClient(String playername) {
            ClientThread client = null;

            for (int i = Clients.size(); --i >= 0;) {
                ClientThread ct = Clients.get(i);

                if (ct.playername.equalsIgnoreCase(playername)) {

                    client = ct;

                    break;
                }
            }

            return client;
        }

        private String getMatchStat() {

            int redTeamMarks = 0, blueTeamMarks = 0;

            for (MatchStat matchStat : MatchStatList) {
                redTeamMarks += matchStat.getRedTeamScore();
                blueTeamMarks += matchStat.getBlueTeamScore();
            }

            return "Player 1: " + blueTeamMarks + "\t" + "Player 2: " + redTeamMarks;
        }

        private int nextRoundPlayerIndex() {

            int index = 0;

            for (MatchStat matchStat : MatchStatList) {
                index = matchStat.getPlayerIndex();
            }

            if (index == maxPlayClientIndex) {
                index = 0;
            } else {
                index++;
            }

            return index;
        }

    }
}

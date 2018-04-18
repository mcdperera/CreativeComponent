using System;
using System.Collections;
using System.Collections.Generic;


public enum MessageType
{

    /**
     * None message type
     */
    NONE = 0,

    /**
     * Client request to connect to server message type
     */
    CONNECTIONESTABLISH_CLIENTREQUEST = 1,

    /**
     * Server response to client that username duplicate message type
     */
    ONNECTIONESTABLISH_CLIENT_USERNAMEDUPLICATE_RESPONSE = 2,

    /**
     * Connection well established  message type
     */
    CONNECTIONESTABLISH_SERVERESPONSE = 3,

    /**
     * Connection established message to other player message type
     */
    CONNECTIONESTABLISH_SERVERESPONSE_OTHERPLAYERS = 4,

    /**
     * Connection established message to previous player message type
     */
    CONNECTIONESTABLISH_SERVERESPONSE_PREVIOUSPLAYERS = 5,

    /**
     * Bidding server request message type
     */
    BIDDING_SERVERREQUEST = 6,

    /**
     * Bidding client response message type
     */
    BIDDING_CLIENTRESPONSE = 7,

    /**
     * Deal cards to clients message type
     */
    DEAL_CARDS_TO_CLIENT = 8,

    /**
     * Deal cards to server message type
     */
    DEAL_CARD_TO_SERVER = 9,

    /**
     * Play game server request message type
     */
    PLAYGAME_SERVERREQUEST = 10,

    /**
     * Play game client response message type
     */
    PLAYGAME_CLIENTRESPONSE = 11,

    /**
     * Play game server response message type
     */
    PLAYGAME_SERVERRESPONSE = 12,

    /**
     * Player won trick server response message type
     */
    PLAYGAME_SERVERRESPONSE_PLAYER_WON_TRICK = 13,

    /**
     * Team score server response message type
     */
    PLAYGAME_SERVERRESPONSE_TEAM_SCORE = 14,

    /**
     * Round won with next round message type
     */
    PLAYGAME_SERVERRESPONSE_TEAM_WON_GAME_WITH_DEAL_CARDS = 15,

    /**
     * Match won server response message type
     */
    PLAYGAME_SERVERRESPONSE_TEAM_WON_MATCH = 16,

    TRUMPH_SELECTION_RESPONSE = 17,

}
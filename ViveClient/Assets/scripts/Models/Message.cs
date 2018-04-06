using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[Serializable]
public class Message
{
    /**
         * The serial version UID
         */
    //protected static final long serialVersionUID = 1112122200L;

    /**
     * The the way of message sending
     */
    public bool isClientToServer;

    /**
     * The message
     */
    public String message;

    /**
     * The Connection message
     */
    public ConnectionMessage ConnectionMessage;

    /**
     * The Bidding message
     */
    public BiddingMessage BiddingMessage;

    ///**
    // * The card message
    // */
    public CardMessage CardMessage;

    /**
     * The play game message
     */
    public PlayGameMessage PlayGameMessage;

    ///**
    // * The game stat message
    // */
    public GameStatMessage GameStatMessage;

    /**
     * The match stst message
     */
    public String matchStatMessage;

    /**
     * The Match won message
     */
    public MatchWonMessage MatchWonMessage;

    public DealMessage DealMessage;

    public String MatchStatMessage;

    /**
     * Whether telling there is an error or not.
     */
    public bool IsError;

    /**
     * The error type.
     */
    public int errorType;

    public MessageType MessageType;

    public int Type;

    //public MessageType Type { get => type; set => type = value; }

    /**
     * Construct the message.
     */
    public Message(int type, bool isClientToServer, String message,
            bool isError, int errorType)
    {

        this.Type = type;
        this.isClientToServer = isClientToServer;
        this.message = message;
        this.IsError = isError;
        this.errorType = errorType;
    }
    
    public MessageType getMessageType()
    {
        return (MessageType)this.Type;
    }
    ///**
    // * Returns the type of the message.
    // */
    //MessageType getType()
    //{
    //    return type;
    //}

    /**
     * Returns the message.
// */
    //String getMessage()
    //{
    //    return message;
    //}

    /**
     * Returns connection message.
     */
    //ConnectionMessage getConnectionMessage()
    //{
    //    return this.connectionMessage;
    //}

    ///**
    // * Setting the connection message
    // */
    //void setConnectionMessage(ConnectionMessage connectionMessage)
    //{
    //    this.connectionMessage = connectionMessage;
    //}

    ///**
    // * Returns the bidding message
    // */
    //BiddingMessage getBiddingMessage()
    //{
    //    return this.biddingMessage;
    //}

    ///**
    // * Setting bidding message
    // */
    //void setBiddingMessage(BiddingMessage biddingMessage)
    //{
    //    this.biddingMessage = biddingMessage;
    //}

    ///**
    // * Returns card game message.
    // */
    //CardMessage getCardMessage()
    //{
    //    return this.cardMessage;
    //}

    ///**
    // * Setting card message.
    // */
    //void setCardMessage(CardMessage cardMessage)
    //{
    //    this.cardMessage = cardMessage;
    //}

    ///**
    // * Returns play game message.
    // */
    //PlayGameMessage getPlayGameMessage()
    //{
    //    return this.playGameMessage;
    //}

    ///**
    // * Setting play game message.
    // */
    //void setPlayGameMessage(PlayGameMessage playGameMessage)
    //{
    //    this.playGameMessage = playGameMessage;
    //}

    ///**
    // * Returns the game stat message.
    // */
    //GameStatMessage getGameStatMessage()
    //{
    //    return this.gameStatMessage;
    //}

    ///**
    // * Setting game game stat message.
    // */
    //void setGameStatMessage(GameStatMessage gameStatMessage)
    //{
    //    this.gameStatMessage = gameStatMessage;
    //}

    /**
     * Returns the match stat message.
     */
    //String getMatchStatMessage()
    //{
    //    return this.matchStatMessage;
    //}

    ///**
    // * Setting the match stat message.
    // */
    //void setMatchStatMessage(String matchStatMessage)
    //{
    //    this.matchStatMessage = matchStatMessage;
    //}

    /**
     * Returns the match won message.
     */
    //MatchWonMessage getMatchWonMessage()
    //{
    //    return this.matchWonMessage;
    //}

    ///**
    // * Setting the match won message
    // */
    //void setMatchWonMessage(MatchWonMessage matchWonMessage)
    //{
    //    this.matchWonMessage = matchWonMessage;
    //}

    /**
     * Returns whether the message is an error or not.
     */
    //bool isError()
    //{
    //    return isError;
    //}

    /**
     * Returns the error type.
     */
    int getErrorType()
    {
        return errorType;
    }

    /**
     * Returns the error message
     */
    //String getErrorMessage()
    //{
    //    return ErrorMessageType.getErrorMessgae(this.errorType);
    //}
}
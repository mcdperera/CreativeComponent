using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class DealMessage : PlayerMessage
{


    /**
     * the card that selected to deal the cards.
     */
    private String selectedCard;


    /**
     * Construct the deal message
     */
    DealMessage(String playerName, String selectedCard) : base(playerName)
    {
        this.selectedCard = selectedCard;
    }


    /**
     * Construct the deal message.
     */
    DealMessage(String playerName) : base(playerName)
    {
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class CardMessage : PlayerMessage
{
    /**
     * The card
     */
    private String card;

    /**
     * Initial set of cards
     */
    public List<String> initialSetOfCards;

    /**
     * Returns the card.
     */
    String getCard()
    {
        return this.card;
    }

    /**
     * Construct the message.
     */
    CardMessage(String playerName, List<String> initialSetOfCards) : base(playerName)
    {
        this.initialSetOfCards = initialSetOfCards;
    }


    /**
//     * Get the initial set of cards
//     */
    //    ArrayList<String> getInitialSetOfCards()
    //{
    //    return this.initialSetOfCards;
    //}

}

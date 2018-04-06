using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class BiddingMessage : PlayerMessage
{
    /**
     * The bidding amount
     */
    public int amount;

    public string trumph;

    /**
     * Construct the Bidding message.
     */
    public BiddingMessage(String playerName, int amount , string trumph) : base(playerName)
    {
        this.amount = amount;

        this.trumph = trumph;
    }

    /**
     * Returns the bidding amount
     */
    int getAmount()
    {
        return this.amount;
    }
    
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class PlayGameMessage : PlayerMessage
{

    /**
     * The card
     */
    public String card;

    /**
     * The panel
     */
    //private Panel panel;

    /**
     * The username
     */
    public String username;

    /**
     * The team
     */
    public String team;

    public String wonPlayerName;

    /**
     * Construct Play Game Message
     */
    public PlayGameMessage(String playerName) : base(playerName)
    {
    }

    /**
     * Construct Play Game Message
     */
    public PlayGameMessage(String playerName, String card) : base(playerName)
    {
        this.card = card;
    }

    /**
     * Construct Play Game Message
     */
    public PlayGameMessage(String playerName, String username, String team) : base(playerName)
    {
        this.username = username;
        this.team = team;
    }

    /**
     * Returns the card.
     */
    String getCard()
    {
        return this.card;
    }

    /**
     * Returns the panel
     */
    //Panel getPanel()
    //{
    //    return this.panel;
    //}

    /**
     * Returns the username
     */
    String getUsername()
    {
        return username;
    }

    /**
     * Returns the team
     */
    String getTeam()
    {
        return team;
    }
}

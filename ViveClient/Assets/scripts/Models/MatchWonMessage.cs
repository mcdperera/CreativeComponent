using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class MatchWonMessage : PlayerMessage
{

    /**
     * The team name.
     */
    private String teamName;

    /**
     * Construct of the match Won message.
     */
    MatchWonMessage(String playerName, String teamName) : base(playerName)
    {
        this.teamName = teamName;
    }

    /**
     * Returns the team name who won the game.
     */
    String getWonTeam()
    {
        return this.teamName;
    }

}
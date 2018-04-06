using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


[Serializable]
public class PlayerMessage
{
    /**
     * The player name
     */
    public string playerName;

    /**
     * Sets the player name
     *
     * @param playerName
     */
    public PlayerMessage(String playerName)
    {
        this.playerName = playerName;
    }

    /**
     * Returns the player name
     *
     * @return
     */
    public String getPlayerName()
    {
        return this.playerName;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


[Serializable]
public class ConnectionMessage : PlayerMessage
{

    /**
     * the username
     */
    public String username;

    public bool isPreviousUser;

    /**
     * Construct of the connection message.
     */
    public ConnectionMessage(string playerName, string username) : base(playerName)
    {       
        this.username = username;
    }

    /**
     * Returns the username
     */
    String getUsername()
    {
        return username;
    }

}
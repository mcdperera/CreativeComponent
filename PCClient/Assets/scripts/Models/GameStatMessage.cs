using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class GameStatMessage : PlayerMessage
{

    /**
    * Returns the bids with usernames
    */
    List<String> getUsernameBids()
    {
        return this.usernamesBids;
    }

    /**
    * Returns the name of the team.
    */
    String getTeam()
    {
        return this.team;
    }

    /**
     * The name of the team.
     */
    public String team;

    public String trumph;

    /**
     * Bids put by the users
     */
    public List<String> usernamesBids;

    /**
    * user bids;
    */
    public Dictionary<string, int> userBids;

    /**
     * Construct the Game stat message object.
     * @param playerName
     * @param team
     * @param usernamesBids
     */
    public GameStatMessage(String playerName, String team, List<String> usernamesBids, Dictionary<string, int> userBids) : base(playerName)
    {
        this.team = team;
        this.usernamesBids = usernamesBids;
        this.userBids = userBids;
    }


    

}
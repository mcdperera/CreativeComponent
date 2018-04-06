
import java.io.Serializable;
import java.util.LinkedHashMap;
import java.util.List;

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
/**
 *
 * @author Charmal
 */
public class GameStatMessage extends PlayerMessage implements Serializable {

     /**
     * Returns the bids with usernames
     */
    List<String> getUsernameBids() {
        return this.usernamesBids;
    }

    
    LinkedHashMap<String, Integer> getUserBids()
    {
         return this.userBids;
    }
     /**
     * Returns the name of the team.
     */
    String getTeam() {
        return this.team;
    }

    /**
     * The name of the team.
     */
    public String team;

    /**
     * Bids put by the users
     */
    public List<String> usernamesBids;

    /**
     * user Bids
     */
    public LinkedHashMap<String, Integer> userBids;
            
    /**
     * Construct the Game stat message object.
     * @param playerName
     * @param team
     * @param usernamesBids
     */
    public GameStatMessage(String playerName, String team, List<String> usernamesBids,LinkedHashMap<String, Integer> userBids) {
        super(playerName);
        this.team = team;
        this.usernamesBids = usernamesBids;
        this.userBids = userBids;
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class MatchStatMessage : PlayerMessage
{
    /**
 * The array list contains the match stat
 */
    public List<MatchStat> MatchStatList;

    /**
     * Construct the Match stat message
     *
     * @param playerName
     * @param MatchStatList
     */
    public MatchStatMessage(String playerName, List<MatchStat> MatchStatList) : base(playerName)
    {
        this.MatchStatList = MatchStatList;
    }

    /**
     * Returns the match stat details.
     */
    List<MatchStat> getMatchStatMessageList()
    {
        return MatchStatList;
    }
}
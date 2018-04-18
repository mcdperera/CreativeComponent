using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class MatchStat
{

    /**
     *
     */
    public int playerIndex;

    /**
     *
     */
    public int round;

    /**
     *
     */
    public int teamRedScore;

    /**
     *
     */
    public int teamBlueScore;

    /**
     *
     * @param playerIndex
     * @param round
     * @param teamRedScore
     * @param teamBlueScore
     */
    public MatchStat(int playerIndex, int round, int teamRedScore, int teamBlueScore)
    {

        this.playerIndex = playerIndex;
        this.round = round;
        this.teamRedScore = teamRedScore;
        this.teamBlueScore = teamBlueScore;
    }

    int getPlayerIndex()
    {
        return this.playerIndex;
    }

    int getRound()
    {
        return this.round;
    }

    int getRedTeamScore()
    {
        return this.teamRedScore;
    }

    int getBlueTeamScore()
    {
        return this.teamBlueScore;
    }

    void setRedTeamScore(int redTeamScore)
    {
        this.teamRedScore = redTeamScore;
    }

    void setBlueTeamScore(int blueTeamScore)
    {
        this.teamBlueScore = blueTeamScore;
    }
}

using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

[System.Serializable]
public class MatchesData 
{
    public List<Match> matches = new List<Match>();
    public void FillMatchesData(JSONNode data)
    {
        foreach(JSONNode match in data)
        {
            Match newMatch = new Match(match["createdAt"], match["standings"].Count);
            for(int i = 0; i < newMatch.standings.Length; i++)
            {
                newMatch.standings[i] = match["standings"][i];
            }
            matches.Add(newMatch);
        }
    }
}

[System.Serializable]
public class Match
{
    public string creationDate;
    public string[] standings;

    public Match(string _creationDate, int _standingsCount)
    {
        this.creationDate = _creationDate;
        this.standings = new string[_standingsCount];
    }
}
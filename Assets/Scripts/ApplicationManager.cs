using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ApplicationManager : MonoBehaviour
{
    private ServerManager serverManager;
    private Dictionary<string, List<int>> totalPlayers = new Dictionary<string, List<int>>();
    [SerializeField] private UIController uiController;

    [SerializeField]
    private List<Player> rankedPlayers = new List<Player>();

    #region Elo Rating Variables
    private const float K_FACTOR = 32f;
    #endregion

    void Awake()
    {
        serverManager = GetComponent<ServerManager>();
    }
    void Start()
    {
        ServerManager.OnMatchesFilled += ProcessMatchesData;
        serverManager.GetServerData();
    }

    void ProcessMatchesData(List<Match> _matches)
    {
        Debug.Log("Total Matches: " + _matches.Count);

        foreach(Match match in _matches)
        {
            for(int i = 0; i < match.standings.Length; i++)
            {
                DictionaryExtensions.AddOrUpdate(totalPlayers, match.standings[i], i + 1);
            }
        }

        foreach(var player in totalPlayers)
        {
            Player newPlayer = new Player(player.Key, player.Value);
            if(newPlayer.nMatches >= 3)
            {
                rankedPlayers.Add(newPlayer);
            }
        }
        CalculateEloRatings(_matches);
    }

    void CalculateEloRatings(List<Match> _matches)
    {
        foreach(Match match in _matches)
        {
            Player[] players = new Player[match.standings.Length];
            for(int i = 0; i < players.Length; i++)
            {
                players[i] = GetPlayerByName(match.standings[i]);
            }
            UpdateEloRatings(players[0], players[1], players[2], players[3], match.creationDate);
        }

        rankedPlayers = rankedPlayers.OrderByDescending(x => x.eloRating).ToList();
        uiController.InitializePlayersList(rankedPlayers);
    }

    Player GetPlayerByName(string _name)
    {
        return rankedPlayers.Find(x => x.name == _name);
    }

    void OnDestroy()
    {
        ServerManager.OnMatchesFilled -= ProcessMatchesData;
    }

    #region Elo Rating System
    private void UpdateEloRatings(Player _player1, Player _player2, Player _player3, Player _player4, string _matchDate)
    {
        // Calculate expected scores for each player
        float expectedScorePlayer1 = 1 / (1 + Mathf.Pow(10, (_player2.eloRating + _player3.eloRating + _player4.eloRating - _player1.eloRating) / 1200f));
        float expectedScorePlayer2 = 1 / (1 + Mathf.Pow(10, (_player1.eloRating + _player3.eloRating + _player4.eloRating - _player2.eloRating) / 1200f));
        float expectedScorePlayer3 = 1 / (1 + Mathf.Pow(10, (_player1.eloRating + _player2.eloRating + _player4.eloRating - _player3.eloRating) / 1200f));
        float expectedScorePlayer4 = 1 / (1 + Mathf.Pow(10, (_player1.eloRating + _player2.eloRating + _player3.eloRating - _player4.eloRating) / 1200f));

        // Calculate rating changes for each player
        _player1.UpdateEloRating(Mathf.RoundToInt(K_FACTOR * (1f - expectedScorePlayer1)), _matchDate);
        _player2.UpdateEloRating(Mathf.RoundToInt(K_FACTOR * (0.5f - expectedScorePlayer2)), _matchDate);
        _player3.UpdateEloRating(Mathf.RoundToInt(K_FACTOR * (0.25f - expectedScorePlayer3)), _matchDate);
        _player4.UpdateEloRating(Mathf.RoundToInt(K_FACTOR * (0f - expectedScorePlayer4)), _matchDate);
    }
    #endregion
}

public static class DictionaryExtensions
{
    public static void AddOrUpdate(this Dictionary<string, List<int>> targetDictionary, string key, int entry)
    {
        if (!targetDictionary.ContainsKey(key))
            targetDictionary.Add(key, new List<int>());

        targetDictionary[key].Add(entry);
    }
}

[System.Serializable]
public class Player
{
    public string name;
    public int nMatches;
    public List<int> places;
    public float eloRating;
    public Dictionary<string, float> matchesPlayed;

    public Player(string _name, List<int> _places)
    {
        this.name = _name;
        this.places = _places;
        this.nMatches = _places.Count;
        this.eloRating = 1500f;
        this.matchesPlayed = new Dictionary<string, float>();
    }

    public void UpdateEloRating(float _ratingAdded, string _matchDate)
    {
        this.eloRating += _ratingAdded;
        this.matchesPlayed.Add(_matchDate, _ratingAdded);
    }
}
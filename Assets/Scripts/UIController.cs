using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class UIController : MonoBehaviour
{
    public GameObject rankingPanel;
    public GameObject playersListContainer;
    public GameObject playerElementPrefab;
    public GameObject playerStatsPanel;
    public GameObject matchesListContainer;
    public GameObject matchElementPrefab;
    public TextMeshProUGUI playerStatsName;
    public TextMeshProUGUI playerEloRatingTxt;
    public TextMeshProUGUI playerPositionTxt;
    public GameObject newMatchPanel;
    public TMP_InputField[] standingsInput = new TMP_InputField[4];
    public GameObject errorPopUp;
    public TextMeshProUGUI errorPopUpTxt;

    private const int playerElementsPoolLength = 20;
    private List<GameObject> playerElementsPool = new List<GameObject>();

    private const int matchElementsPoolLength = 100;
    private List<GameObject> matchElementsPool = new List<GameObject>();

    private List<Player> currentPlayers = new List<Player>();

    private ServerManager serverManager;

    void Awake()
    {
        serverManager = FindObjectOfType<ServerManager>();
    }

    void Start()
    {
        ServerManager.OnRequestError += ShowErrorPopUp;
        CreateMatchElementsPool();
        CreatePlayerElementsPool();
    }

    public void InitializePlayersList(List<Player> _players)
    {
        foreach(Player player in _players)
        {
            GameObject element = GetPlayerElement();
            element.GetComponentInChildren<TextMeshProUGUI>().text = player.name;
            element.GetComponent<PlayerElementButton>().SetPlayerName(player.name);
        }
        currentPlayers = _players;
    }

    public void ActivatePlayerStatsPanel(string _playerName)
    {
        rankingPanel.SetActive(false);
        playerStatsPanel.SetActive(true);
        SeePlayerStats(currentPlayers.Find(x => x.name == _playerName));
    }

    public void ActivateNewMatchPanel()
    {
        rankingPanel.SetActive(false);
        newMatchPanel.SetActive(true);
    }

    public void BackToRankingPanel()
    {
        rankingPanel.SetActive(true);
        ResetMatchElements();
        playerStatsPanel.SetActive(false);
        newMatchPanel.SetActive(false);
    }

    private void SeePlayerStats(Player _player)
    {
        playerStatsName.text = _player.name;
        playerEloRatingTxt.text = "Elo Rating: " + _player.eloRating.ToString();
        playerPositionTxt.text = "Position: " + _player.rankingPosition.ToString() + "°";
        var sortedKeys = _player.matchesPlayed.Keys.OrderByDescending(key => key);
        foreach (var key in sortedKeys)
        {
            float eloRating = _player.matchesPlayed[key];
            ActivateMatchElement(key, eloRating);
        }
    }

    void CreateMatchElementsPool()
    {
        for (int i = 0; i < matchElementsPoolLength; i++)
        {
            GameObject matchElement = Instantiate(matchElementPrefab, matchesListContainer.transform);
            matchElement.SetActive(false);
            matchElementsPool.Add(matchElement);
        }
    }

    void CreatePlayerElementsPool()
    {
        for (int i = 0; i < playerElementsPoolLength; i++)
        {
            GameObject playerElement = Instantiate(playerElementPrefab, playersListContainer.transform);
            playerElement.SetActive(false);
            playerElementsPool.Add(playerElement);
        }
    }

    public void ActivateMatchElement(string _date, float _rating)
    {
        for (int i = 0; i < matchElementsPool.Count; i++)
        {
            if (!matchElementsPool[i].activeInHierarchy)
            {
                matchElementsPool[i].SetActive(true);
                TextMeshProUGUI matchTxt = matchElementsPool[i].GetComponent<TextMeshProUGUI>();
                matchTxt.text = _date + "    -    " + _rating.ToString();
                break;
            }
        }
    }

    public GameObject GetPlayerElement()
    {
        for (int i = 0; i < playerElementsPool.Count; i++)
        {
            if (!playerElementsPool[i].activeSelf)
            {
                playerElementsPool[i].SetActive(true);
                return playerElementsPool[i];
            }
        }
        Debug.Log("Return NULL");
        return null;
    }

    public void ResetMatchElements()
    {
        for (int i = 0; i < matchElementsPool.Count; i++)
        {
            if (matchElementsPool[i].activeInHierarchy)
            {
                TextMeshProUGUI matchTxt = matchElementsPool[i].GetComponent<TextMeshProUGUI>();
                matchTxt.text = "Date - Rating";
                matchElementsPool[i].SetActive(false);
            }
        }
    }

    public void ResetPlayerElements()
    {
        for (int i = 0; i < playerElementsPool.Count; i++)
        {
            if (playerElementsPool[i].activeSelf)
            {
                playerElementsPool[i].SetActive(false);
            }
        }
    }

    public void UploadNewMatch()
    {
        string createdAt =  System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
        string[] standings = new string[4];
        for(int i = 0; i < standingsInput.Length; i++)
        {
            string playerName = standingsInput[i].text;
            if(!string.IsNullOrEmpty(playerName) && PlayerNameIsValid(playerName))
            {
                standings[i] = playerName;
            }
            else 
            {
                Debug.LogError( playerName + " name is not a valid name.");
                return;
            }
        }

        if(IsPlayerRepeated(standings)) return;

        Match newMatch = new Match(createdAt, 4);
        newMatch.standings = standings;
        serverManager.SubmitNewMatchRequest(newMatch);
        ResetInputFields();
    }

    void ShowErrorPopUp(string _error)
    {
        errorPopUp.SetActive(true);
        errorPopUpTxt.text = _error;
        StartCoroutine(HideErrorPopup());
    }

    IEnumerator HideErrorPopup()
    {
        yield return new WaitForSeconds(5f);
        errorPopUp.SetActive(false);
    }

    void ResetInputFields()
    {
        for(int i = 0; i < standingsInput.Length; i++)
        {
            standingsInput[i].text = "";
        }
    }

    bool PlayerNameIsValid(string _playerName)
    {
        string pattern = "^[A-Za-z ]+$";
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
        System.Text.RegularExpressions.Match match = regex.Match(_playerName);
        
        return match.Success;
    }

    bool IsPlayerRepeated(string[] _standings)
    {
        return _standings.Length != _standings.Distinct().Count();
    }

    void OnDestroy()
    {
        ServerManager.OnRequestError -= ShowErrorPopUp;
    }
}

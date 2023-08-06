using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    private const int matchElementsPoolLength = 100;
    private List<GameObject> matchElementsPool = new List<GameObject>();

    private List<Player> currentPlayers = new List<Player>();

    void Start()
    {
        CreateMatchElementsPool();
    }

    public void InitializePlayersList(List<Player> _players)
    {
        foreach(Player player in _players)
        {
            GameObject element = Instantiate(playerElementPrefab, playersListContainer.transform);
            element.GetComponentInChildren<TextMeshProUGUI>().text = player.name;
            element.GetComponent<PlayerElementButton>().SetPlayerName(player.name);
        }
        currentPlayers = _players;

        Debug.Log("TEST");
        foreach(var match in _players[0].matchesPlayed)
        {
            Debug.Log(match.Key + "-" + match.Value.ToString());
        }
    }

    public void ActivatePlayerStatsPanel(string _playerName)
    {
        rankingPanel.SetActive(false);
        playerStatsPanel.SetActive(true);
        SeePlayerStats(currentPlayers.Find(x => x.name == _playerName));
    }

    public void BackToRankingPanel()
    {
        rankingPanel.SetActive(true);
        ResetMatchElements();
        playerStatsPanel.SetActive(false);
    }

    private void SeePlayerStats(Player _player)
    {
        playerStatsName.text = _player.name;
        foreach(var match in _player.matchesPlayed)
        {
            ActivateMatchElement(match.Key, match.Value);
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

    public void ActivateMatchElement(string _date, float _rating)
    {
        for (int i = 0; i < matchElementsPool.Count; i++)
        {
            if (!matchElementsPool[i].activeInHierarchy)
            {
                matchElementsPool[i].SetActive(true);
                TextMeshProUGUI matchTxt = matchElementsPool[i].GetComponent<TextMeshProUGUI>();
                matchTxt.text = _date + " - " + _rating.ToString();
                break;
            }
        }
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
}

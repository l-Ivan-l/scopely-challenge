using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class ServerManager : MonoBehaviour
{
    private const string serverAdress = "http://localhost:3000/matches";
    public MatchesData matchesData;

    public delegate void MatchesFilled(List<Match> _matches);
    public static event MatchesFilled OnMatchesFilled;

    public void GetServerData()
    {
        StartCoroutine(GetMatchesData());
    }

    IEnumerator GetMatchesData()
    {
        UnityWebRequest www = UnityWebRequest.Get(serverAdress);
        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(www.downloadHandler.text);
            FillMatchesData(www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("ERROR retrieving data: " + www.error);
        }
    }

    void FillMatchesData(string _rawData)
    {
        JSONNode node = JSON.Parse(_rawData);
        matchesData = new MatchesData();
        matchesData.FillMatchesData(node["list"]);

        OnMatchesFilled?.Invoke(matchesData.matches);
    }
}

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

    public delegate void MatchAdded();
    public static event MatchAdded OnMatchAdded;

    public delegate void DataUpdated();
    public static event DataUpdated OnDataUpdated;

    public delegate void RequestError(string _message);
    public static event RequestError OnRequestError;

    private const float pollingInterval = 10.0f;
    private string currentData;

    void Start()
    {
        StartCoroutine(PollForUpdates());
    }

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
            currentData = www.downloadHandler.text;
            FillMatchesData(www.downloadHandler.text);
        }
        else
        {
            string error = "ERROR retrieving data: " + www.error;
            Debug.LogError(error);
            OnRequestError?.Invoke(error);
        }
    }

    void FillMatchesData(string _rawData)
    {
        JSONNode node = JSON.Parse(_rawData);
        matchesData = new MatchesData();
        matchesData.FillMatchesData(node["list"]);

        OnMatchesFilled?.Invoke(matchesData.matches);
    }

    public void SubmitNewMatchRequest(Match _newMatch)
    {
        StartCoroutine(UploadNewMatchData(_newMatch));
    }

    IEnumerator UploadNewMatchData(Match _newMatch)
    {
        string matchBody = JsonUtility.ToJson(_newMatch);
        Debug.Log(matchBody);
        UnityWebRequest www = UnityWebRequest.Post(serverAdress, matchBody, "application/json");
        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("New Match UPLOADED succesfully!");
            OnMatchAdded?.Invoke();
        }
        else
        {
            string error = "ERROR uploading data: " + www.error;
            Debug.LogError(error);
            OnRequestError?.Invoke(error);
        }
    }

    IEnumerator PollForUpdates()
    {
        while (true)
        {
            yield return new WaitForSeconds(pollingInterval);
            UnityWebRequest www = UnityWebRequest.Get(serverAdress);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseJson = www.downloadHandler.text;
                if(currentData != responseJson)
                {
                    Debug.Log("Data has been updated");
                    OnDataUpdated?.Invoke();
                }
            }
            else
            {
                string error = "ERROR retrieving data: " + www.error;
                Debug.LogError(error);
                OnRequestError?.Invoke(error);
            }
        }
    }

}
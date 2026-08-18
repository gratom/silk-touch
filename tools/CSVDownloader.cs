using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Tools
{
    public static class CSVDownloader
    {
        private const string URL = "https://docs.google.com/spreadsheets/d/";
        private const string COMMAND = "/export?format=csv";

        public static void Download(string docID, Action<string> onCompleteCallback)
        {
            UnityWebRequest req = UnityWebRequest.Get(URL + docID + COMMAND);
            req.SendWebRequest().completed += (x) =>
            {
                if (req.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError("Network error.");
                }
                else
                {
                    Debug.Log("Download success\n" + req.downloadHandler.text);
                    onCompleteCallback?.Invoke(req.downloadHandler.text);
                }
            };
        }
    }
}
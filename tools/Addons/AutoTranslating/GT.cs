using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using UnityEngine.Networking;

public static class GT
{
    public enum Language
    {
        English,
        Russian,
        French,
        Spanish,
        German,
        Italian,
        ChineseSimplified,
        Japanese,
        Korean,
        Arabic,
        Portuguese,
        Dutch,
        Swedish,
        Norwegian,
        Danish,
        Finnish,
        Greek,
        Turkish,
        Polish,
        Ukrainian
    }

    private static readonly Dictionary<Language, string> language2Code = new Dictionary<Language, string>()
    {
        { Language.English, "en" },
        { Language.Russian, "ru" },
        { Language.French, "fr" },
        { Language.Spanish, "es" },
        { Language.German, "de" },
        { Language.Italian, "it" },
        { Language.ChineseSimplified, "zh-CN" },
        { Language.Japanese, "ja" },
        { Language.Korean, "ko" },
        { Language.Arabic, "ar" },
        { Language.Portuguese, "pt" },
        { Language.Dutch, "nl" },
        { Language.Swedish, "sv" },
        { Language.Norwegian, "no" },
        { Language.Danish, "da" },
        { Language.Finnish, "fi" },
        { Language.Greek, "el" },
        { Language.Turkish, "tr" },
        { Language.Polish, "pl" },
        { Language.Ukrainian, "uk" }
    };

    public static string ToCode(this Language ln)
    {
        return language2Code[ln];
    }

    public static string Translate(this string text, Language inputLanguage, Language outputLanguage)
    {
        // Google Translate API endpoint
        string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={language2Code[inputLanguage]}&tl={language2Code[outputLanguage]}&dt=t&q={UnityWebRequest.EscapeURL(text)}";

        // Create a web request
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        try
        {
            // Get the response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

            // Parse the translation from the response
            string jsonResponse = reader.ReadToEnd();
            string translation = ParseTranslation(jsonResponse);

            // Close the streams
            reader.Close();
            response.Close();

            return translation;
        }
        catch (WebException ex)
        {
            Debug.LogError("Error translating text: " + ex.Message);
            return null;
        }
    }

    // Method to parse the translation from the JSON response
    private static string ParseTranslation(string jsonResponse)
    {
        string translation = jsonResponse.Split(new[] { '"' }, StringSplitOptions.RemoveEmptyEntries)[1];
        return translation;
    }
}
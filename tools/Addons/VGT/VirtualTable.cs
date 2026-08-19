using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Tools
{

    public class VirtualTable
    {
        private const string URL_TEMPLATE = "https://docs.google.com/spreadsheets/d/{0}/export?format=csv&gid={1}";

        private string spreadsheetId;
        private string sheetId;
        private string[][] grid;

        public int RowCount => grid != null ? grid.Length : 0;

        public VirtualTable(string spreadsheetId, string sheetId)
        {
            this.spreadsheetId = spreadsheetId;
            this.sheetId = sheetId;
        }

        public async Task<bool> RefreshAsync()
        {
            string url = string.Format(URL_TEMPLATE, spreadsheetId, sheetId);

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                UnityWebRequestAsyncOperation operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to refresh spreadsheet: {request.error}");
                    return false;
                }

                ParseCsv(request.downloadHandler.text);
                return true;
            }
        }

        public string GetCell(int rowIndex, int columnIndex)
        {
            if (grid == null)
            {
                return string.Empty;
            }
            if (rowIndex < 0 || rowIndex >= grid.Length)
            {
                return string.Empty;
            }
            if (columnIndex < 0 || columnIndex >= grid[rowIndex].Length)
            {
                return string.Empty;
            }

            return grid[rowIndex][columnIndex];
        }

        public string[] GetRow(int rowIndex)
        {
            if (grid == null || rowIndex < 0 || rowIndex >= grid.Length)
            {
                return Array.Empty<string>();
            }
            return grid[rowIndex];
        }

        private void ParseCsv(string csvContent)
        {
            List<string[]> lines = new List<string[]>();
            List<string> currentLine = new List<string>();
            System.Text.StringBuilder currentField = new System.Text.StringBuilder();

            bool inQuotes = false;

            for (int i = 0; i < csvContent.Length; i++)
            {
                char c = csvContent[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < csvContent.Length && csvContent[i + 1] == '"')
                        {
                            currentField.Append('"');
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        currentField.Append(c);
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        inQuotes = true;
                    }
                    else if (c == ',')
                    {
                        currentLine.Add(currentField.ToString());
                        currentField.Clear();
                    }
                    else if (c == '\n' || c == '\r')
                    {
                        currentLine.Add(currentField.ToString());
                        currentField.Clear();

                        if (currentLine.Count > 0)
                        {
                            lines.Add(currentLine.ToArray());
                            currentLine.Clear();
                        }
                        if (c == '\r' && i + 1 < csvContent.Length && csvContent[i + 1] == '\n')
                        {
                            i++;
                        }
                    }
                    else
                    {
                        currentField.Append(c);
                    }
                }
            }
            if (currentField.Length > 0 || currentLine.Count > 0)
            {
                currentLine.Add(currentField.ToString());
                lines.Add(currentLine.ToArray());
            }

            grid = lines.ToArray();
        }
    }
}
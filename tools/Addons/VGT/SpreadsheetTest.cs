using UnityEngine;

namespace Tools
{
    public class SpreadsheetTest : MonoBehaviour
    {
        [SerializeField] private string spreadsheetId;
        [SerializeField] private string sheetId;

        private VirtualTable myTable;

        [ContextMenu("Test")]
        private async void Test()
        {
            myTable = new VirtualTable(spreadsheetId, sheetId);

            Debug.Log("Initial loading...");
            bool success = await myTable.RefreshAsync();

            if (success)
            {
                Debug.Log($"Loaded. Row count: {myTable.RowCount}");

                for (int i = 0; i < myTable.RowCount; i++)
                {
                    string[] row = myTable.GetRow(i);
                    string rowOutput = $"Row {i}: ";

                    for (int j = 0; j < row.Length; j++)
                    {
                        rowOutput += $"[{myTable.GetCell(i, j)}] ";
                    }

                    Debug.Log(rowOutput);
                }
            }
        }
    }
}
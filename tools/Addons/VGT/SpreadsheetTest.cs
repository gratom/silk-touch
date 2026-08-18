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

                // Перебираем все строки таблицы
                for (int i = 0; i < myTable.RowCount; i++)
                {
                    string[] row = myTable.GetRow(i);
                    string rowOutput = $"Row {i}: ";

                    // Перебираем все ячейки в текущей строке по индексу
                    for (int j = 0; j < row.Length; j++)
                    {
                        rowOutput += $"[{myTable.GetCell(i, j)}] ";
                    }

                    Debug.Log(rowOutput);
                }
            }
        }

        // Пример вызова обновления "на лету" (например, по кнопке из UI или для тестов)
        public async void UpdateTableData()
        {
            Debug.Log("Refreshing table data...");
            bool success = await myTable.RefreshAsync();

            if (success)
            {
                Debug.Log($"Table refreshed! New cell [0,0] value: {myTable.GetCell(0, 0)}");
            }
        }
    }
}
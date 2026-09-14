using System;
using System.Collections.Generic;

namespace SilkTouch.Managers.Game
{
    using Data;

    public static class GameSetter
    {
        private static Dictionary<GameData.GameStage, Action<GameData>> gameSetterFromLoading = new Dictionary<GameData.GameStage, Action<GameData>>()
        {
            { GameData.GameStage.Main, OnStageHomeLoad }
        };

        public static void SetGameFrom(GameData gameData)
        {
            gameSetterFromLoading[gameData.currentStageData](gameData);
        }

        #region setting functions

        private static void OnStageHomeLoad(GameData gameData)
        {
            //do some
            //Services.GetManager<UIManager>().ShowWindow<MainWindow>();
        }

        #endregion
    }
}
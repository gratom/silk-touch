using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SilkTouch.Managers
{
    using Tools;
    using Data;
    using Localization;
    using Game;

    public class MainManager : BaseManager
    {
        public override Type ManagerType => typeof(MainManager);

        public override bool IsFunctional => true;
        public AverageFloat aveFPS = new AverageFloat(60);
        public static float FPS = 60;

#pragma warning disable

        protected override async Task<bool> OnInit()
        {
            return true;
        }

#pragma warning restore

        public void EntryPoint()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Pause.TimeScaleFollowPause = true;
            Services.GetManager<DataManager>().StaticData.Balance.InitForRuntime();
            ContinueGame();

            //all another after game loading
            Services.GetManager<LanguageManager>().RefreshLanguages();
        }

        private void Update()
        {
            aveFPS.AddNext(Time.unscaledDeltaTime);
            FPS = aveFPS.AverageIgnoreSpikes.Inv().Clamp(30, 60);
        }

        public void StartNewGame()
        {
            //ContinueGame();
        }

        public void ContinueGame()
        {
            //creating new data (full, from zero)
            if (Services.GetManager<DataManager>().DynamicData.GameData == null || !Services.GetManager<DataManager>().DynamicData.GameData.isInit)
            {
                //TODO make data versioning
                Debug.Log("data is null. create new");
                Services.GetManager<DataManager>().DynamicData.GameData = Services.GetManager<DataManager>().StaticData.DefaultGameData.GetCopyOfData();
            }

            CheckUpdate();
            Services.GetManager<DataManager>().DynamicData.GameData.PostInitData();
            Services.GetManager<GameManager>().StartGame(Services.GetManager<DataManager>().DynamicData.GameData);
        }

        private void CheckUpdate()
        {
            List<BaseStageData> datasListCurrent = Services.GetManager<DataManager>().DynamicData.GameData.GetAllDataAsList();
            List<BaseStageData> datasListDefault = Services.GetManager<DataManager>().StaticData.DefaultGameData.GetCopyOfData().GetAllDataAsList();

            for (int i = 0; i < datasListCurrent.Count; i++)
            {
                BaseStageData stageData = datasListCurrent[i];
                BaseStageData defaultData = datasListDefault.FirstOrDefault(x => x.Stage == stageData.Stage);
                if (defaultData != null)
                {
                    if (defaultData.Version > stageData.Version) //TODO - here is simple add default data to new fields, can make better, with continuous update
                    {
                        Services.GetManager<DataManager>().DynamicData.GameData.SetData(defaultData);
                    }
                }
            }
        }
    }
}
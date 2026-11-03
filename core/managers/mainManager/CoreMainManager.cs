using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SilkTouch.Managers
{
    using Tools;
    using Data;

    public interface IMainManager : IBaseManager
    {
        void EntryPoint();
    }

    public abstract class CoreMainManager : BaseManager, IMainManager
    {
        public override Type ManagerType => typeof(IMainManager);

        public override bool IsFunctional => true;
        public AverageFloat aveFPS = new AverageFloat(60);
        public static float FPS = 60;

        private IDataManager dataManager => Services.GetManager<IDataManager>();

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
            dataManager.StaticData.Balance.InitForRuntime();
            CheckAndWarmUpData();
            GameEntryPoint();
        }

        public abstract void GameEntryPoint();

        protected virtual void Update()
        {
            aveFPS.AddNext(Time.unscaledDeltaTime);
            FPS = aveFPS.AverageIgnoreSpikes().Inv();
        }

        public void CheckAndWarmUpData()
        {
            BaseGameData copy = dataManager.StaticData.DefaultBaseGameData.GetCopyOfData();
            if (dataManager.DynamicData.GameData == null || !dataManager.DynamicData.GameData.isInit)
            {
                Debug.Log("data is null. create new");
                dataManager.DynamicData.GameData = copy;
                dataManager.DynamicData.GameData.AfterLoadingData();
            }
            else
            {
                while (dataManager.DynamicData.GameData.Version < copy.Version)
                {
                    SetupTo(dataManager.DynamicData.GameData, copy, new Action<BaseGameData>(x => dataManager.DynamicData.GameData = x));
                    dataManager.DynamicData.GameData.AfterLoadingData();
                }
            }
        }

        public abstract void SetupTo(BaseGameData current, BaseGameData latestVersion, Action<BaseGameData> setup);
    }
}
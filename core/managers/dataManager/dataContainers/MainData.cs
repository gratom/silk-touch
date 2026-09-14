using System;

namespace SilkTouch.Managers.Data
{
    [Serializable]
    public class MainData : BaseStageData
    {
        //make hash saving + sault

        public override GameData.GameStage Stage => GameData.GameStage.Main;

        public void PostInit()
        {
        }

        public void BeforeSaveData()
        {
        }
    }
}
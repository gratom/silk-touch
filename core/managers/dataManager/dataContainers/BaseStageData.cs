using System;
using UnityEngine;

namespace SilkTouch.Managers.Data
{
    [Serializable]
    public abstract class BaseStageData
    {
        [SerializeField] protected int version;

        public abstract GameData.GameStage Stage { get; }
        public int Version => version;
    }
}
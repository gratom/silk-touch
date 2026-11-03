using System;
using UnityEngine;

namespace SilkTouch.Managers.Data
{
    [Serializable]
    public abstract class BaseGameData : IVersionable
    {
        [SerializeField] protected int version;
        public bool isInit = false;
        public int Version => version;

        public virtual void BeforeSaveData()
        {
        }

        public virtual void AfterLoadingData()
        {
        }
    }

}
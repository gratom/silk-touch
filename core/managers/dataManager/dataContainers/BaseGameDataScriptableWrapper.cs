using System;
using UnityEngine;

namespace SilkTouch.Managers.Data
{
    public abstract class BaseGameDataScriptableWrapper : ScriptableObject
    {
        public abstract BaseGameData GetCopyOfData();
    }
}
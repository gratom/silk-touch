using System;
using UnityEngine;

namespace SilkTouch.Managers.Data
{
    [CreateAssetMenu(fileName = "WrapGameData", menuName = "Scriptables/Wrap game data", order = 51)]
    public class GameDataScriptableWrapper : ScriptableObject
    {
#pragma warning disable
        [SerializeField] private GameData gameData;
#pragma warning restore

        public GameData GetCopyOfData()
        {
            return gameData.Copy();
        }
    }
}
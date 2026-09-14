using System.Collections.Generic;
using UnityEngine;

namespace SilkTouch.Boot
{
    using Managers;
    using Tools;

    [CreateAssetMenu(fileName = "BootSettings", menuName = "Scriptables/Boot setting", order = 51)]
    public class BootSettings : ScriptableObject
    {
        [SerializeField] private float bootTime;
        public float BootTime => bootTime;

        [SerializeField] private int nextSceneIndex = 0;
        public int NextSceneIndex => nextSceneIndex;

        [SerializeField] private List<ManagerContainer> managerContainers;
        public List<ManagerContainer> ManagerContainers => managerContainers;
    }
}
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace SilkTouch.Tools.DependencyResolver
{
    [FilePath("ProjectSettings/SilkTouchEditorConfig.asset", FilePathAttribute.Location.ProjectFolder)]
    public class SilkTouchEditorConfig : ScriptableSingleton<SilkTouchEditorConfig>
    {
        [SerializeField] private bool ignoreDependencyCheck;

        public bool IgnoreDependencyCheck
        {
            get => ignoreDependencyCheck;
            set
            {
                ignoreDependencyCheck = value;
                Save(true);
            }
        }
    }
}

#endif
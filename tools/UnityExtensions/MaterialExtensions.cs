using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public static class MaterialExtensions
    {
        private static readonly Dictionary<Shader, Dictionary<string, int>> _shaderPropertyCache
            = new Dictionary<Shader, Dictionary<string, int>>();

        /// <summary>
        /// Get property ID by name
        /// </summary>
        public static int IDof(this Material mat, string property)
        {
            if (mat == null || mat.shader == null)
            {
                Debug.LogWarning("Material of Shader null in IDof()");
                return -1;
            }

            Shader shader = mat.shader;

            if (!_shaderPropertyCache.TryGetValue(shader, out Dictionary<string, int> propDict))
            {
                propDict = new Dictionary<string, int>();
                _shaderPropertyCache[shader] = propDict;
            }

            if (!propDict.TryGetValue(property, out int id))
            {
                id = Shader.PropertyToID(property);
                propDict[property] = id;
            }

            return id;
        }
    }
}
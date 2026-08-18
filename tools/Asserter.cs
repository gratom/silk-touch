//developer -> gratomov@gmail.com

using UnityEngine;

namespace Tools
{
    /// <summary>
    /// Wrap class for quick assertion for null-value debug function
    /// </summary>
    public static class Asserter
    {
        /// <summary>
        /// Quick assertion for null-value
        /// </summary>
        /// <param name="objectAssert">object, that need to inspect for null-value</param>
        /// <param name="objectMono">Mono-object, that contains inspected object</param>
        /// <typeparam name="T">type of inspected object</typeparam>
        public static void Assert<T>(T objectAssert, MonoBehaviour objectMono)
        {
            Debug.Assert(objectAssert != null, $"The field {typeof(T).Name} is null\nin: {objectMono?.name};    type of: {objectMono?.GetType()}");
        }
    }


}
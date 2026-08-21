using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tools
{
    public static class ReflectionTool
    {
        public static IEnumerable<Type> GetEnumerableOfType<T>() where T : class
        {
            List<Type> objects = new List<Type>();
            foreach (Type type in
                     Assembly
                         .GetAssembly(typeof(T))
                         .GetTypes()
                         .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add(type);
            }

            objects = objects.OrderBy(x => x.Name).ToList();
            return objects;
        }
    }
}
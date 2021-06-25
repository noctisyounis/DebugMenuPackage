using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DebugMenu.CustomAttribute.Runtime
{
    public class DebugAttributeRegistry
    {
        #region Main

        public static void ValidateMethods()
        {
            InitializeDictionnary();
            ValidateDictionary();
        }

        #region Invoke Method

        public static void InvokeMethod(string path)
        {
            if (!Methods.ContainsKey(path) || Methods[path].IsPrivate) return;

            var method = Methods[path];
            try
            {
                method.Invoke(method.ReflectedType, new object[0]);
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
            }
        }

        public static ReturnType InvokeMethod<ReturnType>(string path)
        {
            if (!Methods.ContainsKey(path) || Methods[path].IsPrivate) return default(ReturnType);

            var result = default(ReturnType);
            var method = Methods[path];

            try
            {
                result = (ReturnType)method.Invoke(method.ReflectedType, new object[0]);
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
            }

            return result;
        }

        public static void InvokeMethod(string path, object[] parameters)
        {
            if (!Methods.ContainsKey(path) || Methods[path].IsPrivate) return;

            var method = Methods[path];
            try
            {
                method.Invoke(method.ReflectedType, parameters);
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
            }
        }

        public static ReturnType InvokeMethod<ReturnType>(string path, object[] parameters)
        {
            if (!Methods.ContainsKey(path) || Methods[path].IsPrivate) return default(ReturnType);

            var result = default(ReturnType);
            var method = Methods[path];
            try
            {
                result = (ReturnType)method.Invoke(method.ReflectedType, parameters);
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
            }

            return result;
        }

        #endregion Invoke Method


        #region Get Data

        public static string[] GetPaths()
        {
            return Methods.Keys.ToArray();
        }

        public static MethodInfo GetMethodInfoAt(string path)
        {
            return Methods[path];
        }

        public static string[] GetQuickPaths()
        {
            var result = new List<string>();

            foreach (var item in Methods)
            {
                if (item.Value.GetCustomAttribute<DebugMenuAttribute>().IsQuickMenu)
                {
                    result.Add(item.Key);
                }
            }

            return result.ToArray();
        }

        #endregion Get Data


        #endregion Main


        #region Utils

        private static void InitializeDictionnary()
        {
            _methods = new MergeableDictionary<string, MethodInfo>();

            for (int i = 0; i < Assemblies.Length; i++)
            {
                var assembly = Assemblies[i];
                var assemblyDictionary = assembly
                            .GetTypes()
                            .SelectMany(classType => classType.GetMethods())
                            .Where(classMethod => classMethod.GetCustomAttributes().OfType<DebugMenuAttribute>().Any())
                            .ToDictionary(methodInfo => methodInfo.GetCustomAttributes().OfType<DebugMenuAttribute>().FirstOrDefault<DebugMenuAttribute>().Path);

                if (assemblyDictionary != null)
                {
                    _methods.Merge(assemblyDictionary);
                }
            }
        }

        private static void ValidateDictionary()
        {
            var validCount = 0;
            var initialMethodCount = _methods.Count;

            for (int i = _methods.Count - 1; i >= 0; i--)
            {
                var item = _methods.Keys.ToArray()[i];

                if (!_methods[item].IsStatic)
                {
                    Debug.LogError($"<color=orange>{_methods[item].Name} of class {_methods[item].ReflectedType} must be static</color>");
                    _methods.Remove(item);
                }
                else
                {
                    validCount++;
                }
            }

            var multiplicity = (validCount > 0) ? $"{validCount} {(validCount > 1 ? "were" : "was")}" : "none was";

            Debug.Log($"<color=cyan>{initialMethodCount} methods were tested and {multiplicity} valid</color>");
        }

        private static void InitializeAssemblies()
        {
            _assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        }

        #endregion Utils


        #region Private

        private static Assembly[] _assemblies;

        private static Assembly[] Assemblies
        {
            get
            {
                if (_assemblies == null)
                {
                    InitializeAssemblies();
                }

                return _assemblies;
            }
        }

        private static MergeableDictionary<string, MethodInfo> _methods;

        private static MergeableDictionary<string, MethodInfo> Methods
        {
            get
            {
                if (_methods == null)
                {
                    InitializeDictionnary();
                    ValidateDictionary();
                }

                return _methods;
            }
        }

        #endregion Private
    }
}
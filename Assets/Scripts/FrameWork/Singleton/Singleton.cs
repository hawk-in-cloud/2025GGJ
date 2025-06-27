using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace Framework
{
    /// <summary>
    /// Singleton Parttern Instance => Without MonoBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> where T : class
    {
        private static T instance;
        /// <summary>
        /// Using MemberProperties to get the instance
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    Type type = typeof(T);
                    ConstructorInfo info = type.GetConstructor(
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        null,
                        Type.EmptyTypes,
                        null);
                    if (info != null)
                        instance = info.Invoke(null) as T;
                    else
                        Debug.LogError("Type of" + typeof(T).ToString() + "does not have ConstructorInfo");
                }
                return instance;
            }
        }
        /// <summary>
        /// Using MemberMethods to get the instance
        /// </summary>
        /// <returns>instance</returns>
        public static T GetInstance()
        {
            if (instance == null)
            {
                Type type = typeof(T);
                ConstructorInfo info = type.GetConstructor(
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    Type.EmptyTypes,
                    null);
                if (info != null)
                    instance = info.Invoke(null) as T;
                else
                    Debug.LogError("Type of" + typeof(T).ToString() + "does not have ConstructorInfo");
            }
            return instance;
        }
    }
}

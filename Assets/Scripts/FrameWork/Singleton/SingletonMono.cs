using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton Parttern Instance => Inherit MonoBehavior
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonMono<T> : MonoBehaviour where T: MonoBehaviour
{
    private static T instance;
    /// <summary>
    /// Using MemberProperties to get the instance
    /// </summary>
    public static T Instance
    {
        get
        {
            // Dynamic create the gameobject
            if(instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).ToString();

                instance = obj.AddComponent<T>();

                DontDestroyOnLoad(obj);
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
            GameObject obj = new GameObject();
            obj.name = typeof(T).ToString();

            instance = obj.AddComponent<T>();

            DontDestroyOnLoad(obj);
        }

        return instance;
    }
}

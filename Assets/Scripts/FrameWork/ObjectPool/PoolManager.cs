using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Base class for InfoDataObject to allow type substitution in the dictionary.
    /// </summary>
    public abstract class InfoDataObjectBase { }

    /// <summary>
    /// A container class used for storing data structure classes and logic classes (non-MonoBehaviour).
    /// </summary>
    /// <typeparam name="T">The type of the object to store.</typeparam>
    public class InfoDataObject<T> : InfoDataObjectBase where T : class
    {
        public Queue<T> dataPool = new Queue<T>(); // A queue to hold the reusable objects of type T.
    }

    /// <summary>
    /// Interface that must be implemented by any class that needs to be reused in the object pool.
    /// </summary>
    public interface IDataPoolObject
    {
        void ResetInfo(); // Method to reset the state of the object when it is reused.
    }

    /// <summary>
    /// Manages a collection of object pools, allowing efficient reuse of GameObjects.
    /// </summary>
    public class PoolManager : Singleton<PoolManager>
    {
        private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>(); // Dictionary for storing GameObject pools by name.
        public Dictionary<string, PoolData> PoolDic
        {
            get
            {
                return poolDic;
            }
        }

        private Dictionary<string, InfoDataObjectBase> infoDataPoolDic = new Dictionary<string, InfoDataObjectBase>(); // Dictionary for storing data object pools by name.
        public Dictionary<string, InfoDataObjectBase> InfoDataPoolDic
        {
            get
            {
                return infoDataPoolDic;
            }
        }

        private GameObject poolObj;
        private PoolManager() { } // Private constructor to ensure Singleton pattern.

        private string poolName
        {
            get
            {
                return "PoolManager(GameObjectOnly)"; // Name for the GameObject pool.
            }
        }

        public bool showDebugInfo = false;
        public bool ShowDebugInfo
        {
            get
            {
                return showDebugInfo;
            }
            set
            {
                showDebugInfo = value; // Enable or disable debug information display.
            }
        }

        /// <summary>
        /// Get GameObject from PoolManager (Load with ABResManager)
        /// </summary>
        /// <param name="abName">The name of the asset bundle.</param>
        /// <param name="resName">The name of the resource.</param>
        /// <returns>The requested GameObject.</returns>
        public GameObject GetObj(string abName, string resName)
        {
            string name = abName + "/" + resName;
            GameObject obj = null;
            if (poolDic.ContainsKey(name) && poolDic[name].Count > 0)
            {
                obj = poolDic[name].Pop(); // Get the object from the pool if available.
            }
            else
            {
                // obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
                ABResManager.Instance.LoadResAsync<GameObject>(abName, resName, (res) =>
                {
                    obj = GameObject.Instantiate(res); // Instantiate the GameObject if not found in the pool.
                }, true);

                if (obj == null)
                {
                    if (showDebugInfo)
                    {
                        Debug.LogError(
                        $"PoolManager >>> Can't get gameobject with name {name}.\nPlease check if the ABPath or ResName is correct."
                        ); // Log error if the object could not be instantiated.
                    }

                    return null;
                }

                // When instantiating an object, Unity automatically appends "(Clone)" to the object's name by default.
                // We need to manually modify the name to avoid this.
                obj.name = name;
            }
            return obj;
        }


        /// <summary>
        /// Get GameObject from PoolManager (Customize GameObject)
        /// </summary>
        /// <param name="objName">The name of the object.</param>
        /// <returns>The requested GameObject.</returns>
        public GameObject GetObj(string objName)
        {
            string name = objName;
            GameObject obj = null;
            if (poolDic.ContainsKey(name) && poolDic[name].Count > 0)
            {
                obj = poolDic[name].Pop(); // Get the object from the pool if available.
            }
            else
            {
                if (showDebugInfo)
                {
                    Debug.LogError(
                    $"PoolManager >>> Can't get gameobject with name {name}.\nPlease check if the object is pushed in PoolManager."
                    ); // Log error if the object is not found in the pool.
                }

                obj = null;
            }
            return obj;
        }

        /// <summary>
        /// Get an object from the pool of type T.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="nameSpace">Optional namespace to distinguish objects of the same type.</param>
        /// <returns>The requested object of type T.</returns>
        public T GetObj<T>(string nameSpace = "NullNameSpace") where T : class, IDataPoolObject, new()
        {
            string name = nameSpace + "_" + typeof(T).Name;
            T obj;
            if (infoDataPoolDic.ContainsKey(name))
            {
                InfoDataObject<T> pool = infoDataPoolDic[name] as InfoDataObject<T>;
                if (pool.dataPool.Count > 0)
                {
                    obj = pool.dataPool.Dequeue() as T; // Dequeue an object from the pool if available.
                }
                else
                {
                    obj = new T(); // Create a new object if the pool is empty.
                }
            }
            else
            {
                obj = new T(); // Create a new object if the pool does not exist.
            }
            return obj;
        }

        /// <summary>
        /// Push a GameObject into PoolManager for reuse.
        /// </summary>
        /// <param name="obj">The GameObject to push into the pool.</param>
        public void PushObj(GameObject obj)
        {
#if UNITY_EDITOR
            if (poolObj == null)
                poolObj = new GameObject(poolName); // Create a new GameObject if one does not exist in the editor.
#endif

            string name = obj.name;

            obj.SetActive(false); // Deactivate the GameObject before pushing it into the pool.

            if (!poolDic.ContainsKey(name))
            {
#if UNITY_EDITOR
                poolDic.Add(name, new PoolData(name, poolObj)); // Add a new pool for the object in the editor.
#else
                poolDic.Add(name, new PoolData(name)); // Add a new pool for the object in production.
#endif
            }

            poolDic[name].Push(obj); // Push the object into the pool.
        }

        /// <summary>
        /// Push an object of type T into the data pool for reuse.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="dataObject">The object to push into the pool.</param>
        /// <param name="nameSpace">Optional namespace to distinguish objects of the same type.</param>
        public void PushObj<T>(T dataObject, string nameSpace = "NullNameSpace") where T : class, IDataPoolObject
        {
            if (dataObject == null)
            {
                if (showDebugInfo)
                {
                    Debug.LogError(
                    $"PoolManager >>> You are trying to push a null dataObject in PoolManager.\nPlease make sure that the dataObject has been correctly assigned."
                    ); // Log error if the dataObject is null.
                }

                return;
            }

            string name = nameSpace + "_" + typeof(T).Name;
            InfoDataObject<T> pool;
            if (infoDataPoolDic.ContainsKey(name))
                pool = infoDataPoolDic[name] as InfoDataObject<T>;
            else
            {
                pool = new InfoDataObject<T>(); // Create a new pool if one does not exist.
                infoDataPoolDic.Add(name, pool); // Add the new pool to the dictionary.
            }

            dataObject.ResetInfo(); // Reset the object's state before pushing it into the pool.
            pool.dataPool.Enqueue(dataObject); // Enqueue the object into the pool.
        }

        /// <summary>
        /// Clear all pools in the PoolManager.
        /// </summary>
        public void ClearPool()
        {
            poolDic.Clear(); // Clear the GameObject pool dictionary.
        }
    }
}
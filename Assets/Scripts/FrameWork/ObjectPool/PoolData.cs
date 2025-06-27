using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// PoolManager's Data
    /// </summary>
    public class PoolData
    {
        // Root object, used for layout management.
        private GameObject rootObj;
        // A stack for storing objects.
        private Stack<GameObject> dataStack;

        /// <summary>
        /// Gets the number of GameObjects in the pool.
        /// </summary>
        public int Count => dataStack.Count;
        /// <summary>
        /// Pop a GameObject from the dataStack.
        /// </summary>
        /// <returns>A GameObject in dataStack</returns>
        public GameObject Pop()
        {
            GameObject obj = dataStack.Pop();
            obj.SetActive(true);
#if UNITY_EDITOR
            obj.transform.SetParent(null);
#endif
            return obj;
        }

        /// <summary>
        /// Push a GameObject back into the dataStack
        /// </summary>
        /// <param name="obj">The GameObject to be stored in the pool.</param>
        public void Push(GameObject obj)
        {
            dataStack.Push(obj);
            obj.SetActive(false);
#if UNITY_EDITOR
            obj.transform.SetParent(rootObj.transform);
#endif
        }
        /// <summary>
        /// Initializes the pool with a name and a root GameObject.
        /// </summary>
        /// <param name="name">Data name</param>
        /// <param name="poolObj">poolObj in "PoolManager"</param>
        public PoolData(string name, GameObject poolObj = null)
        {
#if UNITY_EDITOR
            rootObj = new GameObject(name);
            if (poolObj != null)
                rootObj.transform.SetParent(poolObj.transform);
#endif
            dataStack = new Stack<GameObject>();
        }
    }
}
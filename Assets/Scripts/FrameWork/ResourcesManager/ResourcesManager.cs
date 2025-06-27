using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using UnityEngine.Events;
using System;


namespace Framework
{
    /// <summary>
    /// Base class for resource information, mainly used for the Liskov Substitution Principle, allowing parent class containers to hold child class objects.
    /// </summary>
    public abstract class ResInfoBase
    {
        // Reference count
        public int refCount;
    }

    /// <summary>
    /// Resource information object, mainly used to store resource information, asynchronous loading delegate information, and coroutine information.
    /// </summary>
    /// <typeparam name="T">Resource type</typeparam>
    public class ResInfo<T> : ResInfoBase
    {
        // Resource
        public T asset;
        // Delegate used to pass the resource to the outside after asynchronous loading is complete
        public UnityAction<T> callBack;
        // Coroutine used for asynchronous loading
        public Coroutine coroutine;
        // Determines whether the resource should be removed when the reference count reaches 0
        public bool isDel;

        public void AddRefCount()
        {
            ++refCount;
        }

        public void SubRefCount()
        {
            --refCount;
            if (refCount < 0)
                Debug.LogError("Reference count is less than 0, please check if usage and unloading are paired correctly.");
        }
    }

    /// <summary>
    /// Resource loading module manager for Resources.
    /// </summary>
    public class ResourcesManager : Singleton<ResourcesManager>
    {
        // Container for storing loaded or loading resources
        private Dictionary<string, ResInfoBase> resDic = new Dictionary<string, ResInfoBase>();

        private ResourcesManager() { }

        /// <summary>
        /// Synchronously loads a resource from the Resources folder.
        /// </summary>
        /// <typeparam name="T">Resource type</typeparam>
        /// <param name="path">Resource path</param>
        /// <returns>Loaded resource</returns>
        public T Load<T>(string path) where T : UnityEngine.Object
        {
            string resName = path + "_" + typeof(T).Name;
            ResInfo<T> info;
            // If the resource is not in the dictionary
            if (!resDic.ContainsKey(resName))
            {
                // Directly load the resource synchronously and record the resource information in the dictionary for future use
                T res = Resources.Load<T>(path);
                info = new ResInfo<T>();
                info.asset = res;
                // Increase reference count
                info.AddRefCount();
                resDic.Add(resName, info);
                return res;
            }
            else
            {
                // Retrieve the resource information from the dictionary
                info = resDic[resName] as ResInfo<T>;
                // Increase reference count
                info.AddRefCount();
                // If the resource is still being loaded asynchronously
                if (info.asset == null)
                {
                    // Stop the asynchronous loading
                    MonoManager.Instance.StopCoroutine(info.coroutine);
                    // Load the resource synchronously
                    T res = Resources.Load<T>(path);
                    // Record the resource
                    info.asset = res;
                    // Invoke the callbacks waiting for the asynchronous loading to complete
                    info.callBack?.Invoke(res);
                    // Clear unused references
                    info.callBack = null;
                    info.coroutine = null;
                    // Return the resource
                    return res;
                }
                else
                {
                    // If the resource is already loaded, return it directly
                    return info.asset;
                }
            }
        }

        /// <summary>
        /// Asynchronously loads a resource.
        /// </summary>
        /// <typeparam name="T">Resource type</typeparam>
        /// <param name="path">Resource path</param>
        /// <param name="callBack">Callback function invoked after loading is complete</param>
        public void LoadAsync<T>(string path, UnityAction<T> callBack) where T : UnityEngine.Object
        {
            // Unique resource ID, formed by concatenating the path and resource type
            string resName = path + "_" + typeof(T).Name;
            ResInfo<T> info;
            if (!resDic.ContainsKey(resName))
            {
                // Create a resource information object
                info = new ResInfo<T>();
                // Increase reference count
                info.AddRefCount();
                // Add the resource record to the dictionary (resource not yet loaded)
                resDic.Add(resName, info);
                // Record the callback function to be invoked after loading is complete
                info.callBack += callBack;
                // Start a coroutine for asynchronous loading and record the coroutine (for possible stopping later)
                info.coroutine = MonoManager.Instance.StartCoroutine(ReallyLoadAsync<T>(path));
            }
            else
            {
                // Retrieve the resource information from the dictionary
                info = resDic[resName] as ResInfo<T>;
                // Increase reference count
                info.AddRefCount();
                // If the resource is still being loaded asynchronously
                if (info.asset == null)
                    info.callBack += callBack;
                else
                    callBack?.Invoke(info.asset);
            }
        }

        private IEnumerator ReallyLoadAsync<T>(string path) where T : UnityEngine.Object
        {
            // Asynchronously load the resource
            ResourceRequest rq = Resources.LoadAsync<T>(path);
            // Wait until the resource is loaded before continuing
            yield return rq;

            string resName = path + "_" + typeof(T).Name;
            // After the resource is loaded, pass it to the external callback function
            if (resDic.ContainsKey(resName))
            {
                ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;
                // Record the loaded resource
                resInfo.asset = rq.asset as T;

                // If the resource needs to be deleted, remove it
                // Only remove if the reference count is 0
                if (resInfo.refCount == 0)
                    UnloadAsset<T>(path, resInfo.isDel, null, false);
                else
                {
                    // Pass the loaded resource to the callback
                    resInfo.callBack?.Invoke(resInfo.asset);
                    // Clear references to avoid potential memory leaks
                    resInfo.callBack = null;
                    resInfo.coroutine = null;
                }
            }
        }

        /// <summary>
        /// Asynchronously loads a resource using Type.
        /// </summary>
        /// <param name="path">Resource path</param>
        /// <param name="type">Resource type</param>
        /// <param name="callBack">Callback function invoked after loading is complete</param>
        [Obsolete("Note: It is recommended to use the generic loading method. If using Type loading, avoid mixing with generic loading for the same resource.")]
        public void LoadAsync(string path, Type type, UnityAction<UnityEngine.Object> callBack)
        {
            // Unique resource ID, formed by concatenating the path and resource type
            string resName = path + "_" + type.Name;
            ResInfo<UnityEngine.Object> info;
            if (!resDic.ContainsKey(resName))
            {
                // Create a resource information object
                info = new ResInfo<UnityEngine.Object>();
                // Increase reference count
                info.AddRefCount();
                // Add the resource record to the dictionary (resource not yet loaded)
                resDic.Add(resName, info);
                // Record the callback function to be invoked after loading is complete
                info.callBack += callBack;
                // Start a coroutine for asynchronous loading and record the coroutine (for possible stopping later)
                info.coroutine = MonoManager.Instance.StartCoroutine(ReallyLoadAsync(path, type));
            }
            else
            {
                // Retrieve the resource information from the dictionary
                info = resDic[resName] as ResInfo<UnityEngine.Object>;
                // Increase reference count
                info.AddRefCount();
                // If the resource is still being loaded asynchronously
                if (info.asset == null)
                    info.callBack += callBack;
                else
                    callBack?.Invoke(info.asset);
            }
        }

        private IEnumerator ReallyLoadAsync(string path, Type type)
        {
            // Asynchronously load the resource
            ResourceRequest rq = Resources.LoadAsync(path, type);
            // Wait until the resource is loaded before continuing
            yield return rq;

            string resName = path + "_" + type.Name;
            // After the resource is loaded, pass it to the external callback function
            if (resDic.ContainsKey(resName))
            {
                ResInfo<UnityEngine.Object> resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
                // Record the loaded resource
                resInfo.asset = rq.asset;
                // If the resource needs to be deleted, remove it
                // Only remove if the reference count is 0
                if (resInfo.refCount == 0)
                    UnloadAsset(path, type, resInfo.isDel, null, false);
                else
                {
                    // Pass the loaded resource to the callback
                    resInfo.callBack?.Invoke(resInfo.asset);
                    // Clear references to avoid potential memory leaks
                    resInfo.callBack = null;
                    resInfo.coroutine = null;
                }
            }
        }

        /// <summary>
        /// Unloads a specific resource.
        /// </summary>
        /// <param name="assetToUnload">Resource to unload</param>
        public void UnloadAsset<T>(string path, bool isDel = false, UnityAction<T> callBack = null, bool isSub = true)
        {
            string resName = path + "_" + typeof(T).Name;
            // Check if the resource exists
            if (resDic.ContainsKey(resName))
            {
                ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;
                // Decrease reference count
                if (isSub)
                    resInfo.SubRefCount();
                // Record whether to remove the resource immediately when the reference count reaches 0
                resInfo.isDel = isDel;
                // If the resource is already loaded and the reference count is 0, remove it
                if (resInfo.asset != null && resInfo.refCount == 0 && resInfo.isDel)
                {
                    // Remove from the dictionary
                    resDic.Remove(resName);
                    // Unload the resource using the API
                    Resources.UnloadAsset(resInfo.asset as UnityEngine.Object);
                }
                else if (resInfo.asset == null) // If the resource is still being loaded asynchronously
                {
                    // Remove the callback record instead of directly unloading the resource
                    if (callBack != null)
                        resInfo.callBack -= callBack;
                }
            }
        }

        public void UnloadAsset(string path, Type type, bool isDel = false, UnityAction<UnityEngine.Object> callBack = null, bool isSub = true)
        {
            string resName = path + "_" + type.Name;
            // Check if the resource exists
            if (resDic.ContainsKey(resName))
            {
                ResInfo<UnityEngine.Object> resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
                // Decrease reference count
                if (isSub)
                    resInfo.SubRefCount();
                // Record whether to remove the resource immediately when the reference count reaches 0
                resInfo.isDel = isDel;
                // If the resource is already loaded and the reference count is 0, remove it
                if (resInfo.asset != null && resInfo.refCount == 0 && resInfo.isDel)
                {
                    // Remove from the dictionary
                    resDic.Remove(resName);
                    // Unload the resource using the API
                    Resources.UnloadAsset(resInfo.asset);
                }
                else if (resInfo.asset == null) // If the resource is still being loaded asynchronously
                {
                    // Remove the callback record instead of directly unloading the resource
                    if (callBack != null)
                        resInfo.callBack -= callBack;
                }
            }
        }

        /// <summary>
        /// Asynchronously unloads unused Resources-related assets.
        /// </summary>
        /// <param name="callBack">Callback function</param>
        public void UnloadUnusedAssets(UnityAction callBack)
        {
            MonoManager.Instance.StartCoroutine(ReallyUnloadUnusedAssets(callBack));
        }

        private IEnumerator ReallyUnloadUnusedAssets(UnityAction callBack)
        {
            // Before unloading unused resources, remove records of resources with a reference count of 0
            List<string> list = new List<string>();
            foreach (string path in resDic.Keys)
            {
                if (resDic[path].refCount == 0)
                    list.Add(path);
            }
            foreach (string path in list)
            {
                resDic.Remove(path);
            }

            AsyncOperation ao = Resources.UnloadUnusedAssets();
            yield return ao;
            // Notify the outside after unloading is complete
            callBack();
        }

        /// <summary>
        /// Gets the reference count of a specific resource.
        /// </summary>
        /// <typeparam name="T">Resource type</typeparam>
        /// <param name="path">Resource path</param>
        /// <returns>Reference count</returns>
        public int GetRefCount<T>(string path)
        {
            string resName = path + "_" + typeof(T).Name;
            if (resDic.ContainsKey(resName))
            {
                return (resDic[resName] as ResInfo<T>).refCount;
            }
            return 0;
        }

        /// <summary>
        /// Clears the dictionary.
        /// </summary>
        /// <param name="callBack">Callback function</param>
        public void ClearDic(UnityAction callBack)
        {
            MonoManager.Instance.StartCoroutine(ReallyClearDic(callBack));
        }

        private IEnumerator ReallyClearDic(UnityAction callBack)
        {
            resDic.Clear();
            AsyncOperation ao = Resources.UnloadUnusedAssets();
            yield return ao;
            // Notify the outside after clearing is complete
            callBack();
        }
    }
}
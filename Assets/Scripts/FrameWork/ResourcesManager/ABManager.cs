using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    public class ABManager : SingletonMono<ABManager>
    {
        // Main AssetBundle
        private AssetBundle mainAB = null;
        // Manifest for dependency resolution
        private AssetBundleManifest manifest = null;

        // Container to store loaded AssetBundles
        // AssetBundles cannot be loaded repeatedly, otherwise it will cause errors
        // Dictionary is used to store AssetBundle objects
        private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// Gets the path for loading AssetBundles.
        /// </summary>
        private string PathUrl
        {
            get
            {
                return Application.streamingAssetsPath + "/";
            }
        }

        /// <summary>
        /// Main AssetBundle name, which varies by platform.
        /// </summary>
        private string MainName
        {
            get
            {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
                return "PC";
#endif
            }
        }

        /// <summary>
        /// Loads the main AssetBundle and its manifest.
        /// This is necessary because dependency information is required when loading any AssetBundle.
        /// </summary>
        private void LoadMainAB()
        {
            if (mainAB == null)
            {
                mainAB = AssetBundle.LoadFromFile(PathUrl + MainName);
                manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
        }

        /// <summary>
        /// Loads the dependencies of a specified AssetBundle.
        /// </summary>
        /// <param name="abName">The name of the AssetBundle.</param>
        private void LoadDependencies(string abName)
        {
            // Load the main AssetBundle
            LoadMainAB();
            // Get all dependencies
            string[] strs = manifest.GetAllDependencies(abName);
            for (int i = 0; i < strs.Length; i++)
            {
                if (!abDic.ContainsKey(strs[i]))
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                    abDic.Add(strs[i], ab);
                }
            }
        }

        /// <summary>
        /// Asynchronously loads a resource of a specified type from an AssetBundle.
        /// </summary>
        /// <typeparam name="T">The type of the resource to load.</typeparam>
        /// <param name="abName">The name of the AssetBundle.</param>
        /// <param name="resName">The name of the resource.</param>
        /// <param name="callBack">Callback function invoked when the resource is loaded.</param>
        /// <param name="isSync">Whether to load synchronously.</param>
        public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isSync = false) where T : Object
        {
            StartCoroutine(ReallyLoadResAsync<T>(abName, resName, callBack, isSync));
        }

        // The actual coroutine for loading resources asynchronously
        private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isSync) where T : Object
        {
            // Load the main AssetBundle
            LoadMainAB();
            // Get all dependencies
            string[] strs = manifest.GetAllDependencies(abName);
            for (int i = 0; i < strs.Length; i++)
            {
                // If the AssetBundle has not been loaded yet
                if (!abDic.ContainsKey(strs[i]))
                {
                    // Synchronous loading
                    if (isSync)
                    {
                        AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                        abDic.Add(strs[i], ab);
                    }
                    // Asynchronous loading
                    else
                    {
                        // Add a null entry to indicate that the AssetBundle is being loaded asynchronously
                        abDic.Add(strs[i], null);
                        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + strs[i]);
                        yield return req;
                        // Replace the null entry with the loaded AssetBundle
                        abDic[strs[i]] = req.assetBundle;
                    }
                }
                // If the AssetBundle is already being loaded
                else
                {
                    // If the entry is null, it means the AssetBundle is still being loaded
                    // Wait until it is fully loaded
                    while (abDic[strs[i]] == null)
                    {
                        yield return 0;
                    }
                }
            }
            // Load the target AssetBundle
            if (!abDic.ContainsKey(abName))
            {
                // Synchronous loading
                if (isSync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                    abDic.Add(abName, ab);
                }
                // Asynchronous loading
                else
                {
                    // Add a null entry to indicate that the AssetBundle is being loaded asynchronously
                    abDic.Add(abName, null);
                    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                    yield return req;
                    // Replace the null entry with the loaded AssetBundle
                    abDic[abName] = req.assetBundle;
                }
            }
            else
            {
                // If the entry is null, it means the AssetBundle is still being loaded
                // Wait until it is fully loaded
                while (abDic[abName] == null)
                {
                    yield return 0;
                }
            }

            // Synchronously load the resource from the AssetBundle
            if (isSync)
            {
                T res = abDic[abName].LoadAsset<T>(resName);
                callBack(res);
            }
            // Asynchronously load the resource from the AssetBundle
            else
            {
                AssetBundleRequest abq = abDic[abName].LoadAssetAsync<T>(resName);
                yield return abq;

                callBack(abq.asset as T);
            }
        }

        /// <summary>
        /// Asynchronously loads a resource of a specified type from an AssetBundle.
        /// </summary>
        /// <param name="abName">The name of the AssetBundle.</param>
        /// <param name="resName">The name of the resource.</param>
        /// <param name="type">The type of the resource.</param>
        /// <param name="callBack">Callback function invoked when the resource is loaded.</param>
        /// <param name="isSync">Whether to load synchronously.</param>
        public void LoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack, bool isSync = false)
        {
            StartCoroutine(ReallyLoadResAsync(abName, resName, type, callBack, isSync));
        }

        private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack, bool isSync)
        {
            // Load the main AssetBundle
            LoadMainAB();
            // Get all dependencies
            string[] strs = manifest.GetAllDependencies(abName);
            for (int i = 0; i < strs.Length; i++)
            {
                // If the AssetBundle has not been loaded yet
                if (!abDic.ContainsKey(strs[i]))
                {
                    // Synchronous loading
                    if (isSync)
                    {
                        AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                        abDic.Add(strs[i], ab);
                    }
                    // Asynchronous loading
                    else
                    {
                        // Add a null entry to indicate that the AssetBundle is being loaded asynchronously
                        abDic.Add(strs[i], null);
                        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + strs[i]);
                        yield return req;
                        // Replace the null entry with the loaded AssetBundle
                        abDic[strs[i]] = req.assetBundle;
                    }
                }
                // If the AssetBundle is already being loaded
                else
                {
                    // If the entry is null, it means the AssetBundle is still being loaded
                    // Wait until it is fully loaded
                    while (abDic[strs[i]] == null)
                    {
                        yield return 0;
                    }
                }
            }
            // Load the target AssetBundle
            if (!abDic.ContainsKey(abName))
            {
                // Synchronous loading
                if (isSync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                    abDic.Add(abName, ab);
                }
                // Asynchronous loading
                else
                {
                    // Add a null entry to indicate that the AssetBundle is being loaded asynchronously
                    abDic.Add(abName, null);
                    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                    yield return req;
                    // Replace the null entry with the loaded AssetBundle
                    abDic[abName] = req.assetBundle;
                }
            }
            else
            {
                // If the entry is null, it means the AssetBundle is still being loaded
                // Wait until it is fully loaded
                while (abDic[abName] == null)
                {
                    yield return 0;
                }
            }

            // Synchronously load the resource from the AssetBundle
            if (isSync)
            {
                Object res = abDic[abName].LoadAsset(resName, type);
                callBack(res);
            }
            // Asynchronously load the resource from the AssetBundle
            else
            {
                AssetBundleRequest abq = abDic[abName].LoadAssetAsync(resName, type);
                yield return abq;

                callBack(abq.asset);
            }
        }

        /// <summary>
        /// Asynchronously loads a resource by name from an AssetBundle.
        /// </summary>
        /// <param name="abName">The name of the AssetBundle.</param>
        /// <param name="resName">The name of the resource.</param>
        /// <param name="callBack">Callback function invoked when the resource is loaded.</param>
        /// <param name="isSync">Whether to load synchronously.</param>
        public void LoadResAsync(string abName, string resName, UnityAction<Object> callBack, bool isSync = false)
        {
            StartCoroutine(ReallyLoadResAsync(abName, resName, callBack, isSync));
        }

        private IEnumerator ReallyLoadResAsync(string abName, string resName, UnityAction<Object> callBack, bool isSync)
        {
            // Load the main AssetBundle
            LoadMainAB();
            // Get all dependencies
            string[] strs = manifest.GetAllDependencies(abName);
            for (int i = 0; i < strs.Length; i++)
            {
                // If the AssetBundle has not been loaded yet
                if (!abDic.ContainsKey(strs[i]))
                {
                    // Synchronous loading
                    if (isSync)
                    {
                        AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                        abDic.Add(strs[i], ab);
                    }
                    // Asynchronous loading
                    else
                    {
                        // Add a null entry to indicate that the AssetBundle is being loaded asynchronously
                        abDic.Add(strs[i], null);
                        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + strs[i]);
                        yield return req;
                        // Replace the null entry with the loaded AssetBundle
                        abDic[strs[i]] = req.assetBundle;
                    }
                }
                // If the AssetBundle is already being loaded
                else
                {
                    // If the entry is null, it means the AssetBundle is still being loaded
                    // Wait until it is fully loaded
                    while (abDic[strs[i]] == null)
                    {
                        yield return 0;
                    }
                }
            }
            // Load the target AssetBundle
            if (!abDic.ContainsKey(abName))
            {
                // Synchronous loading
                if (isSync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                    abDic.Add(abName, ab);
                }
                // Asynchronous loading
                else
                {
                    // Add a null entry to indicate that the AssetBundle is being loaded asynchronously
                    abDic.Add(abName, null);
                    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                    yield return req;
                    // Replace the null entry with the loaded AssetBundle
                    abDic[abName] = req.assetBundle;
                }
            }
            else
            {
                // If the entry is null, it means the AssetBundle is still being loaded
                // Wait until it is fully loaded
                while (abDic[abName] == null)
                {
                    yield return 0;
                }
            }

            // Synchronously load the resource from the AssetBundle
            if (isSync)
            {
                Object obj = abDic[abName].LoadAsset(resName);
                callBack(obj);
            }
            // Asynchronously load the resource from the AssetBundle
            else
            {
                AssetBundleRequest abq = abDic[abName].LoadAssetAsync(resName);
                yield return abq;

                callBack(abq.asset);
            }
        }

        /// <summary>
        /// Unloads a specified AssetBundle.
        /// </summary>
        /// <param name="name">The name of the AssetBundle.</param>
        /// <param name="callBackResult">Callback function invoked with the result of the unload operation.</param>
        public void UnLoadAB(string name, UnityAction<bool> callBackResult)
        {
            if (abDic.ContainsKey(name))
            {
                if (abDic[name] == null)
                {
                    // If the AssetBundle is still being loaded, the unload operation fails
                    callBackResult(false);
                    return;
                }
                abDic[name].Unload(false);
                abDic.Remove(name);
                // Unload successful
                callBackResult(true);
            }
        }

        /// <summary>
        /// Clears all loaded AssetBundles.
        /// </summary>
        public void ClearAB()
        {
            // Stop all coroutines since AssetBundles are loaded asynchronously
            StopAllCoroutines();
            AssetBundle.UnloadAllAssetBundles(false);
            abDic.Clear();
            // Unload the main AssetBundle
            mainAB = null;
        }
    }
}
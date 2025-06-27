using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Framework
{
    /// <summary>
    /// Editor resource manager.
    /// Note: This manager can only be used during development to load resources for development purposes.
    /// It cannot be used after release because it relies on editor-related functionality.
    /// </summary>
    public class EditorResManager : Singleton<EditorResManager>
    {
        // Path for resources that need to be packed into AssetBundles
        private string rootPath = "Assets/Editor/";

        private EditorResManager() { }

        // 1. Load a single resource
        public T LoadEditorRes<T>(string path) where T : Object
        {
#if UNITY_EDITOR
            string[] suffixNames = null;
            if (typeof(T) == typeof(GameObject))
                suffixNames = new[] { ".prefab" };
            else if (typeof(T) == typeof(Material))
                suffixNames = new[] { ".mat" };
            else if (typeof(T) == typeof(Texture))
                suffixNames = new[] { ".png", ".jpg", ".jpeg", ".tga" };
            else if (typeof(T) == typeof(AudioClip))
                suffixNames = new[] { ".mp3", ".wav", ".ogg", ".aiff" };

            if (suffixNames == null)
            {
                Debug.LogError($"Unsupported resource type: {typeof(T)}");
                return null;
            }

            // foreach every suffixName to find the right one
            foreach (var suffixName in suffixNames)
            {
                string fullPath = rootPath + path + suffixName;
                T res = AssetDatabase.LoadAssetAtPath<T>(fullPath);
                if (res != null)
                    return res;
            }

            Debug.LogError($"Failed to load resource at path: {path}, type: {typeof(T)}");
            return null;
#else
        return null;
#endif
        }

        // 2. Load resources related to sprite atlases
        public Sprite LoadSprite(string path, string spriteName)
        {
#if UNITY_EDITOR
            // Load all sub-assets in the sprite atlas
            Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(rootPath + path);
            // Iterate through all sub-assets and return the one with the matching name
            foreach (var item in sprites)
            {
                if (spriteName == item.name)
                    return item as Sprite;
            }
            return null;
#else
        return null;
#endif
        }

        // Load all sub-images in the sprite atlas file and return them to the caller
        public Dictionary<string, Sprite> LoadSprites(string path)
        {
#if UNITY_EDITOR
            Dictionary<string, Sprite> spriteDic = new Dictionary<string, Sprite>();
            Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(rootPath + path);
            foreach (var item in sprites)
            {
                spriteDic.Add(item.name, item as Sprite);
            }
            return spriteDic;
#else
        return null;
#endif
        }
    }
}
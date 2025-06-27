using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public enum E_GameMode
    {
        DEBUG,
        RELEASE,
    }
    public class GlobalManager : Singleton<GlobalManager>
    {
        public static E_GameMode GameMode;
        private GlobalManager()
        {
#if UNITY_EDITOR
            GameMode = E_GameMode.DEBUG;
#else
            GameMode = GAMEMODE.RELEASE;
#endif // UNITY_EDITOR
        }
    }
}

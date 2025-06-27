using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class BasePanel : MonoBehaviour
    {
        [HideInInspector]
        public string panelName;
        private bool isActive = false;
        public bool IsActive
        {
            get
            {
                return isActive;
            }
        }
        public virtual void ShowPanel()
        {

        }
        public virtual void HidePanel()
        {

        }
    }
}
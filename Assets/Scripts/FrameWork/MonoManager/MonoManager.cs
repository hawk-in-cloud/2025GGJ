using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// Singleton MonoBehaviour manager that facilitates updating logic and coroutine execution
    /// for non-MonoBehaviour scripts.
    /// </summary>
    public class MonoManager : SingletonMono<MonoManager>
    {
        private event UnityAction updateEvent;
        private event UnityAction fixedUpdateEvent;
        private event UnityAction lateUpdateEvent;
        public void AddUpdateListener(UnityAction updateFunc)
        {
            updateEvent += updateFunc;
        }
        public void RemoveUpdateListener(UnityAction updateFunc)
        {
            updateEvent -= updateFunc;
        }
        public void AddFixedUpdateListener(UnityAction updateFunc)
        {
            fixedUpdateEvent += updateFunc;
        }
        public void RemoveFixedUpdateListener(UnityAction updateFunc)
        {
            fixedUpdateEvent -= updateFunc;
        }
        public void AddLateUpdateListener(UnityAction updateFunc)
        {
            lateUpdateEvent += updateFunc;
        }
        public void RemoveLateUpdateListener(UnityAction updateFunc)
        {
            lateUpdateEvent -= updateFunc;
        }
        private void Update()
        {
            updateEvent?.Invoke();
        }
        private void FixedUpdate()
        {
            fixedUpdateEvent?.Invoke();
        }
        private void LateUpdate()
        {
            lateUpdateEvent?.Invoke();
        }
    }
}

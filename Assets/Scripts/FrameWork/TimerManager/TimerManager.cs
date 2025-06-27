using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    public class TimerManager : SingletonMono<TimerManager>
    {
        private List<TimerItem> timers = new List<TimerItem>();
        private TimerManager() { }
        public void StartTimer(float delayTime,UnityAction action)
        {
            StartCoroutine(StartTimerTrue(delayTime,action));
        }
        public IEnumerator StartTimerTrue(float delayTime, UnityAction action)
        {


            yield return null;

            action?.Invoke();
        }
    }
}
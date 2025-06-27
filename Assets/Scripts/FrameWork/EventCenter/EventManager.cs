using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// Used for Liskov Substitution Principle, serves as the parent class for subclasses.
    /// </summary>
    public abstract class EventInfoBase {
        public abstract int GetSubscriberCount();
        public abstract List<string> GetSubscriberInfo();
    }

    /// <summary>
    /// Wrapper class for observer function delegates.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventInfo<T> : EventInfoBase
    {
        // Stores function information related to the actual observer.
        public UnityAction<T> actions;

        public EventInfo(UnityAction<T> action)
        {
            actions += action;
        }

        public override int GetSubscriberCount()
        {
            return actions.GetInvocationList().Length;
        }

        public override List<string> GetSubscriberInfo()
        {
            if (actions != null)
            {
                List<string> str = new List<string>();
                // 获取委托的字段（_invocationList）
                var invocationList = actions.GetInvocationList();
                foreach (var invoker in invocationList)
                {
                    // 获取每个订阅者的方法信息
                    MethodInfo methodInfo = invoker.Method;
                    str.Add("脚本名称: " + methodInfo.DeclaringType + " / 订阅方法: " + methodInfo.Name);
                }
                return str;
            }
            return null;
        }
    }

    /// <summary>
    /// Mainly used to record delegates with no parameters and no return value.
    /// </summary>
    public class EventInfo : EventInfoBase
    {
        public UnityAction actions;

        public EventInfo(UnityAction action)
        {
            actions += action;
        }

        public override int GetSubscriberCount()
        {
            return actions.GetInvocationList().Length;
        }

        public override List<string> GetSubscriberInfo()
        {
            if (actions != null)
            {
                List<string> str = new List<string>();
                // 获取委托的字段（_invocationList）
                var invocationList = actions.GetInvocationList();
                foreach (var invoker in invocationList)
                {
                    // 获取每个订阅者的方法信息
                    MethodInfo methodInfo = invoker.Method;
                    str.Add("脚本名称: " + methodInfo.DeclaringType + " / 订阅方法: " + methodInfo.Name);
                }
                return str;
            }
            return null;
        }
    }

    /// <summary>
    /// Event management module.
    /// </summary>
    public class EventManager : Singleton<EventManager>
    {
        // Dictionary to record event-related logic.
        private Dictionary<E_EventType, EventInfoBase> eventDic = new Dictionary<E_EventType, EventInfoBase>();
        public Dictionary<E_EventType, EventInfoBase> EventDic
        {
            get
            {
                return eventDic;
            }
        }

        private bool showDebugInfo = false;
        public bool ShowDebugInfo
        {
            get
            {
                return showDebugInfo;
            }
            set
            {
                showDebugInfo = value;
            }
        }

        private EventManager() { }

        /// <summary>
        /// Trigger an event with a parameter.
        /// </summary>
        /// <param name="eventName">The event name</param>
        public void EventTrigger<T>(E_EventType eventName, T info)
        {
            // Notify only if there are interested observers.
            if (eventDic.ContainsKey(eventName))
            {
                if (showDebugInfo)
                    UnityEngine.Debug.Log($"EventManager >>> EventTrigger -> EventType: {eventName.ToString()}");

                // Execute corresponding logic.
                (eventDic[eventName] as EventInfo<T>).actions?.Invoke(info);
            }
        }

        /// <summary>
        /// Trigger an event without parameters.
        /// </summary>
        /// <param name="eventName"></param>
        public void EventTrigger(E_EventType eventName)
        {
            // Notify only if there are interested observers.
            if (eventDic.ContainsKey(eventName))
            {
                if (showDebugInfo)
                    UnityEngine.Debug.Log($"EventManager >>> EventTrigger -> EventType: {eventName.ToString()}");

                // Execute corresponding logic.
                (eventDic[eventName] as EventInfo).actions?.Invoke();
            }
        }

        /// <summary>
        /// Add an event listener.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="func"></param>
        public void AddEventListener<T>(E_EventType eventName, UnityAction<T> func)
        {
            // If the event already exists, just add the listener.
            if (eventDic.ContainsKey(eventName))
            {
                if (showDebugInfo)
                {
                    StackTrace stackTrace = new StackTrace(); // 获取当前的堆栈信息
                    StackFrame currentFrame = stackTrace.GetFrame(1);  // 获取调用者的栈帧
                    string callingMethod = currentFrame.GetMethod().Name;  // 获取调用的函数名称
                    string callingClass = currentFrame.GetMethod().DeclaringType.Name;  // 获取调用者的类名

                    UnityEngine.Debug.Log($"EventManager >>> AddEventListener -> EventType: {eventName.ToString()}, " +
                              $"Called from: {callingClass}.{callingMethod}");
                }

                (eventDic[eventName] as EventInfo<T>).actions += func;
            }
            else
            {
                eventDic.Add(eventName, new EventInfo<T>(func));
            }
        }

        public void AddEventListener(E_EventType eventName, UnityAction func)
        {
            // If the event already exists, just add the listener.
            if (eventDic.ContainsKey(eventName))
            {
                if (showDebugInfo)
                {
                    StackTrace stackTrace = new StackTrace(); // 获取当前的堆栈信息
                    StackFrame currentFrame = stackTrace.GetFrame(1);  // 获取调用者的栈帧
                    string callingMethod = currentFrame.GetMethod().Name;  // 获取调用的函数名称
                    string callingClass = currentFrame.GetMethod().DeclaringType.Name;  // 获取调用者的类名

                    UnityEngine.Debug.Log($"EventManager >>> AddEventListener -> EventType: {eventName.ToString()}, " +
                              $"Called from: {callingClass}.{callingMethod}");
                }

                (eventDic[eventName] as EventInfo).actions += func;
            }
            else
            {
                eventDic.Add(eventName, new EventInfo(func));
            }
        }

        /// <summary>
        /// Remove an event listener.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="func"></param>
        public void RemoveEventListener<T>(E_EventType eventName, UnityAction<T> func)
        {
            if (eventDic.ContainsKey(eventName))
            {
                if (showDebugInfo)
                {
                    StackTrace stackTrace = new StackTrace(); // 获取当前的堆栈信息
                    StackFrame currentFrame = stackTrace.GetFrame(1);  // 获取调用者的栈帧
                    string callingMethod = currentFrame.GetMethod().Name;  // 获取调用的函数名称
                    string callingClass = currentFrame.GetMethod().DeclaringType.Name;  // 获取调用者的类名

                    UnityEngine.Debug.Log($"EventManager >>> RemoveEventListener -> EventType: {eventName.ToString()}, " +
                                          $"Called from: {callingClass}.{callingMethod}");
                }

                (eventDic[eventName] as EventInfo<T>).actions -= func;
            }
                
        }

        public void RemoveEventListener(E_EventType eventName, UnityAction func)
        {
            if (eventDic.ContainsKey(eventName))
            {
                if (showDebugInfo)
                {
                    StackTrace stackTrace = new StackTrace(); // 获取当前的堆栈信息
                    StackFrame currentFrame = stackTrace.GetFrame(1);  // 获取调用者的栈帧
                    string callingMethod = currentFrame.GetMethod().Name;  // 获取调用的函数名称
                    string callingClass = currentFrame.GetMethod().DeclaringType.Name;  // 获取调用者的类名

                    UnityEngine.Debug.Log($"EventManager >>> RemoveEventListener -> EventType: {eventName.ToString()}, " +
                                          $"Called from: {callingClass}.{callingMethod}");
                }

                (eventDic[eventName] as EventInfo).actions -= func;
            }
                
        }

        /// <summary>
        /// Clear all event listeners.
        /// </summary>
        public void Clear()
        {
            if (showDebugInfo)
                UnityEngine.Debug.Log("EventManager >> Clear EventDic.");

            eventDic.Clear();
        }

        /// <summary>
        /// Clear all listeners of a specific event.
        /// </summary>
        /// <param name="eventName"></param>
        public void Claer(E_EventType eventName)
        {
            if (eventDic.ContainsKey(eventName))
            {
                if (showDebugInfo)
                    UnityEngine.Debug.Log($"EventManager >> Clear EventListener -> EventType: {eventName.ToString()}.");

                eventDic.Remove(eventName);
            }          
        }
    }
}
using Firebase.Analytics;
using KTool.Cron;
using KTool.Init;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KPlugin.GoogleFirebase.Analytics
{
    public class AnalyticsControl : MonoBehaviour, IIniter
    {
        #region Properties
        private const string ERROR_LOG_EVENT_FAIL = "Firebase Analytics log event [{0}] fail: {1}";
        public static AnalyticsControl Instance
        {
            get;
            private set;
        }

        [SerializeField]
        private bool initIndispensable;
        [SerializeField]
        private UnityEvent onInited;

        private bool isAvailable;

        public bool IsAvailable => isAvailable;
        #endregion

        #region Unity Event
        private void OnDestroy()
        {
            if (Instance != null && Instance.GetInstanceID() == GetInstanceID())
                Instance = null;
        }
        #endregion

        #region Method

        #endregion

        #region Init
        public IInitTracking InitBegin()
        {
            if (Instance != null)
                return IInitTracking.Success;
            //
            Instance = this;
            isAvailable = false;
            //
            InitTrackingSource initTrackingSource = new InitTrackingSource(initIndispensable);
            CronObject.Create()
                .Add(ConditionFunc.Create(FirebaseManager.IsReady))
                .Add(CallbackAction.Create(AnalyticsInit, initTrackingSource))
                .Run();
            return initTrackingSource;
        }
        public void InitEnd()
        {

        }
        #endregion

        #region Analytics
        private void AnalyticsInit(InitTrackingSource initTrackingSource)
        {
            isAvailable = true;
            onInited?.Invoke();
            //
            initTrackingSource.CompleteSuccess();
        }
        #endregion

        #region Log Event
        public static void LogEvent(string eventName)
        {
            if (Instance == null || !Instance.IsAvailable)
                return;
            //
            try
            {
                FirebaseAnalytics.LogEvent(eventName);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(string.Format(ERROR_LOG_EVENT_FAIL, eventName, ex.Message));
            }
        }
        public static void LogEvent(string eventName, string parameterName, string value)
        {
            if (Instance == null || !Instance.IsAvailable)
                return;
            //
            try
            {
                FirebaseAnalytics.LogEvent(eventName, parameterName, value);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(string.Format(ERROR_LOG_EVENT_FAIL, eventName, ex.Message));
            }
        }
        public static void LogEvent(string eventName, string parameterName, int value)
        {
            if (Instance == null || !Instance.IsAvailable)
                return;
            //
            try
            {
                FirebaseAnalytics.LogEvent(eventName, parameterName, value);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(string.Format(ERROR_LOG_EVENT_FAIL, eventName, ex.Message));
            }
        }
        public static void LogEvent(string eventName, string parameterName, long value)
        {
            if (Instance == null || !Instance.IsAvailable)
                return;
            //
            try
            {
                FirebaseAnalytics.LogEvent(eventName, parameterName, value);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(string.Format(ERROR_LOG_EVENT_FAIL, eventName, ex.Message));
            }
        }
        public static void LogEvent(string eventName, string parameterName, float value)
        {
            if (Instance == null || !Instance.IsAvailable)
                return;
            //
            try
            {
                FirebaseAnalytics.LogEvent(eventName, parameterName, value);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(string.Format(ERROR_LOG_EVENT_FAIL, eventName, ex.Message));
            }
        }
        public static void LogEvent(string eventName, string parameterName, double value)
        {
            if (Instance == null || !Instance.IsAvailable)
                return;
            //
            try
            {
                FirebaseAnalytics.LogEvent(eventName, parameterName, value);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(string.Format(ERROR_LOG_EVENT_FAIL, eventName, ex.Message));
            }
        }
        public static void LogEvent(string eventName, Dictionary<string, object> dic)
        {
            if (Instance == null || !Instance.IsAvailable)
                return;
            if (dic == null)
                return;
            //
            try
            {
                Parameter[] parameters = new Parameter[dic.Count];
                int index = 0;
                foreach (var key in dic.Keys)
                {
                    object value = dic[key];
                    if (value is string valueString)
                        parameters[index] = new Parameter(key, valueString);
                    else if (value is int valueInt)
                        parameters[index] = new Parameter(key, valueInt);
                    else if (value is long valueLong)
                        parameters[index] = new Parameter(key, valueLong);
                    else if (value is float valueFloat)
                        parameters[index] = new Parameter(key, valueFloat);
                    else if (value is double valueDouble)
                        parameters[index] = new Parameter(key, valueDouble);
                    index++;
                }
                FirebaseAnalytics.LogEvent(eventName, parameters);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(string.Format(ERROR_LOG_EVENT_FAIL, eventName, ex.Message));
            }
        }
        #endregion
    }
}

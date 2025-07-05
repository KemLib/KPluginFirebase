using Firebase.Analytics;
using KTool.Init;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        private string userName,
            propertyName;

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
        public InitTracking InitBegin()
        {
            if (Instance != null)
                return InitTracking.Success;
            //
            Instance = this;
            isAvailable = false;
            //
            InitTrackingSource initTrackingSource = new InitTrackingSource(initIndispensable);
            StartCoroutine(Firebase_Init(initTrackingSource));
            return initTrackingSource;
        }
        public void InitEnd()
        {

        }
        #endregion

        #region Firebase
        private IEnumerator Firebase_Init(InitTrackingSource initTrackingSource)
        {
            while (!FirebaseManager.Instance.InitComplete)
                yield return new WaitForEndOfFrame();
            //
            if (FirebaseManager.Instance.IsAvailable)
            {
                isAvailable = true;
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(propertyName))
                    FirebaseAnalytics.SetUserProperty(userName, propertyName);
            }
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
            //
            try
            {
                Parameter[] parameters = new Parameter[dic.Count];
                int index = 0;
                foreach (var key in dic.Keys)
                {
                    object value = dic[key];
                    if (value is string)
                        parameters[index] = new Parameter(key, (string)value);
                    else if (value is int)
                        parameters[index] = new Parameter(key, (int)value);
                    else if (value is long)
                        parameters[index] = new Parameter(key, (long)value);
                    else if (value is float)
                        parameters[index] = new Parameter(key, (float)value);
                    else if (value is double)
                        parameters[index] = new Parameter(key, (double)value);
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

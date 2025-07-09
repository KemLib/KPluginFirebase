using Firebase.Extensions;
using Firebase.RemoteConfig;
using KLibStandard.Concurrent;
using KTool.Init;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace KPlugin.GoogleFirebase.RemoteConfig
{
    public class RemoteConfigControl : MonoBehaviour, IIniter
    {
        #region Properties
        private const string ERROR_FETCH_FAIL = "Firebase RemoteConfig fetch faiil",
            ERROR_ACTIVATE_FAIL = "Firebase RemoteConfig activate faill",
            ERROR_ACTIVATE_UPDATE_FAIL = "Firebase RemoteConfig activate update faill";
        private const string LOG_DATA_FORMAT = "Firebase RemoteConfig Key[{0}] - Type[{1}] - Value: {2}";
        public static RemoteConfigControl Instance
        {
            get;
            private set;
        }

        [SerializeField]
        private bool initIndispensable;
        [SerializeField]
        private RemoteConfigAssets[] assets;

        private bool isAvailable,
            isFetch;
        private InterValueBool isUpdateEnable,
            isUpdating;
        private FirebaseRemoteConfig instanceFirebaseRemoteConfig;
        public event UnityAction OnLoadData;

        public bool IsAvailable => isAvailable;
        public int Count => assets.Length;
        public RemoteConfigAssets this[int index] => assets[index];
        public RemoteConfigAssets this[string key]
        {
            get
            {
                foreach (RemoteConfigAssets data in assets)
                    if (data.Key == key)
                        return data;
                return null;
            }
        }
        public FirebaseRemoteConfig InstanceFirebaseRemoteConfig => instanceFirebaseRemoteConfig;
        #endregion

        #region Unity Event
        private void OnDestroy()
        {
            isUpdateEnable.Value = false;
            if (Instance != null && Instance.GetInstanceID() == GetInstanceID())
            {
                Instance = null;
                if (isFetch)
                    InstanceFirebaseRemoteConfig.OnConfigUpdateListener -= Firebase_OnConfigUpdateListener;
            }
        }
        #endregion

        #region Init
        public InitTracking InitBegin()
        {
            if (Instance != null)
                return InitTracking.Success;
            //
            Instance = this;
            isAvailable = false;
            isFetch = false;
            isUpdateEnable = new InterValueBool();
            isUpdating = new InterValueBool();
            //
            InitTrackingSource initTrackingSource = new InitTrackingSource(initIndispensable);
            StartCoroutine(Firebase_Init(initTrackingSource));
            return initTrackingSource;
        }
        public void InitEnd()
        {

        }
        #endregion

        #region Method
        public void DebugData()
        {
            foreach (RemoteConfigAssets data in assets)
            {
                string log;
                switch (data.DataType)
                {
                    case DataType.String:
                        log = string.Format(LOG_DATA_FORMAT, data.Key, data.DataType, data.ValueString);
                        break;
                    case DataType.Long:
                        log = string.Format(LOG_DATA_FORMAT, data.Key, data.DataType, data.ValueLong);
                        break;
                    case DataType.Double:
                        log = string.Format(LOG_DATA_FORMAT, data.Key, data.DataType, data.ValueDouble);
                        break;
                    case DataType.Boolean:
                        log = string.Format(LOG_DATA_FORMAT, data.Key, data.DataType, data.ValueBoolean);
                        break;
                    default:
                        log = string.Empty;
                        break;
                }
                Debug.Log(log);
            }
        }
        #endregion

        #region Firebase
        private IEnumerator Firebase_Init(InitTrackingSource initTrackingSource)
        {
            while (!FirebaseManager.Instance.IsInited)
                yield return new WaitForEndOfFrame();
            //
            if (!FirebaseManager.Instance.IsAvailable)
            {
                initTrackingSource.CompleteFail();
                yield break;
            }
            instanceFirebaseRemoteConfig = FirebaseRemoteConfig.DefaultInstance;
            //
            Dictionary<string, object> defaultData = Firebase_CreateDefaultData();
            while (true)
            {
                Task taskSetDefaultData = InstanceFirebaseRemoteConfig.SetDefaultsAsync(defaultData);
                while (!taskSetDefaultData.IsCompleted)
                    yield return new WaitForEndOfFrame();
                //
                if (taskSetDefaultData.IsCompletedSuccessfully)
                    break;
                else
                    yield return new WaitForSecondsRealtime(1);
            }
            isAvailable = true;
            //
            while(true)
            {
                Task taskFetch = InstanceFirebaseRemoteConfig.FetchAsync(System.TimeSpan.Zero);
                while (!taskFetch.IsCompleted)
                    yield return new WaitForEndOfFrame();
                //
                if(taskFetch.IsCompletedSuccessfully && InstanceFirebaseRemoteConfig.Info.LastFetchStatus == LastFetchStatus.Success)
                {
                    break;
                }
                else
                {
                    Debug.LogWarning(ERROR_FETCH_FAIL);
                    initTrackingSource.CompleteFail();
                    yield return new WaitForSecondsRealtime(5);
                }
            }
            //
            isFetch = true;
            InstanceFirebaseRemoteConfig.OnConfigUpdateListener += Firebase_OnConfigUpdateListener;
            //
            Task<bool> taskActivate = InstanceFirebaseRemoteConfig.ActivateAsync();
            while (!taskActivate.IsCompleted)
                yield return new WaitForEndOfFrame();
            //
            if (taskActivate.IsCompletedSuccessfully && taskActivate.Result)
            {
                foreach (RemoteConfigAssets data in assets)
                    data.DataUpdate();
                initTrackingSource?.CompleteSuccess();
                OnLoadData?.Invoke();
                isUpdateEnable.Value = true;
            }
            else
            {
                Debug.LogWarning(ERROR_ACTIVATE_FAIL);
                initTrackingSource?.CompleteFail();
                isUpdateEnable.Value = true;
                Firebase_OnConfigUpdate();
            }
        }
        private Dictionary<string, object> Firebase_CreateDefaultData()
        {
            Dictionary<string, object> defaultData = new Dictionary<string, object>();
            foreach (var data in assets)
            {
                if (string.IsNullOrEmpty(data.Key))
                    continue;
                switch (data.DataType)
                {
                    case DataType.String:
                        defaultData.Add(data.Key, data.ValueString);
                        break;
                    case DataType.Long:
                        defaultData.Add(data.Key, data.ValueLong);
                        break;
                    case DataType.Double:
                        defaultData.Add(data.Key, data.ValueDouble);
                        break;
                    case DataType.Boolean:
                        defaultData.Add(data.Key, data.ValueBoolean);
                        break;
                }
            }
            //
            return defaultData;
        }
        #endregion

        #region Firebase UpdateConfig
        private void Firebase_OnConfigUpdate()
        {
            if (!isUpdateEnable)
                return;
            if (!isUpdating.TryExchange(true))
                return;
            //
            InstanceFirebaseRemoteConfig.ActivateAsync()
                .ContinueWithOnMainThread(Firebase_OnConfigUpdateComplete);
        }
        private void Firebase_OnConfigUpdateListener(object sender, ConfigUpdateEventArgs args)
        {
            if (args.Error == RemoteConfigError.None || !isUpdateEnable)
                return;
            if (!isUpdating.TryExchange(true))
                return;
            //
            InstanceFirebaseRemoteConfig.ActivateAsync()
                .ContinueWithOnMainThread(Firebase_OnConfigUpdateComplete);
        }
        private void Firebase_OnConfigUpdateComplete(Task<bool> taskActivate)
        {
            if (!isUpdateEnable)
            {
                isUpdating.Value = false;
                return;
            }
            //
            if (taskActivate.IsCompletedSuccessfully && taskActivate.Result)
            {
                foreach (RemoteConfigAssets data in assets)
                    data.DataUpdate();
                OnLoadData?.Invoke();
                //
                isUpdating.Value = false;
            }
            else
            {
                Debug.LogWarning(ERROR_ACTIVATE_UPDATE_FAIL);
                StartCoroutine(Firebase_ConfigUpdate(5));
            }
        }
        private IEnumerator Firebase_ConfigUpdate(int delay)
        {
            if (delay > 0)
                yield return new WaitForSecondsRealtime(delay);
            //
            if (!isUpdateEnable)
            {
                isUpdating.Value = false;
                yield break;
            }
            //
            InstanceFirebaseRemoteConfig.ActivateAsync()
                .ContinueWithOnMainThread(Firebase_OnConfigUpdateComplete);
        }
        #endregion
    }
}

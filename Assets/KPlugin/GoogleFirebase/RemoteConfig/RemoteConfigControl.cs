using Firebase.Extensions;
using Firebase.RemoteConfig;
using KTool.Cron;
using KTool.Init;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace KPlugin.GoogleFirebase.RemoteConfig
{
    public class RemoteConfigControl : MonoBehaviour, IIniter
    {
        #region Properties
        private const string DEBUG_DATA_FORMAT = "Firebase RemoveConfig Key[{0}] - Type[{1}] - Value: {2}";
        private const string ERROR_SET_DEFAULT_FAIL = "Firebase RemoteConfig set default fail",
            ERROR_FETCH_FAIL = "Firebase RemoteConfig fetch fail";

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
            isUpdateListener;
        private FirebaseRemoteConfig instanceFirebaseRemoteConfig;
        public event UnityAction OnDataUpdate;

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
            if (Instance != null && Instance.GetInstanceID() == GetInstanceID())
            {
                Instance = null;
                if (isUpdateListener)
                {
                    isUpdateListener = false;
                    InstanceFirebaseRemoteConfig.OnConfigUpdateListener -= Firebase_OnConfigUpdateListener;
                }
            }
        }
        #endregion

        #region Init
        public IInitTracking InitBegin()
        {
            if (Instance != null)
                return IInitTracking.Success;
            //
            Instance = this;
            isAvailable = false;
            isUpdateListener = false;
            //
            InitTrackingSource initTrackingSource = new InitTrackingSource(initIndispensable);
            CronObject.Create()
                .Add(ConditionFunc.Create(FirebaseManager.IsReady))
                .Add(CallbackAction.Create(RemoteConfig_Init, initTrackingSource))
                .Run();
            return initTrackingSource;
        }
        public void InitEnd()
        {

        }
        #endregion

        #region Firebase
        private void RemoteConfig_Init(InitTrackingSource initTrackingSource)
        {
            instanceFirebaseRemoteConfig = FirebaseRemoteConfig.DefaultInstance;
            //
            RemoteConfigAsset_Init();
            Dictionary<string, object> defaultData = RemoteConfigAsset_CreateDefaultData();
            SetDefaults_Begin(initTrackingSource, defaultData);
        }
        private void SetDefaults_Begin(InitTrackingSource initTrackingSource, Dictionary<string, object> defaultData)
        {
            Task taskSetDefaultData = InstanceFirebaseRemoteConfig.SetDefaultsAsync(defaultData);
            CronObject.Create()
                .Add(ConditionTask.Create(taskSetDefaultData))
                .Add(CallbackAction.Create(SetDefaults_Oncomplete, taskSetDefaultData, initTrackingSource, defaultData))
                .Run();
        }
        private void SetDefaults_Oncomplete(Task taskSetDefaultData, InitTrackingSource initTrackingSource, Dictionary<string, object> defaultData)
        {
            if (taskSetDefaultData.IsCompletedSuccessfully)
            {
                Fetch_Begin(initTrackingSource);
            }
            else
            {
                Debug.LogWarning(ERROR_SET_DEFAULT_FAIL);
                //
                CronObject.Create()
                    .Add(ConditionFrame.Create(1))
                    .Add(CallbackAction.Create(SetDefaults_Begin, initTrackingSource, defaultData))
                    .Run();
            }
        }
        private void Fetch_Begin(InitTrackingSource initTrackingSource)
        {
            Task taskFetch = InstanceFirebaseRemoteConfig.FetchAsync(System.TimeSpan.Zero);
            CronObject.Create()
                .Add(ConditionTask.Create(taskFetch))
                .Add(CallbackAction.Create(Fetch_OnComplete, taskFetch, initTrackingSource))
                .Run();
        }
        private void Fetch_OnComplete(Task taskFetch, InitTrackingSource initTrackingSource)
        {
            if (taskFetch.IsCompletedSuccessfully && InstanceFirebaseRemoteConfig.Info.LastFetchStatus == LastFetchStatus.Success)
            {
                Activate_Begin(initTrackingSource);
            }
            else
            {
                Debug.LogWarning(ERROR_FETCH_FAIL);
                //
                CronObject.Create()
                    .Add(ConditionFrame.Create(1))
                    .Add(CallbackAction.Create(Fetch_Begin, initTrackingSource))
                    .Run();
            }
        }
        private void Activate_Begin(InitTrackingSource initTrackingSource)
        {
            Task<bool> taskActivate = InstanceFirebaseRemoteConfig.ActivateAsync();
            CronObject.Create()
                .Add(ConditionTask.Create(taskActivate))
                .Add(CallbackAction.Create(Activate_OnComplete, initTrackingSource))
                .Run();
        }
        private void Activate_OnComplete(InitTrackingSource initTrackingSource)
        {
            isAvailable = true;
            RemoteConfigAsset_Update();
            OnDataUpdate?.Invoke();
            RemoteConfigAsset_DebugData();
            //
            isUpdateListener = true;
            InstanceFirebaseRemoteConfig.OnConfigUpdateListener += Firebase_OnConfigUpdateListener;
            //
            initTrackingSource?.CompleteSuccess();
        }
        #endregion

        #region Firebase UpdateConfig
        private void Firebase_OnConfigUpdateListener(object sender, ConfigUpdateEventArgs args)
        {
            if (args.Error == RemoteConfigError.None)
                return;
            //
            InstanceFirebaseRemoteConfig.ActivateAsync()
                .ContinueWithOnMainThread(Firebase_OnConfigUpdateComplete);
        }
        private void Firebase_OnConfigUpdateComplete(Task<bool> taskActivate)
        {
            RemoteConfigAsset_Update();
            OnDataUpdate?.Invoke();
            RemoteConfigAsset_DebugData();
        }
        #endregion

        #region RemoteConfigAsset
        private void RemoteConfigAsset_Init()
        {
            foreach (var item in assets)
                item.DataInit();
        }
        private void RemoteConfigAsset_Update()
        {
            foreach (RemoteConfigAssets data in assets)
                data.DataUpdate(InstanceFirebaseRemoteConfig);
        }
        private void RemoteConfigAsset_DebugData()
        {
            foreach (var item in assets)
            {
                string value;
                switch (item.DataType)
                {
                    case DataType.String:
                        value = item.ValueString;
                        break;
                    case DataType.Long:
                        value = item.ValueLong.ToString();
                        break;
                    case DataType.Double:
                        value = item.ValueDouble.ToString();
                        break;
                    case DataType.Boolean:
                        value = item.ValueBoolean.ToString();
                        break;
                    default:
                        value = string.Empty;
                        break;
                }
                string log = string.Format(DEBUG_DATA_FORMAT, item.Key, item.DataType, value);
                Debug.Log(log);
            }
        }
        private Dictionary<string, object> RemoteConfigAsset_CreateDefaultData()
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
    }
}

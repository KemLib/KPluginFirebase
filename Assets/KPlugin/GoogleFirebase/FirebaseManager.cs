using Firebase;
using KTool.Cron;
using KTool.Init;
using System.Threading.Tasks;
using UnityEngine;

namespace KPlugin.GoogleFirebase
{
    public class FirebaseManager : MonoBehaviour, IIniter
    {
        #region Properties
        private const string ERROR_INIT_FAIL = "Firebase Manager init fail: {0}";

        public static FirebaseManager Instance
        {
            get;
            private set;
        }

        [SerializeField]
        private bool initIndispensable;

        private FirebaseApp fbApp;

        public bool IsAvailable => FbApp != null;
        public FirebaseApp FbApp => fbApp;
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
            DontDestroyOnLoad(gameObject);
            //
            InitTrackingSource initTrackingSource = new InitTrackingSource(initIndispensable);
            Task<DependencyStatus> task = FirebaseApp.CheckAndFixDependenciesAsync();
            CronObject.Create()
                .Add(ConditionTask.Create(task))
                .Add(CallbackAction.Create(FirebaseInit_OnComplete, task, initTrackingSource))
                .Run();
            return initTrackingSource;
        }

        public void InitEnd()
        {

        }
        #endregion

        #region Firebase
        public static bool IsReady()
        {
            return Instance != null && Instance.IsAvailable;
        }
        private void FirebaseInit_OnComplete(Task<DependencyStatus> task, InitTrackingSource initTrackingSource)
        {
            if (task.IsCompletedSuccessfully && task.Result == DependencyStatus.Available)
            {
                fbApp = FirebaseApp.DefaultInstance;
            }
            else
            {
                Debug.LogWarning(string.Format(ERROR_INIT_FAIL, task.Result.ToString()));
                fbApp = null;
            }
            //
            initTrackingSource.CompleteSuccess();
        }
        #endregion
    }
}

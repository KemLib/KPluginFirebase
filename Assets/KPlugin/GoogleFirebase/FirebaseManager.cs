using Firebase;
using KTool.Init;
using System.Collections;
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

        private bool isInited;
        private FirebaseApp fbApp;

        public bool IsInited => isInited;
        public FirebaseApp FbApp => fbApp;
        public bool IsAvailable => fbApp != null;
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
            DontDestroyOnLoad(gameObject);
            //
            InitTrackingSource initTrackingSource = new InitTrackingSource(true);
            StartCoroutine(Firebase_IE_Init(initTrackingSource));
            return initTrackingSource;
        }

        public void InitEnd()
        {

        }
        #endregion

        #region Firebase
        private IEnumerator Firebase_IE_Init(InitTrackingSource initTrackingSource)
        {
            Task<DependencyStatus> task = FirebaseApp.CheckAndFixDependenciesAsync();
            while (!task.IsCompleted)
                yield return new WaitForEndOfFrame();
            //
            if (task.IsCompletedSuccessfully && task.Result == DependencyStatus.Available)
            {
                fbApp = FirebaseApp.DefaultInstance;
            }
            else
            {
                Debug.LogWarning(string.Format(ERROR_INIT_FAIL, task.Result.ToString()));
                fbApp = null;
            }
            isInited = true;
            initTrackingSource.CompleteSuccess();
        }
        #endregion
    }
}

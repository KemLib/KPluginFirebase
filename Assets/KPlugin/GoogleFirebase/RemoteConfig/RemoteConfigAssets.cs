using Firebase.RemoteConfig;
using UnityEngine;

namespace KPlugin.GoogleFirebase.RemoteConfig
{
    [CreateAssetMenu(fileName = "RemoteConfigAssets", menuName = "KPlugin/Firebase/Create RemoteConfigAssets")]
    public class RemoteConfigAssets : ScriptableObject
    {
        #region Properties
        [SerializeField]
        private string key;
        [SerializeField]
        private DataType dataType;
        [SerializeField]
        private string valueString;
        [SerializeField]
        private long valueLong;
        [SerializeField]
        private double valueDouble;
        [SerializeField]
        private bool valueBoolean;

        public string Key => key;
        public DataType DataType => dataType;
        public string ValueString => valueString;
        public long ValueLong => valueLong;
        public int ValueInt => (int)valueLong;
        public double ValueDouble => valueDouble;
        public float ValueFloat => (float)valueDouble;
        public bool ValueBoolean => valueBoolean;
        #endregion

        #region Method
        public void DataUpdate()
        {
            RemoteConfigControl firebaseInstance = RemoteConfigControl.Instance;
            if (firebaseInstance == null || !firebaseInstance.IsAvailable)
                return;
            FirebaseRemoteConfig instance = firebaseInstance.InstanceFirebaseRemoteConfig;
            switch (dataType)
            {
                case DataType.String:
                    valueString = instance.GetValue(Key).StringValue;
                    break;
                case DataType.Long:
                    valueLong = instance.GetValue(Key).LongValue;
                    break;
                case DataType.Double:
                    valueDouble = instance.GetValue(Key).DoubleValue;
                    break;
                case DataType.Boolean:
                    valueBoolean = instance.GetValue(Key).BooleanValue;
                    break;
            }
        }
        #endregion
    }
}

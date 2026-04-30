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
        private string defaultValueString;
        [SerializeField]
        private long defaultValueLong;
        [SerializeField]
        private double defaultValueDouble;
        [SerializeField]
        private bool defaultValueBoolean;

        private string valueString;
        private long valueLong;
        private double valueDouble;
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
        public void DataInit()
        {
            valueString = defaultValueString;
            valueLong = defaultValueLong;
            valueDouble = defaultValueDouble;
            valueBoolean = defaultValueBoolean;
        }
        public void DataUpdate(FirebaseRemoteConfig instance)
        {
            if (instance == null)
                return;
            //
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

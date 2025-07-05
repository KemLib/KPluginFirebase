using UnityEditor;
using UnityEngine;

namespace KPlugin.GoogleFirebase.RemoteConfig.Editor
{
    [CustomEditor(typeof(RemoteConfigAssets))]
    public class DataConfigDrawer : UnityEditor.Editor
    {
        #region Properties
        private SerializedProperty propertyKey,
            propertyDataType,
            propertyValueString,
            propertyValueLong,
            propertyValueDouble,
            propertyValueBoolean;
        #endregion Properties

        #region Unity Event
        private void OnEnable()
        {
            propertyKey = serializedObject.FindProperty("key");
            propertyDataType = serializedObject.FindProperty("dataType");
            propertyValueString = serializedObject.FindProperty("valueString");
            propertyValueLong = serializedObject.FindProperty("valueLong");
            propertyValueDouble = serializedObject.FindProperty("valueDouble");
            propertyValueBoolean = serializedObject.FindProperty("valueBoolean");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            //
            EditorGUILayout.PropertyField(propertyKey, new GUIContent("Key"));
            EditorGUILayout.PropertyField(propertyDataType, new GUIContent("Data Type"));
            DataType dataType = (DataType)propertyDataType.enumValueIndex;
            switch (dataType)
            {
                case DataType.String:
                    EditorGUILayout.PropertyField(propertyValueString, new GUIContent("Value String"));
                    break;
                case DataType.Long:
                    EditorGUILayout.PropertyField(propertyValueLong, new GUIContent("Value Long"));
                    break;
                case DataType.Double:
                    EditorGUILayout.PropertyField(propertyValueDouble, new GUIContent("Value Double"));
                    break;
                case DataType.Boolean:
                    EditorGUILayout.PropertyField(propertyValueBoolean, new GUIContent("Value Boolean"));
                    break;
            }
            //
            serializedObject.ApplyModifiedProperties();
        }
        #endregion Unity Event
    }
}

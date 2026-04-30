using UnityEditor;
using UnityEngine;

namespace KPlugin.GoogleFirebase.RemoteConfig.Editor
{
    [CustomEditor(typeof(RemoteValue))]
    public class RemoteValueEditor : UnityEditor.Editor
    {
        #region Properties
        private SerializedProperty propertyIsHide,
            propertyKey,
            propertyDataType,
            propertyDefaultValueString,
            propertyDefaultValueLong,
            propertyDefaultValueDouble,
            propertyDefaultValueBoolean;
        private RemoteValue asset;
        #endregion Properties

        #region Unity Event
        private void OnEnable()
        {
            asset = serializedObject.targetObject as RemoteValue;
            propertyIsHide = serializedObject.FindProperty("isHide");
            propertyKey = serializedObject.FindProperty("key");
            propertyDataType = serializedObject.FindProperty("dataType");
            propertyDefaultValueString = serializedObject.FindProperty("defaultValueString");
            propertyDefaultValueLong = serializedObject.FindProperty("defaultValueLong");
            propertyDefaultValueDouble = serializedObject.FindProperty("defaultValueDouble");
            propertyDefaultValueBoolean = serializedObject.FindProperty("defaultValueBoolean");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            //
            EditorGUILayout.PropertyField(propertyIsHide, new GUIContent("IsHide"));
            EditorGUILayout.PropertyField(propertyKey, new GUIContent("Key"));
            EditorGUILayout.PropertyField(propertyDataType, new GUIContent("Data Type"));
            DataType dataType = (DataType)propertyDataType.enumValueIndex;
            switch (dataType)
            {
                case DataType.String:
                    EditorGUILayout.PropertyField(propertyDefaultValueString, new GUIContent("Default Value String"));
                    break;
                case DataType.Long:
                    EditorGUILayout.PropertyField(propertyDefaultValueLong, new GUIContent("Default Value Long"));
                    break;
                case DataType.Double:
                    EditorGUILayout.PropertyField(propertyDefaultValueDouble, new GUIContent("Default Value Double"));
                    break;
                case DataType.Boolean:
                    EditorGUILayout.PropertyField(propertyDefaultValueBoolean, new GUIContent("Default Value Boolean"));
                    break;
            }
            //
            EditorGUI.BeginDisabledGroup(true);
            switch (dataType)
            {
                case DataType.String:
                    EditorGUILayout.TextField(new GUIContent("Value String"), asset.ValueString);
                    break;
                case DataType.Long:
                    EditorGUILayout.LongField(new GUIContent("Value Long"), asset.ValueLong);
                    break;
                case DataType.Double:
                    EditorGUILayout.DoubleField(new GUIContent("Value Double"), asset.ValueDouble);
                    break;
                case DataType.Boolean:
                    EditorGUILayout.Toggle(new GUIContent("Value Boolean"), asset.ValueBoolean);
                    break;
            }
            EditorGUI.EndDisabledGroup();
            //
            serializedObject.ApplyModifiedProperties();
        }
        #endregion Unity Event
    }
}

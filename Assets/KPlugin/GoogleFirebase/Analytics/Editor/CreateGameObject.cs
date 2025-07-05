using UnityEditor;
using UnityEngine;

namespace KPlugin.GoogleFirebase.Analytics.Editor
{
    public static class CreateGameObject
    {
        #region Properties
        private const string GAME_OBJECT_NAME_ANALYTICS = "KPLugin_Firebase_Analytics";
        #endregion

        #region Methods
        [MenuItem("GameObject/KPLugin/Firebase/Create Analytics", priority = 1)]
        private static void Create_Analytics()
        {
            GameObject newGO = new GameObject(GAME_OBJECT_NAME_ANALYTICS);
            newGO.AddComponent<AnalyticsControl>();
            //
            if (Selection.activeTransform != null)
                newGO.transform.SetParent(Selection.activeTransform);
            Selection.activeGameObject = newGO;
        }
        #endregion
    }
}

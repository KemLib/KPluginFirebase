using UnityEditor;
using UnityEngine;

namespace KPlugin.GoogleFirebase.RemoteConfig.Editor
{
    public static class CreateGameObject
    {
        #region Properties
        private const string GAME_OBJECT_NAME_REMOTE_CONFIG = "KPlugin_Firebase_RemoteConfig";
        #endregion

        #region Methods
        [MenuItem("GameObject/KPlLugin/Firebase/Create RemoteConfig", priority = 2)]
        private static void Create_RemoteConfig()
        {
            GameObject newGO = new GameObject(GAME_OBJECT_NAME_REMOTE_CONFIG);
            newGO.AddComponent<RemoteConfigControl>();
            //
            if (Selection.activeTransform != null)
                newGO.transform.SetParent(Selection.activeTransform);
            Selection.activeGameObject = newGO;
        }
        #endregion
    }
}

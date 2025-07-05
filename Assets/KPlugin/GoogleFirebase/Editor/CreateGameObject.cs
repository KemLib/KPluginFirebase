using UnityEditor;
using UnityEngine;

namespace KPlugin.GoogleFirebase.Editor
{
    public static class CreateGameObject
    {
        #region Properties
        private const string GAME_OBJECT_NAME_MANAGER = "KPLugin_Firebase_Manager";
        #endregion

        #region Methods
        [MenuItem("GameObject/KPLugin/Firebase/Create Manager", priority = 0)]
        private static void Create_InitManager()
        {
            GameObject newGO = new GameObject(GAME_OBJECT_NAME_MANAGER);
            newGO.AddComponent<FirebaseManager>();
            //
            if (Selection.activeTransform != null)
                newGO.transform.SetParent(Selection.activeTransform);
            Selection.activeGameObject = newGO;
        }
        #endregion
    }
}

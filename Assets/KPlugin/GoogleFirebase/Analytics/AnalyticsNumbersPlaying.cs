using UnityEngine;

namespace KPlugin.GoogleFirebase.Analytics
{
    [CreateAssetMenu(fileName = "AnalyticsNumbersPlaying", menuName = "KPlugin/Analytics/Numbers Playing")]
    public class AnalyticsNumbersPlaying : ScriptableObject
    {
        #region Properties
        private const string KEY_NUMBERS_PLAYING = "{0}.NumbersPlaying";

        [SerializeField]
        private string keyData;

        private int numberPlaying;

        public int NumberPlaying => numberPlaying;
        #endregion

        #region Methods Unity

        #endregion

        #region Methods
        public void Init()
        {
            string key_numberPlaying = string.Format(KEY_NUMBERS_PLAYING, keyData);
            //
            numberPlaying = PlayerPrefs.GetInt(key_numberPlaying, 0);
            numberPlaying++;
            PlayerPrefs.SetInt(key_numberPlaying, numberPlaying);
        }
        #endregion
    }
}

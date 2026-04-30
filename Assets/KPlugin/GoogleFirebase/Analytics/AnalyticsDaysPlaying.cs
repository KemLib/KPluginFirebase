using System;
using UnityEngine;

namespace KPlugin.GoogleFirebase.Analytics
{
    [CreateAssetMenu(fileName = "AnalyticsDaysPlaying", menuName = "KPlugin/Analytics/Days Playing")]
    public class AnalyticsDaysPlaying : ScriptableObject
    {
        #region Properties
        private const string KEY_DAYS_PLAYING = "{0}.DaysPlaying",
            KEY_DAY_OF_YEAR = "{0}.DayOfYear",
            KEY_YEAR = "{0}.Year";

        [SerializeField]
        private string keyData;

        private int daysPlaying,
            dayOfYear,
            year;
        private bool isNewDay;

        public int DaysPlaying => daysPlaying;
        public int DayOfYear => dayOfYear;
        public int Year => year;
        public bool IsNewDay => isNewDay;
        #endregion

        #region Methods Unity

        #endregion

        #region Methods
        public void Init()
        {
            string key_daysPlaying = string.Format(KEY_DAYS_PLAYING, keyData),
                key_DayOfYear = string.Format(KEY_DAY_OF_YEAR, keyData),
                key_Year = string.Format(KEY_YEAR, keyData);
            DateTime dateNow = DateTime.Now;
            //
            daysPlaying = PlayerPrefs.GetInt(key_daysPlaying, 0);
            dayOfYear = PlayerPrefs.GetInt(key_DayOfYear, 0);
            year = PlayerPrefs.GetInt(key_Year, 0);
            if (year < dateNow.Year || (year == dateNow.Year && dayOfYear < dateNow.DayOfYear))
            {
                daysPlaying++;
                dayOfYear = dateNow.DayOfYear;
                year = dateNow.Year;
                isNewDay = true;
                PlayerPrefs.SetInt(key_daysPlaying, daysPlaying);
                PlayerPrefs.SetInt(key_DayOfYear, dayOfYear);
                PlayerPrefs.SetInt(key_Year, year);
            }
            else
            {
                isNewDay = false;
            }
        }
        #endregion
    }
}

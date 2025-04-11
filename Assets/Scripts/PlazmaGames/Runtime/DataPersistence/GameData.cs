using PlazmaGames.Audio;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlazmaGames.DataPersistence
{
    [System.Serializable]
    public class GameData
    {
        // General Settings
        public int profileID;

        // Audio MonoSystem Settings
        public float overallVolume = 1;
        public float musicVolume = 1;
        public float soundVolume = 1;
        public float ambentVolume = 1;
    }

}
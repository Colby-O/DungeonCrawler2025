using PlazmaGames.DataPersistence;
using PlazmaGames.Runtime.DataStructures;
using System.Collections.Generic;
using UnityEngine;

namespace DC2025
{
    [System.Serializable]
    public class DCGameData : GameData
    {
        public SerializableDictionary<int, bool> doorLockedStates = new SerializableDictionary<int, bool>();
        public SerializableDictionary<int, bool> keypadLockedStates = new SerializableDictionary<int, bool>();
        public SerializableDictionary<int, SlotData> slots = new SerializableDictionary<int, SlotData>();
        public SerializableDictionary<int, List<SlotData>> chestInvs = new SerializableDictionary<int, List<SlotData>>();
    }
}

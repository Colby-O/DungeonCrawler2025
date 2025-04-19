using PlazmaGames.DataPersistence;
using PlazmaGames.Runtime.DataStructures;
using System.Collections.Generic;
using UnityEngine;

namespace DC2025
{
    [System.Serializable]
    public class DCGameData : GameData
    {
        //public List<SlotData> playerSlots = new List<SlotData>();
        //public SlotData rightSlot;
        //public SlotData leftSlot;
        public SerializableDictionary<int, SlotData> slots = new SerializableDictionary<int, SlotData>();
        public SerializableDictionary<int, List<SlotData>> chestInvs = new SerializableDictionary<int, List<SlotData>>();
    }
}

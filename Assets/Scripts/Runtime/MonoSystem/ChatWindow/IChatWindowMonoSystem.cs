using PlazmaGames.Core.MonoSystem;
using UnityEngine;

namespace DC2025
{
    public interface IChatWindowMonoSystem : IMonoSystem
    {
        public void Send(string msg, Color? color = null);
        public void Clear();
    }
}

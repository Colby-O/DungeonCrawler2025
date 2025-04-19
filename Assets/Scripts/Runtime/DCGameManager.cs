using PlazmaGames.Animation;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;
using UnityEngine.Events;

namespace DC2025
{
    public class DCGameManager : GameManager
    {
        [SerializeField] GameObject _monoSystemHolder;

        [Header("MonoSystems")]
        [SerializeField] private UIMonoSystem _uiSystem;
        [SerializeField] private AnimationMonoSystem _animSystem;
        [SerializeField] private GridMonoSystem _gridSystem;
        [SerializeField] private FightMonoSystem _fightSystem;
        [SerializeField] private ChatWindowMonoSystem _chatSystem;
        [SerializeField] private InventoryMonoSystem _inventorySystem;
        [SerializeField] private SwordBuilderMonoSystem _swordBuilderSystem;
        [SerializeField] private AudioMonoSystem _audioSystem;

        [Header("Databases")]
        [SerializeField] private ItemDatabase _itemDB;

        public static GameSettings settings;

        public static bool IsPaused;
        public static Interactor Player;
        public static PlayerManager PlayerManager;
        public static Player PlayerController;

        public static UnityEvent OnRestart = new UnityEvent();

        public static ItemDatabase ItemDB { get { return ((DCGameManager)GameManager.Instance)._itemDB; } }

        private void AttachMonoSystems()
        {
            AddMonoSystem<UIMonoSystem, IUIMonoSystem>(_uiSystem);
            AddMonoSystem<AnimationMonoSystem, IAnimationMonoSystem>(_animSystem);
            AddMonoSystem<GridMonoSystem, IGridMonoSystem>(_gridSystem);
            AddMonoSystem<FightMonoSystem, IFightMonoSystem>(_fightSystem);
            AddMonoSystem<ChatWindowMonoSystem, IChatWindowMonoSystem>(_chatSystem);
            AddMonoSystem<InventoryMonoSystem, IInventoryMonoSystem>(_inventorySystem);
            AddMonoSystem<SwordBuilderMonoSystem, ISwordBuilderMonoSystem>(_swordBuilderSystem);
            AddMonoSystem<AudioMonoSystem, IAudioMonoSystem>(_audioSystem);
        }

        public override string GetApplicationName()
        {
            return nameof(DCGameManager);
        }

        public override string GetApplicationVersion()
        {
            return "v0.0.1";
        }

        protected override void OnInitalized()
        {
            AttachMonoSystems();

            _monoSystemHolder.SetActive(true);
        }

        private void Awake()
        {
            DCGameManager.settings = Resources.Load<GameSettings>("Settings/GameSettings");
        }

        private void Start()
        {
            Player = FindAnyObjectByType<Interactor>();
            PlayerManager = Player.GetComponent<PlayerManager>();
            PlayerController = Player.GetComponent<Player>();
        }
    }
}
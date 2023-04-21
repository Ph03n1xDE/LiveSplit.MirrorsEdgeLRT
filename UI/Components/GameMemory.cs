using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Timers;

using LiveSplit.ComponentUtil;
using LiveSplit.Model;
using System.Runtime.InteropServices;

namespace LiveSplit.MirrorsEdgeLRT
{
    class GameData : MemoryWatcherList
    {
        /* General / Full Game Addresses */
        public MemoryWatcher<int> IsLoading { get; } // 0 = static load, 1 = no static load
        public MemoryWatcher<int> IsSaving { get; } // 1 = game is saving
        public MemoryWatcher<int> IsLoadingDeath { get; } // 1 = loading on player death
        public MemoryWatcher<int> IsPauseMenu { get; } // 1 = tab or esc menu (all sub-menus except video) is open
        public MemoryWatcher<int> IsWhiteScreen { get; } // > 0 = white screen or some sort of load
        public MemoryWatcher<int> LoadedSubLevels { get; } // number of currently loaded sublevels including the persistent level
        public MemoryWatcher<int> TotalSubLevels { get; } // total number of sublevels including the persistent level
        public StringWatcher RespawnCP { get; } // current minor/soft checkpoint
        public StringWatcher PersistentLevel { get; } // current persistent level

        /* Player State Addresses */
        public MemoryWatcher<Vector3f> PlayerPos { get; } // player position
        public MemoryWatcher<float> ObjectPosZ { get; } // z position of the object the player is standing on

        public MemoryWatcher<byte> MoveState { get; } // movement state of the player pawn
        public MemoryWatcher<byte> AllowMoveChange { get; } // elevator state (1 = no elevator state, 0 = elevator state)

        public MemoryWatcher<PlayerInput> IgnorePlayerInput { get; } // ignore player move and look input state (1 = ignore, 0 = don't ignore)
        public MemoryWatcher<byte> IgnoreButtonInput { get; } // ignore player button input state (1 = ignore, 0 = don't ignore)

        /* 69 Stars Addresses */
        public MemoryWatcher<TTCheckpoint> Checkpoint { get; } // tt checkpoint info (current cp, total cps)
        public MemoryWatcher<byte> ActiveTTStretch { get; } // active tt stretch (order does not match in-game order)
        public MemoryWatcher<float> FinishedTime { get; } // time it took the player to finish the tt
        public MemoryWatcher<TTStars> StarTime { get; } // qualifying times for 1, 2 and 3 stars

        /* Bag Counter */
        public MemoryWatcher<int> BagCounter { get; } // counter for the number of bags collected (does not increment by 1, just goes up)

        /* Custom Structs (the bytes for these addresses follow each other so using these only require one DeepPointer per struct) */
        [StructLayout(LayoutKind.Sequential)]
        public struct TTStars
        {
            public float Three { get; set; }
            public float Two { get; set; }
            public float One { get; set; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TTCheckpoint
        {
            public int TotalCPs { get; set; }
            public int CurrentCP { get; set; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PlayerInput
        {
            public byte Move { get; set; }
            public byte Look { get; set; }
        }

        public GameData(GameVersion version)
        {
            if (version == GameVersion.Steam)
            {
                /* Full Game/General Pointers */
                this.IsLoading = new MemoryWatcher<int>(new DeepPointer(0x01B6443C));
                this.IsSaving = new MemoryWatcher<int>(new DeepPointer(0x01C55EA8, 0xCC));
                this.IsLoadingDeath = new MemoryWatcher<int>(new DeepPointer(0x1BFA630));
                this.IsPauseMenu = new MemoryWatcher<int>(new DeepPointer(0x01B985AC, 0x0, 0x188));
                this.IsWhiteScreen = new MemoryWatcher<int>(new DeepPointer(0x1C00A38));

                this.LoadedSubLevels = new MemoryWatcher<int>(new DeepPointer(0x01BFBCA4, 0x48));
                this.TotalSubLevels = new MemoryWatcher<int>(new DeepPointer(0x01BFBB70, 0x7C, 0x9C, 0xBF0));

                this.RespawnCP = new StringWatcher(new DeepPointer(0x01C55EA8, 0x74, 0x0, 0x3C, 0x0), 58);
                this.PersistentLevel = new StringWatcher(new DeepPointer(0x01BF8B20, 0x3CC, 0x0), 24);

                this.PlayerPos = new MemoryWatcher<Vector3f>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x2F8, 0xE8));

                this.MoveState = new MemoryWatcher<byte>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x2F8, 0x500));
                this.AllowMoveChange = new MemoryWatcher<byte>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x2F8, 0x41D));
                this.IgnorePlayerInput = new MemoryWatcher<PlayerInput>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x2FD));
                this.IgnoreButtonInput = new MemoryWatcher<byte>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x575));

                this.ObjectPosZ = new MemoryWatcher<float>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x2F8, 0x74, 0xF0));

                /* 69 Stars Pointers */
                this.Checkpoint = new MemoryWatcher<TTCheckpoint>(new DeepPointer(0x01BFBCA4, 0x50, 0x1E0, 0x318, 0x3D0));
                this.ActiveTTStretch = new MemoryWatcher<byte>(new DeepPointer(0x01BFBCA4, 0x50, 0x1E0, 0x318, 0x3F9));
                this.FinishedTime = new MemoryWatcher<float>(new DeepPointer(0x01BFBCA4, 0x50, 0x1E0, 0x318, 0x424));
                this.StarTime = new MemoryWatcher<TTStars>(new DeepPointer(0x01BFBCA4, 0x50, 0x1E0, 0x318, 0x408));

                /* Bag Counter */
                this.BagCounter = new MemoryWatcher<int>(new DeepPointer(0x01B73F1C, 0x64, 0x4C, 0x7A4));
            }
            else if (version == GameVersion.Reloaded)
            {
                /* Full Game/General Pointers */
                this.IsLoading = new MemoryWatcher<int>(new DeepPointer(0x01B685BC));
                this.IsSaving = new MemoryWatcher<int>(new DeepPointer(0x01C6EFE0, 0xCC));
                this.IsLoadingDeath = new MemoryWatcher<int>(new DeepPointer(0x1C136E4));
                this.IsPauseMenu = new MemoryWatcher<int>(new DeepPointer(0x01BB166C, 0x0, 0x188));
                this.IsWhiteScreen = new MemoryWatcher<int>(new DeepPointer(0x1C19AF8));

                this.LoadedSubLevels = new MemoryWatcher<int>(new DeepPointer(0x01C14D5C, 0x48));
                this.TotalSubLevels = new MemoryWatcher<int>(new DeepPointer(0x01C14C28, 0x10, 0x9C, 0xBF0));

                this.RespawnCP = new StringWatcher(new DeepPointer(0x01C6EFE0, 0x134, 0xBC, 0x0, 0x3C, 0x0), 58);
                this.PersistentLevel = new StringWatcher(new DeepPointer(0x01C6EFE0, 0x170, 0x1DC, 0x1E8, 0x3C, 0x528, 0x3CC, 0x0), 24);

                this.PlayerPos = new MemoryWatcher<Vector3f>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0xE8));

                this.MoveState = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0x500));
                this.AllowMoveChange = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0x41D));
                this.IgnorePlayerInput = new MemoryWatcher<PlayerInput>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2FD));
                this.IgnoreButtonInput = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x575));

                this.ObjectPosZ = new MemoryWatcher<float>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0x74, 0xF0));

                /* 69 Stars Pointers */
                this.Checkpoint = new MemoryWatcher<TTCheckpoint>(new DeepPointer(0x01C14D5C, 0x54, 0x1E0, 0x318, 0x3D0));
                this.ActiveTTStretch = new MemoryWatcher<byte>(new DeepPointer(0x01C14D5C, 0x54, 0x1E0, 0x318, 0x3F9));
                this.FinishedTime = new MemoryWatcher<float>(new DeepPointer(0x01C14D5C, 0x54, 0x1E0, 0x318, 0x424));
                this.StarTime = new MemoryWatcher<TTStars>(new DeepPointer(0x01C14D5C, 0x54, 0x1E0, 0x318, 0x408));

                /* Bag Counter */
                this.BagCounter = new MemoryWatcher<int>(new DeepPointer(0x01C6EFE0, 0x194, 0x128, 0x3C, 0x11C, 0x64, 0x4C, 0x7A4));
            }
            else if (version == GameVersion.Origin)
            {
                /* Full Game/General Pointers */
                this.IsLoading = new MemoryWatcher<int>(new DeepPointer(0x01B685BC));
                this.IsSaving = new MemoryWatcher<int>(new DeepPointer(0x01C6EFE0, 0xCC));
                this.IsLoadingDeath = new MemoryWatcher<int>(new DeepPointer(0x01C136F0));
                this.IsPauseMenu = new MemoryWatcher<int>(new DeepPointer(0x01C11BE0, 0x32C));
                this.IsWhiteScreen = new MemoryWatcher<int>(new DeepPointer(0x1C19AF8));

                this.LoadedSubLevels = new MemoryWatcher<int>(new DeepPointer(0x01C14D64, 0x48));
                this.TotalSubLevels = new MemoryWatcher<int>(new DeepPointer(0x01C14C30, 0xC, 0x9C, 0xBF0));

                this.RespawnCP = new StringWatcher(new DeepPointer(0x01C6EFE0, 0x134, 0xBC, 0x0, 0x3C, 0x0), 58);
                this.PersistentLevel = new StringWatcher(new DeepPointer(0x01C11BE0, 0x330, 0x0), 24);

                this.PlayerPos = new MemoryWatcher<Vector3f>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0xE8));

                this.MoveState = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0x500));
                this.AllowMoveChange = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0x41D));
                this.IgnorePlayerInput = new MemoryWatcher<PlayerInput>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2FD));
                this.IgnoreButtonInput = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x575));

                this.ObjectPosZ = new MemoryWatcher<float>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0x74, 0xF0));

                /* 69 Stars Pointers */
                this.Checkpoint = new MemoryWatcher<TTCheckpoint>(new DeepPointer(0x01C14D64, 0x54, 0x1E0, 0x318, 0x3D0));
                this.ActiveTTStretch = new MemoryWatcher<byte>(new DeepPointer(0x01C14D64, 0x54, 0x1E0, 0x318, 0x3F9));
                this.FinishedTime = new MemoryWatcher<float>(new DeepPointer(0x01C14D64, 0x54, 0x1E0, 0x318, 0x424));
                this.StarTime = new MemoryWatcher<TTStars>(new DeepPointer(0x01C14D64, 0x54, 0x1E0, 0x318, 0x408));

                /* Bag Counter */
                this.BagCounter = new MemoryWatcher<int>(new DeepPointer(0x01C6EFE0, 0x194, 0x128, 0x3C, 0x11C, 0x64, 0x4C, 0x7A4));
            }

            this.AddRange(this.GetType().GetProperties()
                .Where(p => !p.GetIndexParameters().Any())
                .Select(p => p.GetValue(this, null) as MemoryWatcher)
                .Where(p => p != null));
        }
    }

    class GameMemory
    {
        public event EventHandler OnLoadTrue;
        public event EventHandler OnLoadFalse;
        public event EventHandler OnStart;
        public event EventHandler OnReset;
        public event EventHandler OnSplit;

        private readonly List<int> _ignorePIDs;

        private GameData _data;
        private Process _process;

        private Process game;
        private GameVersion version;

        private readonly MirrorsEdgeLRTSettings UserSettings;
        private readonly TimerModel timer;

        private enum ExpectedDllSizes
        {
            Steam1 = 32632832,
            Steam2 = 32976896,
            Reloaded1 = 60298504,
            Reloaded2 = 42876928,
            Origin = 42889216
        }

        /* integers assigned to persistent levels for splitting */
        private readonly Dictionary<string, int> levels = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            {"TdMainMenu", -2},
            {"tutorial_p", -1},
            {"edge_p", 0},
            {"escape_p", 1},
            {"stormdrain_p", 2},
            {"cranes_p", 3},
            {"subway_p", 4},
            {"mall_p", 5},
            {"factory_p", 6},
            {"boat_p", 7},
            {"convoy_p", 8},
            {"scraper_p", 9}
        };

        /* starting x coordinate for every time trial */
        private readonly Dictionary<int, float> TTCoords = new Dictionary<int, float>()
        {
            {15, 2224},
            {16, 2224},
            {21, 1424},
            {4, -7594.46f},
            {23, 18149},
            {22, 5867.30f},
            {6, 532},
            {7, 1082},
            {8, 5721},
            {9, -1235.96f},
            {1, 2096},
            {20, -7312.65f},
            {3, 34247},
            {2, 37792},
            {14, -7158},
            {17, 2970},
            {24, -2304},
            {12, -32203.00f},
            {13, -19229.02f},
            {10, 144},
            {11, 2542.04f},
            {18, -830.43f},
            {19, -469}
        };

        /* persistent level name of every time trial assigned to its stretch number */
        private readonly Dictionary<int, string> TTNames = new Dictionary<int, string>()
        {
            {15, "tt_TutorialA01_p"},
            {16, "tt_TutorialA02_p"},
            {21, "tt_TutorialA03_p"},
            {4, "tt_EdgeA01_p"},
            {23, "tt_EscapeB01_p"},
            {22, "tt_EscapeA01_p"},
            {6, "tt_StormdrainA02_p"},
            {7, "tt_StormdrainB01_p"},
            {8, "tt_StormdrainB02_p"},
            {9, "tt_StormdrainB03_p"},
            {1, "tt_CranesA01_p"},
            {20, "tt_CranesC01_p"},
            {3, "tt_CranesB02_p"},
            {2, "tt_CranesB01_p"},
            {14, "tt_MallA01_p"},
            {17, "tt_FactoryA01_p"},
            {24, "tt_CranesD01_p"},
            {12, "tt_ConvoyB01_p"},
            {13, "tt_ConvoyB02_p"},
            {10, "tt_ConvoyA01_p"},
            {11, "tt_ConvoyA02_p"},
            {18, "tt_ScraperA01_p"},
            {19, "tt_ScraperB01_p"}
        };

        private readonly Vector3f SDEntryBtn = new Vector3f(1311f, -30027f, -6634f);
        private readonly Vector3f SDExitGate = new Vector3f(1468f, -10471.75f, -7267.36f);

        private bool SDEntranceGateBtnHit = false; // true when stormdrains entrance gate button (start of 2C) has been hit
        private bool SDExitGateBtnHit = false; // true when stormdrains exit gate button (end of 2C) has been hit

        private bool SDExitBtnHit = false; // true if stormdrains exit button (top of 2D) has been hit
        private bool SDExitHasSplit = false; // true after the player hits the exit button to prevent multiple splits

        private bool Truck50Elapsed = false; // true if 50 seconds have elapsed in the truck ride in 7a

        private bool timerStarted = false; // true when the timer has started to prevent unnecessary checks while not running
        private bool menuWhileStreaming = false; // true if the player opens menu while loading a level
        private bool isRunning = false; // true if signature scans or the turck timer are running

        /* Pointers for use counter of the buttons at the start, end and exit of stormdrains */
        private IntPtr SDEntryGateBtnPtr = IntPtr.Zero;
        private IntPtr SDExitGateBtnPtr = IntPtr.Zero;
        private IntPtr SDExitBtnPtr = IntPtr.Zero;

        /* MemoryWatchers for the above pointers */
        private MemoryWatcher<int> EntryGateBtnCount;
        private MemoryWatcher<int> ExitGateBtnCount;
        private MemoryWatcher<int> ExitBtnCount;

        public GameMemory(MirrorsEdgeLRTSettings Settings, TimerModel _timer)
        {
            timer = _timer;
            UserSettings = Settings;
            _ignorePIDs = new List<int>();
        }

        /* This runs at the interval set in MirrorsEdgeLRTComponent.cs (once per frame/every 16ms) */
        public void Update()
        {
            if (_process == null || _process.HasExited)
            {
                if (!this.TryGetGameProcess())
                    return;
            }

            TimedTraceListener.Instance.UpdateCount++;

            //TimeStamp StartTime = TimeStamp.Now;
            _data.UpdateAll(_process);
            //Debug.WriteLine("Time to update all: " + (TimeStamp.Now - StartTime).TotalMilliseconds + "ms");

            bool bIgnoreMoveInput = (_data.IgnorePlayerInput.Current.Move & 1) != 0;
            bool bIgnoreLookInput = (_data.IgnorePlayerInput.Current.Look & 1) != 0;
            bool bIgnoreButtonInput = (_data.IgnoreButtonInput.Current & 1) != 0;
            bool bAllowMoveChange = (_data.AllowMoveChange.Current & (1 << 4)) != 0;

            if (timer.CurrentState.CurrentAttemptDuration == TimeSpan.Zero)
            {
                timerStarted = false;
            }
            else
            {
                timerStarted = true;
            }

            /* -- Starting -- */
            if (UserSettings.Category != 2)
            {
                /* Full Game - Starting when player skips first cutscene */
                if (_data.PersistentLevel.Current == "tutorial_p" && _data.PlayerPos.Current.Z < 5854f && _data.PlayerPos.Current.Z > 5800f && !timerStarted)
                {
                    this.OnStart?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("starting timer");
                }
            }
            else
            {
                /* 69 Stars - Starting when player is at starting checkpoint of selected TT */
                if (_data.ActiveTTStretch.Current == UserSettings.StartTT && bIgnoreMoveInput && bIgnoreLookInput && (int)_data.PlayerPos.Current.X == (int)TTCoords[UserSettings.StartTT] && !timerStarted)
                {
                    this.OnStart?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("starting timer");
                }
            }

            /* -- Resetting -- */
            if (UserSettings.Category != 2)
            {
                /* Full Game - Resetting when switching from menu to tutorial_p (first cutscene) */
                if (_data.PersistentLevel.Old == "TdMainMenu" && _data.PersistentLevel.Current == "tutorial_p")
                {
                    timerStarted = false;
                    this.OnReset?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("resetting timer");
                }
            }
            else
            {
                /* 69 Stars - Resetting when switching from menu to selected starting TT */
                if (_data.PersistentLevel.Old == "TdMainMenu" && _data.PersistentLevel.Current == TTNames[UserSettings.StartTT])
                {
                    timerStarted = false;
                    this.OnReset?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("resetting timer");
                }
            }

            if (timerStarted)
            {
                /* -- Distance Checks -- */
                if (UserSettings.Category != 2)
                {
                    Vector3f PlayerPos = new Vector3f(_data.PlayerPos.Current.X, _data.PlayerPos.Current.Y, _data.PlayerPos.Current.Z);

                    /* Checking if player has come close to the entrance gate button */
                    if (_data.RespawnCP.Current == "Gate1")
                    {
                        if (PlayerPos.Distance(SDEntryBtn) < 130f)
                        {
                            SDEntranceGateBtnHit = true;
                        }
                    }
                    /* Checking if player has come close to the exit gate button */
                    else if (_data.RespawnCP.Current == "Waterfall")
                    {
                        if (PlayerPos.Distance(SDExitGate) < 130f && _data.PlayerPos.Current.Y < -10350f)
                        {
                            SDExitGateBtnHit = true;
                        }
                    }
                    else
                    {
                        SDEntranceGateBtnHit = false;
                        SDExitGateBtnHit = false;
                    }
                }

                /* -- Splitting -- */
                if (UserSettings.Category != 2)
                {
                    /* -- Full Game -- */
                    if (_data.PersistentLevel.Current != _data.PersistentLevel.Old && levels[_data.PersistentLevel.Current] > levels[_data.PersistentLevel.Old] && _data.PersistentLevel.Old != "tutorial_p" && _data.PersistentLevel.Old != "TdMainMenu" && !UserSettings.ChapterSplitDisabled)
                    {
                        this.OnSplit?.Invoke(this, EventArgs.Empty);
                        Debug.WriteLine("chapter split");
                    }
                    else if (SDExitBtnHit && UserSettings.SDSplit && !SDExitHasSplit)
                    {
                        this.OnSplit?.Invoke(this, EventArgs.Empty);
                        SDExitHasSplit = true;
                        Debug.WriteLine("sd exit split");
                    }
                    else if (_data.RespawnCP.Current == "End_game" && _data.ObjectPosZ.Current == 75102 && _data.ObjectPosZ.Old != 75102)
                    {
                        this.OnSplit?.Invoke(this, EventArgs.Empty);
                        Debug.WriteLine("ending split");
                    }
                    else if (levels[_data.PersistentLevel.Current] != 2)
                    {
                        SDExitGateBtnHit = false;
                        SDEntranceGateBtnHit = false;
                        SDExitBtnHit = false;
                        SDExitHasSplit = false;
                    }

                    /* Bag Splits for 100% */
                    if (UserSettings.Category == 1)
                    {
                        if (_data.BagCounter.Current > _data.BagCounter.Old)
                        {
                            this.OnSplit?.Invoke(this, EventArgs.Empty);
                            Debug.WriteLine("bag split");
                        }
                    }
                }
                else if (UserSettings.Category == 2)
                {
                    /* -- 69 Stars -- */
                    if (_data.Checkpoint.Current.CurrentCP > _data.Checkpoint.Old.CurrentCP)
                    {
                        if (_data.Checkpoint.Current.CurrentCP == _data.Checkpoint.Current.TotalCPs)
                        {
                            if (UserSettings.StarsRequired == 0)
                            {
                                this.OnSplit?.Invoke(this, EventArgs.Empty);
                                Debug.WriteLine("tt split");
                            }
                            else if (UserSettings.StarsRequired == 1)
                            {
                                if (_data.StarTime.Current.One > Math.Round(_data.FinishedTime.Current, 2))
                                {
                                    this.OnSplit?.Invoke(this, EventArgs.Empty);
                                    Debug.WriteLine("tt split");
                                }
                            }
                            else if (UserSettings.StarsRequired == 2)
                            {
                                if (_data.StarTime.Current.Two > Math.Round(_data.FinishedTime.Current, 2))
                                {
                                    this.OnSplit?.Invoke(this, EventArgs.Empty);
                                    Debug.WriteLine("tt split");
                                }
                            }
                            else if (UserSettings.StarsRequired == 3)
                            {
                                if (_data.StarTime.Current.Three > Math.Round(_data.FinishedTime.Current, 2))
                                {
                                    this.OnSplit?.Invoke(this, EventArgs.Empty);
                                    Debug.WriteLine("tt split");
                                }
                            }
                        }
                    }
                }

                if (UserSettings.Category != 2)
                {
                    /* Checking for player pause during level streaming */
                    if (_data.IsPauseMenu.Current == 1)
                    {
                        if (CheckLevelStreaming())
                        {
                            menuWhileStreaming = true;
                        }
                        else
                        {
                            menuWhileStreaming = false;
                        }
                    }
                    else if (_data.IsPauseMenu.Current == 0)
                    {
                        menuWhileStreaming = false;
                    }
                }

                /* Stormdrains Button Scanning */
                if (UserSettings.Category != 2)
                {
                    /*if (_data.RespawnCP.Current == "Waterfall" && SDExitGateBtnPtr == IntPtr.Zero && levels[_data.PersistentLevel.Current] == 2)
                    {
                        StartBGTask(2);
                    }
                    else if (_data.RespawnCP.Current == "Gate1" && SDEntryGateBtnPtr == IntPtr.Zero && levels[_data.PersistentLevel.Current] == 2)
                    {
                        StartBGTask(1);
                    }
                    else*/
                    if (_data.RespawnCP.Current == "Chute2" && SDExitBtnPtr == IntPtr.Zero && levels[_data.PersistentLevel.Current] == 2)
                    {
                        StartBGTask(3);
                    }
                    else if (levels[_data.PersistentLevel.Current] == 2)
                    {
                        /*if (_data.RespawnCP.Current == "Waterfall")
                        {
                            ExitGateBtnCount.Update(game);

                            if (ExitGateBtnCount.Current > 4)
                            {
                                Debug.WriteLine("Player used exit gate button");
                                SDExitGateBtnHit = true;
                            }
                        }
                        else if (_data.RespawnCP.Current == "Gate1")
                        {
                            EntryGateBtnCount.Update(game);

                            if (EntryGateBtnCount.Current > 4)
                            {
                                Debug.WriteLine("Player used entrance gate button");
                                SDEntranceGateBtnHit = true;
                            }
                        }
                        else*/
                        if (SDExitBtnPtr != IntPtr.Zero)
                        {
                            ExitBtnCount.Update(game);

                            if (ExitBtnCount.Current > 4)
                            {
                                Debug.WriteLine("Player used exit button");
                                SDExitBtnHit = true;
                            }
                        }
                    }
                    else
                    {
                        SDEntryGateBtnPtr = IntPtr.Zero;
                        SDExitGateBtnPtr = IntPtr.Zero;
                        SDExitBtnPtr = IntPtr.Zero;
                    }
                }

                /* -- Removing Loads -- */
                if (_data.IsLoading.Current == 0)
                {
                    this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("static loading screen or 2D cutscene");
                }
                else if (_data.IsWhiteScreen.Current == 1 && _data.IsLoading.Old == 0 && _data.IsPauseMenu.Current == 0)
                {
                    this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("white screen or static loading screen or cutscene");
                }
                else if (_data.IsWhiteScreen.Current > 1 && _data.RespawnCP.Current != "Boat_Truckride")
                {
                    this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("white screen");
                }
                else if (_data.IsLoadingDeath.Current == 1)
                {
                    this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("death load");
                }
                else if (_data.IsSaving.Current == 1 && _data.IsPauseMenu.Current == 1)
                {
                    this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("menu blocked by save icon");
                }
                else if (UserSettings.Category != 2 && bIgnoreMoveInput && bIgnoreLookInput && bIgnoreButtonInput && _data.MoveState.Current != 38 && _data.RespawnCP.Current != "Training_area_after_cs" && _data.RespawnCP.Current != "Office")
                {
                    this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("block while loading");
                }
                else if (menuWhileStreaming)
                {
                    this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("player paused during level streaming");
                }
                else if (UserSettings.Category != 2 && (
                    /* Checkpoints at the end of chapters */
                    (_data.RespawnCP.Current == "SWAT_Response2" && _data.IsSaving.Current == 1 && _data.ObjectPosZ.Current > 4641) || // Prologue
                    (_data.RespawnCP.Current == "Plaza" && _data.IsSaving.Current == 1 && _data.PlayerPos.Current.Z < -2466) || // Chapter 1
                    (_data.RespawnCP.Current == "Boss_checkpoint1" && _data.IsSaving.Current == 1 && _data.PlayerPos.Current.X == 20804.95117f) || // Chapter 2
                    (_data.RespawnCP.Current.Contains("SP03_Plaza_") && _data.IsSaving.Current == 1 && bIgnoreButtonInput) || // Chapter 3
                    (_data.RespawnCP.Current == "Train_Ride_End" && _data.IsSaving.Current == 1 && _data.ObjectPosZ.Current == -1728) || // Chapter 4
                    (_data.RespawnCP.Current == "LastCP" && _data.IsSaving.Current == 1 && _data.PlayerPos.Current.X == -51617.66406f) || // Chapter 5
                    (_data.RespawnCP.Current == "Pursuit_chase_end" && _data.IsSaving.Current == 1 && _data.ObjectPosZ.Current > 1252) || // Chapter 6
                    (_data.RespawnCP.Current == "Celeste" && _data.IsSaving.Current == 1 && _data.PlayerPos.Current.Z == 349.1499939f) || // Chapter 7
                    (_data.RespawnCP.Current == "Atrium_soft_cp" && _data.IsSaving.Current == 1 && bIgnoreButtonInput) // Chapter 8
                        ))
                {
                    this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("end of chapter save icon");
                }
                else if (UserSettings.Category != 2 && (
                    /* Level Streaming (mostly elevators, some other areas) */
                    (_data.RespawnCP.Current == "Office" &&  _data.PlayerPos.Current.Z > 12381) || // 1A
                    (_data.RespawnCP.Current == "Elev_R1-St1" &&  _data.ObjectPosZ.Current > 4159) || // 1D
                    ((_data.RespawnCP.Current == "Sluice_Out_RoofUp" || _data.RespawnCP.Current == "ChaseJK") && _data.ObjectPosZ.Current > 4159 && !bAllowMoveChange) || // 2D
                    (_data.RespawnCP.Current == "JKfight" && _data.RespawnCP.Current != "after_loading_bo") || // 2G
                    (_data.RespawnCP.Current == "SP03_Plaza_01" && _data.ObjectPosZ.Current > 6943) || // 3C
                    (_data.RespawnCP.Current == "CombatRooftops" && _data.ObjectPosZ.Current > 1566) || // 5A
                    (_data.RespawnCP.Current == "R2_tomall" && _data.ObjectPosZ.Current < 64 && !bAllowMoveChange) || // 5C
                    (_data.RespawnCP.Current == "Pursuit_chase" && _data.ObjectPosZ.Current >= 4192) || // 6D
                    (_data.RespawnCP.Current == "Elevator_to_Conv" && !bAllowMoveChange) || // 8A
                    (_data.RespawnCP.Current == "combat_3" && !bAllowMoveChange) || // 8B
                    (_data.RespawnCP.Current == "scraper_before_lobby" && _data.ObjectPosZ.Current > 32 && !bAllowMoveChange) || // 9B
                    ((_data.RespawnCP.Current == "Scraper_Inside_elevator_lobby" || _data.RespawnCP.Current == "Elevator_shaft") && _data.IsPauseMenu.Current == 1) || // 9C
                    (_data.RespawnCP.Current == "Scraper_Inside_elevator_lobby" && _data.ObjectPosZ.Current > 13167.99f) ||
                    (_data.RespawnCP.Current == "Gate1" && _data.MoveState.Current != 72 && SDEntranceGateBtnHit) || // Stormdrains Entrance Load
                    (_data.RespawnCP.Current == "Waterfall" && SDExitGateBtnHit) || // Stormdrains Exit Load
                    (_data.RespawnCP.Current == "Platform_fight" && _data.IsPauseMenu.Current == 1) || // Chapter 4 Skip Load
                    (_data.RespawnCP.Current == "steamroom_puzzle" && _data.IsPauseMenu.Current == 1) // Factory Skip Load
                        ))
                {
                    if (CheckLevelStreaming())
                    {
                        this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                        Debug.WriteLine("level streaming blocking progress");
                    }
                    else
                    {
                        this.OnLoadFalse?.Invoke(this, EventArgs.Empty);
                    }
                }
                /* level streaming in 9E hallway */
                else if (_data.RespawnCP.Current == "subway_roof-mill" && _data.PersistentLevel.Current != "TdMainMenu")
                {
                    if (CheckLevelStreaming())
                    {
                        this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                        Debug.WriteLine("9E hallway load");
                    }
                    else
                    {
                        this.OnLoadFalse?.Invoke(this, EventArgs.Empty);
                    }
                }
                /* level streaming after 7A truck ride */
                else if (_data.RespawnCP.Current == "Boat_Truckride" && _data.ObjectPosZ.Current == 64 && !isRunning)
                {
                    isRunning = true;
                    System.Timers.Timer truckTimer = new System.Timers.Timer(50000);
                    truckTimer.Elapsed += TruckTimer_Elapsed;
                    truckTimer.Enabled = true;
                }
                else if (_data.RespawnCP.Current == "Boat_Truckride" && _data.ObjectPosZ.Current == 64 && Truck50Elapsed)
                {
                    if (CheckLevelStreaming())
                    {
                        this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                        Debug.WriteLine("still level streaming after 50s in truck");
                    }
                    else
                    {
                        this.OnLoadFalse?.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                /* loading is not blocking progression */
                {
                    this.OnLoadFalse?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                isRunning = false;
            }
        }

        private bool CheckLevelStreaming()
        {
            DeepPointer SubLevelFlags = null;

            /* Checking sub-level loading flags */
            for (int i = 0; i <= _data.TotalSubLevels.Current; i++)
            {
                switch (version)
                {
                    case GameVersion.Steam:
                        SubLevelFlags = new DeepPointer("MirrorsEdge.exe", 0x01C553D0, 0xCC, 0x1CC, 0x2F8, 0x9C, 0xBEC, i * 0x04, 0x60);
                        break;
                    case GameVersion.Reloaded:
                    case GameVersion.Origin:
                        SubLevelFlags = new DeepPointer("MirrorsEdge.exe", 0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0x9C, 0xBEC, i * 0x04, 0x60);
                        break;
                }

                byte flags = SubLevelFlags.Deref<byte>(game);
                bool isLoading = ((flags & 0x01) != 0x01 & (flags & 0x80) == 0x80) | ((flags & 0x01) == 0x01 & (flags & 0x80) != 0x80);

                if (isLoading)
                {
                    return true;
                }
            }

            return false;
        }

        /* timer callback function for 7a truck ride level streaming */
        private void TruckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            isRunning = false;
            Truck50Elapsed = true;
        }

        bool TryGetGameProcess()
        {
            /* hooking the game process and checking the version */
            game = Process.GetProcesses().FirstOrDefault(p => p.ProcessName.ToLower() == "mirrorsedge"
                && !p.HasExited && !_ignorePIDs.Contains(p.Id));
            if (game == null)
                return false;

            if (game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDllSizes.Steam1 || game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDllSizes.Steam2)
            {
                version = GameVersion.Steam;
            }
            else if (game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDllSizes.Reloaded1 || game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDllSizes.Reloaded2)
            {
                version = GameVersion.Reloaded;
            }
            else if (game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDllSizes.Origin)
            {
                version = GameVersion.Origin;
            }
            else
            {
                _ignorePIDs.Add(game.Id);
                MessageBox.Show("Unexpected game version.", "LiveSplit.MirrorsEdgeLRT",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Debug.WriteLine("game version " + version);
            _data = new GameData(version);
            _process = game;

            return true;
        }

        /* signature scans for the buttons in stormdrains (entrance, exit, top of exit), only exit is currently used*/
        IntPtr ExitGateScan()
        {
            IntPtr exitGateBtn = IntPtr.Zero;

            SigScanTarget target = new SigScanTarget("00 80 BA 44 00 20 23 C6 00 00 E2 C5 00 00 00 00 51 7C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 3E 00 00 80 3F 00 00 80 3F 00 00 80 3F");

            TimeStamp StartTime = TimeStamp.Now;

            foreach (var page in game.MemoryPages(true))
            {
                SignatureScanner scanner = new SignatureScanner(game, page.BaseAddress, (int)page.RegionSize);
                exitGateBtn = scanner.Scan(target);
                if (exitGateBtn != IntPtr.Zero)
                    break;
            }

            if (exitGateBtn == IntPtr.Zero)
            {
                Debug.WriteLine("Could not find SD Exit Gate Button Pointer");
                Debug.WriteLine("Time taken: " + (TimeStamp.Now - StartTime).TotalMilliseconds + "ms");
            }
            else
            {
                exitGateBtn -= 156;
                Debug.WriteLine("Found sd exit gate btn ptr: " + exitGateBtn.ToString("X"));
                Debug.WriteLine("Time taken: " + (TimeStamp.Now - StartTime).TotalMilliseconds + "ms");
            }

            return exitGateBtn;
        }

        IntPtr EntryGateScan()
        {
            IntPtr entryGateBtn = IntPtr.Zero;

            SigScanTarget target = new SigScanTarget("45 6B A5 44 82 4D EA C6 00 40 CE C5 00 00 00 00 ED 7D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 3E 00 00 80 3F 00 00 80 3F 00 00 80 3F");

            TimeStamp StartTime = TimeStamp.Now;

            foreach (var page in game.MemoryPages(true))
            {
                SignatureScanner scanner = new SignatureScanner(game, page.BaseAddress, (int)page.RegionSize);
                entryGateBtn = scanner.Scan(target);
                if (entryGateBtn != IntPtr.Zero)
                    break;
            }

            if (entryGateBtn == IntPtr.Zero)
            {
                Debug.WriteLine("Could not find SD Entrance Gate Button Pointer");
                Debug.WriteLine("Time taken: " + (TimeStamp.Now - StartTime).TotalMilliseconds + "ms");
            }
            else
            {
                entryGateBtn -= 156;
                Debug.WriteLine("Found SD Entrance Gate Button Pointer: " + entryGateBtn.ToString("X"));
                Debug.WriteLine("Time taken: " + (TimeStamp.Now - StartTime).TotalMilliseconds + "ms");
            }

            return entryGateBtn;
        }

        IntPtr ExitBtnScan()
        {
            IntPtr exitBtn = IntPtr.Zero;

            SigScanTarget target = new SigScanTarget("FC 01 70 44 22 A6 D6 C5 00 80 41 C5 00 00 00 00 10 21 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 3F 00 00 80 3F 00 00 80 3F 00 00 80 3F");

            TimeStamp StartTime = TimeStamp.Now;

            foreach (var page in game.MemoryPages(true))
            {
                SignatureScanner scanner = new SignatureScanner(game, page.BaseAddress, (int)page.RegionSize);
                exitBtn = scanner.Scan(target);
                if (exitBtn != IntPtr.Zero)
                    break;
            }

            if (exitBtn == IntPtr.Zero)
            {
                Debug.WriteLine("Could not find SD Exit Button Pointer");
                Debug.WriteLine("Time taken: " + (TimeStamp.Now - StartTime).TotalMilliseconds + "ms");
            }
            else
            {
                exitBtn -= 156;
                Debug.WriteLine("Found SD Exit Button Pointer: " + exitBtn.ToString("X"));
                Debug.WriteLine("Time taken: " + (TimeStamp.Now - StartTime).TotalMilliseconds + "ms");
            }

            return exitBtn;
        }

        /* handler to start signature scans as background tasks to avoid freezing the timer */
        private async void StartBGTask(int btnSelect)
        {
            if (btnSelect == 3 && SDExitBtnPtr == IntPtr.Zero && !isRunning)
            {
                isRunning = true;
                SDExitBtnPtr = await Task.Run(() => ExitBtnScan());
                isRunning = false;

                Debug.WriteLine("Exit Button PTR: " + SDExitBtnPtr.ToString("X"));
                ExitBtnCount = new MemoryWatcher<int>(new DeepPointer(SDExitBtnPtr));
                ExitBtnCount.Update(game);
                Debug.WriteLine("Exit Button Use Count: " + ExitBtnCount.Current);
            }
            else if (btnSelect == 2 && SDExitGateBtnPtr == IntPtr.Zero && !isRunning)
            {
                isRunning = true;
                SDExitGateBtnPtr = await Task.Run(() => ExitGateScan());
                isRunning = false;

                Debug.WriteLine("Exit Gate PTR: " + SDExitGateBtnPtr.ToString("X"));
                ExitGateBtnCount = new MemoryWatcher<int>(new DeepPointer(SDExitGateBtnPtr));
                ExitGateBtnCount.Update(game);
                Debug.WriteLine("Exit Gate Button Use Count: " + ExitGateBtnCount.Current);
            }
            else if (btnSelect == 1 && SDEntryGateBtnPtr == IntPtr.Zero && !isRunning)
            {
                isRunning = true;
                SDEntryGateBtnPtr = await Task.Run(() => EntryGateScan());
                isRunning = false;

                Debug.WriteLine("Entry Gate PTR: " + SDEntryGateBtnPtr.ToString("X"));
                EntryGateBtnCount = new MemoryWatcher<int>(new DeepPointer(SDEntryGateBtnPtr));
                EntryGateBtnCount.Update(game);
                Debug.WriteLine("Entry Gate Button Use Count: " + EntryGateBtnCount.Current);
            }
        }
    }

    enum GameVersion
    {
        Steam,
        Reloaded,
        Origin
    }
}

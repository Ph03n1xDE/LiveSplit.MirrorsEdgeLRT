using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using LiveSplit.ComponentUtil;
using LiveSplit.Model;

namespace LiveSplit.MirrorsEdgeLRT
{
    class GameData : MemoryWatcherList
    {
        /* General / Full Game Addresses */

        public MemoryWatcher<int> LoadCounter { get; }
        public MemoryWatcher<int> IsSaving { get; }
        public MemoryWatcher<int> IsLoadingDeath { get; }
        public MemoryWatcher<int> IsPauseMenu { get; }
        public MemoryWatcher<int> IsWhiteScreen { get; }
        public MemoryWatcher<int> LoadedSubLevels { get; }
        public MemoryWatcher<int> TotalSubLevels { get; }
        public StringWatcher RespawnCP { get; }
        public StringWatcher PersistentLevel { get; }

        public MemoryWatcher<float> PlayerPosX { get; }
        public MemoryWatcher<float> PlayerPosY { get; }
        public MemoryWatcher<float> PlayerPosZ { get; }

        public MemoryWatcher<byte> MoveState { get; }
        public MemoryWatcher<byte> AllowMoveChange { get; }
        public MemoryWatcher<byte> IgnoreMoveInput { get; }
        public MemoryWatcher<byte> IgnoreLookInput { get; }
        public MemoryWatcher<byte> IgnoreButtonInput { get; }

        public MemoryWatcher<float> ObjectPosZ { get; }

        /* 69 Stars Addresses */
        public MemoryWatcher<int> TotalCPs { get; }
        public MemoryWatcher<int> CurrentCP { get; }
        public MemoryWatcher<byte> ActiveTTStretch { get; }
        public MemoryWatcher<float> FinishedTime { get; }

        public MemoryWatcher<float> ThreeStarTime { get; }
        public MemoryWatcher<float> TwoStarTime { get; }
        public MemoryWatcher<float> OneStarTime { get; }

        public GameData(GameVersion version)
        {
            if (version == GameVersion.Steam)
            {
                /* Full Game/General Pointers */
                this.LoadCounter = new MemoryWatcher<int>(new DeepPointer("binkw32.dll", 0x233B8));
                this.IsSaving = new MemoryWatcher<int>(new DeepPointer(0x01C55EA8, 0xCC));
                this.IsLoadingDeath = new MemoryWatcher<int>(new DeepPointer(0x1BFA630));
                this.IsPauseMenu = new MemoryWatcher<int>(new DeepPointer(0x01B985AC, 0x0, 0x188));
                this.IsWhiteScreen = new MemoryWatcher<int>(new DeepPointer(0x1C00A38));

                this.LoadedSubLevels = new MemoryWatcher<int>(new DeepPointer(0x01BFBCA4, 0x48));
                this.TotalSubLevels = new MemoryWatcher<int>(new DeepPointer(0x01BFBB70, 0x7C, 0x9C, 0xBF0));

                this.RespawnCP = new StringWatcher(new DeepPointer(0x01C55EA8, 0x74, 0x0, 0x3C, 0x0), 128);
                this.PersistentLevel = new StringWatcher(new DeepPointer(0x01BF8B20, 0x3CC, 0x0), 128);

                this.PlayerPosX = new MemoryWatcher<float>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x2F8, 0xE8));
                this.PlayerPosY = new MemoryWatcher<float>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x2F8, 0xEC));
                this.PlayerPosZ = new MemoryWatcher<float>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x2F8, 0xF0));

                this.MoveState = new MemoryWatcher<byte>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x2F8, 0x500));
                this.AllowMoveChange = new MemoryWatcher<byte>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x2F8, 0x41D));
                this.IgnoreMoveInput = new MemoryWatcher<byte>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x2FD));
                this.IgnoreLookInput = new MemoryWatcher<byte>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x2FE));
                this.IgnoreButtonInput = new MemoryWatcher<byte>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x575));

                this.ObjectPosZ = new MemoryWatcher<float>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x2F8, 0x74, 0xF0));

                /* 69 Stars Pointers */
                this.CurrentCP = new MemoryWatcher<int>(new DeepPointer(0x01BFBCA4, 0x50, 0x1E0, 0x318, 0x3D4));
                this.TotalCPs = new MemoryWatcher<int>(new DeepPointer(0x01BFBCA4, 0x50, 0x1E0, 0x318, 0x3D0));
                this.ActiveTTStretch = new MemoryWatcher<byte>(new DeepPointer(0x01BFBCA4, 0x50, 0x1E0, 0x318, 0x3F9));
                this.FinishedTime = new MemoryWatcher<float>(new DeepPointer(0x01BFBCA4, 0x50, 0x1E0, 0x318, 0x424));

                this.ThreeStarTime = new MemoryWatcher<float>(new DeepPointer(0x01BFBCA4, 0x50, 0x1E0, 0x318, 0x408));
                this.TwoStarTime = new MemoryWatcher<float>(new DeepPointer(0x01BFBCA4, 0x50, 0x1E0, 0x318, 0x40C));
                this.OneStarTime = new MemoryWatcher<float>(new DeepPointer(0x01BFBCA4, 0x50, 0x1E0, 0x318, 0x410));

            }
            else if (version == GameVersion.Reloaded)
            {
                this.LoadCounter = new MemoryWatcher<int>(new DeepPointer("binkw32.dll", 0x233B8));

                this.IsSaving = new MemoryWatcher<int>(new DeepPointer(0x01C6EFE0, 0xCC));
                this.IsLoadingDeath = new MemoryWatcher<int>(new DeepPointer(0x1C136E4));
                this.IsPauseMenu = new MemoryWatcher<int>(new DeepPointer(0x01BB166C, 0x0, 0x188));
                this.IsWhiteScreen = new MemoryWatcher<int>(new DeepPointer(0x1C19AF8));

                this.LoadedSubLevels = new MemoryWatcher<int>(new DeepPointer(0x01C14D5C, 0x48));
                this.TotalSubLevels = new MemoryWatcher<int>(new DeepPointer(0x01C14C28, 0x10, 0x9C, 0xBF0));

                this.RespawnCP = new StringWatcher(new DeepPointer(0x01C6EFE0, 0x134, 0xBC, 0x0, 0x3C, 0x0), 128);
                this.PersistentLevel = new StringWatcher(new DeepPointer(0x01C6EFE0, 0x170, 0x1DC, 0x1E8, 0x3C, 0x528, 0x3CC, 0x0), 128);

                this.PlayerPosX = new MemoryWatcher<float>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0xE8));
                this.PlayerPosY = new MemoryWatcher<float>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0xEC));
                this.PlayerPosZ = new MemoryWatcher<float>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0xF0));

                this.MoveState = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0x500));
                this.AllowMoveChange = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0x41D));
                this.IgnoreMoveInput = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2FD));
                this.IgnoreLookInput = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2FE));
                this.IgnoreButtonInput = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x575));

                this.ObjectPosZ = new MemoryWatcher<float>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0x74, 0xF0));

                /* 69 Stars Pointers */
                this.CurrentCP = new MemoryWatcher<int>(new DeepPointer(0x01C14D5C, 0x54, 0x1E0, 0x318, 0x3D4));
                this.TotalCPs = new MemoryWatcher<int>(new DeepPointer(0x01C14D5C, 0x54, 0x1E0, 0x318, 0x3D0));
                this.ActiveTTStretch = new MemoryWatcher<byte>(new DeepPointer(0x01C14D5C, 0x54, 0x1E0, 0x318, 0x3F9));
                this.FinishedTime = new MemoryWatcher<float>(new DeepPointer(0x01C14D5C, 0x54, 0x1E0, 0x318, 0x424));

                this.ThreeStarTime = new MemoryWatcher<float>(new DeepPointer(0x01C14D5C, 0x54, 0x1E0, 0x318, 0x408));
                this.TwoStarTime = new MemoryWatcher<float>(new DeepPointer(0x01C14D5C, 0x54, 0x1E0, 0x318, 0x40C));
                this.OneStarTime = new MemoryWatcher<float>(new DeepPointer(0x01C14D5C, 0x54, 0x1E0, 0x318, 0x410));
            }
            else if (version == GameVersion.Origin)
            {
                this.LoadCounter = new MemoryWatcher<int>(new DeepPointer("binkw32.dll", 0x233B8));

                this.IsSaving = new MemoryWatcher<int>(new DeepPointer(0x01C6EFE0, 0xCC));
                this.IsLoadingDeath = new MemoryWatcher<int>(new DeepPointer(0x01C136F0));
                this.IsPauseMenu = new MemoryWatcher<int>(new DeepPointer(0x01C11BE0, 0x32C));
                this.IsWhiteScreen = new MemoryWatcher<int>(new DeepPointer(0x1C19AF8));

                this.LoadedSubLevels = new MemoryWatcher<int>(new DeepPointer(0x01C14D64, 0x48));
                this.TotalSubLevels = new MemoryWatcher<int>(new DeepPointer(0x01C14C30, 0xC, 0x9C, 0xBF0));

                this.RespawnCP = new StringWatcher(new DeepPointer(0x01C6EFE0, 0x134, 0xBC, 0x0, 0x3C, 0x0), 128);
                this.PersistentLevel = new StringWatcher(new DeepPointer(0x01C11BE0, 0x330, 0x0), 128);

                this.PlayerPosX = new MemoryWatcher<float>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0xE8));
                this.PlayerPosY = new MemoryWatcher<float>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0xEC));
                this.PlayerPosZ = new MemoryWatcher<float>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0xF0));

                this.MoveState = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0x500));
                this.AllowMoveChange = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0x41D));
                this.IgnoreMoveInput = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2FD));
                this.IgnoreLookInput = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2FE));
                this.IgnoreButtonInput = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x575));

                this.ObjectPosZ = new MemoryWatcher<float>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0x74, 0xF0));

                /* 69 Stars Pointers */
                this.CurrentCP = new MemoryWatcher<int>(new DeepPointer(0x01C14D64, 0x54, 0x1E0, 0x318, 0x3D4));
                this.TotalCPs = new MemoryWatcher<int>(new DeepPointer(0x01C14D64, 0x54, 0x1E0, 0x318, 0x3D0));
                this.ActiveTTStretch = new MemoryWatcher<byte>(new DeepPointer(0x01C14D64, 0x54, 0x1E0, 0x318, 0x3F9));
                this.FinishedTime = new MemoryWatcher<float>(new DeepPointer(0x01C14D64, 0x54, 0x1E0, 0x318, 0x424));

                this.ThreeStarTime = new MemoryWatcher<float>(new DeepPointer(0x01C14D64, 0x54, 0x1E0, 0x318, 0x408));
                this.TwoStarTime = new MemoryWatcher<float>(new DeepPointer(0x01C14D64, 0x54, 0x1E0, 0x318, 0x40C));
                this.OneStarTime = new MemoryWatcher<float>(new DeepPointer(0x01C14D64, 0x54, 0x1E0, 0x318, 0x410));
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

        private enum ExpectedDllSizes
        {
            Steam1 = 32632832,
            Steam2 = 32976896,
            Reloaded1 = 60298504,
            Reloaded2 = 42876928,
            Origin = 42889216,
            binkw32 = 229376
        }

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
            {"scraper_p", 9},
            {"tt_TutorialA01_p", 10},
            {"tt_TutorialA02_p", 11},
            {"tt_TutorialA03_p", 12},
            {"tt_EdgeA01_p", 13},
            {"tt_EscapeB01_p", 14},
            {"tt_EscapeA01_p", 15},
            {"tt_StormdrainA02_p", 16},
            {"tt_StormdrainB01_p", 17},
            {"tt_StormdrainB02_p", 18},
            {"tt_StormdrainB03_p", 19},
            {"tt_CranesA01_p", 20},
            {"tt_CranesC01_p", 21},
            {"tt_CranesB02_p", 22},
            {"tt_CranesB01_p", 23},
            {"tt_MallA01_p", 24},
            {"tt_FactoryA01_p", 25},
            {"tt_CranesD01_p", 26},
            {"tt_ConvoyB01_p", 27},
            {"tt_ConvoyB02_p", 28},
            {"tt_ConvoyA01_p", 29},
            {"tt_ConvoyA02_p", 30},
            {"tt_ScraperA01_p", 31},
            {"tt_ScraperB01_p", 32},
        };

        private List<bool> SubLevelStates = new List<bool>();
        private DeepPointer SubLevelFlags = null;

        private Vector3f SDEntryBtn = new Vector3f(1311f, -30027f, -6634f);
        private Vector3f SDExitGate = new Vector3f(1468f, -10471.75f, -7267.36f);
        private Vector3f SDExitBtn = new Vector3f(953f, -6833f, -3132f);

        private bool SDEntryBtnHit = false;
        private bool SDExitReached = false;
        private bool SDExitBtnHit = false;
        private bool SDExitGateBtnHit = false;
        private bool timerStarted = false;
        private bool menuWhileStreaming = false;

        private GameVersion version;

        private Process game;

        private MirrorsEdgeLRTSettings UserSettings;

        private TimerModel timer;

        public GameMemory(MirrorsEdgeLRTSettings Settings, TimerModel _timer)
        {
            timer = _timer;
            UserSettings  = Settings;
            _ignorePIDs = new List<int>();
            SubLevelStates.Add(false);
        }

        public void Update()
        {
            if (_process == null || _process.HasExited)
            {
                if (!this.TryGetGameProcess())
                    return;
            }

            TimedTraceListener.Instance.UpdateCount++;

            _data.UpdateAll(_process);

            bool isLoadingStatic = _data.LoadCounter.Current > _data.LoadCounter.Old;
            
            bool bIgnoreMoveInput = (_data.IgnoreMoveInput.Current & 1) != 0;
            bool bIgnoreButtonInput = (_data.IgnoreButtonInput.Current & 1) != 0;
            bool bIgnoreLookInput = (_data.IgnoreLookInput.Current & 1) != 0;
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
            if (UserSettings.Category != 4)
            {
                if (_data.PersistentLevel.Current == "tutorial_p" && _data.PlayerPosZ.Current < 5854f && _data.PlayerPosZ.Current > 5800f && !timerStarted)
                {
                    this.OnStart?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("starting timer");
                }
            } 
            else
            {
                if (_data.ActiveTTStretch.Current == 15 && bIgnoreMoveInput && bIgnoreLookInput && _data.PlayerPosX.Current == 2224 && !timerStarted)
                {
                    this.OnStart?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("starting timer");
                } 
            }

            /* -- Resetting -- */
            if (UserSettings.Category != 4)
            {
                if (_data.PersistentLevel.Old == "TdMainMenu" && _data.PersistentLevel.Current == "tutorial_p")
                {
                    timerStarted = false;
                    this.OnReset?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("resetting timer");
                }
            } 
            else
            {
                if (_data.PersistentLevel.Old == "TdMainMenu" && _data.PersistentLevel.Current == "tt_TutorialA01_p")
                {
                    timerStarted = false;
                    this.OnReset?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("resetting timer");
                }
            }

            if (timerStarted)
            {
                /* -- Distance Checks -- */
                if (UserSettings.Category != 4)
                {
                    Vector3f PlayerPos = new Vector3f(_data.PlayerPosX.Current, _data.PlayerPosY.Current, _data.PlayerPosZ.Current);

                    if (levels[_data.PersistentLevel.Current] == 2 && !SDExitReached)
                    {
                        if (PlayerPos.Distance(SDExitBtn) < 100f)
                        {
                            SDExitReached = true;
                            SDExitBtnHit = true;
                        }
                    }
                    else if (levels[_data.PersistentLevel.Current] != 2)
                    {
                        SDExitBtnHit = false;
                        SDExitReached = false;
                        SDExitGateBtnHit = false;
                    }

                    if (_data.RespawnCP.Current == "Gate1")
                    {
                        if (PlayerPos.Distance(SDEntryBtn) < 130f)
                        {
                            SDEntryBtnHit = true;
                        }
                    }
                    else
                    {
                        SDEntryBtnHit = false;
                    }

                    if (_data.RespawnCP.Current == "Waterfall")
                    {
                        if (PlayerPos.Distance(SDExitGate) < 130f && _data.PlayerPosY.Current < -10370f)
                        {
                            SDExitGateBtnHit = true;
                        }
                    }
                    else
                    {
                        SDExitGateBtnHit = false;
                    }
                }

                /* -- Splitting -- */
                if (UserSettings.Category != 4)
                {
                    /* -- Full Game -- */
                    if (_data.PersistentLevel.Current != _data.PersistentLevel.Old && levels[_data.PersistentLevel.Current] > levels[_data.PersistentLevel.Old] && _data.PersistentLevel.Old != "tutorial_p" && _data.PersistentLevel.Old != "TdMainMenu")
                    {
                        this.OnSplit?.Invoke(this, EventArgs.Empty);
                        Debug.WriteLine("chapter split");
                    }
                    else if (SDExitBtnHit && UserSettings.SDSplit)
                    {
                        SDExitBtnHit = false;
                        this.OnSplit?.Invoke(this, EventArgs.Empty);
                        Debug.WriteLine("sd exit split");
                    }
                    else if (_data.RespawnCP.Current == "End_game" && _data.ObjectPosZ.Current == 75102 && _data.ObjectPosZ.Old != 75102)
                    {
                        this.OnSplit?.Invoke(this, EventArgs.Empty);
                        Debug.WriteLine("ending split");
                    }
                }
                else
                {
                    /* -- 69 Stars -- */
                    if (_data.CurrentCP.Current > _data.CurrentCP.Old)
                    {
                        if (_data.CurrentCP.Current == _data.TotalCPs.Current)
                        {
                            if (UserSettings.StarsRequired == 0)
                            {
                                this.OnSplit?.Invoke(this, EventArgs.Empty);
                                Debug.WriteLine("tt split");
                            }
                            else if (UserSettings.StarsRequired == 1)
                            {
                                if (_data.OneStarTime.Current > Math.Round(_data.FinishedTime.Current, 2))
                                {
                                    this.OnSplit?.Invoke(this, EventArgs.Empty);
                                    Debug.WriteLine("tt split");
                                }
                            }
                            else if (UserSettings.StarsRequired == 2)
                            {
                                if (_data.TwoStarTime.Current > Math.Round(_data.FinishedTime.Current, 2))
                                {
                                    this.OnSplit?.Invoke(this, EventArgs.Empty);
                                    Debug.WriteLine("tt split");
                                }
                            }
                            else if (UserSettings.StarsRequired == 3)
                            {
                                if (_data.ThreeStarTime.Current > Math.Round(_data.FinishedTime.Current, 2))
                                {
                                    this.OnSplit?.Invoke(this, EventArgs.Empty);
                                    Debug.WriteLine("tt split");
                                }
                            }
                        }
                    }
                }

                if (UserSettings.Category != 4)
                {
                    if (_data.PersistentLevel.Current != "TdMainMenu" && _data.PersistentLevel.Current != null)
                    {
                        if (SubLevelStates.Count != _data.TotalSubLevels.Current - 1 && _data.TotalSubLevels.Current != 0 && _data.TotalSubLevels.Current <= 83)
                        {
                            SubLevelStates.Clear();

                            for (int i = 0; i < _data.TotalSubLevels.Current; i++)
                            {
                                SubLevelStates.Add(false);
                            }
                        }
                    }

                    for (int i = 0; i < SubLevelStates.Count; i++)
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

                        SubLevelStates[i] = isLoading;
                    }
                }

                if (UserSettings.Category != 4)
                {
                    if (_data.IsPauseMenu.Current == 1 && SubLevelStates.Contains(true))
                    {
                        menuWhileStreaming = true;
                    }
                    else if (_data.IsPauseMenu.Current == 0)
                    {
                        menuWhileStreaming = false;
                    }
                }

                /* -- Removing Loads -- */
                if (isLoadingStatic || _data.IsWhiteScreen.Current > 1)
                {
                    this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("static loading screen");
                }
<<<<<<< HEAD
                //else if (_data.IsWhiteScreen.Current == 1 && !(levels[_data.PersistentLevel.Current] == -1 && bIgnoreButtonInput))
                //{
                //    this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                //    Debug.WriteLine("white screen or static loading screen or cutscene");
                //}
=======
                else if (_data.IsWhiteScreen.Current == 1 && !(levels[_data.PersistentLevel.Current] == -1 && bIgnoreButtonInput))
                {
                    this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("white screen or static loading screen or cutscene");
                }
>>>>>>> 011fa7f (initial commit)
                else if (_data.IsWhiteScreen.Current > 1 && _data.IsSaving.Old == 1)
                {
                    this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("load between save icon and static loading screen/cutscene");
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
                else if (UserSettings.Category != 4 && bIgnoreMoveInput && bIgnoreLookInput && bIgnoreButtonInput && _data.MoveState.Current != 38 && _data.RespawnCP.Current != "Training_area_after_cs")
                {
                    this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("block while loading");
                }
                else if (menuWhileStreaming)
                {
                    this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("player paused during level streaming");
                }
                else if (UserSettings.Category != 4 && (
                    // Checkpoints at the end of chapters
                    (_data.RespawnCP.Current == "SWAT_Response2" && _data.IsSaving.Current == 1 && _data.ObjectPosZ.Current > 4641) || // Prologue
                    (_data.RespawnCP.Current == "Plaza" && _data.IsSaving.Current == 1 && _data.PlayerPosZ.Current < -2466) || // Chapter 1
                    (_data.RespawnCP.Current == "Boss_checkpoint1" && _data.IsSaving.Current == 1 && _data.PlayerPosX.Current == 20804.95117f) || // Chapter 2
                    (_data.RespawnCP.Current.Contains("SP03_Plaza_") && _data.IsSaving.Current == 1 && bIgnoreButtonInput) || // Chapter 3
                    (_data.RespawnCP.Current == "Train_Ride_End" && _data.IsSaving.Current == 1 && _data.ObjectPosZ.Current == -1728) || // Chapter 4
                    (_data.RespawnCP.Current == "LastCP" && _data.IsSaving.Current == 1 && _data.PlayerPosX.Current == -51617.66406f) || // Chapter 5
                    (_data.RespawnCP.Current == "Pursuit_chase_end" && _data.IsSaving.Current == 1 && _data.ObjectPosZ.Current > 1252) || // Chapter 6
                    (_data.RespawnCP.Current == "Celeste" && _data.IsSaving.Current == 1 && _data.PlayerPosZ.Current == 349.1499939f) || // Chapter 7
                    (_data.RespawnCP.Current == "Atrium_soft_cp" && _data.IsSaving.Current == 1 && bIgnoreButtonInput) // Chapter 8
                        ))
                {
                    this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("end of chapter save icon");
                }
                else if (UserSettings.Category != 4 && (
                    (_data.RespawnCP.Current == "Office" && SubLevelStates.Contains(true) && _data.PlayerPosZ.Current > 12381) || // 1A
                    (_data.RespawnCP.Current == "Elev_R1-St1" && SubLevelStates.Contains(true) && _data.ObjectPosZ.Current > 4159) || // 1D
                    ((_data.RespawnCP.Current == "Sluice_Out_RoofUp" || _data.RespawnCP.Current == "ChaseJK") && SubLevelStates.Contains(true) && _data.ObjectPosZ.Current > 4159 && !bAllowMoveChange) || // 2D
                    (_data.RespawnCP.Current == "JKfight" && _data.RespawnCP.Current != "after_loading_bo" && SubLevelStates.Contains(true)) || // 2G
                    (_data.RespawnCP.Current == "SP03_Plaza_01" && SubLevelStates.Contains(true) && _data.ObjectPosZ.Current > 6943) || // 3C
                    (_data.RespawnCP.Current == "CombatRooftops" && SubLevelStates.Contains(true) && _data.ObjectPosZ.Current > 1566) || // 5A
                    (_data.RespawnCP.Current == "R2_tomall" && SubLevelStates.Contains(true) && _data.ObjectPosZ.Current < 64 && !bAllowMoveChange) || // 5C
                    (_data.RespawnCP.Current == "Pursuit_chase" && SubLevelStates.Contains(true) && _data.ObjectPosZ.Current >= 4192) || // 6D
                    (_data.RespawnCP.Current == "Elevator_to_Conv" && SubLevelStates.Contains(true) && !bAllowMoveChange) || // 8A
                    (_data.RespawnCP.Current == "combat_3" && SubLevelStates.Contains(true) && !bAllowMoveChange) || // 8B
                    (_data.RespawnCP.Current == "scraper_before_lobby" && SubLevelStates.Contains(true) && _data.ObjectPosZ.Current > 32 && !bAllowMoveChange) || // 9B
                    ((_data.RespawnCP.Current == "Scraper_Inside_elevator_lobby" || _data.RespawnCP.Current == "Elevator_shaft") && SubLevelStates.Contains(true) && _data.IsPauseMenu.Current == 1) || // 9C
                    (_data.RespawnCP.Current == "Gate1" && SubLevelStates.Contains(true) && _data.MoveState.Current != 72 && SDEntryBtnHit) || // Stormdrains Entrance Load
                    (_data.RespawnCP.Current == "Waterfall" && SubLevelStates.Contains(true) && SDExitGateBtnHit) || // Stormdrains Exit Load
                    (_data.RespawnCP.Current == "Platform_fight" && SubLevelStates.Contains(true) && _data.IsPauseMenu.Current == 1) || // Chapter 4 Skip Load
                    (_data.RespawnCP.Current == "steamroom_puzzle" && SubLevelStates.Contains(true) && _data.IsPauseMenu.Current == 1) // Factory Skip Load
                        ))
                {
                    this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("level streaming blocking progress");
                }
                else if (_data.RespawnCP.Current == "subway_roof-mill" && SubLevelStates.Contains(true) && _data.PersistentLevel.Current != "TdMainMenu")
                {
                    this.OnLoadTrue?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("9E hallway load");
                }
                else
                {
                    this.OnLoadFalse?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        bool TryGetGameProcess()
        {
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

            ProcessModuleWow64Safe binkw = game.ModulesWow64Safe().FirstOrDefault(p => p.ModuleName.ToLower() == "binkw32.dll");
            if (binkw == null)
                return false;

            if (binkw.ModuleMemorySize != (int)ExpectedDllSizes.binkw32)
                return false;

            Debug.WriteLine("game version " + version);
            Debug.WriteLine("binkw32 size: " + binkw.ModuleMemorySize);
            _data = new GameData(version);
            _process = game;

            return true;
        }
    }

    enum GameVersion
    {
        Steam,
        Reloaded,
        Origin,
    }
}

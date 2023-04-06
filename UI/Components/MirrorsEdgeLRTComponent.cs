using LiveSplit.Model;
using LiveSplit.UI.Components;
using LiveSplit.UI;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Windows.Forms;

namespace LiveSplit.MirrorsEdgeLRT
{
    public class MirrorsEdgeLRTComponent : LogicComponent
    {
        public override string ComponentName => "Mirror's Edge LRT";

        public MirrorsEdgeLRTSettings Settings { get; set; }

        private TimerModel _timer;
        private GameMemory _gameMemory;
        private Timer _updateTimer;

        public MirrorsEdgeLRTComponent(LiveSplitState state)
        {
#if DEBUG
            Debug.Listeners.Clear();
            Debug.Listeners.Add(TimedTraceListener.Instance);
#endif

            this.Settings = new MirrorsEdgeLRTSettings();

            _timer = new TimerModel { CurrentState = state };
            _timer.CurrentState.OnStart += timer_OnStart;

            _updateTimer = new Timer() { Interval = 16, Enabled = true };
            _updateTimer.Tick += updateTimer_Tick;

            _gameMemory = new GameMemory(this.Settings, this._timer);
            _gameMemory.OnLoadTrue += gameMemory_OnLoadTrue;
            _gameMemory.OnLoadFalse += gameMemory_OnLoadFalse;
            _gameMemory.OnStart += gameMemory_OnStart;
            _gameMemory.OnReset += gameMemory_OnReset;
            _gameMemory.OnSplit += gameMemory_OnSplit;

        }

        public override void Dispose()
        {
            _timer.CurrentState.OnStart -= timer_OnStart;
            _updateTimer?.Dispose();
        }

        void updateTimer_Tick(object sender, EventArgs eventArgs)
        {
            try
            {
                _gameMemory.Update();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        void timer_OnStart(object sender, EventArgs e)
        {
            _timer.InitializeGameTime();
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {

        }

        void gameMemory_OnLoadTrue(object sender, EventArgs e)
        {
            _timer.CurrentState.IsGameTimePaused = true;
        }

        void gameMemory_OnLoadFalse(object sender, EventArgs e)
        {
            _timer.CurrentState.IsGameTimePaused = false;
        }

        void gameMemory_OnStart(object sender, EventArgs e)
        {
            if (this.Settings.AutoStart)
                _timer.Start();
        }

        void gameMemory_OnReset(object sender, EventArgs e)
        {
            if (this.Settings.AutoReset)
                _timer.Reset();
        }

        void gameMemory_OnSplit(object sender, EventArgs e)
        {
            if (this.Settings.AutoSplit)
                _timer.Split();
        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public override Control GetSettingsControl(LayoutMode mode)
        {
            return this.Settings;
        }

        public override void SetSettings(XmlNode settings)
        {
            Settings.SetSettings(settings);
        }

        public int GetSettingsHashCode() => Settings.GetSettingsHashCode();
    }

    public class TimedTraceListener : DefaultTraceListener
    {
        private static TimedTraceListener _instance;
        public static TimedTraceListener Instance => _instance ?? (_instance = new TimedTraceListener());

        private TimedTraceListener() { }

        public int UpdateCount
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public override void WriteLine(string message)
        {
            base.WriteLine("[MELRT]: " + this.UpdateCount + " " + message);
        }
    }
}

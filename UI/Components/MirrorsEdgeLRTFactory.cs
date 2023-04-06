using LiveSplit.UI.Components;
using System;
using LiveSplit.Model;

namespace LiveSplit.MirrorsEdgeLRT
{
    public class MirrorsEdgeLRTFactory : IComponentFactory
    {
        public string ComponentName => "Mirror's Edge LRT";
        public string Description => "Automates splitting and load removal for Mirror's Edge.";
        public ComponentCategory Category => ComponentCategory.Control;

        public IComponent Create(LiveSplitState state) => new MirrorsEdgeLRTComponent(state);

        public string UpdateName => this.ComponentName;
        public string UpdateURL => "";
        public Version Version => Version.Parse("1.0.0");
        public string XMLURL => "";
    }
}

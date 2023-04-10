using LiveSplit.Options;
using LiveSplit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.MirrorsEdgeLRT
{
    public partial class MirrorsEdgeLRTSettings : UserControl
    {
        public bool AutoStart { get; set; }

        public bool AutoSplit { get; set; }

        public bool AutoReset { get; set; }

        public bool SDSplit { get; set; }

        public int Category { get; set; }

        public int StarsRequired { get; set; }

        public bool BagSplit { get; set; }

        public bool ChapterSplit100 { get; set; }

        public int StartTTInd { get; set; }
        public int StartTT { get; set; }

        public MirrorsEdgeLRTSettings()
        {
            InitializeComponent();

            catSelect.Items.AddRange(new object[] { "Any%", "Glitchless", "Inbounds", "100%", "69 stars" });
            starSelect.Items.AddRange(new object[] { "None", "1 Star", "2 Stars", "3 Stars" });
            startTTSelect.Items.AddRange(new object[] { "Playground One", "Playground Two", "Playground Three", "Edge", "Arland", "Flight",
                "Chase", "Stromdrains One", "Stormdrains Two", "Stormdrains Three", "Heat", "Burfield", "Cranes One", "Cranes Two", "New Eden",
                "Factory", "Office", "Convoy One", "Convoy Two", "Atrium One", "Atrium Two", "Shard One", "Shard Two" });

            // default
            this.AutoStart = true;
            this.AutoSplit = true;
            this.AutoReset = true;
            this.SDSplit = false;
            this.Category = 0;
            this.StarsRequired = 0;
            this.BagSplit = false;
            this.ChapterSplit100 = false;
            this.StartTT = 15;

            this.chkAutoStart.DataBindings.Add("Checked", this, "AutoStart", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoSplit.DataBindings.Add("Checked", this, "AutoSplit", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoReset.DataBindings.Add("Checked", this, "AutoReset", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkSDSplit.DataBindings.Add("Checked", this, "SDSplit", false, DataSourceUpdateMode.OnPropertyChanged);
            this.catSelect.DataBindings.Add("SelectedIndex", this, "Category", false, DataSourceUpdateMode.OnPropertyChanged);
            this.starSelect.DataBindings.Add("SelectedIndex", this, "StarsRequired", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkBagSplit.DataBindings.Add("Checked", this, "BagSplit", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkChapterSplit100.DataBindings.Add("Checked", this, "ChapterSplit100", false, DataSourceUpdateMode.OnPropertyChanged);
            this.startTTSelect.DataBindings.Add("SelectedIndex", this, "StartTTInd", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            AutoStart = SettingsHelper.ParseBool(element["AutoStart"]);
            AutoSplit = SettingsHelper.ParseBool(element["AutoSplit"]);
            AutoReset = SettingsHelper.ParseBool(element["AutoReset"]);
            SDSplit = SettingsHelper.ParseBool(element["SDSplit"]);
            Category = SettingsHelper.ParseInt(element["Category"]);
            StarsRequired = SettingsHelper.ParseInt(element["StarsRequired"]);
            BagSplit = SettingsHelper.ParseBool(element["BagSplit"]);
            ChapterSplit100 = SettingsHelper.ParseBool(element["ChapterSplit100"]);
            StartTT = SettingsHelper.ParseInt(element["StartTT"]);
            StartTTInd = SettingsHelper.ParseInt(element["StartTTInd"]);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            return SettingsHelper.CreateSetting(document, parent, "AutoStart", AutoStart) ^
                SettingsHelper.CreateSetting(document, parent, "AutoSplit", AutoSplit) ^
                SettingsHelper.CreateSetting(document, parent, "AutoReset", AutoReset) ^
                SettingsHelper.CreateSetting(document, parent, "SDSplit", SDSplit) ^
                SettingsHelper.CreateSetting(document, parent, "Category", Category) ^
                SettingsHelper.CreateSetting(document, parent, "StarsRequired", StarsRequired) ^
                SettingsHelper.CreateSetting(document, parent, "BagSplit", BagSplit) ^
                SettingsHelper.CreateSetting(document, parent, "ChapterSplit100", ChapterSplit100) ^
                SettingsHelper.CreateSetting(document, parent, "StartTT", StartTT) ^
                SettingsHelper.CreateSetting(document, parent, "StartTTInd", StartTTInd);
        }

        private void catSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (catSelect.SelectedIndex == 4)
            {
                
                labelStars.Visible = true;
                starSelect.Visible = true;
                startTTSelect.Visible = true;
                startTTLbl.Visible = true;

                chkBagSplit.Visible = false;
                chkChapterSplit100.Visible = false;
            } 
            else if (catSelect.SelectedIndex == 3)
            {
                chkBagSplit.Visible = true;
                chkChapterSplit100.Visible = true;

                labelStars.Visible = false;
                starSelect.Visible = false;
                startTTSelect.Visible = false;
                startTTLbl.Visible = false;
            }
            else
            {
                chkBagSplit.Visible = false;
                chkChapterSplit100.Visible = false;

                labelStars.Visible = false;
                starSelect.Visible = false;
                startTTSelect.Visible = false;
                startTTLbl.Visible = false;
            }
            Category = catSelect.SelectedIndex;
        }

        private void starSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            StarsRequired = starSelect.SelectedIndex;
        }

        private void chkSDSplit_CheckedChanged(object sender, EventArgs e)
        {
            SDSplit = chkSDSplit.Checked;
        }

        private void chkAutoReset_CheckedChanged(object sender, EventArgs e)
        {
            AutoReset = chkAutoReset.Checked;
        }

        private void chkAutoSplit_CheckedChanged(object sender, EventArgs e)
        {
            AutoSplit = chkAutoSplit.Checked;
        }

        private void chkAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            AutoStart = chkAutoStart.Checked;
        }

        private void chkBagSplit_CheckedChanged(object sender, EventArgs e)
        {
            BagSplit = chkBagSplit.Checked;
        }

        private void chkChapterSplit100_CheckedChanged(object sender, EventArgs e)
        {
            ChapterSplit100 = chkChapterSplit100.Checked;
        }

        private void startTTSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            StartTTInd = startTTSelect.SelectedIndex;

            switch (StartTTInd)
            {
                case 0:
                    StartTT = 15;
                    break;
                case 1:
                    StartTT = 16;
                    break;
                case 2:
                    StartTT = 21;
                    break;
                case 3:
                    StartTT = 4;
                    break;
                case 4:
                    StartTT = 23;
                    break;
                case 5:
                    StartTT = 22;
                    break;
                case 6:
                    StartTT = 6;
                    break;
                case 7:
                    StartTT = 7;
                    break;
                case 8:
                    StartTT = 8;
                    break;
                case 9:
                    StartTT = 9;
                    break;
                case 10:
                    StartTT = 1;
                    break;
                case 11:
                    StartTT = 20;
                    break;
                case 12:
                    StartTT = 3;
                    break;
                case 13:
                    StartTT = 2;
                    break;
                case 14:
                    StartTT = 14;
                    break;
                case 15:
                    StartTT = 17;
                    break;
                case 16:
                    StartTT = 24;
                    break;
                case 17:
                    StartTT = 12;
                    break;
                case 18:
                    StartTT = 13;
                    break;
                case 19:
                    StartTT = 10;
                    break;
                case 20:
                    StartTT = 11;
                    break;
                case 21:
                    StartTT = 18;
                    break;
                case 22:
                    StartTT = 19;
                    break;
            }
        }
    }
}
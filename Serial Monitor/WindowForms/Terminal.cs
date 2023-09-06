﻿using ODModules;
using Serial_Monitor.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Serial_Monitor.WindowForms {
    public partial class Terminal : Form, Interfaces.ITheme {
        SerialManager? manager = null;
        private SerialManager? Manager {
            get { return manager; }
        }
        public Terminal(SerialManager Manager) {
            this.manager = Manager;
            Manager.CommandProcessed += Manager_CommandProcessed;
            Manager.DataReceived += Manager_DataReceived;
            Manager.NameChanged += Manager_NameChanged;
            InitializeComponent();
            ChangeFormName(manager.StateName);
            if (DesignerSetup.IsWindows10OrGreater() == true) {
                DesignerSetup.UseImmersiveDarkMode(this.Handle, true);
            }
            AddIcons();
        }

        private void Manager_NameChanged(object sender, string Data) {
            if (manager == null) { return; }
            ChangeFormName(manager.StateName);
        }
        private void ChangeFormName(string Item) {
            if (Item.Length == 0) { Text = "Terminal"; }
            Text = Item + " - Terminal";
        }
        private void Manager_DataReceived(object sender, bool PrintLine, string Data) {
            string SourceName = "";
            if (sender.GetType() == typeof(SerialManager)) {
                SerialManager SM = (SerialManager)sender;
                SourceName = SM.Port.PortName;
            }
            if (PrintLine == true) {
                Output.Print(SourceName, Data);
            }
            else {
                Output.AttendToLastLine(SourceName, Data, true);
            }
        }
        private void Manager_CommandProcessed(object sender, string Data) {
            string SourceName = "";
            if (sender.GetType() == typeof(SerialManager)) {
                SerialManager SM = (SerialManager)sender;
                SourceName = SM.Port.PortName;
            }
            Output.Print(SourceName, Data);
        }
        private void Terminal_Load(object sender, EventArgs e) {
            RecolorAll();
        }
        public void ApplyTheme() {

            RecolorAll();
            AddIcons();
        }
        private void AddIcons() {
            DesignerSetup.LinkSVGtoControl(Properties.Resources.ClearWindowContent, btnMenuClearTerminal, DesignerSetup.GetSize(DesignerSetup.IconSize.Small));
            DesignerSetup.LinkSVGtoControl(Properties.Resources.ClearWindowContent, btnClearTerminal, DesignerSetup.GetSize(DesignerSetup.IconSize.Small));
            DesignerSetup.LinkSVGtoControl(Properties.Resources.BringForward, btnTopMost, DesignerSetup.GetSize(DesignerSetup.IconSize.Small));

            DesignerSetup.LinkSVGtoControl(Properties.Resources.Run_16x, btnStartLogging, DesignerSetup.GetSize(DesignerSetup.IconSize.Small));
            DesignerSetup.LinkSVGtoControl(Properties.Resources.Run_16x, btnMenuStartLogging, DesignerSetup.GetSize(DesignerSetup.IconSize.Small));

            DesignerSetup.LinkSVGtoControl(Properties.Resources.Stop_16x, btnMenuStopLogging, DesignerSetup.GetSize(DesignerSetup.IconSize.Small));
            DesignerSetup.LinkSVGtoControl(Properties.Resources.Stop_16x, btnStopLogging, DesignerSetup.GetSize(DesignerSetup.IconSize.Small));

            DesignerSetup.LinkSVGtoControl(Properties.Resources.Save_16x, saveToolStripMenuItem, DesignerSetup.GetSize(DesignerSetup.IconSize.Small));
            DesignerSetup.LinkSVGtoControl(Properties.Resources.OpenFile_16x, openToolStripMenuItem, DesignerSetup.GetSize(DesignerSetup.IconSize.Small));
        }
        private void RecolorAll() {
            ApplicationManager.IsDark = Properties.Settings.Default.THM_SET_IsDark;
            this.SuspendLayout();
            BackColor = Properties.Settings.Default.THM_COL_Editor;
            Output.ForeColor = Properties.Settings.Default.THM_COL_TerminalForeColor;

            Output.BackColor = Properties.Settings.Default.THM_COL_Editor;

            Output.ScrollBarNorth = Properties.Settings.Default.THM_COL_ScrollColor;
            Output.ScrollBarSouth = Properties.Settings.Default.THM_COL_ScrollColor;

            tsMain.BackColor = Properties.Settings.Default.THM_COL_MenuBack;
            tsMain.BackColorNorth = Properties.Settings.Default.THM_COL_MenuBack;
            tsMain.BackColorSouth = Properties.Settings.Default.THM_COL_MenuBack;
            tsMain.MenuBackColorNorth = Properties.Settings.Default.THM_COL_MenuBack;
            tsMain.MenuBackColorSouth = Properties.Settings.Default.THM_COL_MenuBack;
            tsMain.ItemSelectedBackColorNorth = Properties.Settings.Default.THM_COL_ButtonSelected;
            tsMain.ItemSelectedBackColorSouth = Properties.Settings.Default.THM_COL_ButtonSelected;
            tsMain.StripItemSelectedBackColorNorth = Properties.Settings.Default.THM_COL_ButtonSelected;
            tsMain.StripItemSelectedBackColorSouth = Properties.Settings.Default.THM_COL_ButtonSelected;
            tsMain.MenuBorderColor = Properties.Settings.Default.THM_COL_BorderColor;
            tsMain.MenuSeparatorColor = Properties.Settings.Default.THM_COL_SeperatorColor;
            tsMain.MenuSymbolColor = Properties.Settings.Default.THM_COL_SymbolColor;

            tsMain.ItemCheckedBackColorNorth = Properties.Settings.Default.THM_COL_SymbolColor;
            tsMain.ItemCheckedBackColorSouth = Properties.Settings.Default.THM_COL_SymbolColor;

            tsMain.ForeColor = Properties.Settings.Default.THM_COL_ForeColor;
            tsMain.ItemForeColor = Properties.Settings.Default.THM_COL_ForeColor;
            tsMain.ItemSelectedForeColor = Properties.Settings.Default.THM_COL_ForeColor;

            msMain.BackColor = Properties.Settings.Default.THM_COL_MenuBack;
            msMain.BackColorNorth = Properties.Settings.Default.THM_COL_MenuBack;
            msMain.BackColorSouth = Properties.Settings.Default.THM_COL_MenuBack;
            msMain.MenuBackColorNorth = Properties.Settings.Default.THM_COL_MenuBack;
            msMain.MenuBackColorSouth = Properties.Settings.Default.THM_COL_MenuBack;
            msMain.ItemSelectedBackColorNorth = Properties.Settings.Default.THM_COL_ButtonSelected;
            msMain.ItemSelectedBackColorSouth = Properties.Settings.Default.THM_COL_ButtonSelected;
            msMain.StripItemSelectedBackColorNorth = Properties.Settings.Default.THM_COL_ButtonSelected;
            msMain.StripItemSelectedBackColorSouth = Properties.Settings.Default.THM_COL_ButtonSelected;
            msMain.MenuBorderColor = Properties.Settings.Default.THM_COL_BorderColor;
            msMain.MenuSeparatorColor = Properties.Settings.Default.THM_COL_SeperatorColor;
            msMain.MenuSymbolColor = Properties.Settings.Default.THM_COL_SymbolColor;
            msMain.ItemForeColor = Properties.Settings.Default.THM_COL_ForeColor;
            msMain.ItemSelectedForeColor = Properties.Settings.Default.THM_COL_ForeColor;

            foreach (ToolStripItem Itm in tsMain.Items) {
                Itm.ForeColor = Properties.Settings.Default.THM_COL_ForeColor;
            }
            this.ResumeLayout();
        }
        private void Terminal_FormClosing(object sender, FormClosingEventArgs e) {
            if (Manager == null) { return; }
            Manager.CommandProcessed -= Manager_CommandProcessed;
            Manager.DataReceived -= Manager_DataReceived;
            Manager.NameChanged -= Manager_NameChanged;
        }
        private void Output_CommandEntered(object sender, CommandEnteredEventArgs e) {
            try {
                if (Manager != null) {
                    Manager.Post(e.Command, false);
                }
            }
            catch {

            }
        }
        private void Terminal_KeyPress(object sender, KeyPressEventArgs e) {
            Output.Focus();
        }
        private void SetFormat(object Index) {
            int FormatIndex = -1; int.TryParse(Index.ToString(), out FormatIndex);
            foreach (ToolStripItem MItem in ddbDisplayTime.DropDownItems) {
                if (MItem.Tag != null) {
                    int Indx = -1; int.TryParse(MItem.Tag.ToString(), out Indx);
                    if (Indx == FormatIndex) {
                        ddbDisplayTime.Text = MItem.Text;
                        // break;
                    }
                    else {
                        if (MItem.GetType() == typeof(ToolStripMenuItem)) {
                            ((ToolStripMenuItem)MItem).Checked = false;
                        }
                    }
                }
            }
            switch (FormatIndex) {
                case 0:
                    Output.TimeStamps = ConsoleInterface.TimeStampFormat.NoTimeStamps;
                    btnMenuDispDataOnly.Checked = true;
                    btnMenuDispTime.Checked = false;
                    btnMenuDispDate.Checked = false;
                    btnMenuDispDateTime.Checked = false;
                    break;
                case 1:
                    Output.TimeStamps = ConsoleInterface.TimeStampFormat.Time;
                    btnMenuDispDataOnly.Checked = false;
                    btnMenuDispTime.Checked = true;
                    btnMenuDispDate.Checked = false;
                    btnMenuDispDateTime.Checked = false;
                    break;
                case 3:
                    Output.TimeStamps = ConsoleInterface.TimeStampFormat.Date;
                    btnMenuDispDataOnly.Checked = false;
                    btnMenuDispTime.Checked = false;
                    btnMenuDispDate.Checked = true;
                    btnMenuDispDateTime.Checked = false;
                    break;
                case 4:
                    Output.TimeStamps = ConsoleInterface.TimeStampFormat.DateTime;
                    btnMenuDispDataOnly.Checked = false;
                    btnMenuDispTime.Checked = false;
                    btnMenuDispDate.Checked = false;
                    btnMenuDispDateTime.Checked = true;
                    break;
            }
        }
        private void dataOnlyToolStripMenuItem_Click(object sender, EventArgs e) {
            SetFormat(dataOnlyToolStripMenuItem.Tag);
        }
        private void timeStampsToolStripMenuItem_Click(object sender, EventArgs e) {
            SetFormat(timeStampsToolStripMenuItem.Tag);
        }
        private void dateStampsToolStripMenuItem_Click(object sender, EventArgs e) {
            SetFormat(dateStampsToolStripMenuItem.Tag);
        }
        private void dateTimeStampsToolStripMenuItem_Click(object sender, EventArgs e) {
            SetFormat(dateTimeStampsToolStripMenuItem.Tag);
        }
        private void dataOnlyToolStripMenuItem1_Click(object sender, EventArgs e) {
            SetFormat(btnMenuDispDataOnly.Tag);
        }
        private void btnMenuDispTime_Click(object sender, EventArgs e) {
            SetFormat(btnMenuDispTime.Tag);
        }
        private void btnMenuDispDate_Click(object sender, EventArgs e) {
            SetFormat(btnMenuDispDate.Tag);
        }
        private void btnMenuDispDateTime_Click(object sender, EventArgs e) {
            SetFormat(btnMenuDispDateTime.Tag);
        }
        private void TopMostSetting() {
            btnTopMost.Checked = !btnTopMost.Checked;
            btnMenuTopMost.Checked = !btnMenuTopMost.Checked;
            this.TopMost = !this.TopMost;
        }
        private void topMostToolStripMenuItem_Click(object sender, EventArgs e) {
            btnTopMost.Checked = !btnTopMost.Checked;
            btnMenuTopMost.Checked = !btnMenuTopMost.Checked;
            this.TopMost = !this.TopMost;
        }
        private void toolStripButton2_Click(object sender, EventArgs e) {
            TopMostSetting();
        }

        private void btnClearTerminal_Click(object sender, EventArgs e) {
            Output.Clear();
        }
        private void btnMenuClearTerminal_Click(object sender, EventArgs e) {
            Output.Clear();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Close();
        }

        private void btnMenuZoom050_Click(object sender, EventArgs e) {
            Output.Zoom = 50;
        }
        private void btnMenuZoom075_Click(object sender, EventArgs e) {
            Output.Zoom = 75;
        }
        private void btnMenuZoom100_Click(object sender, EventArgs e) {
            Output.Zoom = 100;
        }
        private void btnMenuZoom110_Click(object sender, EventArgs e) {
            Output.Zoom = 110;
        }
        private void btnMenuZoom120_Click(object sender, EventArgs e) {
            Output.Zoom = 120;
        }
        private void btnMenuZoom150_Click(object sender, EventArgs e) {
            Output.Zoom = 150;
        }
        private void btnMenuZoom200_Click(object sender, EventArgs e) {
            Output.Zoom = 200;
        }
        private void btnMenuZoom300_Click(object sender, EventArgs e) {
            Output.Zoom = 300;
        }
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e) {
            Settings ConfigApp = new Settings();
            ApplicationManager.OpenInternalApplicationOnce(ConfigApp, true);
        }
    }
}

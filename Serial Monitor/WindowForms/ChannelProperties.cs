﻿using Serial_Monitor.Classes;
using Serial_Monitor.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Serial_Monitor.WindowForms {
    public partial class ChannelProperties : Form, ITheme {
        SerialManager? manager = null;
        public SerialManager? Manager {
            get { return manager; }
            set { 
                manager = value;
                if (manager != null) {
                    propertyGrid1.SelectedObject = Manager;
                }
            }
        }
        public ChannelProperties(SerialManager Manager) {
            InitializeComponent();
            this.Manager = Manager;
            if (DesignerSetup.IsWindows10OrGreater() == true) {
                DesignerSetup.UseImmersiveDarkMode(this.Handle, true);
            }
         
        }

        public void ApplyTheme() {
            BackColor = Properties.Settings.Default.THM_COL_Editor;
            propertyGrid1.ViewBackColor = Properties.Settings.Default.THM_COL_Editor;
            propertyGrid1.LineColor = Properties.Settings.Default.THM_COL_MenuBack;
            propertyGrid1.CategoryForeColor = Properties.Settings.Default.THM_COL_ForeColor;
            propertyGrid1.ViewForeColor = Properties.Settings.Default.THM_COL_ForeColor;
            propertyGrid1.DisabledItemForeColor = Properties.Settings.Default.THM_COL_ForeColor;
            propertyGrid1.CategorySplitterColor = Properties.Settings.Default.THM_COL_GridLineColor;
            propertyGrid1.ViewBorderColor = Properties.Settings.Default.THM_COL_GridLineColor;
            propertyGrid1.BackColor = Properties.Settings.Default.THM_COL_Editor;
            propertyGrid1.CommandsBackColor = Properties.Settings.Default.THM_COL_Editor;
            propertyGrid1.HelpBackColor = Properties.Settings.Default.THM_COL_Editor;
            // propertyGrid1.color = Properties.Settings.Default.THM_COL_Editor;

            propertyGrid1.SelectedItemWithFocusBackColor = Properties.Settings.Default.THM_COL_SelectedColor;
            propertyGrid1.SelectedItemWithFocusForeColor = Properties.Settings.Default.THM_COL_ForeColor;
        }

        private void ChannelProperties_Load(object sender, EventArgs e) {
            ApplyTheme();
        }

        private void ChannelProperties_VisibleChanged(object sender, EventArgs e) {
            Classes.ApplicationManager.InvokeApplicationEvent();
        }
        private void ChannelProperties_SizeChanged(object sender, EventArgs e) {
            Classes.ApplicationManager.InvokeApplicationEvent();
        }
        private void ChannelProperties_FormClosed(object sender, FormClosedEventArgs e) {
            Classes.ApplicationManager.InvokeApplicationEvent();
        }
    }
}

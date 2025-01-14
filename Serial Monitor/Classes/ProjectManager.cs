﻿using Handlers;
using ODModules;
using Serial_Monitor.Classes.Button_Commands;
using Serial_Monitor.Classes.Step_Programs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataType = Handlers.DataType;

namespace Serial_Monitor.Classes {
    public static class ProjectManager {
        public static List<KeypadButton> Buttons = new List<KeypadButton>();
        private const int MaximumButtons = 25;
        public static void SetKeypadButton(int Index, string CmdType, string CmcLine, string DisplayText, string Channel, int Symbol, int CommandShortcut) {
            if (Buttons.Count == MaximumButtons) {
                if (Index < MaximumButtons) {
                    KeypadButton Btn = Buttons[Index];
                    object? TagData = Btn.Tag;
                    if (TagData != null) {
                        if (TagData.GetType() == typeof(BtnCommand)) {
                            Btn.Text = DisplayText;
                            BtnCommand Data = (BtnCommand)TagData;
                            Data.SetValue(EnumManager.StringToCommandType(CmdType), CmcLine, Channel, Symbol, CommandShortcut);
                        }
                    }
                }
            }
        }
        public static void ClearKeypadButtons() {
            for (int i = 0; i < Buttons.Count; i++) {
                object? TagData = Buttons[i].Tag;
                if (TagData != null) {
                    if (TagData.GetType() == typeof(BtnCommand)) {
                        Buttons[i].Text = "";
                        BtnCommand Data = (BtnCommand)TagData;
                        Data.Reset();
                    }
                }
            }
        }
        public static void LoadGenericKeypadButtons() {
            for (int i = 0; i < MaximumButtons; i++) {
                BtnCommand Cmd = new BtnCommand();
                KeypadButton kBtn = new KeypadButton();
                kBtn.Tag = Cmd;
                Buttons.Add(kBtn);
                Cmd.LinkedButton = Buttons[i];
            }
        }
        public static event FileOpenedHandler? ProgramNameChanged;
        public delegate void FileOpenedHandler(string Address);

        public static event ButtonPropertyChangedHandler? ButtonPropertyChanged;
        public delegate void ButtonPropertyChangedHandler(KeypadButton sender);
        public static void InvokeButtonPropertyChanged(KeypadButton sender) {
            ButtonPropertyChanged?.Invoke(sender);
        }
        public static void WriteFile(string FileAddress) {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string TempVer = fvi.ProductMajorPart + "." + fvi.ProductMinorPart.ToString();
            decimal ProgramVersion = 0; decimal.TryParse(TempVer, out ProgramVersion);
            using (StreamWriter Sw = new StreamWriter(FileAddress)) {
                Sw.WriteLine("--------------------------");
                Sw.WriteLine("-- SERIAL MONITOR");
                Sw.WriteLine("-- VERSION " + ProgramVersion + " (Build " + fvi.ProductBuildPart.ToString() + ")");
                Sw.WriteLine("--------------------------");
                Sw.WriteLine("");
                DocumentHandler.WriteEntry(Sw, "1");
                DocumentHandler.WriteComment(Sw, 0, "  Document Details");
                DocumentHandler.Write(Sw, 1, "ProgramName", "");
                DocumentHandler.Write(Sw, 1, "Author", Environment.UserName);
                DocumentHandler.Write(Sw, 1, "Version", ProgramVersion);
                Sw.WriteLine("");
                if (SystemManager.SerialManagers.Count > 0) {
                    DocumentHandler.WriteComment(Sw, 0, "  Channels");
                    int i = 0;
                    foreach (SerialManager Sm in SystemManager.SerialManagers) {
                        DocumentHandler.Write(Sw, 1, "CHAN_" + i.ToString());
                        DocumentHandler.Write(Sw, 2, "Name", Sm.Name);
                        DocumentHandler.Write(Sw, 2, "Port", Sm.Port.PortName);
                        DocumentHandler.Write(Sw, 2, "Baud", Sm.Port.BaudRate);
                        DocumentHandler.Write(Sw, 2, "DataSize", Sm.Port.DataBits);
                        DocumentHandler.Write(Sw, 2, "StopBits", EnumManager.StopBitsToString(Sm.Port.StopBits));
                        DocumentHandler.Write(Sw, 2, "Parity", EnumManager.ParityToString(Sm.Port.Parity));
                        DocumentHandler.Write(Sw, 2, "ControlFlow", EnumManager.HandshakeToString(Sm.Port.Handshake));
                        DocumentHandler.Write(Sw, 2, "InType", EnumManager.InputFormatToString(Sm.InputFormat).B);
                        DocumentHandler.Write(Sw, 2, "OutType", EnumManager.OutputFormatToString(Sm.OutputFormat).B);
                        DocumentHandler.Write(Sw, 2, "LineFormat", EnumManager.LineFormattingToString(Sm.LineFormat));
                        DocumentHandler.Write(Sw, 2, "ModbusMstr", Sm.IsMaster);
                        DocumentHandler.Write(Sw, 2, "OutputToMstr", Sm.OutputToMasterTerminal);
                        Sw.WriteLine(StringHandler.AddTabs(1, "}"));
                        i++;
                    }
                }
                Sw.WriteLine("");
                if (ProgramManager.Programs.Count > 0) {
                    DocumentHandler.WriteComment(Sw, 0, "  Step Programs");
                    int Cnt = 0;
                    foreach (ProgramObject Prg in ProgramManager.Programs) {
                        if (Prg.Program.Count > 0) {
                            DocumentHandler.Write(Sw, 1, "STEP_" + Cnt.ToString());
                            DocumentHandler.Write(Sw, 2, "Name", Prg.Name);
                            DocumentHandler.Write(Sw, 2, "Command", Prg.Command);
                            Sw.WriteLine(StringHandler.AddTabs(2, "def,a(str):Data={"));
                            foreach (ListItem LstItm in Prg.Program) {
                                if (LstItm.SubItems.Count == 3) {
                                    string EnableTxt = LstItm.SubItems[0].Checked == true ? "1" : "0";
                                    string Command = "";
                                    object? CommandObj = LstItm.SubItems[1].Tag;
                                    if (CommandObj != null) {
                                        if (CommandObj.GetType() == typeof(StepEnumerations.StepExecutable)) {      //4294967295
                                            Command = ((long)((StepEnumerations.StepExecutable)CommandObj)).ToString("0000000000");
                                        }
                                    }
                                    string Arguments = LstItm.SubItems[2].Text;
                                    Sw.WriteLine(StringHandler.AddTabs(3, StringHandler.EncapsulateString(EnableTxt + ":" + Command + ":" + Arguments)));
                                }
                            }
                            Sw.WriteLine(StringHandler.AddTabs(2, "}"));
                            Sw.WriteLine(StringHandler.AddTabs(1, "}"));
                        }
                        Cnt++;
                    }
                }
                if (Buttons.Count > 0) {
                    DocumentHandler.WriteComment(Sw, 0, "  Keypad Buttons");
                    int Cnt = 0;
                    foreach (KeypadButton Btn in Buttons) {
                        if (Btn.Tag != null) {
                            if (Btn.Tag.GetType() == typeof(BtnCommand)) {
                                BtnCommand CmdSet = (BtnCommand)Btn.Tag;
                                if ((CmdSet.IsSet == true) || (CmdSet.IsEdited == true)) {
                                    DocumentHandler.Write(Sw, 1, "KBTN_" + Cnt.ToString());
                                    DocumentHandler.Write(Sw, 2, "Text", Btn.Text);
                                    DocumentHandler.Write(Sw, 2, "Command", CmdSet.CommandLine);
                                    DocumentHandler.Write(Sw, 2, "Type", EnumManager.CommandTypeToString(CmdSet.Type));
                                    DocumentHandler.Write(Sw, 2, "Channel", CmdSet.Channel);
                                    DocumentHandler.Write(Sw, 2, "Symbol", (int)CmdSet.DisplaySymbol);
                                    DocumentHandler.Write(Sw, 2, "Shortcut", (int)CmdSet.Shortcut);
                                    Sw.WriteLine(StringHandler.AddTabs(1, "}"));
                                }
                            }
                        }
                        Cnt++;
                    }
                }
            }
        }
        private static void Write(StreamWriter StrWriter, int Tabs, string Name, int Value) {

        }
        private static List<string> GetList(object Input) {
            if (Input == null) {
                return new List<string>();
            }
            if (Input.GetType() == typeof(List<string>)) {
                return (List<string>)Input;
            }
            return new List<string>();
        }
        public static void ReadSMPFile(string FileAddress, SerialManager.CommandProcessedHandler CmdProc, SerialManager.DataProcessedHandler DataProc) {
            ProgramManager.Programs.Add(new ProgramObject());
            int CurrentProgramIndex = 0;
            for (int i = 0; i < DocumentHandler.PARM.Count; i++) {
                string ParameterName = DocumentHandler.PARM[i].Name;
                if (ParameterName.StartsWith("CHAN")) {
                    ParameterStructure Pstrc = DocumentHandler.PARM[i];
                    SerialManager Sm = new SerialManager();
                    Sm.Name = DocumentHandler.GetStringVariable(Pstrc, "Name", "");
                    try {
                        Sm.Port.PortName = DocumentHandler.GetStringVariable(Pstrc, "Port", "");
                    }
                    catch { }
                    try {
                        Sm.BaudRate = DocumentHandler.GetIntegerVariable(Pstrc, "Baud", 9600);
                    }
                    catch { }
                    try {
                        Sm.Port.DataBits = DocumentHandler.GetIntegerVariable(Pstrc, "DataSize", 8);
                    }
                    catch { }
                    try {
                        Sm.Port.StopBits = EnumManager.StringToStopBits(DocumentHandler.GetStringVariable(Pstrc, "StopBits", "1"));
                    }
                    catch { }
                    try {
                        Sm.Port.Parity = EnumManager.StringToParity(DocumentHandler.GetStringVariable(Pstrc, "Parity", "N"));
                    }
                    catch { }
                    try {
                        Sm.Port.Handshake = EnumManager.StringToHandshake(DocumentHandler.GetStringVariable(Pstrc, "ControlFlow", ""));
                    }
                    catch { }
                    try {
                        Sm.InputFormat = EnumManager.StringToInputFormat(DocumentHandler.GetStringVariable(Pstrc, "InType", "frmTxt"));
                    }
                    catch { }
                    try {
                        Sm.OutputFormat = EnumManager.StringToOutputFormat(DocumentHandler.GetStringVariable(Pstrc, "OutType", "frmTxt"));
                    }
                    catch { }
                    try {
                        Sm.LineFormat = EnumManager.StringToLineFormatting(DocumentHandler.GetStringVariable(Pstrc, "LineFormat", "frmLineNone"));
                    }
                    catch { }
                    try {
                        Sm.IsMaster = DocumentHandler.GetBooleanVariable(Pstrc, "ModbusMstr");
                    }
                    catch { }
                    try {
                        Sm.OutputToMasterTerminal = DocumentHandler.GetBooleanVariable(Pstrc, "OutputToMstr");
                    }
                    catch { }

                    Sm.CommandProcessed += CmdProc;// SerManager_CommandProcessed;
                    Sm.DataReceived += DataProc;// SerMan_DataReceived;
                    SystemManager.SerialManagers.Add(Sm);
                }
                else if (ParameterName.StartsWith("KBTN")) {
                    if (ParameterName.Split('_').Length == 2) {
                        string IndexStr = ParameterName.Split('_')[1];
                        int Index = 0; int.TryParse(IndexStr, out Index);
                        string ButtonText = DocumentHandler.GetStringVariable(DocumentHandler.PARM[i], "Text", "");
                        string CommandText = DocumentHandler.GetStringVariable(DocumentHandler.PARM[i], "Command", "");
                        string CommandType = DocumentHandler.GetStringVariable(DocumentHandler.PARM[i], "Type", "NONE");
                        string CommandChannel = DocumentHandler.GetStringVariable(DocumentHandler.PARM[i], "Channel", "");
                        int CommandSymbol = DocumentHandler.GetIntegerVariable(DocumentHandler.PARM[i], "Symbol", 0);
                        int CommandShortcut = DocumentHandler.GetIntegerVariable(DocumentHandler.PARM[i], "Shortcut", 0);
                        SetKeypadButton(Index, CommandType, CommandText, ButtonText, CommandChannel, CommandSymbol, CommandShortcut);
                    }
                }
                else if (ParameterName.StartsWith("STEP")) {
                    if (CurrentProgramIndex > 0) {
                        ProgramManager.Programs.Add(new ProgramObject());
                    }
                    //E:00000000:
                    ProgramManager.Programs[CurrentProgramIndex].Name = DocumentHandler.GetStringVariable(DocumentHandler.PARM[i], "Name", "");
                    ProgramManager.Programs[CurrentProgramIndex].Command = DocumentHandler.GetStringVariable(DocumentHandler.PARM[i], "Command", "");
                    if (DocumentHandler.IsDefinedInParameter("Data", DocumentHandler.PARM[i])) {

                        List<string> Data = GetList(DocumentHandler.PARM[i].GetVariable("Data", false, DataType.STR));
                        for (int j = 0; j < Data.Count; j++) {
                            ProgramManager.Programs[CurrentProgramIndex].DecodeFileCommand(Data[j]);
                        }
                    }
                    CurrentProgramIndex++;

                }
            }
        }
        public static void ReadCMSLFile(string FileAddress, SerialManager.CommandProcessedHandler CmdProc, SerialManager.DataProcessedHandler DataProc) {
            SerialManager Sm = new SerialManager();
            Sm.CommandProcessed += CmdProc;// SerManager_CommandProcessed;
            Sm.DataReceived += DataProc;// SerMan_DataReceived;
            SystemManager.SerialManagers.Add(Sm);
            ProgramManager.Programs.Add(new ProgramObject("Legacy Program"));
            try {
                using (StreamReader TxtReader = new StreamReader(FileAddress)) {
                    while (TxtReader.Peek() > -1) {
                        ProgramManager.Programs[0].DecodeLegacyFileCommand(TxtReader.ReadLine() ?? "");
                    }
                }
            }
            catch { }
        }
        public static StepEnumerations.StepExecutable ExecutableFromLegacyString(string Input) {
            Input = Input.ToUpper();
            if (Input == "SND") { return StepEnumerations.StepExecutable.SendString; }
            else if (Input == "STXT") { return StepEnumerations.StepExecutable.SendText; }
            else if (Input == "PRNT") { return StepEnumerations.StepExecutable.Print; }
            else if (Input == "END") { return StepEnumerations.StepExecutable.Close; }
            else if (Input == "OPEN") { return StepEnumerations.StepExecutable.Open; }
            else if (Input == "SNDL") { return StepEnumerations.StepExecutable.SendString; }
            else if (Input == "CLR") { return StepEnumerations.StepExecutable.Clear; }
            else if (Input == "DLY") { return StepEnumerations.StepExecutable.Delay; }
            else if (Input == "GOTO") { return StepEnumerations.StepExecutable.GoToLine; }
            // else if (Input == "WHEN_TCK") { return StepEnumerations.StepExecutable.GoTo; }
            else if (Input == "SBYTE") { return StepEnumerations.StepExecutable.SendByte; }
            else if (Input == "SWS") { return StepEnumerations.StepExecutable.SwitchSender; }
            else { return StepEnumerations.StepExecutable.NoOperation; }
        }
    }
}

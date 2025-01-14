﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serial_Monitor.Classes.Step_Programs {
    public static class StepEnumerations {
        public enum StepState {
            Stopped = 0x00,
            Paused = 0x01,
            Running = 0x02
        }
        public enum StepExecutable {
            NoOperation =       0x000000,
            GoTo =              0x010080,
            GoToLine =          0x010042,
            Call =              0x010041,
            Label =             0x010040,
            Delay =             0x010100,
            End =               0x010200,
            SetProgram =        0x010001,
            If =                0x010081,
            EndIf =             0x01FFFF,
            SwitchSender    =   0x020001,
            Open =              0x020020,
            Close =             0x020040,
            SendByte =          0x030002,
            SendString =        0x030004,
            SendLine =          0x030008,
            SendText =          0x030010,
            Print =             0x040040,
            PrintVariable =     0x040060,
            PrintText =         0x040061,
            Clear =             0x040080,
            DeclareVariable =   0x050001,
            IncrementVariable = 0x050002,
            DecrementVariable = 0x050003,
           
            ///SelectChannel = 0x050400,
            //NewChannel = 0x050800,
            //DeleteChannel = 0x051000,
            //JumpOnPress = 0x060001,
            MousePosition =     0x090001,
            MouseLeftClick =    0x090002,
            SendKeys =          0x090010,
            ResetChannelCounter = 0x10050400
        }
    }
}

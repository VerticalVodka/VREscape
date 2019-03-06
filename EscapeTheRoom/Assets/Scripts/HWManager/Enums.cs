using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public static class Enums
{
    public enum ButtonEnum
    {
        Button1,
        Button2,
        Button3,
        Button4,
        Rotary1,
        Rotary2,
        Button5
    }

    public enum RotaryEnum
    {
        Rotary1,
        Rotary2
    }
    
    public enum UnlockEnum
    {
        [Description("s")]
        Safe,
        [Description("d")]
        Drawer,
        [Description("x")]
        Test,
		[Description("c")]
        CloseSafe
    }
}
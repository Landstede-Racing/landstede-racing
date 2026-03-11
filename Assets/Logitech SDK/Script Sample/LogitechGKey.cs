// Logitech Gaming SDK
//
// Copyright (C) 2011-2014 Logitech. All rights reserved.
// Author: Tiziano Pigliucci
// Email: devtechsupport@logitech.com

using System;
using UnityEngine;

public class LogitechGKey : MonoBehaviour
{
    private static string lastKeyPress = "";
    public bool usingCallback;

    private string descriptionLabel = "";

    // Use this for initialization
    private void Start()
    {
        descriptionLabel = "Last g-key event : ";
        lastKeyPress = "No g-key event";
        //Value used to show the two different ways to implement g-keys support in your game
        //change it to false to try the non-callback version
        if (usingCallback)
        {
            LogitechGSDK.logiGKeyCbContext cbContext;
            var cbInstance = new LogitechGSDK.logiGkeyCB(GkeySDKCallback);
            cbContext.gkeyCallBack = cbInstance;
            cbContext.gkeyContext = IntPtr.Zero;
            LogitechGSDK.LogiGkeyInit(ref cbContext);
        }
        else
        {
            LogitechGSDK.LogiGkeyInitWithoutCallback();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!usingCallback)
        {
            for (var index = 6; index <= LogitechGSDK.LOGITECH_MAX_MOUSE_BUTTONS; index++)
                if (LogitechGSDK.LogiGkeyIsMouseButtonPressed(index) == 1)
                    lastKeyPress = "MOUSE DOWN Button : " + index;

            for (var index = 1; index <= LogitechGSDK.LOGITECH_MAX_GKEYS; index++)
            for (var mKeyIndex = 1; mKeyIndex <= LogitechGSDK.LOGITECH_MAX_M_STATES; mKeyIndex++)
                if (LogitechGSDK.LogiGkeyIsKeyboardGkeyPressed(index, mKeyIndex) == 1)
                    lastKeyPress = "KEYBOARD/HEADSET DOWN Button : " + index;
        }
    }

    private void OnDestroy()
    {
        //Free G-Keys SDKs before quitting the game
        LogitechGSDK.LogiGkeyShutdown();
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 450, 200, 50), descriptionLabel + "" + lastKeyPress);
    }

    private void GkeySDKCallback(LogitechGSDK.GkeyCode gKeyCode, string gKeyOrButtonString, IntPtr context)
    {
        if (gKeyCode.keyDown == 0)
        {
            if (gKeyCode.mouse == 1)
                lastKeyPress = "MOUSE UP" + gKeyOrButtonString;
            else
                lastKeyPress = "KEYBOARD/HEADSET RELEASED " + gKeyOrButtonString;
        }
        else
        {
            if (gKeyCode.mouse == 1)
                lastKeyPress = "MOUSE DOWN " + gKeyOrButtonString;
            else
                lastKeyPress = "KEYBOARD/HEADSET PRESSED " + gKeyOrButtonString;
        }
    }
}
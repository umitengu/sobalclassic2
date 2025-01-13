//
// Copyright 2023 Sony Corporation
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mocopi.Sensor.DataReader.Sample
{
    public class AndroidBackButton : MonoBehaviour
    {
        private void Update()
        {
#if UNITY_EDITOR
            // DoNothing.
#elif UNITY_ANDROID
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
#endif
        }
    }
}

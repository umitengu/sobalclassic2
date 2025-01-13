/*
* Copyright 2023 Sony Corporation
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mocopi.Ui.Enums;
using Mocopi.Mobile.Sdk.Common;

namespace Mocopi.Ui
{
    //[ExecuteInEditMode]
    [RequireComponent(typeof(Graphic))]
    public class UIDesignAdjuster : MonoBehaviour
    {
        public EnumUIDesignType UIDesignType = EnumUIDesignType.Default;

        private EnumUIDesignType _prevUIDesignType;

        public void OnEnable()
        {
            UpdateUIDesign();
        }

        public void Update()
        {
            if (UIDesignType != _prevUIDesignType)
            {
                UpdateUIDesign();
            }
        }

        public void UpdateUIDesign()
        {
            if (TryGetComponent<Graphic>(out var graphic))
            {
                Color prevColor = graphic.color;
                Color color = Constants.MocopiUiConst.UI_DESIGN_COLOR_DICTIONARY[UIDesignType];
                graphic.color = color;

                if (color != prevColor)
                {
                    LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), string.Format("Change graphic color {0} to {1}", prevColor, graphic.color));
                }
            }

            _prevUIDesignType = UIDesignType;
        }
    }
}

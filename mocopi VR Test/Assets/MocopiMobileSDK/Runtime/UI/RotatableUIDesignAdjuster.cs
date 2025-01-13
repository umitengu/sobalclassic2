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
    public class RotatableUIDesignAdjuster : MonoBehaviour
    {
		public EnumUIDesignType UIDesignTypePortrait = EnumUIDesignType.Default;
		public EnumUIDesignType UIDesignTypeLandscapeLeft = EnumUIDesignType.Default;
		public EnumUIDesignType UIDesignTypeLandscapeRight = EnumUIDesignType.Default;

        private EnumUIDesignType _currentUIDesignType;
		private EnumUIDesignType _prevUIDesignType;

        private EnumUIDesignType TargetUIDesignType
        {
            get
            {
                switch (Screen.orientation)
                {
                    case ScreenOrientation.Portrait:
                        return UIDesignTypePortrait;
                    case ScreenOrientation.LandscapeLeft:
                        return UIDesignTypeLandscapeLeft;
                    case ScreenOrientation.LandscapeRight:
                        return UIDesignTypeLandscapeRight;
                    default:
                        return UIDesignTypePortrait;
                }
            }
        }

        public void Update()
        {
            if (_currentUIDesignType != TargetUIDesignType || _currentUIDesignType != _prevUIDesignType)
            {
				_currentUIDesignType = TargetUIDesignType;
                UpdateUIDesign();
            }
        }

		public void UpdateUIDesign()
		{
            EnumUIDesignType designType = TargetUIDesignType;

			if (TryGetComponent<Graphic>(out var graphic))
			{
				Color prevColor = graphic.color;
				Color color = Constants.MocopiUiConst.UI_DESIGN_COLOR_DICTIONARY[designType];
				graphic.color = color;

				if (color != prevColor)
				{
					LogUtility.Debug(LogUtility.GetClassName(), LogUtility.GetMethodName(), string.Format("Change graphic color {0} to {1}", prevColor, graphic.color));
				}
			}

			_prevUIDesignType = _currentUIDesignType;
            _currentUIDesignType = designType;
		}
	}
}

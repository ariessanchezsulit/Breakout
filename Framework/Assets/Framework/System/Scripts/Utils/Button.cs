using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

using FullInspector;

using Common;
using Common.Extensions;
using Common.Signal;

namespace Framework
{
    // alias
    using CColor = Common.Extensions.Color;
    using UScene = UnityEngine.SceneManagement.Scene;

    /// <summary>
    /// This component should be attached to a button.
    /// This publishes signals for when the button is clicked, hovered, unhovered, pressed, and released.
    /// This can be extended by buttons that need to pass data when clicked (ex. ItemButton).
    /// </summary>
	public class Button : BaseBehavior
    {
        private static readonly string ERROR = CColor.red.LogHeader("[ERROR]");
        private static readonly string WARNING = CColor.yellow.LogHeader("[WARNING]");


        //FULL INSPECTOR FEATURE
        //===============================================================================================================================
        #region FULL INSPECTOR FEATURE
        bool IfShowDropdown;
        bool IfHideDropdown;

        //ENABLES PREVIEW OF DROPDOWN LIST OF SCENES
        [InspectorShowIf("IfShowDropdown"), InspectorOrder(3)]
        public List<TypeOfStringsClass> buttonClassList;

        //BUTTONS INSIDE THE LIST OVERWRITES THE CURRENT ENUM EBUTTONTYPE UPON CLICKING
        public void UpdateEnum(EButtonType type)
        {
            _Button = type;
        }

        //CHECKS IF STRING IS EMPTY IN WHICH CASE USES THE DEFAULT VALUE OF THE ENUM, IF STRING IS NOT EMPTY IT OVERWRITES THE ENUM VALUE
        void PresetValues()
        {
            if (string.IsNullOrEmpty(ButtonTypeString))
            {
                ButtonTypeString = _Button.ToString();
            }
            else
            {
                _Button = (EButtonType)Enum.Parse(typeof(EButtonType), ButtonTypeString);
            }
        }

        //AN INSPECTOR BUTTON WHICH HIDES THE DROPDOWN
        [InspectorButton, InspectorHideIf("IfHideDropdown"), InspectorOrder(2)]
        void ShowDropDown()
        {
            buttonClassList = new List<TypeOfStringsClass>();

            for (int i = 0; i < EButtonType.GetValues(typeof(EButtonType)).Length; i++)
            {
                TypeOfStringsClass typertemp = new TypeOfStringsClass();
                typertemp.name = ((EButtonType)i).ToString();
                typertemp.SetButtonReference(this);
                buttonClassList.Add(typertemp);
            }
            IfShowDropdown = true;
            IfHideDropdown = true;
            _Button = (EButtonType)Enum.Parse(typeof(EButtonType), ButtonTypeString);
        }

        //AN INSPECTOR BUTTON WHICH HIDES THE DROPDOWN
        [InspectorButton, InspectorShowIf("IfShowDropdown"), InspectorOrder(2)]
        public void HideDropDown()
        {
            IfShowDropdown = false;
            IfHideDropdown = false;
            _Button = (EButtonType)Enum.Parse(typeof(EButtonType), ButtonTypeString);
        }
        #endregion
        //===============================================================================================================================

        /// <summary>
        /// Do not edit! cached values for Editor.
        /// Stores the string value of EButton enum of this button.
        /// </summary>

        [SerializeField, HideInInspector]
        private string _ButtonTypeString = string.Empty;
        public string ButtonTypeString
        {
            get
            {
                return _ButtonTypeString;
            }
            set
            {
                Debug.LogWarningFormat(WARNING + " Button::ButtonType Only the ButtonEditor.cs is allowed to call this method!\n");
                _ButtonTypeString = value;
            }
        }
        /// <summary>
        /// The type of button this is.
        /// </summary>

        [InspectorDisabled,     SerializeField,    InspectorOrder(1)]
        protected EButtonType _Button;
        
        private void Start()
        {
            Assertion.Assert(_Button != EButtonType.Invalid);

        }

        private void OnEnable()
        {
            // Update Button Type Editor
            _Button = ButtonTypeString.ToEnum<EButtonType>();

            PresetValues();
        }

        /// <summary>
        /// This should be called when the button is clicked.
        /// This publishes ButtonClickedSignal.
        /// </summary>
        public void OnClickedButton()
        {
            this.Publish(new ButtonClickedSignal()
            {
                ButtonType = _Button
            });
		}
        
        /// <summary>
        /// This should be called when the button is hovered.
        /// This publishes ButtonHoveredSignal.
        /// </summary>
        public void OnHoveredButton()
        {
            this.Publish(new ButtonHoveredSignal()
            {
                ButtonType = _Button
            });
        }

        /// <summary>
        /// This should be called when the button is unhovered.
        /// This publishes ButtonUnhoveredSignal.
        /// </summary>
        public void OnUnhoveredButton()
        {
            this.Publish(new ButtonUnhoveredSignal()
            {
                ButtonType = _Button
            });
        }

        /// <summary>
        /// This should be called when the button is pressed.
        /// This publishes ButtonPressedSignal.
        /// </summary>
        public void OnPressedButton()
        {
            this.Publish(new ButtonPressedSignal()
            {
                ButtonType = _Button
            });
        }

        /// <summary>
        /// This should be called when the pointer is released on top of the button.
        /// This publishes ButtonReleasedSignal.
        /// </summary>
        public void OnReleasedButton()
        {
            this.Publish(new ButtonReleasedSignal()
            {
                ButtonType = _Button
            });
        }
    }

    //CLASS BEING USED TO POPULATE THE LIST OF USABLE BUTTONS, HAS A BUTTON WHICH SELECTS AND UPDATES THE CURRENT LIST BEING USED ON THE BUTTON INSTANCE
    [Serializable]
    public class TypeOfStringsClass
    {
        private Button buttonReference;
        public void SetButtonReference(Button temp)
        {
            buttonReference = temp;
        }
        [InspectorButton]
        void Select()
        {
            buttonReference.ButtonTypeString = name;
            buttonReference.UpdateEnum((EButtonType)(Enum.Parse(typeof(EButtonType), name)));
            buttonReference.HideDropDown();
        }
        [InspectorDisabled]
        public string name = "";
    }
}
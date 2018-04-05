using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using FullInspector;

using UniRx;
using UniRx.Triggers;

using Common;
using Common.Extensions;

namespace Sandbox.Shooting
{
    public class InputValues : BaseBehavior
    {
        public const string FORMAT = "{0:0.0000}";

        private InputField Field;

        private void Start()
        {
            Field = GetComponent<InputField>();

            /*
            Field.OnValueChangedAsObservable()
                .Subscribe(_ => UpdateDisplay(_.ToFloat()))
                .AddTo(this);
           //*/

            Field.OnEndEditAsObservable()
                .Subscribe(_ => UpdateDisplay(_.ToFloat()))
                .AddTo(this);
        }
        
        public void UpdateDisplay(float val)
        {
            string sVal = string.Format(FORMAT, val);
            Field.text = sVal;
        }

        public void UpdateDisplay(string val)
        {
            UpdateDisplay(val.ToFloat());
        }
    }
}
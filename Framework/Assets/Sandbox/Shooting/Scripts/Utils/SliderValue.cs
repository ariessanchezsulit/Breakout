using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using FullInspector;

using UniRx;
using UniRx.Triggers;

namespace Sandbox.Shooting
{
    public class SliderValue : BaseBehavior
    {
        public const string FORMAT = "{0} - {1:0.0000}";

        public string Display;
        public Slider Slider;

        private void Start()
        {
            Text text = GetComponent<Text>();
            text.text = string.Format(FORMAT, Display, Slider.value);

            Slider.OnValueChangedAsObservable()
                .Subscribe(_ => text.text = string.Format(FORMAT, Display, _))
                .AddTo(this);
        }

        [InspectorButton]
        public void UpdateDisplay()
        {
            Text text = GetComponent<Text>();
            text.text = string.Format(FORMAT, Display, Slider.value);
        }
    }
}
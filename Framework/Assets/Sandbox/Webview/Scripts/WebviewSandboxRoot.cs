using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using FullInspector;

using UniRx;

using MiniJSON;

using Common.Utils;

using Framework;

namespace Sandbox.Webview
{
    public class WebviewSandboxRoot : Scene
    {
        [InspectorCategory("General")]
        [SerializeField]
        private InputField Input;

        [InspectorCategory("General")]
        [SerializeField]
        private GUISkin Display;

        [InspectorCategory("General")]
        [SerializeField]
        private WebviewScaler WebScaler;
        
        public void ActivateWebView()
        {
            WebScaler.Position(Input.text);
        }

        public void DeactivateWebView()
        {
            WebScaler.Hide();
        }

        public void ShowArguments()
        {
            Message message = WebScaler.GetMessage();

            Debug.LogErrorFormat("Path:{0} Args:{1}\n", message.Path, Json.Serialize(message.Arguments));
        }
    }
}
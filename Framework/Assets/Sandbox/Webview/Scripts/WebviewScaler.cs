using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

using FullInspector;

namespace Sandbox.Webview
{
    public class WebviewScaler : BaseBehavior
    {
        public const string HTTP = "http://";
        public const string HTTPS = "https://";
        public const string BLANK = "about:blank";
        public const string DEFAULT = "http://keijiro.github.com/unity-webview-integration/index.html";

        [SerializeField]
        private Canvas Canvas;

        [SerializeField]
        private RectTransform WebviewRect;

        private void Start()
        {
            WebMediator.Install();
        }

        public void Position()
        {
            Position(DEFAULT);
        }

        int counter = 0;

        public void Position(string url)
        {
            //if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url))
            if (string.IsNullOrEmpty(url))
            {
                url = DEFAULT;
            }

            if (!url.StartsWith(HTTP))
            {
                url = string.Format("{0}{1}", HTTPS, url);
            }

            WebMediator.LoadUrl(url);

            Rect wr = WebviewRect.rect;
            Rect cr = Canvas.GetComponent<RectTransform>().rect;

            float w = wr.width / cr.width;
            float h = wr.height / cr.height;
            Vector2 boxScale = new Vector2(w * Screen.width, h * Screen.height);
            Vector2 boxPosition = Canvas.worldCamera.WorldToScreenPoint(WebviewRect.transform.position);
            boxPosition.x = boxPosition.x - (boxScale.x * 0.5f);
            boxPosition.y = Screen.height - (boxPosition.y + boxScale.y);
            Position(boxPosition, boxScale);
        }

        public void Position(Vector2 viewPos, Vector2 viewScale)
        {
            WebMediator.SetMargin((int)viewPos.x, (int)viewPos.y, (int)(Screen.width - (viewPos.x + viewScale.x)), (int)(Screen.height - (viewPos.y + viewScale.y)));
            WebMediator.MakeTransparentWebViewBackground();
            WebMediator.Show();
        }

        public void Hide()
        {
            // Clear the state of the web view (by loading a blank page).
            WebMediator.Hide();
            WebMediator.LoadUrl(BLANK);
        }

        public Message GetMessage()
        {
            var message = WebMediator.PollMessage();
            Message msg = new Message();
            msg.Path = message.path;
            msg.Arguments = message.args;

            return msg;
        }
    }

    public struct Message
    {
        public string Path;
        public Hashtable Arguments;
    }
}
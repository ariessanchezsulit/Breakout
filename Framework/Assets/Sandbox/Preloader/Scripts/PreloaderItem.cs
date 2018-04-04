using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using uPromise;

using UniRx;

using FullInspector;

using Common;
using Common.Extensions;
using Common.Fsm;
using Common.Query;
using Common.Signal;
using Common.Utils;

namespace Framework
{
    // alias
    using CColor = Common.Extensions.Color;

    public class PreloaderItem : BaseBehavior
    {
        /// <summary>
        /// Holder for subscriptions to be disposed when the service is terminated.
        /// </summary>
        private CompositeDisposable TerminationDisposables = new CompositeDisposable();

        [SerializeField]
        private Canvas _Canvas;
        public Canvas Canvas
        {
            get
            {
                return _Canvas;
            }
            private set
            {
                _Canvas = value;
            }
        }

        [SerializeField]
        private LoadingImages _Item;
        public LoadingImages Item
        {
            get
            {
                return _Item;
            }
        }

        public string Name
        {
            get
            {
                return gameObject.name;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            Canvas = gameObject.GetComponent<Canvas>();
        }

        private void Start()
        {
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using FullInspector;

using Common.Extensions;

namespace Framework
{
    public class SceneOption
    {
        public struct OnSelectOptionSignal
        {
            public string Scene { get; set; }
        }

        [InspectorDisabled]
        public string Scene;
        
        private MessageBroker _Broker = new MessageBroker();
        public MessageBroker Broker
        {
            get { return _Broker; }
            private set { _Broker = value; }
        }

        public SceneOption(string scene)
        {
            SetScene(scene);
        }

        public void SetScene(string scene)
        {
            Scene = scene;
        }

        [InspectorButton]
        public void Select()
        {
            Broker = Broker ?? new MessageBroker();
            Broker.Publish(new OnSelectOptionSignal() { Scene = Scene });
        }
    }

    public class SceneOptions
    {
        [SerializeField, InspectorDisabled, InspectorOrder(1)]
        private string _Scene;
        public string Scene
        {
            get { return _Scene; }
            set { _Scene = value; }
        }

        [SerializeField, HideInInspector]
        private EScene _SceneType;
        public EScene SceneType
        {
            get
            {
                if (_SceneType.Equals(EScene.Invalid))
                {
                    _SceneType = Scene.ToEnum<EScene>();
                }

                return _SceneType;
            }
            set { _SceneType = value; }
        }

        private bool ShowDrop;
        private bool HideDrop;

        [SerializeField]
        private bool Edit;

        [InspectorShowIf("ShowDrop"), InspectorOrder(3)]
        public List<SceneOption> Options;

        [SerializeField, HideInInspector, InspectorDisabled]
        private List<string> Scenes;

        private CompositeDisposable Disposables;

        public SceneOptions()
        {
            Disposables = new CompositeDisposable();
        }

        ~SceneOptions()
        {
            Disposables.Clear();
        }

        private void PresetValues()
        {
            Scene = string.Empty;
            SceneType = EScene.Invalid;
        }

        [InspectorName("TEST")]
        public void UpdateScene(string scene)
        {
            Scene = scene;
            SceneType = scene.ToEnum<EScene>();
            HideDropDown();
        }

        [InspectorButton(), InspectorHideIf("HideDrop"), InspectorOrder(2)]
        public void ShowDropDown()
        {
            RecreateOptions();

            ShowDrop = true;
            HideDrop = true;
        }

        [InspectorButton, InspectorShowIf("HideDrop"), InspectorOrder(2)]
        public void HideDropDown()
        {
            ShowDrop = false;
            HideDrop = false;
        }

        [InspectorButton, InspectorShowIf("Edit")]
        public void Refresh()
        {
            Options = null;
            Scenes = null;

            RecreateOptions();
        }

        private void RecreateOptions()
        {
            if (Scenes == null || Scenes.Count <= 0)
            {
                Scenes = Scenes ?? new List<string>(File.ReadAllLines("FrameworkFiles/FrameworkScenes.dat"));
            }

            if (Options == null || Options.Count <= 0)
            {
                Disposables.Clear();
                Options = Options ?? new List<SceneOption>();
                Scenes.ForEach(c =>
                {
                    SceneOption option = new SceneOption(c);
                    option.Broker.Receive<SceneOption.OnSelectOptionSignal>()
                        .Subscribe(_ => UpdateScene(_.Scene))
                        .AddTo(Disposables);

                    Options.Add(option);
                });
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using FullInspector;

using uPromise;

using UniRx;

using Common;
using Common.Extensions;
using Common.Query;
using Common.Signal;

namespace Framework
{
    // alias
    using CColor = Common.Extensions.Color;
    using UScene = UnityEngine.SceneManagement.Scene;

    /// <summary>
    /// This is the base MVP Presenter class to be extended by each scene root.
    /// </summary>
    public partial class Scene : BaseBehavior
    {
        [SerializeField]
        private List<ConcreteInstaller> _Installers;
        private static readonly string ERROR = CColor.red.LogHeader("[ERROR]");
        private static readonly string WARNING = CColor.yellow.LogHeader("[WARNING]");

        private static Dictionary<EScene, GameObject> CachedScenes = new Dictionary<EScene, GameObject>();

        //FULL INSPECTOR FEATURE
        //===============================================================================================================================
        #region FULL INSPECTOR FEATURE SCENE TYPE
        bool ShowDrop1;
        bool HideDrop1;

        //ENABLES PREVIEW OF DROPDOWN LIST OF SCENES
        [InspectorShowIf("ShowDrop1"), InspectorOrder(3)]
        public List<TypeOfSceneClass> sceneClassList;

        //BUTTONS INSIDE THE LIST OVERWRITES THE CURRENT ENUM ESCENE UPON CLICKING
        public void UpdateEnum(EScene type)
        {
            _SceneType = type;
        }

        //CHECKS IF STRING IS EMPTY IN WHICH CASE USES THE DEFAULT VALUE OF THE ENUM, IF STRING IS NOT EMPTY IT OVERWRITES THE ENUM VALUE
        private void PresetValues()
        {
            if (string.IsNullOrEmpty(SceneTypeString))
            {
                SceneTypeString = SceneType.ToString();
            }
            else
            {
                SceneType = (EScene)Enum.Parse(typeof(EScene), SceneTypeString);
            }
        }

        //AN INSPECTOR BUTTON WHICH REVEALS THE DROPDOWN
        [InspectorButton, InspectorHideIf("HideDrop1"), InspectorOrder(2)]
        void ShowSceneTypeDropdown()
        {
            sceneClassList = new List<TypeOfSceneClass>();

            for (int i = 0; i < EScene.GetValues(typeof(EScene)).Length; i++)
            {
                TypeOfSceneClass typertemp = new TypeOfSceneClass();
                typertemp.name = ((EScene)i).ToString();
                typertemp.SetSceneReference(this);
                sceneClassList.Add(typertemp);
            }

            ShowDrop1 = true;
            HideDrop1 = true;
            _SceneType = (EScene)Enum.Parse(typeof(EScene), SceneTypeString);
        }

        //AN INSPECTOR BUTTON WHICH HIDES THE DROPDOWN
        [InspectorButton, InspectorShowIf("HideDrop1"), InspectorOrder(2)]
        public void HideSceneTypeDropdown()
        {
            ShowDrop1 = false;
            HideDrop1 = false;
            _SceneType = (EScene)Enum.Parse(typeof(EScene), SceneTypeString);
        }
        #endregion
        
        /// <summary>
        /// Do not edit! cached values for Editor
        /// </summary>
        [SerializeField, HideInInspector]
        private string _SceneTypeString;// = string.Empty;
        public string SceneTypeString
        {
            get
            {
                return _SceneTypeString;
            }
            set
            {
                Debug.LogWarningFormat(WARNING + " Scene::SceneTypeString Only the SceneEditor.cs is allowed to call this method!\n");
                _SceneTypeString = value;
            }
        }
        
        /// <summary>
        /// The type/ID of the scene this root is for.
        /// This should match the scene's name.
        /// </summary>
        [SerializeField, InspectorDisabled, InspectorOrder(1)]
        private EScene _SceneType;
        public EScene SceneType
        {
            get
            {
                return _SceneType;
            }
            protected set
            {
                _SceneType = value;
            }
        }
        
        /// <summary>
        /// Data container passed upon loading this scene.
        /// Note: 
        ///     When there is no data passed, this value is set to null.
        ///     Access this only after Awake and OnEnable.
        /// </summary>
        private ISceneData _SceneData = null;
        public ISceneData SceneData
        {
            get
            {
                return _SceneData;
            }
            private set
            {
                _SceneData = value;
            }
        }

        /// <summary>
        /// Data container passed upon loading this scene.
        /// Note: 
        ///     When there is no data passed, this value is set to null.
        ///     Access this only after Awake and OnEnable.
        /// </summary>
        public T GetSceneData<T>() where T : ISceneData
        {
            return (T)SceneData;
        }

        /// <summary>
        /// Returns the name of the GameObject where the presenter is attached.
        /// </summary>
        public string Name
        {
            get
            {
                return gameObject.name;
            }
        }

        /// <summary>
        /// Persistent scenes indicates that they are exempted from UnloadScenes.
        /// Developers must manually unload the scene
        /// </summary>
        [SerializeField]
        private bool _IsPersistent = false;
        public bool IsPersistent
        {
            get
            {
                return _IsPersistent;
            }
            private set
            {
                _IsPersistent = value;
            }
        }
        
        /// <summary>
        /// Mapping of button types and click handlers.
        /// </summary>
        [SerializeField]
        protected Dictionary<EButtonType, Action<ButtonClickedSignal>> ButtonMap;

        /// <summary>
        /// Holder for subscriptions to be disposed when this Scene is disabled.
        /// </summary>
        protected CompositeDisposable OnDisableDisposables = new CompositeDisposable();

        #region Unity Life Cycle

        protected override void Awake()
        {
            base.Awake();

            PresetValues();
            
            // Initialize button map
            ButtonMap = new Dictionary<EButtonType, Action<ButtonClickedSignal>>();

            // Cache the Root scene object
            CachedScenes[SceneType] = this.gameObject;
        }

        protected virtual void Start()
        {
            // Update Scene Type & Depth from Editor
            //SceneType = SceneTypeString.ToEnum<EScene>();
            //SceneDepthType = SceneDepthString.ToEnum<ESceneDepth>();
        }

        protected virtual void OnEnable()
        {
            // Update Scene Type & Depth from Editor
            SceneType = SceneTypeString.ToEnum<EScene>();

            this.Receive<ButtonClickedSignal>()
                .Subscribe(sig => OnClickedButton(sig))
                .AddTo(OnDisableDisposables);
        }

        protected virtual void OnDisable()
        {
            // dispose all subscriptions and clear list
            OnDisableDisposables.Clear();
        }

        protected virtual void OnDestroy()
        {
            if (ButtonMap != null)
            {
                ButtonMap.Clear();
                ButtonMap = null;
            }

            CachedScenes[SceneType] = null;
            CachedScenes.Remove(SceneType);
        }

        protected virtual void Install()
        {
            _Installers.ForEach(i => i.Install());
        }

        #endregion

        /// <summary>
        /// Sets the scene's handler for the given button type.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="action"></param>
        protected void AddButtonHandler(EButtonType button, Action<ButtonClickedSignal> action)
        {
            ButtonMap[button] = action;
        }

        /// <summary>
        /// Returns true if this scene has data.
        /// </summary>
        /// <returns></returns>
        protected bool HasSceneData()
        {
            return SceneData != null;
        }

        

        #region Signals

        private void OnClickedButton(ButtonClickedSignal signal)
        {
            EButtonType button = signal.ButtonType;

            if (ButtonMap.ContainsKey(button) && gameObject.activeSelf)
            {
                Debug.LogFormat("Scene::OnClickedButton Button:{0}\n", button);
                ButtonMap[button](signal);
            }
        }

        #endregion

        #region Helpers

        public static bool ShowScene<T>(EScene scene) where T : Scene
        {
            if (CachedScenes.ContainsKey(scene))
            {
                CachedScenes[scene].gameObject.SetActive(true);
                return true;
            }

            return false;
        }

        public static bool HasScene<T>(EScene scene) where T : Scene
        {
            return CachedScenes.ContainsKey(scene);
        }

        public static T GetScene<T>(EScene scene) where T : Scene
        {
            if (!HasScene<T>(scene))
            {
                return default(T);
            }

            return CachedScenes[scene].GetComponent<T>();
        }

        public static void UnloadScene(EScene scene)
        {
            Debug.LogFormat("[Framework] Scene::UnloadScene Scene:{0} Cached:{1} Loaded:{2}\n", scene, CachedScenes.ContainsKey(scene), SceneManager.GetSceneByName(scene.ToString()));

            // unload if cached
            if (CachedScenes.ContainsKey(scene))
            {
                GameObject.Destroy(CachedScenes[scene].gameObject);
                CachedScenes.Remove(scene);
            }

            // unload if loaded
            if (SceneManager.GetSceneByName(scene.ToString()) != null)
            {
                SceneManager.UnloadScene(scene.ToString());
            }
        }

        /// <summary>
        /// Unloads every scene except for the scenes that marked as Persistent.
        /// </summary> 
        public static void UnloadScenes()
        {
            int sceneCount = SceneManager.sceneCount;
            UScene[] scenes = new UScene[sceneCount];

            for (int i = 0; i < sceneCount; i++)
            {
                scenes[i] = SceneManager.GetSceneAt(i);
            }

            foreach (UScene scene in scenes)
            {
                EScene loadScene = EScene.Invalid;

                try
                {
                    loadScene = scene.name.ToEnum<EScene>();
                }
                // Catch the loaded non Synergy Scenes
                catch (ArgumentException)
                {
                    loadScene = EScene.Invalid;
                    AsyncOperation async = SceneManager.UnloadSceneAsync(scene);
                }
                finally
                {
                    if (loadScene != EScene.Invalid && !GetScene<Scene>(loadScene).IsPersistent)
                    {
                        UnloadScene(loadScene);
                    }
                }
            }

            scenes = null;
        }

        public static bool IsLoaded(EScene scene)
        {
            //return CachedScenes.ContainsKey(scene);
            return SceneManager.GetSceneByName(scene.ToString()) != null;
        }

        #endregion

    }
}
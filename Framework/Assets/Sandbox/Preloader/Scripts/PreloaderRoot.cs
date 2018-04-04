using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using uPromise;

using UniRx;
using UniRx.Triggers;

using Common;
using Common.Extensions;
using Common.Query;
using Common.Signal;
using Common.Utils;

// alias
using UColor = UnityEngine.Color;
using URandom = UnityEngine.Random;
using UScene = UnityEngine.SceneManagement.Scene;
using CColor = Common.Extensions.Color;
using CSScene = Framework.Scene;

namespace Framework
{
    /// <summary>
    /// Enum of loading screen IDs.
    /// </summary>
    [Flags]
    public enum LoadingImages
    {
        Invalid = 0x0,

        Loading001 = 0x1 << 0,
        Loading002 = 0x1 << 1,
        Loading003 = 0x1 << 2,
        Loading004 = 0x1 << 3,

        Max = 0x1 << 4,
    };

    /// <summary>
    /// This is an interface to be implemented by preloaders that can fade in and fade out.
    /// </summary>
    public interface IPreloader
    {
        /// <summary>
        /// Loads/initializes the preloader.
        /// </summary>
        /// <returns>Promise</returns>
        Promise LoadPromise();
        /// <summary>
        /// Fades in the preloader.
        /// </summary>
        /// <returns></returns>
        Promise FadeInPromise();
        /// <summary>
        /// Fades out the preloader.
        /// </summary>
        /// <returns></returns>
        Promise FadeOutPromise();
    };

    /// <summary>
    /// Scene loading transision helper class.
    /// Touch input blocker.
    /// To use, sequence the following promises:
    /// 1) LoadLoadingScreen()
    /// 2) FadeInLoadingScreen()
    /// 3) ... load additive scenes to be loaded
    /// 4) FadeOutLoadingScreen().
    /// </summary>
    public class PreloaderRoot : Scene
    {
        public const float IN_DURATION = 0.5f;
        public const float OUT_DURATION = 0.5f;
        public const float FIXED_DELTA = 0.01656668f;

        public static readonly Vector2 TARGET_RESOLUTION = new Vector2(1536.0f, 2048.0f);

        /// <summary>
        /// True while the loading screen is being loaded or active.
        /// </summary>
        [SerializeField]
        protected bool _IsLoading;
        public bool IsLoading
        {
            get
            {
                return _IsLoading;
            }
            protected set
            {
                _IsLoading = value;
            }
        }

        /// <summary>
        /// This swallows all the touch/mouse input when enabled
        /// </summary>
        [SerializeField]
        protected Image _Blocker;
        public Image Blocker
        {
            get { return _Blocker; }
        }

        /// <summary>
        /// This parents loaded loading screens.
        /// </summary>
        [SerializeField]
        protected Transform _Container;
        public Transform Container
        {
            get
            {
                return _Container;
            }
        }

        /// <summary>
        /// The currently loaded loading screen.
        /// </summary>
        [SerializeField]
        protected List<CanvasGroup> CanvasGroups;

        private CompositeDisposable Disposables = new CompositeDisposable();
        private FloatReactiveProperty Progress = new FloatReactiveProperty(0f);

        #region Unity Life Cycle

        protected override void Awake()
        {
            base.Awake();

            // force set scene type and depth
            SceneType = EScene.Preloader;

            Assertion.Assert(Blocker, string.Format(CColor.red.LogHeader("[ERROR]") + " PreloaderRoot::Awake Blocker:{0} is null!\n", Blocker));
            Assertion.Assert(Container, string.Format(CColor.red.LogHeader("[ERROR]") + " PreloaderRoot::Awake Container:{0} is null!\n", Container));

            QuerySystem.RegisterResolver(QueryIds.LoadingProgress, delegate (IQueryRequest req, IMutableQueryResult result)
            {
                result.Set(Progress);
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            QuerySystem.RemoveResolver(QueryIds.LoadingProgress);
        }
        #endregion

        /// <summary>
        /// Sets the image's alpha.
        /// </summary>
        protected UColor SetImageAlpha
        {
            set
            {
                CanvasGroups.ForEach(g => g.alpha = value.a);
            }
        }

        #region Preloader Promise

        /// <summary>
        /// Loads the preloader by enabling the blocker and loading the loading screen to be displayed.
        /// </summary>
        /// <returns></returns>
        public Promise LoadLoadingScreenPromise()
        {
            LoadingImages images = LoadingImages.Loading001 | LoadingImages.Loading002 | LoadingImages.Loading003 | LoadingImages.Loading004;
            return LoadLoadingScreenPromise(images);
        }

        public Promise LoadLoadingScreenPromise(LoadingImages images)
        {
            if (IsLoading)
            {
                return EndFramePromise();
            }

            var matches = Enum.GetValues(typeof(LoadingImages))
                            .Cast<LoadingImages>()
                            .Where(i => images.Has(i))
                            .ToList();

            matches.Remove(LoadingImages.Invalid);
            matches.Remove(LoadingImages.Max);

            var match = matches.Random();

            //Debug.LogErrorFormat("PreloaderRoot::LoadLoadingScreenPromise Match:{0}\n", match);

            IsLoading = true;
            Blocker.gameObject.SetActive(true);

            // load scene with loading screen
            AsyncOperation operation = SceneManager.LoadSceneAsync(match.ToString(), LoadSceneMode.Additive);
            /*
            operation.AsObservable()
                .Subscribe(_ =>
                {
                    Debug.LogErrorFormat("PreloaderRoot::LoadLoadingScreen A Progress:{0}\n", _.progress);
                })
                .AddTo(Disposables);

            operation.ObserveEveryValueChanged(_ => _.progress)
                .Subscribe(_ =>
                {
                    Debug.LogErrorFormat("PreloaderRoot::LoadLoadingScreen B Progress:{0}\n", _);
                })
                .AddTo(Disposables);
            //*/

            Deferred deferred = new Deferred();
            StartCoroutine(LoadLoadingScreen(deferred, match, operation));
            return deferred.Promise;
        }

        /// <summary>
        /// Fades in the loading screen.
        /// </summary>
        /// <returns></returns>
        public Promise FadeInLoadingScreenPromise()
        {
            Deferred deferred = new Deferred();
            StartCoroutine(FadeInLoadingScreen(deferred));
            return deferred.Promise;
        }

        /// <summary>
        /// Fades out the loading screen.
        /// </summary>
        /// <returns></returns>
        public Promise FadeOutLoadingScreenPromise()
        {
            Deferred deferred = new Deferred();
            StartCoroutine(FadeOutLoadingScreen(deferred));
            return deferred.Promise;
        }


        /// <summary>
        /// Fades out the loading screen.
        /// </summary>
        /// <returns></returns>
        public Promise RemoveLoadingScreenPromise()
        {
            Deferred deferred = new Deferred();
            StartCoroutine(RemoveLoadingScreen(deferred));
            return deferred.Promise;
        }


        #endregion

        #region Coroutines

        /// <summary>
        /// Loads a random loading screen.
        /// </summary>
        /// <param name="deffered"></param>
        /// <returns></returns>
        protected virtual IEnumerator LoadLoadingScreen(Deferred deffered, LoadingImages image, AsyncOperation operation)
        {
            yield return null;

            // +AS:180404 TODO
            //  - Fix canvas sorting with SystemCanvas (Add SystemCanvas Support)
            //  - Disabled temporarily
            /*
            string imageScene = image.ToString();

            // load scene with loading screen
            yield return operation;

            Disposables.Clear();
            Progress.Value = operation.progress;

            // get objects in the scene
            UScene loadedScene = SceneManager.GetSceneByName(imageScene);
            List<GameObject> objects = new List<GameObject>(loadedScene.GetRootGameObjects());
            List<Canvas> items = objects.ToArray<Canvas>();

            // make sure the scenes only has 1 root object
            Assertion.Assert(items.Count >= 1);

            // get the first and only object (which should be the loading screen image)
            Canvas obj = items.FirstOrDefault();
            Assertion.AssertNotNull(obj);

            // fix object parenting setup
            Transform root = Container;
            obj.transform.SetParent(root);
            obj.transform.SetAsFirstSibling();
            obj.gameObject.SetActive(true);

            // set loaded image
            CanvasGroup group = obj.GetComponent<CanvasGroup>();
            group.gameObject.SetActive(true);
            CanvasGroups.Add(group);

            CanvasList.Add(obj.GetComponent<Canvas>());
            
            SetupSceneCanvas();

            //SceneManager.UnloadScene(imageScene);
            yield return SceneManager.UnloadSceneAsync(imageScene);

            deffered.Resolve();
            //*/
        }

        /// <summary>
        /// Fades in the loading screen.
        /// </summary>
        /// <param name="deferred"></param>
        /// <returns></returns>
        protected virtual IEnumerator FadeInLoadingScreen(Deferred deferred)
        {
            yield return null;

            float timer = 0.0f;
            UColor color = UColor.white;
            Deferred def = deferred;

            while (timer <= IN_DURATION)
            {
                float scale = Mathf.Clamp((timer += FIXED_DELTA), 0.0f, IN_DURATION) / IN_DURATION;
                color.a = scale;
                SetImageAlpha = color;
                yield return null;
            }

            color.a = 1.0f;
            SetImageAlpha = color;
            yield return null;

            def.Resolve();
        }

        /// <summary>
        /// Fades out the loading screen and disables the blocker.
        /// </summary>
        /// <param name="deferred"></param>
        /// <returns></returns>
        protected virtual IEnumerator FadeOutLoadingScreen(Deferred deferred)
        {
            yield return null;

            // +AS:180404 TODO
            //  - Fix canvas sorting with SystemCanvas (Add SystemCanvas Support)
            //  - Disabled temporarily
            /*
            float timer = 0.0f;
            UColor color = UColor.white;
            Deferred def = deferred;

            while (timer <= OUT_DURATION)
            {
                float scale = 1.0f - Mathf.Clamp((timer += FIXED_DELTA), 0.0f, OUT_DURATION) / OUT_DURATION;
                color.a = scale;
                SetImageAlpha = color;
                yield return null;
            }

            color.a = 0.0f;
            SetImageAlpha = color;
            yield return null;

            IsLoading = false;
            Blocker.gameObject.SetActive(false);
            CanvasGroups.ForEach(g => CanvasList.Remove(g.GetComponent<Canvas>()));
            CanvasGroups.ForEach(g => Destroy(g.gameObject));
            CanvasGroups.Clear();

            def.Resolve();
            //*/
        }
        
        /// <summary>
        /// Fades out the loading screen and disables the blocker.
        /// </summary>
        /// <param name="deferred"></param>
        /// <returns></returns>
        protected virtual IEnumerator RemoveLoadingScreen(Deferred deferred)
        {
            yield return null;

            // +AS:180404 TODO
            //  - Fix canvas sorting with SystemCanvas (Add SystemCanvas Support)
            //  - Disabled temporarily
            /*
            Deferred def = deferred;
            
            yield return null;

            IsLoading = false;
            Blocker.gameObject.SetActive(false);
            CanvasGroups.ForEach(g => CanvasList.Remove(g.GetComponent<Canvas>()));
            CanvasGroups.ForEach(g => Destroy(g.gameObject));
            CanvasGroups.Clear();

            def.Resolve();
            //*/
        }
        #endregion
    }

}
 
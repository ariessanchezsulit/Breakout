using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uPromise;

using UniRx;
using UniRx.Triggers;

using FullInspector;

using Common;
using Common.Extensions;
using Common.Query;
using Common.Signal;

// alias
using CColor = Common.Extensions.Color;
using UScene = UnityEngine.SceneManagement.Scene;

namespace Framework
{
    public partial class SceneObject : BaseBehavior
    {
        #region Getter
        public static string GetSceneName()
        {
            return "SceneObject";
        }

        public virtual string GetName()
        {
            return gameObject.name;
        }
        #endregion

        #region Frames
        public Promise EndFrame()
        {
            Deferred deferred = new Deferred();
            StartCoroutine(EndFrame(deferred));
            return deferred.Promise;
        }

        public Promise Wait()
        {
            return Wait(1f);
        }

        public Promise Wait(float seconds)
        {
            Deferred deferred = new Deferred();
            StartCoroutine(Wait(deferred, seconds));
            return deferred.Promise;
        }

        protected IEnumerator EndFrame(Deferred deferred)
        {
            yield return null;
            deferred.Resolve();
        }

        protected IEnumerator Wait(Deferred deferred, float seconds = 1.0f)
        {
            yield return null;
            yield return new WaitForSeconds(seconds);
            deferred.Resolve();
        }

        protected virtual IEnumerator Wait(Deferred deferred)
        {
            yield return null;
            deferred.Resolve();
        }
        #endregion

        #region Scenes
        public Promise LoadScene<T>(string scene) where T : SceneObject
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadScene<T>(deferred, scene));
            return deferred.Promise;
        }

        public Promise LoadScene<T>(string scene, ISceneData data) where T : SceneObject
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadScene<T>(deferred, scene, data));
            return deferred.Promise;
        }

        public Promise LoadSceneAdditive<T>(string scene) where T : SceneObject
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadSceneAdditive<T>(deferred, scene));
            return deferred.Promise;
        }

        public Promise LoadSceneAdditive<T>(string scene, ISceneData data) where T : SceneObject
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadSceneAdditive<T>(deferred, scene, data));
            return deferred.Promise;
        }

        public Promise UnloadScene<T>(string scene) where T : SceneObject
        {
            Deferred deferred = new Deferred();
            StartCoroutine(UnloadScene<T>(deferred, scene));
            return deferred.Promise;
        }

        public IEnumerator LoadScene<T>(Deferred deferred, string scene) where T : SceneObject
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
            yield return operation;
            deferred.Resolve();
        }

        public IEnumerator LoadScene<T>(Deferred deferred, string scene, ISceneData data) where T : SceneObject
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
            yield return operation;

            PassDataToScene<T>(scene, data);

            deferred.Resolve();
        }

        public IEnumerator LoadSceneAdditive<T>(Deferred deferred, string scene) where T : SceneObject
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            yield return operation;
            deferred.Resolve();
        }

        public IEnumerator LoadSceneAdditive<T>(Deferred deferred, string scene, ISceneData data) where T : SceneObject
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            yield return operation;

            PassDataToScene<T>(scene, data);

            deferred.Resolve();
        }

        public IEnumerator UnloadScene<T>(Deferred deferred, string scene) where T : SceneObject
        {
            yield return SceneManager.UnloadSceneAsync(scene);
            deferred.Resolve();
        }

        /// <summary>
        /// Data container passed upon loading this scene.
        /// Note: 
        ///     When there is no data passed, this value is set to null.
        ///     Access this only after Awake and OnEnable.
        /// </summary>
        public ISceneData SceneData { get; protected set; }

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

        public void PassDataToScene<T>(string scene, ISceneData data) where T : SceneObject
        {
            UScene loadedScene = SceneManager.GetSceneByName(scene);
            GameObject[] objects = loadedScene.GetRootGameObjects();
            List<SceneObject> scenes = new List<GameObject>(objects).ToArray<SceneObject>();

            // pass the data 
            scenes.ForEach(s => s.SceneData = data);
        }

        public T GetScene<T>(string scene) where T : SceneObject
        {
            UScene loadedScene = SceneManager.GetSceneByName(scene);
            GameObject[] objects = loadedScene.GetRootGameObjects();
            List<T> scenes = new List<GameObject>(objects).ToArray<T>();

            // pass the data 
            return scenes.FirstOrDefault();
        }
        #endregion
    }
}


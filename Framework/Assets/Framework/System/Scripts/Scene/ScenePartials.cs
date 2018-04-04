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

    #region Scene extension (Load, Unload, and Wait)

    public partial class Scene : BaseBehavior
    {
        public void PassDataToScene<T>(string scene, ISceneData data) where T : Scene
        {
            UScene loadedScene = SceneManager.GetSceneByName(scene);
            List<GameObject> objects = new List<GameObject>(loadedScene.GetRootGameObjects());
            List<GameObject> scenes = objects.FindAll(g => g.GetComponent<T>() != null);
            List<T> items = scenes.ToArray<T>();

            Assertion.Assert(items.Count > 0, "Error! Scene:" + gameObject.name);
            
            // pass the data 
            items.FirstOrDefault().SceneData = data;
        }
        
        /// <summary>
        /// Loads the given scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene">The type/ID of the scene to be loaded</param>
        /// <returns></returns>
        public Promise LoadScenePromise<T>(string sceneName) where T : Scene
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadSceneAsync<T>(deferred, sceneName));
            return deferred.Promise;
        }

        public IEnumerator LoadSceneAsync<T>(Deferred deffered, string scene) where T : Scene
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
            yield return operation;
            deffered.Resolve();
        }

        /// <summary>
        /// Loads the given scene with data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="sScenee">The name of the scene to be loaded</param>
        /// <param name="data">Data to be passed to the scene</param>
        /// <returns></returns>
        public Promise LoadScenePromise<T>(string sScenee, ISceneData data) where T : Scene
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadSceneAsync<T>(deferred, sScenee, data));
            return deferred.Promise;
        }

        public IEnumerator LoadSceneAsync<T>(Deferred deffered, string scene, ISceneData data) where T : Scene
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
            yield return operation;

            PassDataToScene<T>(scene, data);
            deffered.Resolve();
        }

        /// <summary>
        /// Loads the given scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene">The type/ID of the scene to be loaded</param>
        /// <returns></returns>
        public Promise LoadScenePromise<T>(EScene eScene) where T : Scene
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadSceneAsync<T>(deferred, eScene));
            return deferred.Promise;
        }

        /// <summary>
        /// Unloads everything except the SystemRoot then loads the target scene.
        /// </summary>
        public IEnumerator LoadSceneAsync<T>(Deferred deffered, EScene scene) where T : Scene
        {
            Assertion.Assert(scene != EScene.System);

            // Unload all other scenes except flagged as persistent
            UnloadScenes();

            //AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Single);
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
            yield return operation;
            deffered.Resolve();

            this.Publish(new OnLoadSceneSignal() { Scene = scene });
        }

        /// <summary>
        /// Loads the given scene with data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene">The type/ID of the scene to be loaded</param>
        /// <param name="data">Data to be passed to the scene</param>
        /// <returns></returns>
        public Promise LoadScenePromise<T>(EScene eScene, ISceneData data) where T : Scene
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadSceneAsync<T>(deferred, eScene, data));
            return deferred.Promise;
        }

        public IEnumerator LoadSceneAsync<T>(Deferred deffered, EScene scene, ISceneData data) where T : Scene
        {
            Assertion.Assert(scene != EScene.System);

            // Unload all other scenes except flagged as persistent
            UnloadScenes();

            //AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Single);
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
            yield return operation;

            PassDataToScene<T>(scene.ToString(), data);
            deffered.Resolve();

            this.Publish(new OnLoadSceneSignal() { Scene = scene });
        }

        /// <summary>
        /// Loads the given scene additively.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="sScenee"></param>
        /// <returns></returns>
        public Promise LoadSceneAdditivePromise(string sScenee)
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadAdditiveSceneAsync(deferred, sScenee));
            return deferred.Promise;
        }

        public IEnumerator LoadAdditiveSceneAsync(Deferred deffered, string scene)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            yield return operation;
            deffered.Resolve();
        }

        /// <summary>
        /// Loads the given scene additively.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene"></param>
        /// <returns></returns>
        public Promise LoadSceneAdditivePromise<T>(EScene eScene) where T : Scene
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadAdditiveSceneAsync<T>(deferred, eScene));
            return deferred.Promise;
        }

        public IEnumerator LoadAdditiveSceneAsync<T>(Deferred deffered, EScene scene) where T : Scene
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
            yield return operation;
            deffered.Resolve();

            this.Publish(new OnLoadSceneSignal() { Scene = scene });
        }

        /// <summary>
        /// Loads the given scene additively.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene"></param>
        /// <returns></returns>
        public Promise LoadSceneAdditivePromise<T>(string sceneName) where T : Scene
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadAdditiveSceneAsync<T>(deferred, sceneName));
            return deferred.Promise;
        }

        public IEnumerator LoadAdditiveSceneAsync<T>(Deferred deffered, string sceneName) where T : Scene
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            yield return operation;
            deffered.Resolve();

            this.Publish(new OnLoadSceneSignal() { SceneName = sceneName });
        }

        /// <summary>
        /// Loads the given scene additively with data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Promise LoadSceneAdditivePromise<T>(EScene eScene, ISceneData data) where T : Scene
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadAdditiveSceneAsync<T>(deferred, eScene, data));
            return deferred.Promise;
        }

        public IEnumerator LoadAdditiveSceneAsync<T>(Deferred deffered, EScene scene, ISceneData data) where T : Scene
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
            yield return operation;

            PassDataToScene<T>(scene.ToString(), data);
            deffered.Resolve();

            this.Publish(new OnLoadSceneSignal() { Scene = scene });
        }

        /// <summary>
        /// Loads the given scene additively with data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Promise LoadSceneAdditivePromise<T>(string sceneName, ISceneData data) where T : Scene
        {
            Deferred deferred = new Deferred();
            StartCoroutine(LoadAdditiveSceneAsync<T>(deferred, sceneName, data));
            return deferred.Promise;
        }

        public IEnumerator LoadAdditiveSceneAsync<T>(Deferred deffered, string sceneName, ISceneData data) where T : Scene
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            yield return operation;

            PassDataToScene<T>(sceneName, data);
            deffered.Resolve();

            this.Publish(new OnLoadSceneSignal() { SceneName = sceneName });
        }

        public Promise EndFramePromise()
        {
            Deferred deferred = new Deferred();
            this.StartCoroutine(this.EndFrame(deferred));
            return deferred.Promise;
        }

        protected IEnumerator EndFrame(Deferred deferred)
        {
            yield return null;
            deferred.Resolve();
        }

        public Promise WaitPromise(float seconds = 1.0f)
        {
            Deferred deferred = new Deferred();
            this.StartCoroutine(this.Wait(deferred, seconds));
            return deferred.Promise;
        }
        
        protected IEnumerator Wait(Deferred deferred, float seconds = 1.0f)
        {
            yield return null;
            yield return new WaitForSeconds(seconds);
            deferred.Resolve();
        }
    }
    #endregion
}
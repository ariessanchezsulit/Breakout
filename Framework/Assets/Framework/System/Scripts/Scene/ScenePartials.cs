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

    #region Partial for Scene Loading
    
    public partial class Scene : BaseBehavior
    {
        private void PassDataToScene<T>(string scene, ISceneData data) where T : Scene
        {
            UScene loadedScene = SceneManager.GetSceneByName(scene);
            List<GameObject> objects = new List<GameObject>(loadedScene.GetRootGameObjects());
            List<GameObject> scenes = objects.FindAll(g => g.GetComponent<T>() != null);
            List<T> items = scenes.ToArray<T>();

            Assertion.Assert(items.Count > 0, "Error! Scene:" + gameObject.name);
            
            // pass the data 
            items.FirstOrDefault().SceneData = data;
        }

        public IEnumerator LoadSceneAsync<T>(Deferred deffered, string scene) where T : Scene
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
            yield return operation;
            deffered.Resolve();
        }

        public IEnumerator LoadSceneAsync<T>(Deferred deffered, string scene, ISceneData data) where T : Scene
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
            yield return operation;

            PassDataToScene<T>(scene, data);
            deffered.Resolve();
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

            this.Publish(new OnLoadSceneSignal() { SceneName = scene });
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

            this.Publish(new OnLoadSceneSignal() { SceneName = scene });
        }

        public IEnumerator LoadAdditiveSceneAsync(Deferred deffered, string scene)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            yield return operation;
            deffered.Resolve();
        }

        public IEnumerator LoadAdditiveSceneAsync<T>(Deferred deffered, EScene scene) where T : Scene
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
            yield return operation;
            deffered.Resolve();

            this.Publish(new OnLoadSceneSignal() { SceneName = scene });
        }

        public IEnumerator LoadAdditiveSceneAsync<T>(Deferred deffered, EScene scene, ISceneData data) where T : Scene
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
            yield return operation;

            PassDataToScene<T>(scene.ToString(), data);
            deffered.Resolve();

            this.Publish(new OnLoadSceneSignal() { SceneName = scene });
        }

        public Promise EndFramePromise()
        {
            Deferred deferred = new Deferred();
            this.StartCoroutine(this.EndFrame(deferred));
            return deferred.Promise;
        }

        public Promise WaitPromise(float seconds = 1.0f)
        {
            Deferred deferred = new Deferred();
            this.StartCoroutine(this.Wait(deferred, seconds));
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
    }
    #endregion
}
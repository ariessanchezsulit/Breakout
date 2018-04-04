using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using FullInspector;

using uPromise;

using Common;
using Common.Query;
using Common.Signal;

namespace Framework
{
    using UniRx;
    using UniRx.Triggers;

    public static class SceneExtensions
    {
        /// <summary>
        /// Loads the given scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene">The type/ID of the scene to be loaded</param>
        /// <returns></returns>
        public static Promise LoadScenePromise<T>(this Scene scene, EScene eScene) where T : Scene
        {
            Deferred deferred = new Deferred();
            scene.StartCoroutine(scene.LoadSceneAsync<T>(deferred, eScene));
            return deferred.Promise;
        }

        /// <summary>
        /// Loads the given scene with data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene">The type/ID of the scene to be loaded</param>
        /// <param name="data">Data to be passed to the scene</param>
        /// <returns></returns>
        public static Promise LoadScenePromise<T>(this Scene scene, EScene eScene, ISceneData data) where T : Scene
        {
            Deferred deferred = new Deferred();
            scene.StartCoroutine(scene.LoadSceneAsync<T>(deferred, eScene, data));
            return deferred.Promise;
        }

        /// <summary>
        /// Loads the given scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="sScenee">The name of the scene to be loaded</param>
        /// <returns></returns>
        public static Promise LoadScenePromise<T>(this Scene scene, string sScenee) where T : Scene
        {
            Deferred deferred = new Deferred();
            scene.StartCoroutine(scene.LoadSceneAsync<T>(deferred, sScenee));
            return deferred.Promise;
        }

        /// <summary>
        /// Loads the given scene with data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="sScenee">The name of the scene to be loaded</param>
        /// <param name="data">Data to be passed to the scene</param>
        /// <returns></returns>
        public static Promise LoadScenePromise<T>(this Scene scene, string sScenee, ISceneData data) where T : Scene
        {
            Deferred deferred = new Deferred();
            scene.StartCoroutine(scene.LoadSceneAsync<T>(deferred, sScenee, data));
            return deferred.Promise;
        }

        /// <summary>
        /// Loads the given scene additively.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene"></param>
        /// <returns></returns>
        public static Promise LoadSceneAdditivePromise<T>(this Scene scene, EScene eScene) where T : Scene
        {
            Deferred deferred = new Deferred();
            scene.StartCoroutine(scene.LoadAdditiveSceneAsync<T>(deferred, eScene));
            return deferred.Promise;
        }

        /// <summary>
        /// Loads the given scene additively with data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Promise LoadSceneAdditivePromise<T>(this Scene scene, EScene eScene, ISceneData data) where T : Scene
        {
            Deferred deferred = new Deferred();
            scene.StartCoroutine(scene.LoadAdditiveSceneAsync<T>(deferred, eScene, data));
            return deferred.Promise;
        }

        /// <summary>
        /// Loads the given scene additively.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="sScenee"></param>
        /// <returns></returns>
        public static Promise LoadSceneAdditivePromise(this Scene scene, string sScenee)
        {
            Deferred deferred = new Deferred();
            scene.StartCoroutine(scene.LoadAdditiveSceneAsync(deferred, sScenee));
            return deferred.Promise;
        }
        
        public static Promise EndFramePromise(this MonoBehaviour scene)
        {
            Deferred deferred = new Deferred();
            scene.StartCoroutine(scene.EndFrame(deferred));
            return deferred.Promise;
        }

        public static Promise WaitPromise(this MonoBehaviour scene, float seconds = 1.0f)
        {
            Deferred deferred = new Deferred();
            scene.StartCoroutine(scene.Wait(deferred, seconds));
            return deferred.Promise;
        }

        public static IEnumerator EndFrame(this MonoBehaviour scene, Deferred deferred)
        {
            yield return null;
            deferred.Resolve();
        }

        public static IEnumerator Wait(this MonoBehaviour scene, Deferred deferred, float seconds = 1.0f)
        {
            yield return null;
            yield return new WaitForSeconds(seconds);
            deferred.Resolve();
        }

        public static void Publish<T>(this Scene scene, T message)
        {
            MessageBroker.Default.Publish<T>(message);
        }

        public static IObservable<T> Receive<T>(this Scene scene)
        {
            return MessageBroker.Default.Receive<T>();
        }

        public static void Publish<T>(this MonoBehaviour scene, T message)
        {
            MessageBroker.Default.Publish<T>(message);
        }

        public static IObservable<T> Receive<T>(this MonoBehaviour scene)
        {
            return MessageBroker.Default.Receive<T>();
        }

        public static void Publish<T>(this BaseBehavior scene, T message)
        {
            MessageBroker.Default.Publish<T>(message);
        }

        public static IObservable<T> Receive<T>(this BaseBehavior scene)
        {
            return MessageBroker.Default.Receive<T>();
        }
    }
}


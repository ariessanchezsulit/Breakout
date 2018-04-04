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


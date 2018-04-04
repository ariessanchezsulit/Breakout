using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using uPromise;

using UniRx;

using Common;
using Common.Extensions;
using Common.Fsm;
using Common.Query;
using Common.Signal;

// alias
using CColor = Common.Extensions.Color;

namespace Framework
{
    using Sandbox.Shooting;

    public class SystemRoot : Scene
    {
        [SerializeField]
        private Camera _SystemCamera;
        public Camera SystemCamera
        {
            get
            {
                return _SystemCamera;
            }
            private set
            {
                _SystemCamera = value;
            }
        }

        [SerializeField]
        private SystemVersion _SystemVersion;
        public SystemVersion SystemVersion
        {
            get
            {
                return _SystemVersion;
            }
            private set
            {
                _SystemVersion = value;
            }
        }
        
        #region Unity Life Cycle

        protected override void Awake()
        {
            // Setup DI Queries
            QuerySystem.RegisterResolver(QueryIds.SystemCamera, delegate (IQueryRequest request, IMutableQueryResult result)
            {
                result.Set(SystemCamera);
            });
            
            // Assert cache objects
            Assertion.AssertNotNull(SystemCamera);
            Assertion.AssertNotNull(SystemVersion);

            // Call parent's awake
            base.Awake();
            
            Install();

            LoadSceneAdditivePromise("Shooting");
        }

        protected override void OnDestroy()
        {
            QuerySystem.RemoveResolver(QueryIds.SystemCamera);
        }

        #endregion
    }
}
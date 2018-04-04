using UnityEngine;
using UnityEngine.SceneManagement;

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

namespace Framework
{
    public class ConcreteInstaller : BaseBehavior, IInstaller
    {
        protected Scene Scene;

        public virtual void Add()
        {

        }

        public virtual void Install()
        {
            Scene = Scene.GetScene<SystemRoot>(EScene.System);
        }

        public virtual void UnInstall()
        {

        }
    }
}
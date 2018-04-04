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
    public interface IInstaller
    {
        void Add();
        void Install();
        void UnInstall();
    }
}
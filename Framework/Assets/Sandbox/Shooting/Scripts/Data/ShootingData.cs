using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

using FullInspector;

using uPromise;

using Common;
using Common.Extensions;
using Common.Query;
using Common.Utils;

using Framework;

namespace Sandbox.Shooting
{
    [CreateAssetMenu]
    [Serializable]
    public class ShootingData : BaseScriptableObject
    {
        public float FlightDuration = 1.2500f;
        public float GravityScale = 1.0000f;

        public float ZPosition = 0.0000f;
        public float XPosition = 0.0000f;

        public float SwipeWeight = 0.1200f;
        public float SwipeMin = 2.0000f;

        public AnimationCurve SwipeCurve;
    }
}
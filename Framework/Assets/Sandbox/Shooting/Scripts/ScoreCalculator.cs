using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using FullInspector;

using UniRx;
using UniRx.Triggers;

using Common.Extensions;

using Framework;

namespace Sandbox.Shooting
{
    public struct OnScoreSignal
    {
        public int Score { get; set; }
    }

    public class ScoreCalculator : BaseBehavior
    {
        [SerializeField]
        private Dictionary<GameObject, bool> Colliders;
        
        public void CheckShoot(GameObject ring)
        {
            if (!Colliders.ContainsKey(ring))
            {
                Colliders.Add(ring, true);
            }

            if (Colliders.Count >= 2)
            {
                this.Publish(new OnScoreSignal() { Score = 1 });
            }
        }
    }
}
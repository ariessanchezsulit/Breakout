using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using FullInspector;

using UniRx;
using UniRx.Triggers;

namespace Sandbox.Shooting
{
    public class RingTargetTrigger : BaseBehavior
    {
        void OnTriggerEnter(Collider other)
        {
            ScoreCalculator score = other.GetComponent<ScoreCalculator>();

            if (score != null)
            {
                score.CheckShoot(gameObject);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using FullInspector;

using UniRx;
using UniRx.Triggers;

namespace Sandbox.Shooting
{
    public class PlayerPosition : BaseBehavior
    {
        [SerializeField]
        [InspectorRange(0f, 18f)]
        private float HorizontalThreshold = 8f;

        [SerializeField]
        [InspectorRange(0f, 8f)]
        private float VerticalThreshold = 8f;
        
        public void UpadteHorizontalPosition(float scale)
        {
            Vector3 pos = transform.position;
            pos.x = HorizontalThreshold * scale;
            transform.position = pos;
        }

        public void UpdateVerticalPosition(float scale)
        {
            Vector3 pos = transform.position;
            pos.z = VerticalThreshold * scale;
            transform.position = pos;
        }
    }
}
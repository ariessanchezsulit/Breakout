using System;
using System.Collections;

using UnityEngine;

using FullInspector;

using UniRx;
using UniRx.Triggers;

namespace Sandbox.Shooting
{
    public class AddImpulseForce : BaseBehavior
    {
        public Transform Target;
        public ForceMode Mode = ForceMode.Impulse;

        [InspectorButton]
        public void AddForce()
        {
            Vector3 force = Target.position - transform.position;
            GetComponent<Rigidbody>().AddForce(force, Mode);
        }

        [InspectorButton]
        public void Stop()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.angularDrag = 0f;
            rb.position = Vector3.zero;
        }

        private void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.S))
            {
                Stop();
            }

            if (Input.GetKey(KeyCode.A))
            {
                AddForce();
            }
        }
    }
}
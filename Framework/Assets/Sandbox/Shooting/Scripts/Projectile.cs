using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using FullInspector;

namespace Sandbox.Shooting
{
    using URandom = UnityEngine.Random;

    public enum ShootKey
    {
        SPACE = KeyCode.Space,
        Q = KeyCode.Q,
        E = KeyCode.E,
        R = KeyCode.R,
        T = KeyCode.T,
    }

    public class Projectile : BaseBehavior
    {
        [InspectorCategory("Projectile")]
        public Transform Source;

        [InspectorCategory("Projectile")]
        public Transform Target;
        
        [InspectorCategory("Projectile")]
        [InspectorRange(0f, 10f)]
        public float Duration;

        [InspectorCategory("Projectile")]
        [InspectorRange(0f, 10f)]
        public float DurationScaler;

        [InspectorCategory("Projectile")]
        [InspectorRange(0f, 10f)]
        public float GravityScale;
        
        [InspectorCategory("Projectile")]
        public bool UseGravity = false;

        [InspectorCategory("Projectile")]
        public bool ModifyEulerAngles = false;

        [InspectorCategory("Projectile")]
        public bool UseAngularVelocity = false;

        [InspectorDisabled]
        [InspectorCategory("Projectile")]
        [SerializeField]
        private bool IsThrown = false;

        [InspectorCategory("Debug")]
        public ShootKey ShotKey;

        private Rigidbody Rigidbody;
        private static float CachedDistance = -1f;
        
        protected override void Awake()
        {
            base.Awake();

            Rigidbody = GetComponent<Rigidbody>();
            Stop();
        }

        private void Start()
        {
            if (CachedDistance <= -1f)
            {
                CachedDistance = Vector3.Distance(transform.position, Target.position);
            }
        }

        private void FixedUpdate()
        {
            /*
            if (Input.GetKey(KeyCode.S))
            {
                Stop();
            }

            if (Input.GetKey((KeyCode)ShotKey))
            {
                Throw();
            }
            //*/

            if (IsThrown && !Rigidbody.useGravity)
            {
                Rigidbody.AddForce(Physics.gravity * GravityScale, ForceMode.Force);
            }

            if (!IsThrown)
            {
                DurationScaler = Vector3.Distance(transform.position, Target.position) / CachedDistance;
            }
        }

        [InspectorCategory("Projectile")]
        [InspectorButton]
        public Vector3 Throw()
        {
            Stop();
            
            return Throw(Target.position - Source.position);
        }

        public Vector3 Throw(Vector3 direction)
        {
            Rigidbody = GetComponent<Rigidbody>();
            Rigidbody.useGravity = UseGravity;

            if (ModifyEulerAngles)
            {
                Rigidbody.transform.eulerAngles = new Vector3(0f, 0f, URandom.Range(0, 360f));
            }

            if (UseAngularVelocity)
            {
                Rigidbody.angularVelocity = Vector3.one * URandom.Range(-360f, 360f);
            }

            Vector3 velocity = GetVelocity(direction, Duration, Physics.gravity * GravityScale);
            Rigidbody.velocity = velocity;

            IsThrown = true;
            
            return velocity;
        }

        [InspectorCategory("Projectile")]
        [InspectorButton]
        public void Stop()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Rigidbody.position = Source.position;
            Rigidbody.useGravity = false;
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;

            transform.position = Source.position;

            IsThrown = false;
        }

        /// <summary>
        /// Returns the pre-calculated velocity
        /// </summary>
        /// <returns></returns>
        public Vector3 GetVelocity()
        {
            return GetVelocity(Target.position, Source.position, Duration, Physics.gravity * GravityScale);
        }

        public Vector3 GetVelocity(Vector3 target, Vector3 start, float timeOfFlight, Vector3 gravity)
        {
            float duration = TimeOfFlight(timeOfFlight);
            return ((target - start) / duration) - 0.5f * gravity * duration;
        }

        public Vector3 GetVelocity(Vector3 direction, float magnitude, float timeOfFlight, Vector3 gravity)
        {
            float duration = TimeOfFlight(timeOfFlight);
            return ((direction.normalized * magnitude) / duration) - 0.5f * gravity * duration;
        }

        public Vector3 GetVelocity(Vector3 direction, float timeOfFlight, Vector3 gravity)
        {
            float duration = TimeOfFlight(timeOfFlight);
            return (direction / duration) - 0.5f * gravity * duration;
        }

        public float TimeOfFlight(float timeOfFlight)
        {
            // Ignore TimeOfFlight scaler for now
            //return Mathf.Max(timeOfFlight, timeOfFlight * DurationScaler);
            return timeOfFlight;
        }
    }
}
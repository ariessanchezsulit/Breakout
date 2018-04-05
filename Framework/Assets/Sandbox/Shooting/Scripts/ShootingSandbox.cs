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
    public struct VectorAccuracy
    {
        public float Angle;
        public float Accuracy;
        public float Magnitude;
    }

    public class ShootingSandbox : SceneObject
    {
        public const string BALL = "Ball";
        public const float CENTER_RING = 0.5f;
        
        [InspectorCategory("Object")] public Slider VelocityScale;
        [InspectorCategory("Object")] public Slider HorizontalScale;
        [InspectorCategory("Object")] public Slider VerticalScale;
        [InspectorCategory("Object")] public Slider SwipeDistance;

        [InspectorCategory("Object")] public InputField Duration;
        [InspectorCategory("Object")] public InputField GravityScale;

        [InspectorCategory("Object")] public TargetPosition TargetPosition;
        [InspectorCategory("Object")] public PlayerPosition PlayerPosition;
        [InspectorCategory("Object")] public Rigidbody RingRef;
        [InspectorCategory("Object")] public Rigidbody BallRef;
        [InspectorCategory("Object")] public PrefabManager Balls;
        [InspectorCategory("Object")] public Text Score;

        [InspectorCategory("Data")] public ShootingData Data;
        
        private Projectile Ball;
        private int ScoreValue = 0;
        
        private void Start()
        {
            TKSwipeRecognizer recognizer = new TKSwipeRecognizer();
            recognizer.gestureRecognizedEvent += (r) =>
            {
                VectorAccuracy result = CheckAccuracy(r.endPoint - r.startPoint);
                float durationScale = Mathf.Clamp01(r.swipeVelocity / 50f);

                TargetPosition.UpdateShotTarget(1f - (result.Angle / 180f));
                Throw(r.swipeVelocity);
            };

            TouchKit.addGestureRecognizer(recognizer);

            BallRef.gameObject.SetActive(false);
            Ball = GetBall();

            // Reset data from Scriptable object
            Ball.Duration = Data.FlightDuration;
            Ball.GravityScale = Data.GravityScale;
            PlayerPosition.UpadteHorizontalPosition(Data.XPosition);
            PlayerPosition.UpdateVerticalPosition(Data.ZPosition);
            VelocityScale.value = Data.SwipeWeight;
            SwipeDistance.value = Data.SwipeMin;
            recognizer.UpdateMinimumDistance(Data.SwipeMin);

            Duration.GetComponent<InputValues>().UpdateDisplay(Ball.Duration);
            GravityScale.GetComponent<InputValues>().UpdateDisplay(Ball.GravityScale);
            
            this.Receive<OnScoreSignal>()
                .Subscribe(_ =>
                {
                    ScoreValue += _.Score;
                    Score.text = string.Format("{0}", ScoreValue);
                })
                .AddTo(this);
            
            VelocityScale.OnValueChangedAsObservable()
                .Subscribe(_ => Data.SwipeWeight = _)
                .AddTo(this);

            HorizontalScale.OnValueChangedAsObservable()
                .Subscribe(_ => PlayerPosition.UpadteHorizontalPosition(_))
                .AddTo(this);

            VerticalScale.OnValueChangedAsObservable()
               .Subscribe(_ => PlayerPosition.UpdateVerticalPosition(_))
               .AddTo(this);

            SwipeDistance.OnValueChangedAsObservable()
                .Subscribe(_ => recognizer.UpdateMinimumDistance(_))
                .AddTo(this);

            Duration.OnValueChangedAsObservable()
                .Subscribe(_ =>
                {
                    Ball.Duration = _.ToFloat();
                    BallRef.GetComponent<Projectile>().Duration = _.ToFloat();
                })
                .AddTo(this);

            GravityScale.OnValueChangedAsObservable()
                .Subscribe(_ =>
                {
                    Ball.GravityScale = _.ToFloat();
                    BallRef.GetComponent<Projectile>().GravityScale = _.ToFloat();
                })
                .AddTo(this);
        }
        
        private void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Throw();
            }
        }
        
        [InspectorButton]
        public void Throw()
        {
            TargetPosition.UpdateShotTarget(CENTER_RING);
            
            Ball.Throw();

            // Enable collider
            Ball.GetComponent<Collider>().enabled = true;
            Ball.GetComponent<SwarmItem>().minimumLifeSpan = 10f;
            
            // Create new ball
            Ball = GetBall();
        }

        public void Throw(float swipeVelocity)
        {
            Vector3 velocity = (RingRef.transform.position - Ball.transform.position).normalized * swipeVelocity * Data.SwipeWeight;
            velocity = Ball.Throw(velocity);

            // Enable collider
            Ball.GetComponent<Collider>().enabled = true;
            Ball.GetComponent<SwarmItem>().minimumLifeSpan = 10f;
            
            // Create new ball
            Ball = GetBall();
        }

        public Projectile GetBall()
        {
            Rigidbody projectile = Balls.Request(BALL).GetComponent<Rigidbody>();
            projectile.position = Vector3.zero;
            projectile.useGravity = false;
            projectile.velocity = Vector3.zero;
            projectile.angularVelocity = Vector3.zero;
            projectile.gameObject.SetActive(true);
            
            projectile.transform.position = BallRef.transform.position;
            projectile.transform.localScale = BallRef.transform.localScale;
            projectile.transform.localRotation = BallRef.transform.localRotation;
            projectile.GetComponent<Collider>().enabled = false;

            return projectile.GetComponent<Projectile>();
        }

        public VectorAccuracy CheckAccuracy(Vector2 direction)
        {
            VectorAccuracy result;
            result.Accuracy = Vector3.Dot(Vector2.up, direction.normalized);
            //result.Angle = Vector3.Angle(direction, Vector2.up);
            result.Angle = Vector3.Angle(direction, Vector2.right);
            result.Magnitude = direction.magnitude;

            return result;
        }
    }
}

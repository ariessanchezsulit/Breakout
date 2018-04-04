using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

using FullInspector;

using uPromise;

using Common;
using Common.Extensions;
using Common.Query;
using Common.Utils;

namespace Framework
{
    using System;
    using Framework;

    public struct CanvasSetup
    {
        [InspectorRange(0f, 100f)]
        public int PlaneDistance;

        public RenderMode RenderMode;
        public ESceneDepth SceneDepth;
        public Canvas Canvas;
    }

    public class SystemCanvas : BaseBehavior
    {
        [SerializeField]
        private bool UseSystemCamera = false;

        [SerializeField]
        private Camera Camera;

        [SerializeField]
        private List<CanvasSetup> CanvasList;

        protected override void Awake()
        {
            base.Awake();
        }

        [InspectorButton]
        public void SetupSceneCanvas()
    {
            if (UseSystemCamera && QuerySystem.HasResolver(QueryIds.SystemCamera))
            {
                Camera = QuerySystem.Query<Camera>(QueryIds.SystemCamera);
            }

            CanvasList.ForEach(setup => {
                setup.Canvas.renderMode = setup.RenderMode;
                setup.Canvas.planeDistance = setup.PlaneDistance;
                setup.Canvas.sortingOrder = setup.SceneDepth.ToInt();
                setup.Canvas.worldCamera = Camera;
            });
        }
    }
}

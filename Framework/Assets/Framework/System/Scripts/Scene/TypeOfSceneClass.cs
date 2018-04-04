using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using uPromise;

using UniRx;

using Common;
using Common.Extensions;
using Common.Query;
using Common.Signal;

// alias
using CColor = Common.Extensions.Color;
using UScene = UnityEngine.SceneManagement.Scene;
using FullInspector;

namespace Framework
{
    //CLASS BEING USED TO POPULATE THE LIST OF USABLE SCENES, HAS A BUTTON WHICH SELECTS AND UPDATES THE CURRENT LIST BEING USED ON THE SCENE INSTANCE
    [Serializable]
    public class TypeOfSceneClass
    {
        private Scene sceneReference;
        public void SetSceneReference(Scene temp)
        {
            sceneReference = temp;
        }
        [InspectorButton]
        void Select()
        {
            sceneReference.SceneTypeString = name;
            sceneReference.UpdateEnum((EScene)(Enum.Parse(typeof(EScene), name)));
            sceneReference.HideSceneTypeDropdown();
        }
        [InspectorDisabled]
        public string name = "";
    }
}
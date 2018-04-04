using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Common;
using Common.Extensions;

// alias
using CColor = Common.Extensions.Color;

namespace Framework
{

    //[CustomEditor(typeof(Button), true)]
    public class ButtonEditor : DrawButtonEditor
    {
        private static readonly string ERROR = CColor.red.LogHeader("[ERROR]");
        private static readonly string[] HIDDEN = new string[] { "m_Script" };

        /// <summary>
        /// List of SceneTypes (Enum Representation)
        /// </summary>
        private string[] CachedButtonTypes;
        private List<string> ButtonTypes;
        
        private int ButtonTypeIndex = -1;

        private SerializedProperty ButtonType;

        /// <summary>
        /// This method is called on every focus of this gameobject (Button).
        /// This updates the Button enum automatically.
        /// </summary>
        private void Awake()
        {
            CachedButtonTypes = File.ReadAllLines("FrameworkFiles/FrameworkButtons.dat");
            ButtonTypes = new List<string>(CachedButtonTypes);
            
            EditorUtility.SetDirty(this.GetTarget<Button>().gameObject);
        }

        private void OnEnable()
        {
            // Update the cache values
            CachedButtonTypes = ButtonTypes.ToArray();

            // Update data from editor
            ButtonType = serializedObject.FindProperty("_ButtonType");
            ButtonTypeIndex = ButtonTypes.IndexOf(ButtonType.stringValue);

            Assertion.Assert(ButtonTypeIndex >= 0, string.Format(ERROR + " ButtonEditor::OnEnable Invalid cached ButtonType:{0} Button:{1}\n", ButtonType.stringValue, GetTarget<Button>().name));
        }

        public override void OnInspectorGUI()
        {
            // Draw Script
            serializedObject.Update();
            SerializedProperty prop = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
            serializedObject.ApplyModifiedProperties();

            // Update Button from Editor cache
            UpdateButtonIndices();

            // Hides script field on Editor (Since it was already drawn above the Scene Indices)
            DrawPropertiesExcluding(serializedObject, HIDDEN);

            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateButtonIndices()
        {
            // Updates Button Index based on the selected value from editor
            {
                int sceneIndex = EditorGUILayout.Popup("Button Type", ButtonTypeIndex, CachedButtonTypes);

                if (sceneIndex >= 0 && ButtonTypeIndex != sceneIndex)
                {
                    ButtonTypeIndex = sceneIndex;
                    ButtonType.stringValue = ButtonTypes[sceneIndex];
                    serializedObject.Update();

                    GetTarget<Button>().ButtonTypeString = ButtonTypes[sceneIndex];
                }
            }
        }
        
    }

}
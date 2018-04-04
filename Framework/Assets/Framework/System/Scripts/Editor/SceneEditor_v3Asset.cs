using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneEditor_v3Asset
{
    //CREATES A SCRIPTABLE OBJECT THAT HANDLES THE LIST OF SCENES BEING USED THROUGH OUT THE GAME
    [MenuItem("Assets/Create/SceneEditor_v3")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<SceneEditor_v3>();
    }
}
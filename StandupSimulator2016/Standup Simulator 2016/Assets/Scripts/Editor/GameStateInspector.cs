using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameState), true)]
public class GameStateInspector : Editor
{
    /// <summary>
    /// Draw the inspector properties.
    /// </summary>

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
    }

}

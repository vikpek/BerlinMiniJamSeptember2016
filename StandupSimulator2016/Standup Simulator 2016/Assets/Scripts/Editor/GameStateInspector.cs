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
        this.ShowDepartmentPrefabs();
        base.DrawDefaultInspector();
    }

    void ShowDepartmentPrefabs()
    {
        var pGameState = this.target as GameState;

        if (pGameState.PersonPrefabs == null ||
            pGameState.PersonPrefabs.Count != (int)GameState.Department.Num)
        {
            if (pGameState.PersonPrefabs == null)
            {
                pGameState.PersonPrefabs = new List<GameObject>();
            }
            var pOldPersons = pGameState.PersonPrefabs;
            pGameState.PersonPrefabs.Clear();

            for (int i = 0; i < (int)GameState.Department.Num; ++i)
            {
                if (pOldPersons != null && pOldPersons.Count < i)
                {
                    pGameState.PersonPrefabs.Add(pOldPersons[i]);
                }
                else
                {
                    pGameState.PersonPrefabs.Add(null);
                }
            }
        }
        for (int i = 0; i < (int)GameState.Department.Num; ++i)
        {
            var sDepartment = ((GameState.Department)i).ToString();
            var pNewPerson = EditorGUILayout.ObjectField(sDepartment, pGameState.PersonPrefabs[i], typeof(GameObject), false) as GameObject;
            if (pNewPerson != pGameState.PersonPrefabs[i])
            {
                pGameState.PersonPrefabs[i] = pNewPerson;
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FontManagerDB))]
public class FontManagerDBEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Populate Managed Text"))
        {
            FontManagerDB db = (FontManagerDB)target;
            db.PopulateManagedText();
        }
    }
}

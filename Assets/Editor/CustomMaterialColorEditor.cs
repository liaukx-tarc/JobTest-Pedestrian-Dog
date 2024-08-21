using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomMaterialColor))]
public class MyScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CustomMaterialColor customMaterialColor = (CustomMaterialColor)target;

        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            customMaterialColor.ApplyColor();
        }
    }
}
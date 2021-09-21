using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Chunk))]
[CanEditMultipleObjects]
public class ChunkEditor : Editor
{
    SerializedProperty scale;

    private void OnEnable()
    {
        scale = serializedObject.FindProperty("scale");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Chunk chunk = (Chunk) target;

        EditorGUILayout.PropertyField(scale);
        GUILayout.Space(5);
        if (chunk.noiseTexture != null)
        {
            EditorGUI.LabelField(new Rect(25, 40, 100, 25), "Heightmap");
            GUILayout.Space(5);
            EditorGUI.DrawPreviewTexture(new Rect(25, 60, 100, 100), chunk.noiseTexture);
            GUILayout.Space(150);
        }

        if (chunk.generator != null)
        {

        }
        serializedObject.ApplyModifiedProperties();
    }
}

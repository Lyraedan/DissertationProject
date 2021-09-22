using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Chunk))]
[CanEditMultipleObjects]
public class ChunkEditor : Editor
{
    SerializedProperty noiseSettings;

    private void OnEnable()
    {
        noiseSettings = serializedObject.FindProperty("noiseSettings");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Chunk chunk = (Chunk)target;

        if (chunk.noiseTexture != null)
        {
            GUILayout.Space(5);
            EditorGUI.LabelField(new Rect(25, 5, 100, 25), "Heightmap");
            GUILayout.Space(5);
            EditorGUI.DrawPreviewTexture(new Rect(25, 30, 100, 100), chunk.noiseTexture);
            GUILayout.Space(120);
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(noiseSettings);
        if (EditorGUI.EndChangeCheck())
        {
            if (chunk.generator != null)
            {
                chunk.UpdateChunk();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}

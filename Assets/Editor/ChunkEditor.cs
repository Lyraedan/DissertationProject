using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Chunk))]
[CanEditMultipleObjects]
public class ChunkEditor : Editor
{
    SerializedProperty perlinMultiplier;
    SerializedProperty heightMultiplier;

    private void OnEnable()
    {
        perlinMultiplier = serializedObject.FindProperty("perlinMultiplier");
        heightMultiplier = serializedObject.FindProperty("heightMultiplier");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Chunk chunk = (Chunk) target;

        EditorGUILayout.PropertyField(perlinMultiplier);
        EditorGUILayout.PropertyField(heightMultiplier);
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

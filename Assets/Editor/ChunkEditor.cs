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
    SerializedProperty noiseLayers;
    SerializedProperty amplitude;

    private void OnEnable()
    {
        perlinMultiplier = serializedObject.FindProperty("perlinMultiplier");
        heightMultiplier = serializedObject.FindProperty("heightMultiplier");
        noiseLayers = serializedObject.FindProperty("noiseLayers");
        amplitude = serializedObject.FindProperty("amplitude");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Chunk chunk = (Chunk) target;

        EditorGUILayout.PropertyField(perlinMultiplier);
        EditorGUILayout.PropertyField(heightMultiplier);
        EditorGUILayout.PropertyField(noiseLayers);
        EditorGUILayout.PropertyField(amplitude);
        GUILayout.Space(5);
        if (chunk.noiseTexture != null)
        {
            GUILayout.Space(5);
            EditorGUI.LabelField(new Rect(25, 100, 100, 25), "Heightmap");
            GUILayout.Space(5);
            EditorGUI.DrawPreviewTexture(new Rect(25, 125, 100, 100), chunk.noiseTexture);
            GUILayout.Space(150);
        }

        if (chunk.generator != null)
        {

        }
        serializedObject.ApplyModifiedProperties();
    }
}

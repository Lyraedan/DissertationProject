using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[CustomEditor(typeof(Chunk))]
[CanEditMultipleObjects]
public class ChunkEditor : Editor
{
    SerializedProperty noiseSettings;
    SerializedProperty speed;
    SerializedProperty numberOfIterations;
    SerializedProperty iterationScale;
    SerializedProperty depositionRate;
    SerializedProperty erosionRate;
    SerializedProperty friction;

    int droplets = 1000;

    private void OnEnable()
    {
        noiseSettings = serializedObject.FindProperty("noiseSettings");
        speed = serializedObject.FindProperty("speed");
        numberOfIterations = serializedObject.FindProperty("numberOfIterations");
        iterationScale = serializedObject.FindProperty("iterationScale");
        depositionRate = serializedObject.FindProperty("depositionRate");
        erosionRate = serializedObject.FindProperty("erosionRate");
        friction = serializedObject.FindProperty("friction");
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
                Debug.Log("Update!");
                chunk.UpdateChunk();
            }
        }
        EditorGUILayout.Vector3Field("Worldspace", chunk.worldSpace);
        GUILayout.Space(10);
        GUILayout.Label("Erosion");
        droplets = EditorGUILayout.IntField("Droplets", droplets);
        EditorGUILayout.PropertyField(speed);
        EditorGUILayout.PropertyField(numberOfIterations);
        EditorGUILayout.PropertyField(iterationScale);
        EditorGUILayout.PropertyField(depositionRate);
        EditorGUILayout.PropertyField(erosionRate);
        EditorGUILayout.PropertyField(friction);
        bool erode = GUILayout.Button("Erode");
        if(erode)
        {
            DateTime before = DateTime.Now;
            for (int i = 0; i < droplets; i++)
            {
                chunk.Erode(Random.Range(0, MeshGenerator.resolution.x),
                            Random.Range(0, MeshGenerator.resolution.y));
            }
            chunk.Refresh();
            DateTime after = DateTime.Now;
            TimeSpan duration = after.Subtract(before);
            Debug.Log($"Eroded in {duration.Milliseconds}ms");
        }
        bool reset = GUILayout.Button("Undo erosion");
        if(reset)
        {
            chunk.UpdateChunk();
        }

        //DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Joystick), true)]
public class JoystickEditor : Editor
{
    private SerializedProperty handleRange;
    private SerializedProperty deadZone;
    private SerializedProperty axisOptions;
    private SerializedProperty snapX;
    private SerializedProperty snapY;
    protected SerializedProperty background;
    private SerializedProperty handle;

    // New properties added to Joystick
    private SerializedProperty followTargetProp;
    private SerializedProperty playerController1;


    protected Vector2 center = new Vector2(0.5f, 0.5f);

    protected virtual void OnEnable()
    {
        handleRange = serializedObject.FindProperty("handleRange");
        deadZone = serializedObject.FindProperty("deadZone");
        axisOptions = serializedObject.FindProperty("axisOptions");
        snapX = serializedObject.FindProperty("snapX");
        snapY = serializedObject.FindProperty("snapY");
        background = serializedObject.FindProperty("background");
        handle = serializedObject.FindProperty("handle");

        // Find new properties if they exist in the Joystick class
        followTargetProp = serializedObject.FindProperty("followTarget");
        playerController1 = serializedObject.FindProperty("playerController");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawValues();
        EditorGUILayout.Space();
        DrawSmoothing();
        EditorGUILayout.Space();
        DrawComponents();

        serializedObject.ApplyModifiedProperties();

        if (handle != null && handle.objectReferenceValue != null)
        {
            RectTransform handleRect = (RectTransform)handle.objectReferenceValue;
            if (handleRect != null)
            {
                handleRect.anchorMax = center;
                handleRect.anchorMin = center;
                handleRect.pivot = center;
                handleRect.anchoredPosition = Vector2.zero;
            }
        }
    }

    protected virtual void DrawValues()
    {
        EditorGUILayout.PropertyField(handleRange, new GUIContent("Handle Range", "The distance the visual handle can move from the center of the joystick."));
        EditorGUILayout.PropertyField(deadZone, new GUIContent("Dead Zone", "The distance away from the center input has to be before registering."));
        EditorGUILayout.PropertyField(axisOptions, new GUIContent("Axis Options", "Which axes the joystick uses."));
        EditorGUILayout.PropertyField(snapX, new GUIContent("Snap X", "Snap the horizontal input to a whole value."));
        EditorGUILayout.PropertyField(snapY, new GUIContent("Snap Y", "Snap the vertical input to a whole value."));
    }

    protected virtual void DrawSmoothing()
    {
        // Only draw smoothing fields if they exist on the target class
        if ( followTargetProp != null)
        {
            EditorGUILayout.LabelField("Joystick Extras", EditorStyles.boldLabel);

            if (followTargetProp != null)
            {
                EditorGUILayout.PropertyField(followTargetProp, new GUIContent("Follow Target", "World transform the joystick background will follow on screen."));
            }

            // Draw player controller field if present on Joystick
            if (playerController1 != null)
            {
                EditorGUILayout.PropertyField(playerController1, new GUIContent("Player Controller", "Assign the PlayerController GameObject (drag & drop)."));
            }
        }
    }

    protected virtual void DrawComponents()
    {
        EditorGUILayout.ObjectField(background, new GUIContent("Background", "The background's RectTransform component."));
        EditorGUILayout.ObjectField(handle, new GUIContent("Handle", "The handle's RectTransform component."));
    }
}
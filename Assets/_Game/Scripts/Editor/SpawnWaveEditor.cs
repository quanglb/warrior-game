using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SpawnWave))]

public class SpawnWaveEditor : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var spawnTimeRect = new Rect(position.x, position.y, 50, position.height);
        var meleeRect = new Rect(position.x + 80, position.y, 50, position.height);
        var rangeRect = new Rect(position.x + 150, position.y, 50, position.height);
        var eliteRect = new Rect(position.x + 220, position.y, 50, position.height);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(spawnTimeRect, property.FindPropertyRelative("spawnTime"), GUIContent.none);
        EditorGUI.PropertyField(meleeRect, property.FindPropertyRelative("MeleeCount"), GUIContent.none);
        EditorGUI.PropertyField(rangeRect, property.FindPropertyRelative("RangeCount"), GUIContent.none);
        EditorGUI.PropertyField(eliteRect, property.FindPropertyRelative("EliteCount"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
using UnityEditor;
using UnityEngine;

//New one
[CustomPropertyDrawer(typeof(ModularCurveBaseHolder))]
public class ModularCurveBaseHolderDrawer: PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;


        //calculate rects
        float rect1width;
        float rect2width;

        if (position.width / 10f * 4f > 160)
        {
            rect1width = 160;
            rect2width = position.width - 160 - 12f;
        }
        else
        {
            rect1width = position.width / 10f * 4;
            rect2width = position.width / 10f * 6 - 12;
        }

        float rect1posX = position.x;
        float rect2posX = position.x + rect1width + 12;

        Rect rect1 = new(rect1posX, position.y, rect1width, 18f);
        Rect rect2 = new(rect2posX, position.y, rect2width, 18f);


        //Draw all field names
        EditorGUI.LabelField(rect1, "Modular Curve Base");
        EditorGUI.PropertyField(rect2, property.FindPropertyRelative("modularCurveBase"), GUIContent.none);

        rect1.y += 18;
        rect2.y += 18;

        EditorGUI.LabelField(rect1, "Value Cycle Duration");
        EditorGUI.PropertyField(rect2, property.FindPropertyRelative("valueCycleDuration"), GUIContent.none);

        rect1.y += 18;
        rect2.y += 18;

        EditorGUI.LabelField(rect1, "Weight Value");
        EditorGUI.PropertyField(rect2, property.FindPropertyRelative("weight"), GUIContent.none);

        rect1.y += 18;
        rect2.y += 18;

        using (new EditorGUI.DisabledScope(true)) EditorGUI.LabelField(rect1, "Average Force");
        using (new EditorGUI.DisabledScope(true)) EditorGUI.PropertyField(rect2, property.FindPropertyRelative("averageForce"), GUIContent.none);

        rect1.y += 18;
        rect2.y += 18;

        using (new EditorGUI.DisabledScope(true)) EditorGUI.LabelField(rect1, "Adjustment Coefficient");
        using (new EditorGUI.DisabledScope(true)) EditorGUI.PropertyField(rect2, property.FindPropertyRelative("adjustmentCoefficient"), GUIContent.none);

        rect1.y += 18;
        rect2.y += 18;

        EditorGUI.LabelField(rect1, "Offset");
        EditorGUI.PropertyField(rect2, property.FindPropertyRelative("offset"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;


        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 108;
    }
}
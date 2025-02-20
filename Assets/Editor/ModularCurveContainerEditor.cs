using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ModularCurveContainer))]
public class ModularCurveContainerEditor : Editor
{
    private ModularCurveContainer modularCurveContainer;

    SerializedProperty modularCurveBaseHolders;
    SerializedProperty averageForce;

    private void OnEnable()
    {
        modularCurveContainer = (ModularCurveContainer)target;

        modularCurveBaseHolders = serializedObject.FindProperty("modularCurveBaseHolders");
        averageForce = serializedObject.FindProperty("averageForce");
    }

    public override void OnInspectorGUI()
    {
        using (new EditorGUI.DisabledScope(true)) EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ScriptableObject)target), typeof(MonoScript), false);
        using (new EditorGUI.DisabledScope(true)) EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ScriptableObject)this), typeof(MonoScript), false);

        serializedObject.Update();

        EditorGUILayout.PropertyField(modularCurveBaseHolders, true);
        averageForce.floatValue = EditorGUILayout.FloatField("Average Force", averageForce.floatValue);
        //modularCurveBaseHolders.objectReferenceValue = EditorGUILayout.ObjectField("Modular Curve Base Holders", modularCurveBaseHolders.objectReferenceValue, typeof(List<ModularCurveBaseHolder>), false);


        if (GUILayout.Button("Calculate Adjustment Coefficients"))
        {
            Debug.Log("Calculate Adjustment Coefficients");
            modularCurveContainer.CalculateAdjustmentCoefficients();
        }

        serializedObject.ApplyModifiedProperties();
    }

    //public override bool RequiresConstantRepaint() { return true; }// Debug.Log("When Is this called"); 
}
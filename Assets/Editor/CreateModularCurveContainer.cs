using UnityEditor;
using UnityEngine;

public class CreateModularCurveContainer
{
    [MenuItem("Assets/Create/ScriptableObjects/ModularCurveContainer")]
    public static void CreateMyAsset()
    {
        ModularCurveContainer asset = ScriptableObject.CreateInstance<ModularCurveContainer>();

        string name = AssetDatabase.GenerateUniqueAssetPath("Assets/ModularCurveContainer.asset");
        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}

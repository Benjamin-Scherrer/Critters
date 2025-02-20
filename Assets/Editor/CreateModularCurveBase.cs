using UnityEditor;
using UnityEngine;

public class CreateModularCurveBase
{
    [MenuItem("Assets/Create/ScriptableObjects/ModularCurveBase")]
    public static void CreateMyAsset()
    {
        ModularCurveBase asset = ScriptableObject.CreateInstance<ModularCurveBase>();

        string name = AssetDatabase.GenerateUniqueAssetPath("Assets/ModularCurveBase.asset");
        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}

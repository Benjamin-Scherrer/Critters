using UnityEngine;
using UnityEditor;

public class CreateSeparateMovementData : Editor
{
    [MenuItem("Assets/Create/ScriptableObjects/SeparateMovementData")]
    public static void CreateMyAsset()
    {
        SeparateMovementData asset = ScriptableObject.CreateInstance<SeparateMovementData>();

        string name = AssetDatabase.GenerateUniqueAssetPath("Assets/SeparateMovementData.asset");
        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}

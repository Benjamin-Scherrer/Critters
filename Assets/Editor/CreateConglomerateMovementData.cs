using UnityEditor;
using UnityEngine;

public class CreateConglomerateMovementData
{ 
    [MenuItem("Assets/Create/ScriptableObjects/ConglomerateMovementData")]
    public static void CreateMyAsset()
    {
        ConglomerateMovementData asset = ScriptableObject.CreateInstance<ConglomerateMovementData>();

        string name = AssetDatabase.GenerateUniqueAssetPath("Assets/ConglomerateMovementData.asset");
        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}

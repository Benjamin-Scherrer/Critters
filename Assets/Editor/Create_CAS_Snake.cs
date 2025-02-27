using UnityEditor;
using UnityEngine;

public class Create_CAS_Snake
{
    [MenuItem("Assets/Create/ScriptableObjects/CAS_Snake")]
    public static void CreateMyAsset()
    {
        CAS_Snake asset = ScriptableObject.CreateInstance<CAS_Snake>();

        string name = AssetDatabase.GenerateUniqueAssetPath("Assets/CAS_Snake.asset");
        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}


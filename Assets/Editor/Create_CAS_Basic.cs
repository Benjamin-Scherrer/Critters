using UnityEditor;
using UnityEngine;

public class Create_CAS_Basic
{
    [MenuItem("Assets/Create/ScriptableObjects/CAS_Basic")]
    public static void CreateMyAsset()
    {
        CAS_Basic asset = ScriptableObject.CreateInstance<CAS_Basic>();

        string name = AssetDatabase.GenerateUniqueAssetPath("Assets/CAS_Basic.asset");
        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}

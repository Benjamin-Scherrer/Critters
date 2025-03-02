using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Create_CAS_Jellyfish 
{
    [MenuItem("Assets/Create/ScriptableObjects/CAS_Jellyfish")]
    public static void CreateMyAsset()
    {
        CAS_Jellyfish asset = ScriptableObject.CreateInstance<CAS_Jellyfish>();

        string name = AssetDatabase.GenerateUniqueAssetPath("Assets/CAS_Jellyfish.asset");
        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}


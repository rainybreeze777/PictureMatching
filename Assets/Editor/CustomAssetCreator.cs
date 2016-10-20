using UnityEngine;
using UnityEditor;

public class CustomAssetCreator : MonoBehaviour
{
    [MenuItem("Assets/Create/Custom Asset/Weapon")]
    static void CreateWeapon() {
    	Weapon weapon = ScriptableObject.CreateInstance<Weapon>();
    	AssetDatabase.CreateAsset(weapon, "Assets/Resources/Weapons/Weapon.asset");
    }
}
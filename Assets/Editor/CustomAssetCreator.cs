using UnityEngine;
using UnityEditor;

public class CustomAssetCreator : MonoBehaviour
{
    [MenuItem("Assets/Create/Custom Asset/Weapon")]
    static void CreateWeapon() {
        Weapon weapon = ScriptableObject.CreateInstance<Weapon>();
        AssetDatabase.CreateAsset(weapon, "Assets/Resources/Weapons/Weapon.asset");
    }

    [MenuItem("Assets/Create/Custom Asset/Character")]
    static void CreateCharacter() {
        Character c = ScriptableObject.CreateInstance<Character>();
        AssetDatabase.CreateAsset(c, "Assets/Resources/Characters/Character.asset");
    }

    [MenuItem("Assets/Create/Custom Asset/EnemyData")]
    static void CreateEnemyData() {
        EnemyData e = ScriptableObject.CreateInstance<EnemyData>();
        AssetDatabase.CreateAsset(e, "Assets/Resources/Enemies/Enemy.asset");
    }
}
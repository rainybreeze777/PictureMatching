using System;
using UnityEditor;
using UnityEngine;

// String to String dictionary
[Serializable] public class StringStringDict : SerializableDictionary<string, string> { }
[CustomPropertyDrawer(typeof(StringStringDict))]
public class StringStringDictDrawer : DictionaryDrawer<string, string> { }

[Serializable] public class ESceneChangeTextAssetDict : SerializableDictionary<ESceneChange, TextAsset> { }
[CustomPropertyDrawer(typeof(ESceneChangeTextAssetDict))]
public class ESceneChangeTextAssetDictDrawer : DictionaryDrawer<ESceneChange, TextAsset> { }
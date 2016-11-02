using System;
using UnityEditor;
using UnityEngine;

// String to String dictionary
[Serializable] public class StringStringDict : SerializableDictionary<string, string> { }
[CustomPropertyDrawer(typeof(StringStringDict))]
public class StringStringDictDrawer : DictionaryDrawer<string, string> { }

[Serializable] public class EMapChangeTextAssetDict : SerializableDictionary<EMapChange, TextAsset> { }
[CustomPropertyDrawer(typeof(EMapChangeTextAssetDict))]
public class EMapChangeTextAssetDictDrawer : DictionaryDrawer<EMapChange, TextAsset> { }
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : ScriptableObject {

    [SerializeField] private int characterId;
    public int CharId { get { return characterId; } }
    // http://unicode.org/repos/cldr-tmp/trunk/diff/supplemental/language_territory_information.html
    // Locale code to localized name
    // [SerializeField] StringStringDict nameDict = new StringStringDict();
    [SerializeField] SerializableDictionary<string, string> nameDict;
    [SerializeField] private string en_caName;
    [SerializeField] private string zh_cnName;

    private string language = "zh_cn";

    public string DisplayName { get {
        // return nameDict[language];
        return zh_cnName;
    } }

    public Character() {
    	// nameDict.Add("zh_cn", zh_cnName);
    	// nameDict.Add("en_ca", en_caName);
    }
}

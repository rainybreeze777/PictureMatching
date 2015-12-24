using UnityEngine;
using System.IO;
using System.Collections;
using Random = UnityEngine.Random;
using SimpleJSON;

public class UtilFunctions {

	public static double nextGaussian(int max, int min)
	{
		return 0;
	}

	public static string getSpriteInfo(int index)
    {
        TextAsset jsonText = Resources.Load("SpritesInfo") as TextAsset;
        // Debug.Log(jsonText.text);
        var jsonData = JSON.Parse(jsonText.text);
        var tileRay = jsonData["tiles"];
        return tileRay[index]["name"];
	}

}

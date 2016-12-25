using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressBar : MonoBehaviour {

    Image foregroundImage;
    
    public float Value {
        get {
            if (foregroundImage != null)
                return (foregroundImage.fillAmount * 100);   
            else
                return 0;   
        }
        set {
            if (foregroundImage != null)
                foregroundImage.fillAmount = (float) value / 100.0f;    
        } 
    }

    void Start () {
        foregroundImage = gameObject.GetComponent<Image>();     
        Value = 0;
    }  
}
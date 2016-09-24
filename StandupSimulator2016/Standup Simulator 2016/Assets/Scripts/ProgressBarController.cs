using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressBarController : MonoBehaviour {

	public Image foregroundImage;
	float currentQuality = 0f;

	void Start(){
		SetQualityToValue (0f);
	}
		
	public void SetQualityToValue(float value){
		currentQuality = value/100;
		foregroundImage.fillAmount = currentQuality;
	}

	public void IncreaseQualityByValue(float value){
		currentQuality += value/100;
		foregroundImage.fillAmount = currentQuality;
	}
}


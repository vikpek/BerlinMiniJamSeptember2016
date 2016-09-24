using UnityEngine;
using System;
using System.Collections;

public class CountdownArmAnimator : MonoBehaviour {

	public Transform arm;

	private const float secondsToDegrees = 180f / 60f;

	void Start(){
		InitateCountDown (120);
	}
	public void InitateCountDown(int seconds){
		StartCoroutine (StartCountdownWithSeconds(seconds));
	}

	IEnumerator StartCountdownWithSeconds(int seconds){
		int restSeconds = seconds;
		float tmp = 90f;
		while (restSeconds > 0) {
			DateTime time = DateTime.Now;
			tmp -= secondsToDegrees;
			arm.localRotation = Quaternion.Euler (0f, 0f, tmp);
			restSeconds--;
			yield return StartCoroutine(WaitAndPrint(1.0F));
		}
		yield return null;
	}

	IEnumerator WaitAndPrint(float waitTime) {
		yield return new WaitForSeconds(waitTime);	
	}
}

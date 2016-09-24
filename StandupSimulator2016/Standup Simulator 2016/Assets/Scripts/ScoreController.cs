using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {

	GameObject scoreField;

	void Start () {
		scoreField = GameObject.FindGameObjectWithTag ("Score");
	}

	void Update () {
		scoreField.GetComponent<Text> ().text = GetHighScoreFromGameState () + " Points";
	}

	float GetHighScoreFromGameState(){
		return GameState.Instance.Score;
	}
}

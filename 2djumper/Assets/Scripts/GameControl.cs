using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //for UI... probably not needed?
using UnityEngine.SceneManagement;
using TMPro; //textmeshpro stuff

public class GameControl : MonoBehaviour {


	private int score = 0;
	public Rigidbody2D player;

	public TextMeshProUGUI scoreText, gameOverText, bonusText, timerText;

	private int timesHitWall = 0; //To be used when we implement bonus
	private int bonusTransformation = 1;

	private float bonusTime = 0.0f;

	// Use this for initialization
	void Start () {
//		player = GetComponent<Rigidbody2D>(); 
	}

	// Update is called once per frame
	void Update () {
		Vector2 position = player.position;


		if(position.y > (score + 1) * 2.5f)
		{
			addScore(1);
		}

		if (bonusTime >= 0) {
			bonusTime -= Time.deltaTime; //Counting down time, if bonus is active.
			double timeDisp = System.Math.Round(bonusTime,1);
			timerText.SetText(timeDisp.ToString() + "seconds left!");
		} else {
			bonusTimer (false);
			bonusText.enabled = false;
			timerText.enabled = false;
		}

	}

//	public void changeMultiplier (int multVal) { // For bonus
//		multiplier = multVal;
//	}


	public void addScore (int pnts) {
		// first add score
		score = score + pnts * bonusTransformation;

		// then write to text
		scoreText.SetText("Score:" + score.ToString()); // can also use scoreText.text = "<msg>"
	}

	public void bonusTimer (bool flag) { // Flag = 0 / 1 , if 1 timer for bonus starts. Can be called at each restart.

		if (flag == true) {
			bonusTime = 3;
			timesHitWall += 1; // Doubling multiplier everytime a wall is hit during bonus
			changeBonus(timesHitWall);
			changeBonusText(timesHitWall);
		}

		if (flag == false) {
			timesHitWall = 0;
			changeBonus(timesHitWall);
		}

	}

	private void changeBonus(int multiplier) {
		bonusTransformation = Mathf.RoundToInt (Mathf.Log (multiplier+ 1));
	}

	private void changeBonusText(int bonusValue) {
		bonusText.enabled = true;
		timerText.enabled = true;
		bonusText.SetText("Bonus! " + bonusTransformation.ToString() + "X!");

	}

	public void PlayerDied () {
		gameOverText.enabled = true;
		Time.timeScale = 0; //freezing game
	}


}

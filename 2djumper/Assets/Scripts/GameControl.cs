using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //for UI... probably not needed?
using UnityEngine.SceneManagement;
using TMPro; //textmeshpro stuff

public class GameControl : MonoBehaviour {


	private int score = 0;
	public Rigidbody2D player;

	public TextMeshProUGUI scoreText, gameOverText;

	public int multiplier = 1; //To be used when we implement bonus

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

	}

	public void changeMultiplier (int multVal) { // For bonus
		multiplier = multVal;
	}


	public void addScore (int pnts) {


		scoreText.SetText("Score:" + score.ToString()); // can also use scoreText.text = "<msg>"

		score = score + pnts * multiplier;
//		scoreText.text = "Score: " + score.ToString ();
	}

	public void PlayerDied () {
		gameOverText.enabled = true;
		Time.timeScale = 0; //freezing game
	}
}

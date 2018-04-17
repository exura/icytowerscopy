using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //for UI... probably not needed?
using UnityEngine.SceneManagement;
using TMPro; //textmeshpro stuff

public class GameControl : MonoBehaviour {

	public Text gameOverText;
	private int score = -50;
	public Rigidbody2D player;

	public TextMeshProUGUI scoreText;

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


	public void addScore (int pnts) {


		scoreText.SetText("hej" + score.ToString()); // can also use scoreText.text = "<msg>"

		score = score + pnts;
		scoreText.text = "Score: " + score.ToString ();
	}

	public void PlayerDied () {
		gameOverText.enabled = true;
		Time.timeScale = 0; //freezing game
	}
}

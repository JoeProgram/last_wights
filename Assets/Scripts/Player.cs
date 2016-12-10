using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// used code from Unity Tutorial: https://unity3d.com/learn/tutorials/topics/2d-game-creation/2d-character-controllers

public class Player : MonoBehaviour {

    float speed = 2f;
    bool facingRight = true;

	//running variables
	public SpriteRenderer runBarHolder;
	public SpriteRenderer runBar;
	float runBarMaxSize;
	bool isRunning;
	float runBonusSpeed = 2.25f;
	float maxStamina = 1.75f;
	float minStaminaRequired = 0.5f;
	float stamina;
	float staminaRechargeRate = 0.5f;
	float runCoolDownTime = 0.5f; // it takes a moment for stamina to recharge
	float runCoolDown;
	float currentRunSpeed = 0;

	Color runBarRed = new Color(0.75390625f, 0.17578125f, 0);
	Color runBarYellow = new Color(0.89453125f, 0.82421875f, 0.203125f);
	Color runBarGreen = new Color(0, 0.5546875f, 0.0625f);

	void Start () {
	
		stamina = maxStamina;
		runBarMaxSize = 0.4217646f;

	}
	
    

	void FixedUpdate () {


		// only turn on run if certain conditions are met
		if(!isRunning && Input.GetAxis("Run") > 0 && stamina >= minStaminaRequired) {
			isRunning = true;

			if(runBar.color.a == 0) ShowRunBar();
		} else if(Input.GetAxis("Run") == 0) {
			isRunning = false;

		}
			
		float currentSpeed = speed;
		if( isRunning ) currentSpeed += runBonusSpeed;

		// move the character around
        float move = Input.GetAxis("Horizontal");
		GetComponent<Rigidbody2D>().velocity = new Vector2(move * (currentSpeed), GetComponent<Rigidbody2D>().velocity.y);

		// show and update the run bar if it's not maxed out
		if(isRunning || stamina < maxStamina) {
			UpdateRunBar();
		}

		// see if we can recharge stamina
		if(stamina < maxStamina && ( move != 0 || !isRunning) ) {

			// have to wait a certain amount of time from the last run
			if(runCoolDown > 0) {
				runCoolDown = Mathf.Clamp( runCoolDown - Time.fixedDeltaTime, 0, runCoolDownTime);
			}

			if(runCoolDown == 0) {
				stamina += staminaRechargeRate * Time.fixedDeltaTime;

				if( stamina == maxStamina ) HideRunBar();
			}


		}

		// if we're running and moving, depelete stamina
		if(move != 0 && isRunning) {

			stamina = Mathf.Clamp(stamina - Time.deltaTime,0,maxStamina);
			runCoolDown = runCoolDownTime;

			// see if running has run out.
			if(isRunning && stamina == 0) {
				isRunning = false;
			}
		}



	}

	void ShowRunBar(){
		DOTween.To(
			() => runBarHolder.color,
			x => runBarHolder.color = x,
			new Color(1, 1, 1, 1),
			0.5f);

		DOTween.To(
			() => runBar.color,
			x => runBar.color = x,
			runBarGreen,
			0.5f);

	}

	void HideRunBar(){
		DOTween.To(
			() => runBarHolder.color,
			x => runBarHolder.color = x,
			new Color(1, 1, 1, 0),
			0.5f);

		DOTween.To(
			() => runBar.color,
			x => runBar.color = x,
			new Color(0, 1, 0, 0),
			0.5f);
	}

	void UpdateRunBar(){
		float staminaPercentage = (stamina / maxStamina);

		//runBar.transform.localScale = new Vector3(1,0.05950472f,1);
		runBar.transform.localScale = new Vector3(staminaPercentage * runBarMaxSize,0.05950472f,1);

		// change color
		if(staminaPercentage < 0.25f) {
			runBar.color = runBarRed;
		} else if(staminaPercentage < 0.5f) {
			runBar.color = runBarYellow; //yellow
		} else {
			runBar.color = runBarGreen;
		}
		runBarHolder.color = Color.white;
	}

	
	void OnCollisionEnter2D( Collision2D collision ){

		
		// did we run into a ghost?
		if(collision.rigidbody != null) {
			Ghost ghost = collision.rigidbody.GetComponent<Ghost>();

			if(ghost != null) {
				if(ghost.isDangerous) {
					Game.instance.LoseGame();
				}
			}
		}
	}


}

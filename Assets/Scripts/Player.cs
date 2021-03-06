﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

// used code from Unity Tutorial: https://unity3d.com/learn/tutorials/topics/2d-game-creation/2d-character-controllers

public class Player : MonoBehaviour {

    float speed = 2f;
    bool facingRight = true;

	public bool isAlive = true;

	//animation variables
	public Animator animator;

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

	// candle variables
	bool hasCandle = false;
	bool usedCandle = false;
	bool usedCandleKeyDown = false;
	public SpriteRenderer candle;
	public bool canUseCandle = false;
	public float useCandleTime = 0.5f;
	public float useCandle;
	public ParticleSystem redFlame;
	public ParticleSystem blueFlame;

	//zoom in for candle action
	public float normalCameraSize = 3.2f;
	public Vector3 normalCameraPosition = new Vector3(0,3,-10);
	public float candleCameraSize = 2;
	public Vector3 candleCameraPosition = new Vector3(0,1,-10);

	//room variables
	public Room activeRoom;

	//sounds
	public AudioSource audioCannotUseCandle;
	public AudioClip sfxTired;

	void Start () {
	
		stamina = maxStamina;
		runBarMaxSize = 0.4217646f;
		animator = GetComponent<Animator>();

	}
	
    

	void FixedUpdate () {

		// you have to be alive to do things
		if( !isAlive ) return;

		// if you used the candle, you're done controlling the character
		if(usedCandle) return;

		/*********************************
		 * 
		 *  MOVEMENT
		 * 
		 ******************************/

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

		// flip sprite
		if( move != 0 ){
			GetComponent<SpriteRenderer>().flipX = move < 0;

		}

		//animation
		if(move != 0 ) {
			animator.SetFloat("speed", currentSpeed);
		} else {
			animator.SetFloat("speed", 0);
		}

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
				AudioSource.PlayClipAtPoint(sfxTired, Camera.main.transform.position);
			}
		}

		/*********************************
		 * 
		 * USE CANDLE
		 * 
		 * *******************************/

		bool animatorIsUsingCandle = false;

		if(hasCandle) {

			candle.flipX = !GetComponent<SpriteRenderer>().flipX; // candle and player are backwards. Boo.....
			if(candle.flipX) {
				candle.transform.localPosition = new Vector2(0.188f, 0.43f);
			} else if(move < 0) {
				candle.transform.localPosition = new Vector2(-0.188f, 0.43f);
			}

			// see whether all the ghosts are in the same room
			canUseCandle = GetActiveRoom().HasAllGhosts();
			if(canUseCandle) {
				redFlame.gameObject.SetActive(false);
				blueFlame.gameObject.SetActive(true);
				UI.instance.candleActionPrompt.alpha = 1;
			} else {
				redFlame.gameObject.SetActive(true);
				blueFlame.gameObject.SetActive(false);
				UI.instance.candleActionPrompt.alpha = 0.5f;
			}

			// the player is trying to use the candle, but they cannot yet
			if( !canUseCandle && Input.GetAxis("UseCandle") > 0 && !usedCandleKeyDown ){
				if(!audioCannotUseCandle.isPlaying ) {
					audioCannotUseCandle.Play();
				}

			} else if(canUseCandle && Input.GetAxis("UseCandle") > 0) {

				animatorIsUsingCandle = true;

				if(candle.flipX) {
					candle.transform.localPosition = new Vector2(0.084f, 0.43f);
				} else if(move < 0) {
					candle.transform.localPosition = new Vector2(-0.0848f, 0.43f);
				}

				if(!GetComponent<AudioSource>().isPlaying) {
					GetComponent<AudioSource>().Play();
				}

				useCandle += Time.fixedDeltaTime;

				if(useCandle >= useCandleTime)
					Game.instance.ActivateCandle( activeRoom );

				Camera.main.transform.localPosition = Vector3.Lerp(normalCameraPosition, candleCameraPosition, useCandle / useCandleTime);
				Camera.main.orthographicSize = Mathf.Lerp(normalCameraSize, candleCameraSize, useCandle / useCandleTime);

			} else if(useCandle > 0) {

				if(GetComponent<AudioSource>().isPlaying) {
					GetComponent<AudioSource>().Stop();
				}

				useCandle = Mathf.Clamp(useCandle - Time.fixedDeltaTime, 0, useCandleTime);

				Camera.main.transform.localPosition = Vector3.Lerp(normalCameraPosition, candleCameraPosition, useCandle / useCandleTime);
				Camera.main.orthographicSize = Mathf.Lerp(normalCameraSize, candleCameraSize, useCandle / useCandleTime);
			}
		}

		// simulate key press down
		animator.SetBool( "usingCandle", animatorIsUsingCandle);
		usedCandleKeyDown = Input.GetAxis("UseCandle") > 0;



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


	void OnTriggerStay2D( Collider2D other ){

		// keep track of the current room you're in
		// the one that counts is the one that your point position
		if(other.gameObject.CompareTag("room")) {
			if( IsInRoom( other.gameObject.GetComponent<Room>())){
				activeRoom = other.gameObject.GetComponent<Room>();
			}
		}
	}

	public bool IsInRoom( Room room ){
		return room.gameObject.GetComponent<BoxCollider2D>().OverlapPoint((Vector2)transform.position + Vector2.up);
	}

	public Room GetActiveRoom(){
		return activeRoom;
	}



	void OnCollisionEnter2D( Collision2D collision ){

		
		// did we run into a ghost?
		if(collision.rigidbody != null) {
			Ghost ghost = collision.rigidbody.GetComponent<Ghost>();

			if(ghost != null) {
				if(ghost.isDangerous) {
					Game.instance.LoseGame();
					isAlive = false;
					animator.SetBool("isAlive", false);
					candle.gameObject.SetActive(false);
					GetComponent<Rigidbody2D>().isKinematic = true;
					GetComponent<Rigidbody2D>().velocity = Vector2.zero;

					Analytics.CustomEvent("level_lost", new Dictionary<string, object> {
						{ "number", SceneManager.GetActiveScene().buildIndex },
						{ "reason", "hit_ghost" },
						{ "ghost", ghost.name  },
						{ "current_room", GetActiveRoom().name },
						{ "version", Version.Number },
					});

				}
			}
		}
	}

	// the player has picked up the candle
	public void GetCandle(){

		hasCandle = true;
		candle.gameObject.SetActive(true);

	}

	public bool HasCandle(){
		return hasCandle;
	}

	public void UsedCandle(){
		hasCandle = false;
		usedCandle = true;
	}

	// the player has used up the candle
	public void UsedUpCandle(){

		//candle.gameObject.SetActive(false);
		blueFlame.gameObject.SetActive( false );

	}
		
	public Vector3 GetCandlePosition(){
		return candle.transform.position;
	}


	// singleton
	private static Player _instance;
	public static Player instance{
		get{
			if(_instance == null) {
				_instance = FindObjectOfType<Player>();
			}
			return _instance;
		}
	}


}

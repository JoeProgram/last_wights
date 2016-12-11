using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum GhostState
{
    HIDDEN,
    WAITING,
    FOLLOWING,
	GOING_TO_DOOR,
    ATTACKING,
	ENDING,
}

public class Ghost : MonoBehaviour {


	public GhostState state;
    public SpriteRenderer sprite;
	public StaircaseDoor staircaseDoor;


	// whether this is thinking about the player
	public GameObject player;
	public BoxCollider2D awarenessTrigger;
	public bool isAwareOfPlayer = false;

	// distances from behavior determine base actions
	public float startRevealRange = 3; // the ghost starts to be revealed at this distance
    public float revealRange = 2; // the ghost is revealed at this distance!
    public float attackRange; // the ghost will prepare to attack at this distance!
    public float followRange = 3; // the ghost will start to follow you at this range!

	//following variables
	public float speed = 3;

	//attack variables
	public bool isDangerous; // the ghost only hurts the player if they're dangerous
	protected float attackCoolDown; // how long the ghost has left to cooldown from an attack

	public float attackPrepTime; // how long the ghost takes to prepare an attack once starting
	public float attackTime; // how long the ghost takes in the dangerous state
	public float attackAnimationTime; // we animate a little longer than the actual attack
	public float attackCoolDownTime; // how long it takes the ghost between attacks


	public virtual void Awake(){
		Game.instance.AddGhost(this);
	}

	// Use this for initialization
	protected virtual void Start () {

		// set initial variables
		player = GameObject.Find("player");
		sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
		awarenessTrigger = GetComponent<BoxCollider2D>();

		// hide the ghost
		sprite.color = new Color( sprite.color.r, sprite.color.g, sprite.color.b, 0);
		
	}
	
	// Update is called once per frame
	protected virtual void Update () {

		// adjust the cooldown time if needed
		if(attackCoolDown != 0) {
			attackCoolDown = Mathf.Clamp(attackCoolDown - Time.deltaTime, 0, attackCoolDownTime);
		}

		// go to state-based update behavior
		switch(state) {
		case GhostState.HIDDEN:
			UpdateHidden();
			break;
		case GhostState.WAITING:
			UpdateWaiting();
			break;
		case GhostState.FOLLOWING:
			UpdateFollowing();
			break;
		case GhostState.GOING_TO_DOOR:
			UpdateGoingToDoor();
			break;
		}

	}


	protected virtual void UpdateHidden(){
 
		if(!isAwareOfPlayer) return;

		//slowly reveal ghost
		float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
		float alpha = Mathf.Clamp01((startRevealRange - distanceToPlayer) / (startRevealRange - revealRange));
		sprite.color = new Color( sprite.color.r, sprite.color.g, sprite.color.b, alpha);

		//if player is close enough, switch to new behavior
		if( distanceToPlayer <= attackRange ) SwitchToAttacking();
		else if(distanceToPlayer <= revealRange ) SwitchToFollowing(); 

	}

	protected virtual void SwitchToWaiting(){
		state = GhostState.WAITING;
	}

	protected virtual void UpdateWaiting(){

		if(!isAwareOfPlayer) return;

		float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
		if(distanceToPlayer <= followRange) SwitchToFollowing();

	}

	protected virtual void SwitchToFollowing(){
		state = GhostState.FOLLOWING;
	}
	protected virtual void UpdateFollowing(){

		// move the ghost towards the player
		float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
		transform.position += new Vector3(speed * Time.deltaTime * Mathf.Sign(player.transform.position.x - transform.position.x), 0, 0);



		// see if we should switch to a new behavior
		if(!isAwareOfPlayer) {

			if(IsAwareOfLastUsedStaircaseDoor()) {
				SwitchToGoingToDoor(StaircaseDoor.LastUsedDoor);
				return;
			}

			SwitchToWaiting();  // if the player is too far away, stop following
			return;
		}
			
		if(distanceToPlayer <= attackRange && attackCoolDown == 0) {
			SwitchToAttacking();
			return;
		}

	}

	protected bool IsAwareOfLastUsedStaircaseDoor(){

		Debug.Log(1);
		if(StaircaseDoor.LastUsedDoor != null) {
			Debug.Log(2);
			return StaircaseDoor.LastUsedDoor.GetComponent<BoxCollider2D>().bounds.Intersects( GetComponent<BoxCollider2D>().bounds );
		}

		return false;

	}

	protected virtual void SwitchToGoingToDoor(StaircaseDoor sd){

		Debug.Log(4);
		staircaseDoor = sd;
		state = GhostState.GOING_TO_DOOR;
	}

	protected virtual void UpdateGoingToDoor(){

		Debug.Log(5);
		// move the ghost towards the door
		transform.position += new Vector3(speed * Time.deltaTime * Mathf.Sign(staircaseDoor.transform.position.x - transform.position.x), 0, 0);

		// move into the door
		if(sprite.bounds.Intersects(staircaseDoor.GetComponent<BoxCollider2D>().bounds)) {
			transform.position = staircaseDoor.connectedDoor.transform.position;
			SwitchToWaiting();
		}
	}

	protected virtual void SwitchToAttacking(){
		state = GhostState.ATTACKING;

		StartCoroutine( Attack() );
	}


	protected virtual IEnumerator Attack(){

		Vector2 attackPosition = player.transform.position; // target the place the player was standing when attack started

		// cue the attack
		sprite.transform.DOShakePosition(attackPrepTime, new Vector3(0.1f,0,0),20,45,false,false);
		yield return new WaitForSeconds(attackPrepTime);

		// perform the attack
		isDangerous = true;
		sprite.gameObject.layer = LayerMask.NameToLayer("ghost");
		sprite.color = new Color(1, 0, 0);
		transform.DOJump(attackPosition, -0.25f, 1, attackAnimationTime);
		yield return new WaitForSeconds(attackTime);

		// the attack ends a little earlier than the animation
		isDangerous = false;
		sprite.gameObject.layer = LayerMask.NameToLayer("intangible");
		sprite.color = new Color(1, 1, 1);
		yield return new WaitForSeconds(attackAnimationTime - attackTime);

		// recharge - switch to another state while we recover
		attackCoolDown = attackCoolDownTime;

		//if player is close enough, switch to new behavior
		if( Vector2.Distance(player.transform.position, transform.position) <= followRange ) SwitchToFollowing();
		else if( IsAwareOfLastUsedStaircaseDoor()) SwitchToGoingToDoor( StaircaseDoor.LastUsedDoor );
		else SwitchToWaiting(); 

	}

	public void SwitchToEnding(){
		state = GhostState.ENDING;
		sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
		sprite.GetComponent<BoxCollider2D>().enabled = false;
	}
		

	// the trigger sets whether the ghost is going to make decisions based
	// off the player
	void OnTriggerEnter2D(Collider2D other){

		if(other.name == "player") {
			isAwareOfPlayer = true;
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if(other.name == "player") {
			isAwareOfPlayer = false;
		}
	}


}

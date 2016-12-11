using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PrickleGhost : Ghost {

	// this ghost has to attack constantly
	protected override void UpdateFollowing(){

		if(attackCoolDown == 0) {
			SwitchToAttacking();
			return;
		}
	}
	protected override void SwitchToWaiting(){ state = GhostState.FOLLOWING; }

	protected override IEnumerator Attack(){
		

		// cue the attack
		sprite.transform.DOShakePosition(attackPrepTime, new Vector3(0.1f,0,0),20,45,false,false);
		AudioSource.PlayClipAtPoint(sfxPrepareAttack, Camera.main.transform.position);
		yield return new WaitForSeconds(attackPrepTime);

		// perform the attack
		isDangerous = true;
		sprite.gameObject.layer = LayerMask.NameToLayer("ghost");
		sprite.color = attackColor;
		AudioSource.PlayClipAtPoint(sfxAttack, Camera.main.transform.position);
		sprite.transform.DOShakePosition(attackPrepTime, new Vector3(0.2f,0,0),20,45,false,false);
		yield return new WaitForSeconds(attackTime);

		// the attack ends a little earlier than the animation
		isDangerous = false;
		sprite.gameObject.layer = LayerMask.NameToLayer("intangible");
		sprite.color = normalColor;
		yield return new WaitForSeconds(attackAnimationTime - attackTime);

		// recharge - switch to another state while we recover
		attackCoolDown = attackCoolDownTime;

		//if player is close enough, switch to new behavior
		SwitchToFollowing(); 

	}

}

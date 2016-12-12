using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShyGhost : Ghost {

	public Sprite shySprite;

	public float minRevealRange;

	protected override void UpdateHidden(){

		if(!isAwareOfPlayer) return;
		if( isPlayerLookingAtMe()) return;

		//slowly reveal ghost
		float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
		//float alpha = Mathf.Clamp01((startRevealRange - distanceToPlayer) / (startRevealRange - revealRange));
		//sprite.color = new Color( sprite.color.r, sprite.color.g, sprite.color.b, alpha);

		//if player is close enough, switch to new behavior
		//if( distanceToPlayer <= attackRange ) SwitchToAttacking();
		if(distanceToPlayer <= revealRange && distanceToPlayer >= minRevealRange) {
			SwitchToFollowing(); 
			DOTween.To(
				() => sprite.color,
				x => sprite.color = x,
				normalColor,
				1);
		}

		// if we switched, play a sound
		if( state != GhostState.HIDDEN ) AudioSource.PlayClipAtPoint(sfxReveal, Camera.main.transform.position);

	}



	// this ghost likes to hide
	protected void SwitchToHiding(){

		state = GhostState.HIDDEN;
		sprite.sprite = shySprite;
		DOTween.To(
			() => sprite.color,
			x => sprite.color = x,
			new Color(normalColor.r,normalColor.g, normalColor.b,0),
			2f);

	}

	protected override void SwitchToFollowing(){
		base.SwitchToFollowing();
		sprite.sprite = normalSprite;
	}

	protected override void UpdateFollowing(){

		if(isPlayerLookingAtMe()) {
			SwitchToHiding();

		} else {
			base.UpdateFollowing();
		}

	}

	protected bool isPlayerLookingAtMe(){
		if(isAwareOfPlayer) {
			if(!Player.instance.GetComponent<SpriteRenderer>().flipX && Player.instance.transform.position.x < transform.position.x) {
				return true;
			} else if(Player.instance.GetComponent<SpriteRenderer>().flipX && Player.instance.transform.position.x > transform.position.x) {
				return true;
			}
		}

		return false;
	}

}

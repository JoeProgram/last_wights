using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandlePickupTrigger : ActionTrigger {

	protected override void DoAction(){

		Player.instance.GetCandle();

		Destroy(gameObject);
	}
}

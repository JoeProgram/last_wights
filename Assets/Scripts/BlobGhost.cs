using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobGhost : Ghost {

	// this ghost can't move
	protected override void SwitchToAttacking(){ state = GhostState.WAITING; }
	protected override void SwitchToFollowing(){ state = GhostState.WAITING; }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hover : MonoBehaviour {

	public float hoverAmount = 0.1f;
	public float hoverTime = 5;

	// Use this for initialization
	void Start () {
		transform.DOLocalMoveY(hoverAmount, hoverTime).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);	
	}
}

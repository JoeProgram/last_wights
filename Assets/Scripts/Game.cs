using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

// keeps track of state of level
public class Game : MonoBehaviour {

	public CanvasGroup lostMessage;
	public CanvasGroup winMessage;
	public Image candleActionPrompt;

	public List<Ghost> ghosts;

	//sounds
	public AudioClip sfxLose;
	public AudioClip sfxWin;
	public AudioClip sfxGhostSuckedIn;

	public void AddGhost(Ghost ghost){
		ghosts.Add(ghost);
	}

	public void LoseGame(){
		StartCoroutine(LoseGameHelper());
	}

	protected IEnumerator LoseGameHelper(){

		AudioSource.PlayClipAtPoint(sfxLose, Camera.main.transform.position);

		// reveal the lost message
		DOTween.To(
			() => lostMessage.alpha,
			x => lostMessage.alpha = x,
			1,
			0.5f
		);
		yield return new WaitForSeconds(0.5f);



		yield return new WaitForSeconds(2f);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);


	}

	public void TurnOnCandlePrompt(){

		candleActionPrompt.gameObject.SetActive(true);

	}

	public void ActivateCandle(Room room)
	{
		Player.instance.UsedCandle();
		candleActionPrompt.gameObject.SetActive(false);
		StartCoroutine(ActivateCandleHelper(room));
	}

	protected IEnumerator ActivateCandleHelper(Room room){

		List<Ghost> allGhosts = new List<Ghost>(ghosts);

		// time to see if you've won or not.
		// first - we don't want players or the ghosts moving - they should stay still
		foreach( Ghost ghost in allGhosts) {
			ghost.gameObject.SetActive(true);
			ghost.SwitchToEnding();
			ghost.transform.parent = null;
			//Player.instance.GetComponent<Rigidbody2D>() = true;
		}
			

		// next, all ghosts in this room (even just touching it a little ) get sucked into the candle
		Debug.Log( "Ghosts in " + Player.instance.activeRoom.name + ": " + Player.instance.activeRoom.GetGhosts().Count); 
		foreach(Ghost deadGhost in Player.instance.activeRoom.GetGhosts()) {
			
			allGhosts.Remove(deadGhost);
			deadGhost.transform.DOMove(Player.instance.GetCandlePosition(), 0.5f).OnComplete( () => GhostSuckedIn(deadGhost));
			deadGhost.transform.DOScale(Vector3.one * 0.1f, 0.5f);
			yield return new WaitForSeconds(0.2f);
		}

		yield return new WaitForSeconds(0.5f);
		Player.instance.UsedUpCandle();

		yield return new WaitForSeconds(0.5f);

		Debug.Log("all ghosts length " + allGhosts.Count);

		// if that's all the ghosts there are, you won!
		if(allGhosts.Count == 0) {
			WinGame();

		// if there are any more ghosts, the first one comes and attacks you
		} else {

			yield return new WaitForSeconds(0.25f);
			allGhosts[0].transform.DOMove(Player.instance.transform.position,0.5f);
			yield return new WaitForSeconds(0.5f);
			LoseGame();

		}



	}


	protected void GhostSuckedIn(Ghost deadGhost){
		AudioSource.PlayClipAtPoint(sfxGhostSuckedIn, Camera.main.transform.position);
		deadGhost.gameObject.SetActive(false);
	}

	public void WinGame(){
		StartCoroutine(WinGameHelper());
	}

	protected IEnumerator WinGameHelper(){

		AudioSource.PlayClipAtPoint(sfxWin, Camera.main.transform.position);

		// reveal the lost message
		DOTween.To(
			() => winMessage.alpha,
			x => winMessage.alpha = x,
			1,
			0.5f
		);
		yield return new WaitForSeconds(0.5f);

		yield return new WaitForSeconds(2f);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);


	}








	// singleton
	private static Game _instance;
	public static Game instance{
		get{
			if(_instance == null) {
				_instance = FindObjectOfType<Game>();
			}
			return _instance;
		}
	}






}

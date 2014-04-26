using UnityEngine;
using System.Collections;
using CapsaicinGames.Shark;

public class MenuCamera : MonoBehaviour {

    public GameObject endGameCamera;
    public SwimmerSpawner swimmerSpawner;

    private SharkInput _sharkInput;

	// Use this for initialization
	void Start () {
        _sharkInput = FindObjectOfType<SharkInput>();
        swimmerSpawner.OnEndGameCallback += swimmerSpawner_OnEndGameCallback;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void swimmerSpawner_OnEndGameCallback()
    {
        _sharkInput.enabled = false;
        endGameCamera.SetActive(true);
        swimmerSpawner.OnEndGameCallback -= swimmerSpawner_OnEndGameCallback;
    }

    private void Restart()
    {
        SceneControl.RestartGame();
    }
}

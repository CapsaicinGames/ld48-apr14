using UnityEngine;
using System.Collections;

public class SharkAttack : MonoBehaviour 
{
    enum MouthState
    {
        Closed = -1,
        Neutral = 0,
        Open = 1
    };

    public GameObject childMesh;
    public float moneyShotTimeScale;
    public float YoffsetNoControl;
    public float YoffsetAttack;

    private bool performedEat;
    private MouthState currentMouthOpen;

    void Start()
    {
        currentMouthOpen = MouthState.Neutral;
        performedEat = false;
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        // Eating code
        MouthState newMouthOpen = MouthState.Neutral;
        float jawAxis = Input.GetAxis("JawAxis");


        if (jawAxis > 0)
        {
            newMouthOpen = MouthState.Open;
        }
        else if (jawAxis < 0)
        {
            newMouthOpen = MouthState.Closed;
        }

        if ((currentMouthOpen == MouthState.Closed) && (newMouthOpen == MouthState.Open))
        {
            currentMouthOpen = MouthState.Open;
            Debug.Log("Bite (closed)");
        }
        if ((currentMouthOpen == MouthState.Open) && (newMouthOpen == MouthState.Closed))
        {
            currentMouthOpen = MouthState.Closed;
            performedEat = true; // this will possibly change to on player action
            Debug.Log("Bite (open)");
        }

        if ((newMouthOpen == MouthState.Closed) || (newMouthOpen == MouthState.Open))
        {
            currentMouthOpen = newMouthOpen;
            // Store new non-neutral state of jaw
        }


        // No swimming outside of the sea
        if (rigidbody.position.y > YoffsetNoControl)
        {
            childMesh.renderer.material.color = Color.red;
        }

        // Attack time if doing a decent jump and holding a swimmer
        if (rigidbody.position.y > YoffsetAttack)
        {
            bool haveSwimmer = false;
            foreach (var child in gameObject.GetComponentsInChildren<Transform>())
            {
                if (child.tag == "Swimmer")
                {
                    haveSwimmer = true;
                    childMesh.renderer.material.color = Color.yellow;
                }
            }

            if (haveSwimmer)
            {
                Time.timeScale = moneyShotTimeScale;
            }

        }

        if (rigidbody.position.y <= YoffsetNoControl)
        {
            Time.timeScale = 1f;
            childMesh.renderer.material.color = Color.green;
        }

        if (performedEat)
        {
            foreach (var child in gameObject.GetComponentsInChildren<Transform>())
            {
                if (child.tag == "Swimmer")
                {
                    Destroy(child.gameObject);
                    break;
                }
            }
            performedEat = false;
        }
	}
}

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
    public GameObject cameraObject;
    public GameObject gibbObject;

    public float moneyShotTimeScale;
    public float YoffsetNoControl;
    public float YoffsetAttack;
    public int gibbsPerSwimmer;
    // Life of new gibbs in seconds
    public float gibbLife;
    public float gibbRadius;

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

        /*if ((currentMouthOpen == MouthState.Closed) && (newMouthOpen == MouthState.Open))
        {
            currentMouthOpen = MouthState.Open;
        }*/
        if ((currentMouthOpen == MouthState.Open) && (newMouthOpen == MouthState.Closed))
        {
            //currentMouthOpen = MouthState.Closed;
            performedEat = true; // this will possibly change to on player action
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
            cameraObject.GetComponent<SharkAttackCamera>().enabled = true;
            cameraObject.GetComponent<FollowCamera>().enabled = false;

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
            cameraObject.GetComponent<SharkAttackCamera>().enabled = false;
            cameraObject.GetComponent<FollowCamera>().enabled = true;
        }

        if (performedEat)
        {
            EatSwimmer();
            performedEat = false;
        }
	}

    void EatSwimmer()
    {

        var drag = FindObjectOfType<PlayerController>().airDrag;
        // Eat the first swimmer grabbed (if you have more than one)
        foreach (var child in gameObject.GetComponentsInChildren<Transform>())
        {
            if (child.tag == "Swimmer")
            {
                Destroy(child.gameObject);
                for (int i = 0; i < gibbsPerSwimmer; ++i)
                {
                    var newGibb = (GameObject)Instantiate(gibbObject);
                    Rigidbody rb = newGibb.GetComponentInChildren<Rigidbody>();
                    rb.velocity = rigidbody.velocity + (Random.insideUnitSphere * gibbRadius);
                    rb.drag = drag; // get this from player controller at some point
                    newGibb.transform.position = transform.TransformPoint(0f, 0f, 3f);
                    Destroy(newGibb, gibbLife);
                }
                break;
            }
        }
    }

}

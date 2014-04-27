using UnityEngine;
using System.Collections;
using CapsaicinGames.TerrorMap;

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

    public AudioClip[] chompSounds;

    public float gibbTerrorStrength;

    public float moneyShotTimeScale;
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

        if (performedEat)
        {
            EatSwimmer();
            performedEat = false;
        }
	}

    void OnLeaveWater(WaterLeaveEvent type)
    {
        cameraObject.GetComponent<SharkAttackCamera>().enabled = true;
        cameraObject.GetComponent<FollowCamera>().enabled = false;

        if (type == WaterLeaveEvent.MoneyShot)
        {
            Time.timeScale = moneyShotTimeScale;
        }
    }

    void OnEnterWater(WaterEnterEvent type)
    {
        Time.timeScale = 1f;
        //childMesh.renderer.material.color = Color.green;
        cameraObject.GetComponent<SharkAttackCamera>().enabled = false;
        cameraObject.GetComponent<FollowCamera>().enabled = true;
    }

    void EatSwimmer()
    {

        var drag = FindObjectOfType<PlayerController>().airDrag;
        // Eat the first swimmer grabbed (if you have more than one)
        foreach (var child in gameObject.GetComponentsInChildren<Transform>())
        {
            if (child.tag == "Swimmer")
            {
                audio.pitch = Random.Range(0.8f, 1.2f);
                audio.PlayOneShot(chompSounds[Random.Range(0,chompSounds.Length-1)], 1.0f);
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
                if (transform.position.y > 0f) 
                {
                    TerrorMap.Instance.WriteEvent(transform.position,
                                                  gibbRadius,
                                                  gibbTerrorStrength);
                }
                
                break;
            }
        }
    }

}

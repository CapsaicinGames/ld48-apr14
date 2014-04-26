using UnityEngine;
using System.Collections;

public class gibbBehaviour : MonoBehaviour 
{
    private float waterDrag;
    public float bouyancy;
	// Use this for initialization
	void Start ()
    {
	    waterDrag = FindObjectOfType<PlayerController>().waterDrag;
	}
	
	void FixedUpdate ()
    {
	    if (transform.position.y < 0)
        {
            rigidbody.useGravity = false;
            rigidbody.drag = waterDrag;
            rigidbody.AddForce(0f, bouyancy * Time.deltaTime, 0f);
        }
        else
        {
            rigidbody.useGravity = true;
        }
	}
}

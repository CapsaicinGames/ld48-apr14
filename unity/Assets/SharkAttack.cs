using UnityEngine;
using System.Collections;

public class SharkAttack : MonoBehaviour 
{
    public GameObject childMesh;

	// Update is called once per frame
	void Update ()
    {
	    if (rigidbody.position.y > 0)
        {
            childMesh.renderer.material.color = Color.red;
            gameObject.GetComponent<PlayerController>().enabled = false;
            rigidbody.useGravity = true;
            rigidbody.drag = 0.5f;

        }
        else
        {
            childMesh.renderer.material.color = Color.green;
            gameObject.GetComponent<PlayerController>().enabled = true;
            rigidbody.useGravity = false;
            rigidbody.drag = 2f;
        }
	}
}

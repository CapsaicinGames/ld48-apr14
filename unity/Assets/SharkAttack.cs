using UnityEngine;
using System.Collections;

public class SharkAttack : MonoBehaviour 
{
    public GameObject childMesh;
    public float moneyShotTimeScale;

	// Update is called once per frame
	void FixedUpdate ()
    {
	    if (rigidbody.position.y > 0)
        {
            childMesh.renderer.material.color = Color.red;
            gameObject.GetComponent<PlayerController>().enabled = false;
            rigidbody.useGravity = true;
            rigidbody.drag = 0.5f;
            Time.timeScale = moneyShotTimeScale;

        }
        else
        {
            Time.timeScale = 1f;
            childMesh.renderer.material.color = Color.green;
            gameObject.GetComponent<PlayerController>().enabled = true;
            rigidbody.useGravity = false;
            rigidbody.drag = 2f;
        }
	}
}

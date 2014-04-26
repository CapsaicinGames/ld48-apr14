﻿using UnityEngine;
using System.Collections;

public class SharkAttack : MonoBehaviour 
{
    public GameObject childMesh;
    public float moneyShotTimeScale;

    private bool performedEat;

    void Start()
    {
        performedEat = false;
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        if (rigidbody.position.y > 0)
        {
            childMesh.renderer.material.color = Color.red;
            gameObject.GetComponent<PlayerController>().enabled = false;
            rigidbody.useGravity = true;
            rigidbody.drag = 0.5f;
            bool haveSwimmer = false;
            foreach (var child in gameObject.GetComponentsInChildren<Transform>())
            {
                if (child.tag == "Swimmer")
                {
                    haveSwimmer = true;
                    childMesh.renderer.material.color = Color.yellow;
                    performedEat = true; // this will possibly change to on player action
                }
            }

            if (haveSwimmer)
            {
                Time.timeScale = moneyShotTimeScale;
            }

        }
        else
        {
            Time.timeScale = 1f;
            childMesh.renderer.material.color = Color.green;
            gameObject.GetComponent<PlayerController>().enabled = true;
            rigidbody.useGravity = false;
            rigidbody.drag = 2f;

            if (performedEat)
            {
                foreach (var child in gameObject.GetComponentsInChildren<Transform>())
                {
                    if (child.tag == "Swimmer")
                    {
                        Destroy(child.gameObject);
                    }
                }
                performedEat = false;
            }
        }
	}
}
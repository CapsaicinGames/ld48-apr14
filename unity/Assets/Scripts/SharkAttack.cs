﻿using UnityEngine;
using System.Collections;

public class SharkAttack : MonoBehaviour 
{
    public GameObject childMesh;
    public float moneyShotTimeScale;
    public float YoffsetNoControl;
    public float YoffsetAttack;

    private bool performedEat;

    void Start()
    {
        performedEat = false;
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        if (rigidbody.position.y > YoffsetNoControl)
        {
            childMesh.renderer.material.color = Color.red;
        }

        if (rigidbody.position.y > YoffsetAttack)
        {
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

        if (rigidbody.position.y <= YoffsetNoControl)
        {
            Time.timeScale = 1f;
            childMesh.renderer.material.color = Color.green;

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

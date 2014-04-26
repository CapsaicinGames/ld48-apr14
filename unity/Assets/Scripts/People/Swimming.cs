using UnityEngine;
using System.Collections.Generic;

namespace CapsaicinGames.Shark 
{
    public class Swimming : MonoBehaviour 
    {
        //////////////////////////////////////////////////
        public Transform shark;
        public GameObject beach;

        public float proximityTrigger = 20.0f;
        public float sharkFleeStrength = 4.0f;
        public float fleeForce = 4.0f;

        public bool panicMode = false;

        //////////////////////////////////////////////////

        void Update()
        {
            if (SharkNearby())
            {
                panicMode = true;
            }

            if (OnTheBeach())
            {
                Destroy(this);
            }
        }

        void FixedUpdate() 
        {
            var direction = CalculateDirection();

            rigidbody.AddForce(direction * fleeForce);
        }

        // Works out the direction for the swimmer to move in
        Vector3 CalculateDirection()
        {
            var steerAwayFromShark = SteerAwayFromShark();
            var steerTowardsBeach = SteerTowardsBeach();

            return (steerAwayFromShark + steerTowardsBeach).normalized;
        }

        // The direction away from the shark from the swimmer
        Vector3 SteerAwayFromShark()
        {
            var awayFromShark = transform.position - shark.transform.position;
            var distance = awayFromShark.magnitude;

            // You might have been eaten!
            if (distance == 0)
            {
                return Vector3.forward * sharkFleeStrength;
            }

            var steerDirection = awayFromShark / distance;
            var normalisedDistance = distance / proximityTrigger;

            return Mathf.Lerp(sharkFleeStrength, 0, normalisedDistance) * steerDirection;
        }

        // The direction of the beach from the swimmer
        Vector3 SteerTowardsBeach()
        {
            if (!panicMode)
            {
                return Vector3.zero;
            }

            return -beach.transform.forward;
        }

        // Checks to see if a shark is nearby to the swimmer.
        private bool SharkNearby()
        {
            Vector3 offset = shark.transform.position - transform.position;
            float distance = offset.sqrMagnitude;

            if (distance < proximityTrigger * proximityTrigger)
            {
                return true;
            }
            else
            {
                return false;
            }    
        }

        // Detects if the swimmer is on the beach or not
        private bool OnTheBeach()
        {
            var location = (beach.transform.position - transform.position).normalized;

            return false;
        }

        // When the swimmer is grabbed by a shark they are destroyed
        private void onGrabbed()
        {
            Destroy(this);
        }
    }
}
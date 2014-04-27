using UnityEngine;
using System.Collections;

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
        public bool onBeach = false;

        public AnimationCurve m_dragFromTerror;

        TerrorMap.PersonTerror m_terrorSource;

        //////////////////////////////////////////////////

        void Start()
        {
            m_terrorSource = GetComponent<TerrorMap.PersonTerror>();
        }

        void Update()
        {
            if (!onBeach && OnTheBeach())
            {
                onBeach = true;
                StartCoroutine(Escape());
            }
        }

        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == "Gib")
            {
                renderer.material = other.gameObject.renderer.material;
                Destroy(other.gameObject);
            }
        }

        void FixedUpdate() 
        {
            var direction = CalculateDirection();
            panicMode = panicMode || direction.sqrMagnitude > 0.15f;

            float terrorDrag = m_dragFromTerror.Evaluate(m_terrorSource.Terror);
            Debug.DrawRay(transform.position + Vector3.right * 0.1f,
                          Vector3.up * terrorDrag * 5f, Color.blue);
            rigidbody.drag = terrorDrag;
            rigidbody.AddForce(direction * fleeForce);
        }

        // Works out the direction for the swimmer to move in
        Vector3 CalculateDirection()
        {
            var steerAwayFromShark = SteerAwayFromTerror();
            var steerTowardsBeach = SteerTowardsBeach();

            return (steerAwayFromShark + steerTowardsBeach).normalized;
        }

        // The direction away from the shark from the swimmer
        Vector3 SteerAwayFromTerror()
        {
            var steerDirAndTerror = 
                TerrorMap.TerrorMap.Instance.CalculateMinimumTerrorDirection(transform.position);

            if (steerDirAndTerror == Vector4.zero)
            {
                return Vector3.zero;
            }
            else
            {
                var dir = (Vector3)steerDirAndTerror;
                var steer = dir * steerDirAndTerror.w * sharkFleeStrength;
                return steer;
            }
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

        // Detects if the swimmer is on the beach or not
        private bool OnTheBeach()
        {
            var beachTowardsSwimmer = (transform.position - beach.transform.position).normalized;

            return Vector3.Dot(beachTowardsSwimmer, beach.transform.forward) < 0;
        }

        // Destroy the swimmer object as they have escaped to the beach
        IEnumerator Escape()
        {
            yield return new WaitForSeconds(3.0f);
            Destroy(gameObject);
        }

        // When the swimmer is grabbed by a shark they are destroyed
        private void onGrabbed()
        {
            Destroy(this);
        }
    }
}
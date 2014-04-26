using UnityEngine;
using CapsaicinGames.TerrorMap;

public class SurfacePatrolTerror : MonoBehaviour
{
    //////////////////////////////////////////////////

    [SerializeField] float m_maxDepth;
    [SerializeField] float m_terror;
    [SerializeField] float m_worldRadius;

    float m_stampTimer;

    //////////////////////////////////////////////////

    void Update()
    {
        var parentRigidBody = transform.parent.rigidbody;

        m_stampTimer += Time.deltaTime;
        float stampInterval = 0f;
        while (m_stampTimer >= stampInterval)
        {
            float depthTerror = 
                m_terror * Mathf.InverseLerp(m_maxDepth, 0f, transform.position.y);
            if (depthTerror > 0f)
            {
                TerrorMap.Instance.WriteEvent(transform.position, m_worldRadius, depthTerror);
            }
            m_stampTimer -= stampInterval;

            float speed = Mathf.Max(5f, parentRigidBody.velocity.magnitude);
            stampInterval = m_worldRadius / speed;
        }
    }
}
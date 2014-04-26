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
        float speed = Mathf.Max(10f, parentRigidBody.velocity.magnitude);
        float stampInterval = 1.5f * m_worldRadius / speed;

        m_stampTimer += Time.deltaTime;
        while (m_stampTimer >= stampInterval)
        {
            float depthTerror = 
                m_terror * Mathf.InverseLerp(m_maxDepth, 0f, transform.position.y);
            if (depthTerror > 0f)
            {
                TerrorMap.Instance.WriteEvent(transform.position, m_worldRadius, depthTerror);
            }
            m_stampTimer -= stampInterval;
        }
    }
}
using UnityEngine;

namespace CapsaicinGames.Shark 
{
    public class WaveHeightSampler : MonoBehaviour 
    {
        //////////////////////////////////////////////////

        [SerializeField] AnimationCurve m_normalizedBob;
        [SerializeField] Vector2 m_bobMagnitudeRange;
        [SerializeField] Vector2 m_bobDurationRange;
        float m_startTimeOffset = 0f;
        float m_animTime = 0f;
        float m_bobMagnitude;
        float m_bobDuration;

        //////////////////////////////////////////////////

        void Start() {
            m_startTimeOffset = Random.Range(0f, 3f);
            m_bobMagnitude = Random.Range(m_bobMagnitudeRange.x, m_bobMagnitudeRange.y);
            m_bobDuration = Random.Range(m_bobDurationRange.x, m_bobDurationRange.y);
        }

        void FixedUpdate() {
            
            m_animTime += Time.fixedDeltaTime;

            float normalisedBobTime = m_animTime / m_bobDuration;
            float normalisedBob = m_normalizedBob.Evaluate(normalisedBobTime);
            float bobMagnitude = normalisedBob * m_bobMagnitude;

            var newPos = transform.position;

            newPos.y = bobMagnitude;

            transform.position = newPos;
        }
    }
}
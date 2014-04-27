using UnityEngine;

namespace CapsaicinGames.TerrorMap
{
    // reads terror from map, decays over time
    class PersonTerror : MonoBehaviour
    {
        public float Terror
        {
            get; private set;
        }

        //////////////////////////////////////////////////

        [SerializeField] float m_decay;

        //////////////////////////////////////////////////

        void Awake()
        {
            Terror = 0f;
        }

        void Update()
        {
            Terror -= m_decay * Time.deltaTime;
            float minTerror = TerrorMap.Instance.SampleTerrorAt(transform.position);
            Terror = Mathf.Max(minTerror, Terror);
            Debug.DrawRay(transform.position, Vector3.up * Terror * 5f, Color.cyan);
        }
    }
}
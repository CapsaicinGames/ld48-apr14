using System.Collections;
using UnityEngine;

namespace CapsaicinGames.Shark 
{
    public class SwimmerSpawner : MonoBehaviour
    {
        //////////////////////////////////////////////////

        [SerializeField] float m_regionRadius = 0f;
        [SerializeField] GameObject m_swimmerPrefab = null;
        [SerializeField] float m_targetDensity = 0f;

        public event System.Action OnEndGameCallback;

        //////////////////////////////////////////////////

        void Start() 
        {
            GenerateSwimmers();
        }

        /* Create a mass of swimmers in a certain area
         */
        void GenerateSwimmers() 
        {
            float totalArea = Mathf.PI * m_regionRadius * m_regionRadius;
            int swimmersToGenerate = Mathf.FloorToInt(m_targetDensity * totalArea);

            var shark = FindObjectOfType<PlayerController>().transform;
            var beach = GameObject.FindWithTag("BeachPoint");           

            while(swimmersToGenerate > 0) {

                var newSwimmer = (GameObject)Instantiate(m_swimmerPrefab);
                newSwimmer.GetComponent<Swimming>().shark = shark;
                newSwimmer.GetComponent<Swimming>().beach = beach;
                var planePos = Random.insideUnitCircle * m_regionRadius;
                var swimmerPos = new Vector3(planePos.x, 0f, planePos.y);

                float shrinkScalar = Random.Range(0f, 0.6f);

                newSwimmer.transform.parent = transform;
                newSwimmer.transform.position = swimmerPos;
                newSwimmer.transform.localScale -= new Vector3(shrinkScalar, shrinkScalar, shrinkScalar);
                
                --swimmersToGenerate;
            }
        }

        void Update()
        {
            var numSwimmers = transform.childCount;
            if (numSwimmers == 0)
            {
                if (OnEndGameCallback != null)
                {
                    OnEndGameCallback();
                }
            }
        }

    }
}
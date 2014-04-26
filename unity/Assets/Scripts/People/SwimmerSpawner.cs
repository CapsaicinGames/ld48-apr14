using UnityEngine;

namespace CapsaicinGames.Shark 
{
    public class SwimmerSpawner : MonoBehaviour
    {
        //////////////////////////////////////////////////

        [SerializeField] float m_regionRadius = 0f;
        [SerializeField] GameObject m_swimmerPrefab = null;
        [SerializeField] float m_targetDensity = 0f;

        //////////////////////////////////////////////////

        void Start() {
            GenerateSwimmers();
        }

        void GenerateSwimmers() {
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

                newSwimmer.transform.parent = transform;
                newSwimmer.transform.position = swimmerPos;
                
                --swimmersToGenerate;
            }
        }
    }
}
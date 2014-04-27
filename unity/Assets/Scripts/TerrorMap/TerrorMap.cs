using UnityEngine;

namespace CapsaicinGames.TerrorMap
{
    public class TerrorMap : MonoBehaviour
    {
        public static TerrorMap Instance
        {
            get { return s_map; }
        }

        public void WriteEvent(Vector3 worldPosition, float worldRadius, float strength)
        {
            var gridCenter = GridFromWorld(worldPosition);
            if (gridCenter == s_invalid)
            {
                return;
            }

            int gridRadius = Mathf.FloorToInt(worldRadius * m_cellsPerMeter);
            float invSqrGridRadius = 1f / (gridRadius * gridRadius);
            
            int startX = Mathf.Max(0, ((int)gridCenter.x) - gridRadius);
            int lastX = Mathf.Min(m_dimension - 1, ((int)gridCenter.x) + gridRadius);

            int startY = Mathf.Max(0, ((int)gridCenter.y) - gridRadius);
            int lastY = Mathf.Min(m_dimension - 1, ((int)gridCenter.y) + gridRadius);

            for(int xIndex = startX; xIndex <= lastX; ++xIndex)
            {
                for(int yIndex = startY; yIndex <= lastY; ++yIndex)
                {
                    var sqrDistToCenter = 
                        (new Vector2(xIndex, yIndex) - gridCenter).sqrMagnitude;
                    float thisStrength = 
                        Mathf.Max(0f, 1f - (sqrDistToCenter * invSqrGridRadius));
                    float newTerror = Mathf.Min(1f, m_map[xIndex, yIndex] + thisStrength);
                    m_map[xIndex, yIndex] = newTerror;
                }
            }
        }

        // if zero, no terror found.
        // w is size of terror.
        public Vector4 CalculateMinimumTerrorDirection(Vector3 fromPos)
        {
            var fromGrid = GridFromWorld(fromPos);
            if (fromGrid == s_invalid)
            {
                return Vector4.zero;
            }

            int centerX = (int)fromGrid.x;
            int centerY = (int)fromGrid.y;

            float centerTerror = m_map[centerX, centerY];

            Vector3 aways = Vector3.zero;
            int awaysCount = 0;
            float maxTerror = 0f;

            for(int deltaX = -1; deltaX <= 2; ++deltaX)
            {
                int xIndex = centerX + deltaX;
                if (xIndex < 0 || xIndex >= m_dimension)
                {
                    continue;
                }

                for(int deltaY = -1; deltaY <= 2; ++deltaY)
                {
                    int yIndex = centerY + deltaY;
                    if (yIndex < 0 || yIndex >= m_dimension)
                    {
                        continue;
                    }

                    float localTerror = m_map[xIndex, yIndex];
                    if (localTerror > centerTerror)
                    {
                        aways -= new Vector3(deltaX, 0f, deltaY).normalized;
                        ++awaysCount;
                        maxTerror = Mathf.Max(maxTerror, localTerror);
                    }
                }
            }

            var steer = awaysCount == 0 ? Vector3.zero
                : aways / (float)awaysCount;
            return new Vector4(steer.x, steer.y, steer.z, maxTerror);
        }

        public float SampleTerrorAt(Vector3 worldPos)
        {
            var gridPos = GridFromWorld(worldPos);
            var terror = gridPos == s_invalid ? 0f
                : m_map[(int)gridPos.x, (int)gridPos.y];
            return terror;
        }

        //////////////////////////////////////////////////

        [SerializeField] float m_mapRadius;
        [SerializeField] float m_metersPerCell;
        [SerializeField] int m_cellsUpdatedPerFrame;
        [SerializeField] float m_decayRate;
        [SerializeField] bool m_isDebugRenderingEnabled;
        
        float[,] m_map;
        int m_dimension;
        Vector3 m_gridCenterToCorner;
        Transform m_gridTransform;
        Vector3 m_halfMetersPerCell;
        float m_cellsPerMeter;

        static TerrorMap s_map;

        //////////////////////////////////////////////////

        void Awake()
        {
            s_map = this;
        }

        void Start()
        {
            int halfDimension = Mathf.CeilToInt(m_mapRadius / m_metersPerCell);
            m_dimension = 2 * halfDimension;
            m_map = new float[m_dimension,m_dimension];
            
            m_gridCenterToCorner = new Vector3(-m_mapRadius, 0f, -m_mapRadius);
            m_gridTransform = transform;

            m_halfMetersPerCell = new Vector3(m_metersPerCell, 0f, m_metersPerCell) * 0.5f;

            m_cellsPerMeter = 1f / m_metersPerCell;

            StartCoroutine(Decay());
        }

        void Update()
        {
            if (m_isDebugRenderingEnabled)
            {
                DebugRender();
            }
        }

        Vector3 WorldFromGrid(int x, int y)
        {
            var worldDeltaFromCorner = 
                new Vector3(x * m_metersPerCell, 0f, y * m_metersPerCell);
            var worldPos = 
                m_gridTransform.position + worldDeltaFromCorner + m_gridCenterToCorner;
            return worldPos;
        }

        // if -1,-1 then out of range
        static readonly Vector2 s_invalid = new Vector2(-1f,-1f);
        Vector2 GridFromWorld(Vector3 worldPos)
        {
            var cornerRelativeWorld = 
                worldPos - m_gridTransform.position - m_gridCenterToCorner;
            int x = Mathf.FloorToInt(cornerRelativeWorld.x * m_cellsPerMeter);
            int y = Mathf.FloorToInt(cornerRelativeWorld.z * m_cellsPerMeter);

            bool isOutOfRange = x < 0 || x >= m_dimension || y < 0 || y >= m_dimension;
            var gridPos = isOutOfRange ? s_invalid : new Vector2(x, y);
            
            return gridPos;
        }

        void DebugRender()
        {
            Debug.DrawRay(transform.position, Vector3.up * 10f, Color.blue);

            for(int xIndex = 0; xIndex < m_dimension; ++xIndex)
            {
                for(int yIndex = 0; yIndex < m_dimension; ++yIndex)
                {
                    var worldPos = WorldFromGrid(xIndex, yIndex) + m_halfMetersPerCell;
                    float value = m_map[xIndex,yIndex];
                    var col = Color.Lerp(Color.green, Color.red, value);
                    Debug.DrawRay(worldPos, Vector3.up * (0.1f + value * 3f), col);
                }
            }
            
        }

        // reduce heat across the map, bit by bit
        System.Collections.IEnumerator Decay()
        {
            int updatesCompleted = 0;
            float updatesPerCompleteLoop = 
                (m_dimension * m_dimension) / m_cellsUpdatedPerFrame;
            float cellDeltaRate = m_decayRate * updatesPerCompleteLoop;

            while(true)
            {
                for(int xIndex = 0; xIndex < m_dimension; ++xIndex)
                {
                    for(int yIndex = 0; yIndex < m_dimension; ++yIndex)
                    {
                        float currentValue = m_map[xIndex, yIndex];
                        currentValue = 
                            Mathf.Max(0f, currentValue - cellDeltaRate * Time.deltaTime);
                        m_map[xIndex, yIndex] = currentValue;

                        ++updatesCompleted;
                        if (updatesCompleted > m_cellsUpdatedPerFrame)
                        {
                            // stop for this frame
                            updatesCompleted = 0;
                            yield return null;
                        }
                    }
                }    
            }
        }
    }
}
using UnityEngine;
using CapsaicinGames.TerrorMap;

public class JumpTerror : MonoBehaviour
{
    //////////////////////////////////////////////////

    [SerializeField] Vector2 m_bellySplashRadiusStrength; 
    [SerializeField] Vector2 m_backFlopRadiusStrength;
    [SerializeField] Vector2 m_noseInRadiusStrength;
    [SerializeField] Vector2 m_tailInRadiusStrength;

    //////////////////////////////////////////////////

    void OnEnterWater(WaterEnterEvent type)
    {
        var radiusStrength = GetRadiusStrength(type);
        TerrorMap.Instance.WriteEvent(transform.position,
                                      radiusStrength.x,
                                      radiusStrength.y);
    }

    Vector2 GetRadiusStrength(WaterEnterEvent type)
    {
        switch(type)
        {
        case WaterEnterEvent.NoseIn: return m_noseInRadiusStrength;
        case WaterEnterEvent.BellyFlop: return m_bellySplashRadiusStrength;
        case WaterEnterEvent.BackFlop: return m_backFlopRadiusStrength;
        case WaterEnterEvent.TailIn: return m_tailInRadiusStrength;
        default: return Vector2.zero;
        }
    }
}
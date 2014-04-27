using UnityEngine;
using System.Linq;

// events sent:
// - OnEnterWater(WaterEnterEvent)
public enum WaterEnterEvent
{
    NoseIn,
    BellyFlop,
    BackFlop,
    TailIn, 
}
// - OnLeaveWater(WaterLeaveEvent)
public enum WaterLeaveEvent
{
    MoneyShot, // jumping high in the air with someone in your mouth
    Display, // a money shot with no swimmers
    Other,
}
// todo:
// - more events
public class SharkEvents : MonoBehaviour
{
    //////////////////////////////////////////////////

    bool m_isInWater = true;
    
    [SerializeField] float m_leaveWaterHeight;
    [SerializeField] float m_enterWaterHeight;

    //////////////////////////////////////////////////

    void Update()
    {
        bool isNewInWater = (m_isInWater && transform.position.y < m_leaveWaterHeight)
            || (m_isInWater == false && transform.position.y < m_enterWaterHeight);

        if (isNewInWater != m_isInWater)
        {
            if (isNewInWater == false)
            {
                // detect money shot
                var leaveEvent = CalculateWaterLeaveEvent();
                BroadcastMessage("OnLeaveWater", leaveEvent);
            }
            else
            {
                // how did we re-enter the water?
                var enterEvent = CalculateWaterEnterEvent();
                BroadcastMessage("OnEnterWater", enterEvent);
            }

            m_isInWater = isNewInWater;
        }
    }

    WaterEnterEvent CalculateWaterEnterEvent()
    {
        float upness = Vector3.Dot(transform.forward, Vector3.up);
        
        bool isNoseUp = upness > 0.65f;
        if (isNoseUp)
        {
            return WaterEnterEvent.TailIn;
        }

        bool isNoseDown = upness < -0.65f;
        if (isNoseDown)
        {
            return WaterEnterEvent.NoseIn;
        }

        float rightness = Vector3.Dot(transform.up, Vector3.up);
        bool isUpsideDown = rightness < -0.4f;
        return isUpsideDown ? WaterEnterEvent.BackFlop : WaterEnterEvent.BellyFlop;
    }

    WaterLeaveEvent CalculateWaterLeaveEvent()
    {
        float upness = Vector3.Dot(transform.forward, Vector3.up);
        bool isHeadingUp = upness > 0.5f;

        if (isHeadingUp == false)
        {
            return WaterLeaveEvent.Other;
        }

        bool isCarryingPrey = gameObject.transform.Cast<Transform>()
            .Any(childT => childT.tag == "Swimmer");

        return isCarryingPrey ? WaterLeaveEvent.MoneyShot : WaterLeaveEvent.Display;
    }
}
using UnityEngine;

public class SharkInput : MonoBehaviour
{
    public float GetHorizontal() 
    {
        if (!enabled)
        {
            return 0;
        }

        return Input.GetAxis("Horizontal") * m_steeringSensitivity;
    }

    public float GetVertical() 
    {
        if (!enabled)
        {
            return 0;
        }

        return Input.GetAxis("Vertical") * m_steeringSensitivity;
    }

    public float GetSpeed()
    {
        if (!enabled)
        {
            return 0;
        }

        if (Input.GetKey(KeyCode.LeftShift)) 
        {
            return 1f;
        }
        else
        {
            return -Input.GetAxis("ForwardAxis");
        }
    }

    //////////////////////////////////////////////////

    [SerializeField] float m_steeringSensitivity = 3f;

    //////////////////////////////////////////////////

}
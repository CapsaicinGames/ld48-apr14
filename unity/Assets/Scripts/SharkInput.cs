using UnityEngine;

public class SharkInput : MonoBehaviour
{
    public float GetHorizontal() 
    {
        return Input.GetAxis("Horizontal") * m_steeringSensitivity;
    }

    public float GetVertical() 
    {
        return Input.GetAxis("Vertical") * m_steeringSensitivity;
    }

    public float GetSpeed()
    {
        if (Input.GetKey(KeyCode.LeftShift)) 
        {
            return 1f;
        }
        else
        {
            return Input.GetAxis("ForwardAxis");
        }
    }

    //////////////////////////////////////////////////

    [SerializeField] float m_steeringSensitivity = 3f;

    //////////////////////////////////////////////////

}
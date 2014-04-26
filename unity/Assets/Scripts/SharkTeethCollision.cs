using UnityEngine;
using System.Collections;

public class SharkTeethCollision : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        transform.parent.SendMessage("OnTriggerEnter", other);
    }
}

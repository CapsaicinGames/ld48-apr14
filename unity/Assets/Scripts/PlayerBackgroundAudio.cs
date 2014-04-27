using UnityEngine;
using System.Collections;

/*
 * Fades up and down background sounds
 */
public class PlayerBackgroundAudio : MonoBehaviour 
{
        public AudioSource finSound;
    public AudioSource idleSound;
	
	void FixedUpdate ()
    {
        float ypos = transform.parent.transform.position.y;
        float spdSqr = transform.parent.rigidbody.velocity.sqrMagnitude;
        float spdFactor = Mathf.InverseLerp(0, 200, spdSqr);
        float yFactor = Mathf.InverseLerp(-3, 0, ypos);
        float volume;

        if (ypos > 0.5)
        {
            volume = 0;
        }
        else
        {
            volume = yFactor * spdFactor;
        }
        finSound.volume = volume;
        finSound.pitch = 1 + (spdFactor / 3);

        spdFactor = Mathf.InverseLerp(0, 64, spdSqr);
        idleSound.volume = yFactor * (0.5f - (spdFactor/2f));
	}
}

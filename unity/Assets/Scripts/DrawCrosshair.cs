using UnityEngine;
using System.Collections;
using System.Linq;

public class DrawCrosshair : MonoBehaviour 
{
    public Camera ourCam;
    public float crosshairDistance;
    private Renderer[] childRenderers;

    void Start()
    {
        childRenderers = transform.Cast<Transform>().Select(t => t.renderer).ToArray();
    }

	// Update is called once per frame
	void Update () 
    {
        Ray ray = new Ray(transform.parent.TransformPoint(new Vector3(0f,0f,4f)), 
                                                            transform.parent.forward);
        Plane hPlane = new Plane(Vector3.up, Vector3.zero);

        float distance = 0;
        // if the ray hits the plane...
        if (hPlane.Raycast(ray, out distance))
        {
            // get the hit point:
            Vector3 seaIntersect = ray.GetPoint(distance);
            Vector3 angle = seaIntersect - ourCam.transform.position;

            transform.position = ourCam.transform.position + (crosshairDistance * angle.normalized);

            foreach (var r in childRenderers)
            {
                r.enabled = true;
            }
        }
        else
        {
            foreach (var r in childRenderers)
            {
                r.enabled = false;
            }
        }
	}
}

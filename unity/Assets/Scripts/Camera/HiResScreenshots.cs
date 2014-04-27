using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class HiResScreenshots : MonoBehaviour
{
    public int resWidth = 2550;
    public int resHeight = 3300;

    private bool takeHiResShot = false;
    private string screenShotFolder;

    public List<byte[]> photos;

    public static string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png",
        Application.dataPath,
        width, height,
        System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    public void Start()
    {
        photos = new List<byte[]>();
        screenShotFolder = Application.dataPath + "/screenshots/";

        if (!Directory.Exists(screenShotFolder))
        {
            Directory.CreateDirectory(screenShotFolder);
        }
    }

    public void TakeHiResShot()
    {
        takeHiResShot = true;
    }

    void LateUpdate()
    {
        takeHiResShot |= Input.GetKeyDown("joystick button 0");
        if (takeHiResShot)
        {
            if (photos.Count >= 5)
            {
                Debug.Log("Too many photos, not saving");
                takeHiResShot = false;
                return;
            }

            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            camera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidth, resHeight);

            photos.Add(bytes);
            takeHiResShot = false;
            Debug.Log(string.Format("Added screenshot to memory. {0} photos in total now.", photos.Count));

            Texture2D texture = new Texture2D(resWidth, resHeight);
            texture.LoadImage(bytes);
        }
    }
}

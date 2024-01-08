using RosMessageTypes.Sensor;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine.UI;
using UnityEngine;

public class ImageViewer : MonoBehaviour
{
    [SerializeField] private string ImageTopic;

    Texture2D texRos;

    private RawImage rawImage;

    private ROSConnection rosConnection;

    public void Start()
    {
        rawImage = gameObject.GetComponent<RawImage>();
        Debug.Assert(rawImage != null);

        rosConnection = ROSConnection.GetOrCreateInstance();
        rosConnection.Subscribe<ImageMsg>(ImageTopic, start_video_feed);
    }

    public void start_video_feed(ImageMsg img)
    {
        texRos = new Texture2D((int)img.width, (int)img.height, TextureFormat.RGB24, false); // , TextureFormat.RGB24
        BgrToRgb(img.data);
        texRos.LoadRawTextureData(img.data);

        texRos.Apply();
        rawImage.texture = texRos;
    }

    public void BgrToRgb(byte[] data)
    {
        for (int i = 0; i < data.Length; i += 3)
        {
            byte dummy = data[i];
            data[i] = data[i + 2];
            data[i + 2] = dummy;
        }
    }
}

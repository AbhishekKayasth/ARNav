using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;

public class QRCodeRecenter : MonoBehaviour
{
    [SerializeField]
    private ARSession session;
    [SerializeField]
    private ARSessionOrigin sessionOrigin;
    [SerializeField]
    private ARCameraManager cameraManager;
    [SerializeField]
    private GameObject qrScanningPanel;


    private GameObject navigationDropDownGO;
    private GameObject miniMapGO;
    private ApplicationManager manager;
    private bool scanEnable = false;
    private List<Transform> navigationTargetObjects = new List<Transform>();
    //public List<Transform> qrCodePos = new List<Transform>();
    private Texture2D cameraImageTexture;
    private IBarcodeReader reader = new BarcodeReader(); // Create a barcode reader instance

	private void Start()
	{
        manager = GetComponent<ApplicationManager>();
        navigationTargetObjects = manager.navigationTargetObjects;
        navigationDropDownGO = manager.navigationTargetDropdown.gameObject;
        miniMapGO = manager.miniMap;
	}

	private void Update()
	{
        if(Input.GetKeyDown(KeyCode.Space)) {
            SetQRCodeRecenterTarget("Lab01");
		}
	}

	private void OnEnable()
	{
        cameraManager.frameReceived += OnCameraFrameReceived;
	}

	private void OnDisable()
	{
        cameraManager.frameReceived -= OnCameraFrameReceived;
	}

    private void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        if (!scanEnable)
            return;

        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
            return;

        var conversionParams = new XRCpuImage.ConversionParams
        {
            // Get the entire image.
            inputRect = new RectInt(0, 0, image.width, image.height),

            // Downsample by 2.
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),

            // Choose RGBA format.
            outputFormat = TextureFormat.RGBA32,

            // Flip across the vertical axis (mirror image).
            transformation = XRCpuImage.Transformation.MirrorY
        };

        // See how many bytes you need to store final image.
        int size = image.GetConvertedDataSize(conversionParams);

        // Allocate a buffer to store the image.
        var buffer = new NativeArray<byte>(size, Allocator.Temp);

        // Extract the image data
        image.Convert(conversionParams, buffer);

        // The image was converted to RGBA32 format and wriiten into the provided buffer
        // so you can dispose of the XRCpuImage, You must do this or it will leak resources.
        image.Dispose();

        // At this point, you can process the image, pass it to a computer vision algorithm, etc.
        // In this example, you apply it to a texture to visulize it.
        cameraImageTexture = new Texture2D(
                conversionParams.outputDimensions.x,
                conversionParams.outputDimensions.y,
                conversionParams.outputFormat,
                false);

        cameraImageTexture.LoadRawTextureData(buffer);
        cameraImageTexture.Apply();

        // Done with your temporary data, so you can dispose it.
        buffer.Dispose();

        // Detect and decode the barcode inside the bitmap
        var result = reader.Decode(cameraImageTexture.GetPixels32(), cameraImageTexture.width, cameraImageTexture.height);

        // Do something woth the result
        if (result != null)
		{
            SetQRCodeRecenterTarget(result.Text);
            ToggleScan();
		}
	} 

    public void ToggleScan()
	{
        scanEnable = !scanEnable;
        qrScanningPanel.SetActive(scanEnable);
        navigationDropDownGO.SetActive(!scanEnable);
        miniMapGO.SetActive(!scanEnable);
	}

    private void SetQRCodeRecenterTarget(string targetText)
	{
        Transform currentTarget = navigationTargetObjects.Find(x => x.gameObject.name.ToLower().Equals(targetText.ToLower()));
        //Transform currentQRTarget = qrCodePos.Find(x => x.gameObject.name.ToLower().Equals(targetText.ToLower()));
        if (currentTarget != null)
		{
            // Reset position and roatation ARSession
            session.Reset();

            // Add offset for recentering
            sessionOrigin.transform.position = currentTarget.position;
            sessionOrigin.transform.rotation = currentTarget.rotation;
		}
	}
}
using UnityEngine;
using OpenCvSharp.Demo;
using OmiyaGames.MVC;

namespace GGJ2022
{
	public class TestWebcamScene : WebCamera
	{
		public TextAsset faces;
		public TextAsset eyes;
		public TextAsset shapes;

		private FaceProcessorLive<WebCamTexture> processor;
		WebCamModel webCamModel;


		public override string DeviceName
		{
			get => webCamModel != null ? webCamModel.DeviceName.Value : base.DeviceName;
			set
			{
				if (webCamModel != null)
				{
					webCamModel.DeviceName.Value = value;
				}
				else
				{
					base.DeviceName = value;
				}
			}
		}

		public override WebCamDevice? WebCamDevice
		{
			get => webCamModel != null ? webCamModel.CameraInfo : base.WebCamDevice;
			set
			{
				if (webCamModel != null)
				{
					webCamModel.CameraInfo = value;
				}
				else
				{
					base.WebCamDevice = value;
				}
			}
		}
		public override WebCamTexture WebCamTexture
		{
			get => webCamModel != null ? webCamModel.CameraTexture : base.WebCamTexture;
			set
			{
				if (webCamModel != null)
				{
					webCamModel.CameraTexture.Value = value;
				}
				else
				{
					base.WebCamTexture = value;
				}
			}
		}

		/// <summary>
		/// Default initializer for MonoBehavior sub-classes
		/// </summary>
		protected override void Awake()
		{
			// Setup webcam
			webCamModel = WebCam.SetupWebCam();
			base.forceFrontalCamera = true; // we work with frontal cams here, let's force it for macOS s MacBook doesn't state frontal cam correctly

			// Sign up for device name changes
			//webCamModel.DeviceName.OnAfterValueChanged += DeviceName_OnAfterValueChanged;

			// Setup other stuff
			byte[] shapeDat = shapes.bytes;
			if (shapeDat.Length == 0)
			{
				string errorMessage =
					"In order to have Face Landmarks working you must download special pre-trained shape predictor " +
					"available for free via DLib library website and replace a placeholder file located at " +
					"\"OpenCV+Unity/Assets/Resources/shape_predictor_68_face_landmarks.bytes\"\n\n" +
					"Without shape predictor demo will only detect face rects.";

#if UNITY_EDITOR
				// query user to download the proper shape predictor
				if (UnityEditor.EditorUtility.DisplayDialog("Shape predictor data missing", errorMessage, "Download", "OK, process with face rects only"))
					Application.OpenURL("http://dlib.net/files/shape_predictor_68_face_landmarks.dat.bz2");
#else
             UnityEngine.Debug.Log(errorMessage);
#endif
			}

			processor = new FaceProcessorLive<WebCamTexture>();
			processor.Initialize(faces.text, eyes.text, shapes.bytes);

			// data stabilizer - affects face rects, face landmarks etc.
			processor.DataStabilizer.Enabled = true;        // enable stabilizer
			processor.DataStabilizer.Threshold = 2.0;       // threshold value in pixels
			processor.DataStabilizer.SamplesCount = 2;      // how many samples do we need to compute stable data

			// performance data - some tricks to make it work faster
			processor.Performance.Downscale = 256;          // processed image is pre-scaled down to N px by long side
			processor.Performance.SkipRate = 0;             // we actually process only each Nth frame (and every frame for skipRate = 0)
		}

		/// <summary>
		/// Per-frame video capture processor
		/// </summary>
		protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
		{
			// detect everything we're interested in
			processor.ProcessTexture(input, TextureParameters);

			// mark detected objects
			processor.MarkDetected();

			// processor.Image now holds data we'd like to visualize
			output = OpenCvSharp.Unity.MatToTexture(processor.Image, output);   // if output is valid texture it's buffer will be re-used, otherwise it will be re-created

			return true;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			//webCamModel.DeviceName.OnAfterValueChanged -= DeviceName_OnAfterValueChanged;
		}

		//void DeviceName_OnAfterValueChanged(string oldValue, string newValue)
		//{
		//	// Change devices
		//	DeviceName = newValue;
		//}
	}
}
using UnityEngine;
using OmiyaGames.MVC;

namespace GGJ2022
{
	public static class WebCam
	{
		const string DEVICE_NAME_KEY = "webCamName";
		public static WebCamModel SetupWebCam()
		{
			WebCamModel webCamModel;
			if (ModelFactory.TryGet(out webCamModel) == false)
			{
				webCamModel = ModelFactory.Create<WebCamModel>();
				webCamModel.DeviceName.OnAfterValueChanged += DeviceName_OnAfterValueChanged;
			}

			// Grab the last device name from PlayerPrefs
			string deviceName = PlayerPrefs.GetString(DEVICE_NAME_KEY, null);

			// Check to see if there are any devices
			if (WebCamTexture.devices.Length == 0)
			{
				// If not, set device name to null
				deviceName = null;
			}
			else
			{
				// Search if the device with matching name exists
				bool deviceFound = false;
				if (string.IsNullOrEmpty(deviceName) == false)
				{
					foreach (var device in WebCamTexture.devices)
					{
						if (device.name == deviceName)
						{
							deviceFound = true;
							break;
						}
					}
				}

				// If not, grab the first one
				if (deviceFound == false)
				{
					deviceName = WebCamTexture.devices[0].name;
				}
			}

			// Update device name
			webCamModel.DeviceName.Value = deviceName;
			return webCamModel;
		}

		static void DeviceName_OnAfterValueChanged(string oldValue, string newValue)
		{
			// Save the new name into settings
			PlayerPrefs.SetString(DEVICE_NAME_KEY, newValue);

			// quick test
			if (oldValue == newValue)
			{
				return;
			}

			Debug.Log($"Switching to WebCam: {newValue}");
			var webCamModel = ModelFactory.Get<WebCamModel>();
			webCamModel.Dispose();

			// get device index
			int cameraIndex = -1;
			for (int i = 0; i < WebCamTexture.devices.Length; i++)
			{
				if (WebCamTexture.devices[i].name == newValue)
				{
					cameraIndex = i;
					break;
				}
			}

			// set device up
			if (cameraIndex >= 0)
			{
				webCamModel.CameraInfo = WebCamTexture.devices[cameraIndex];
				webCamModel.CameraTexture.Value = new WebCamTexture(webCamModel.CameraInfo.Value.name);
				webCamModel.CameraTexture.Value.Play();
			}
			else
			{
				throw new System.ArgumentException($"provided DeviceName \"{newValue}\"is not correct device identifier");
			}
		}
	}
}

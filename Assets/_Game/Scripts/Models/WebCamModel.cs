using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames;
using OmiyaGames.MVC;

namespace GGJ2022
{
	public class WebCamModel : Model, System.IDisposable
	{
		public Trackable<string> DeviceName = new Trackable<string>();
		[HideInInspector]
		public Trackable<WebCamTexture> CameraTexture = new Trackable<WebCamTexture>();
		[HideInInspector]
		public WebCamDevice? CameraInfo = null;

		public void OnDestroy()
		{
			Dispose();
		}

		public virtual void Dispose()
		{
			if (CameraTexture.HasValue)
			{
				if (CameraTexture.Value.isPlaying)
				{
					CameraTexture.Value.Stop();
				}

				Destroy(CameraTexture.Value);
				CameraTexture.Value = null;
			}

			if (CameraInfo.HasValue)
			{
				CameraInfo = null;
			}
		}
	}
}

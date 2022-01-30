using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames;
using OmiyaGames.MVC;
using OpenCvSharp.Demo;

namespace GGJ2022
{
	public class PlayerModel : Model
	{
		public float defaultSkipDurationSeconds = 0.5f;
		public Trackable<DetectedObject> nose = new Trackable<DetectedObject>();
		public Trackable<DetectedObject> leftEye = new Trackable<DetectedObject>();
		public Trackable<DetectedObject> rightEye = new Trackable<DetectedObject>();
		public Trackable<DetectedObject> outerLip = new Trackable<DetectedObject>();
		public Trackable<Texture2D> webcamTexture = new Trackable<Texture2D>();
		public Trackable<bool> isFaceDetected = new Trackable<bool>(false);

		public Controller.Action<float> SkipTime;
	}
}

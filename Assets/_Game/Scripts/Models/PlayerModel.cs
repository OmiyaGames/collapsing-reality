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
		public float raycastDistance = 20f;
		public LayerMask raycastMask;
		[HideInInspector]
		public Trackable<DetectedFace> face = new Trackable<DetectedFace>();
		[HideInInspector]
		public Trackable<DetectedObject> nose = new Trackable<DetectedObject>();
		[HideInInspector]
		public Trackable<DetectedObject> leftEye = new Trackable<DetectedObject>();
		[HideInInspector]
		public Trackable<DetectedObject> rightEye = new Trackable<DetectedObject>();
		[HideInInspector]
		public Trackable<DetectedObject> outerLip = new Trackable<DetectedObject>();
		[HideInInspector]
		public Trackable<Texture2D> webcamTexture = new Trackable<Texture2D>();
		public Trackable<bool> isFaceDetected = new Trackable<bool>(false);
		public Transform leftEyeTransform;
		public Transform rightEyeTransform;
		public SerializableDictionary<Collider, QuantumTrigger> colliderToTriggerMap = new SerializableDictionary<Collider, QuantumTrigger>();

		public Controller.Action<float> SkipTime;
	}
}

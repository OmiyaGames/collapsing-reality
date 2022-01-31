using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using OmiyaGames;
using OmiyaGames.MVC;
using OpenCvSharp.Demo;

namespace GGJ2022
{
	public class PlayerModel : Model
	{
		public enum Mode
		{
			Tutorial,
			Starting,
			Playing,
			Done
		}

		public float defaultSkipDurationSeconds = 0.5f;
		public float raycastDistance = 20f;
		public LayerMask raycastMask;
		public float gameDuration;
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
		public Trackable<Vector2> webcamDimensionsPixels = new Trackable<Vector2>();
		public Trackable<bool> isFaceDetected = new Trackable<bool>(false);
		public Trackable<int> score = new Trackable<int>(0);
		public Transform leftEyeTransform;
		public Transform rightEyeTransform;
		public SerializableDictionary<Collider, QuantumTrigger> colliderToTriggerMap = new SerializableDictionary<Collider, QuantumTrigger>();
		public IObjectPool<QuantumTrigger> triggerPool = null;

		public Controller.Action<float> SkipTime;
		public Controller.Action RaycastFromEyes;
		public Controller.Func<QuantumTrigger> CreateTrigger;
		public Controller.Action<QuantumTrigger> DestroyTrigger;
		public Controller.Func<float> GetTimePassed;
		public Controller.Func<Mode> GetMode;
	}
}

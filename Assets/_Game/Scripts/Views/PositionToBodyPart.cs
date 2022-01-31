using OmiyaGames;
using OmiyaGames.MVC;
using OpenCvSharp;
using OpenCvSharp.Demo;
using UnityEngine;

namespace GGJ2022
{
	public class PositionToBodyPart : MonoBehaviour
	{
		static readonly Point ZERO = new Point(0, 0);

		[SerializeField]
		RectTransform webCamTransform;
		[SerializeField]
		DetectedFace.FaceElements bodyPart = DetectedFace.FaceElements.Nose;
		[SerializeField]
		bool fixedUpdate = false;

		PlayerModel playerModel;
		Vector3[] localCorners = new Vector3[4];

		void Start()
		{
			playerModel = ModelFactory.Get<PlayerModel>();

			switch (bodyPart)
			{
				case DetectedFace.FaceElements.LeftEye:
					playerModel.leftEyeTransform = transform;
					break;
				case DetectedFace.FaceElements.RightEye:
					playerModel.rightEyeTransform = transform;
					break;
			}
		}

		void Update()
		{
			if (!fixedUpdate)
			{
				UpdatePosition();
			}
		}

		void FixedUpdate()
		{
			if (fixedUpdate)
			{
				UpdatePosition();
			}
		}

		void UpdatePosition()
		{
			// Grab pixel coordinates of the facial body part
			Point pixelCoord;
			bool canPosition;
			switch (bodyPart)
			{
				case DetectedFace.FaceElements.LeftEye:
					canPosition = TryGetEyePosition(playerModel.leftEye, out pixelCoord);
					break;
				case DetectedFace.FaceElements.RightEye:
					canPosition = TryGetEyePosition(playerModel.rightEye, out pixelCoord);
					break;
				default:
					canPosition = TryGetNosePosition(out pixelCoord);
					break;
			}

			// For now, position at the nose
			if (canPosition)
			{
				Vector2 timeVector = new Vector2(pixelCoord.X, pixelCoord.Y);
				timeVector.x /= playerModel.webcamDimensionsPixels.Value.x;
				timeVector.y /= playerModel.webcamDimensionsPixels.Value.y;

				webCamTransform.GetLocalCorners(localCorners);
				var topLeft = localCorners[1];
				var bottomRight = localCorners[3];
				//Debug.Log($"pixelCoord: ({pixelCoord.X}, {pixelCoord.Y}), time: {timeVector}, topLeft: {topLeft}, bottomRight: {bottomRight}");

				Vector2 localPosition = new Vector2(
					Mathf.Lerp(topLeft.x, bottomRight.x, timeVector.x) * webCamTransform.localScale.x,
					Mathf.Lerp(topLeft.y, bottomRight.y, timeVector.y) * webCamTransform.localScale.y);
				transform.localPosition = localPosition;
			}
		}

		bool TryGetNosePosition(out Point returnMark)
		{
			const int NOSE_TIP_INDEX = 0;

			returnMark = ZERO;
			if (playerModel.nose.HasValue && (playerModel.nose.Value.Marks != null) && (playerModel.nose.Value.Marks.Length > NOSE_TIP_INDEX))
			{
				returnMark = playerModel.nose.Value.Marks[NOSE_TIP_INDEX];
				return true;
			}
			return false;
		}

		bool TryGetEyePosition(Trackable<DetectedObject> eye, out Point returnMark)
		{
			returnMark = ZERO;
			if (eye.HasValue && (eye.Value.Marks != null) && (eye.Value.Marks.Length > 0))
			{
				// Average the positions of all the marks
				foreach (var mark in eye.Value.Marks)
				{
					returnMark.X += mark.X;
					returnMark.Y += mark.Y;
				}
				returnMark.X /= eye.Value.Marks.Length;
				returnMark.Y /= eye.Value.Marks.Length;

				return true;
			}
			return false;
		}
	}
}

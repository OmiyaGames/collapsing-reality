using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OmiyaGames.MVC;

namespace GGJ2022
{
	[RequireComponent(typeof(RawImage))]
	public class FaceMask : MonoBehaviour
	{
		const string CENTER = "_Center";
		const string RADIUS = "_Radius";

		[SerializeField]
		RawImage mask;
		[SerializeField]
		Transform noseTransform;
		[SerializeField]
		Vector3 defaultCenter = new Vector3(100f, 100f, 100f);

		[Header("Speed")]
		[SerializeField]
		float smoothMoveCenter = 5f;
		[SerializeField]
		float smoothExpandRadius = 5f;

		PlayerModel playerModel;
		RectTransform rectTransform;
		Vector3 currentCenter, targetCenter;
		float currentRadius, targetRadius;

		void Start()
		{
			playerModel = ModelFactory.Get<PlayerModel>();
			rectTransform = (RectTransform)transform;

			currentCenter = defaultCenter;
			currentRadius = 1;
		}

		void Update()
		{
			if(playerModel.face.HasValue)
			{
				targetRadius = playerModel.face.Value.Region.Height;
				targetRadius /= playerModel.webcamDimensionsPixels.Value.y;
				targetRadius *= (rectTransform.rect.height * rectTransform.lossyScale.y) / 2f;
			}

			targetCenter = noseTransform.position;
		}

		void FixedUpdate()
		{
			currentRadius = Mathf.Lerp(currentRadius, targetRadius, (Time.deltaTime * smoothExpandRadius));
			mask.material.SetFloat(RADIUS, currentRadius);

			//currentCenter = Vector3.Lerp(currentCenter, targetCenter, (Time.deltaTime * smoothMoveCenter));
			mask.material.SetVector(CENTER, noseTransform.position);
		}

	void Reset()
		{
			mask = GetComponent<RawImage>();
		}
	}
}

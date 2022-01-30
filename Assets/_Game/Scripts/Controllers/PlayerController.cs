using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames.MVC;

namespace GGJ2022
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField]
		float defaultSkipDurationSeconds = 0.5f;
		[SerializeField]
		float raycastDistance = 20f;
		[SerializeField]
		LayerMask raycastMask;

		PlayerModel playerModel;
		QuantumTrigger leftEyeTrigger = null, rightEyeTrigger = null;

		void Awake()
		{
			playerModel = ModelFactory.Create<PlayerModel>();
			playerModel.defaultSkipDurationSeconds = defaultSkipDurationSeconds;
			playerModel.raycastDistance = raycastDistance;
			playerModel.raycastMask = raycastMask;
		}

		void Start()
		{
			playerModel.SkipTime += (source, duration) =>
			{
				if (duration > Time.fixedDeltaTime)
				{
					Physics.autoSimulation = false;

					// Skip forward in time
					for (float totalTimePassed = 0f; totalTimePassed < duration; totalTimePassed += Time.fixedDeltaTime)
					{
						// Brute force the simulation
						Physics.Simulate(Time.fixedDeltaTime);
					}

					Physics.autoSimulation = true;
				}
			};

			playerModel.RaycastFromEyes += (source) =>
			{
				RaycastHit hit;

				// Ray-trace from each eye
				leftEyeTrigger = GetTrigger(playerModel.leftEyeTransform, leftEyeTrigger);
				rightEyeTrigger = GetTrigger(playerModel.rightEyeTransform, rightEyeTrigger);

				QuantumTrigger GetTrigger(Transform eyeTransform, QuantumTrigger lastTrigger)
				{
					QuantumTrigger returnTrigger = null;

					// Make sure transforms are set
					if (eyeTransform != null)
					{
						// Reset the left eye trigger
						if (lastTrigger != null)
						{
							lastTrigger.IsFocused = false;
						}

						// Raycast from eye
						if (Physics.Raycast(eyeTransform.position, eyeTransform.forward, out hit, playerModel.raycastDistance, playerModel.raycastMask) && playerModel.colliderToTriggerMap.TryGetValue(hit.collider, out returnTrigger))
						{
							returnTrigger.IsFocused = true;
						}
					}
					return returnTrigger;
				}
			};
		}

		void Update()
		{
			playerModel.RaycastFromEyes?.Invoke(this);
		}
	}
}

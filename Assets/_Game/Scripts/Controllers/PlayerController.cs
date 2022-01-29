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

		PlayerModel playerModel;

		void Awake()
		{
			playerModel = ModelFactory.Create<PlayerModel>();
			playerModel.defaultSkipDurationSeconds = defaultSkipDurationSeconds;
		}

		void Start()
		{
			playerModel.SkipTime = (source, duration) =>
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
		}
	}
}

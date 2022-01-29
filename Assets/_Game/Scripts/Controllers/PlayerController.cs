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
				Physics.autoSimulation = false;

				// Skip forward in time
				float totalTimePassed = 0f;
				while (totalTimePassed < duration)
				{
					Physics.Simulate(Time.fixedDeltaTime);
					totalTimePassed += Time.fixedDeltaTime;
				}

				Physics.autoSimulation = true;
			};
		}
	}
}

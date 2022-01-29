using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames.MVC;

namespace GGJ2022
{
	public class PlayerModel : Model
	{
		public float defaultSkipDurationSeconds = 0.5f;

		public Controller.Action<float> SkipTime;
	}
}

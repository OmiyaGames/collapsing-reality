using UnityEngine;
using UnityEngine.InputSystem;
using OmiyaGames.MVC;

namespace GGJ2022
{
	public class SkipTime : MonoBehaviour
	{
		PlayerModel playerModel;
		void Start()
		{
			playerModel = ModelFactory.Get<PlayerModel>();
		}

		public void OnSkip(InputValue value)
		{
			if (value.isPressed)
			{
				playerModel.SkipTime?.Invoke(this, playerModel.defaultSkipDurationSeconds);
			}
		}
	}
}

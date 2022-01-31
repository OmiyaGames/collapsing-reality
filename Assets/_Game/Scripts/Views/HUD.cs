using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames.MVC;

namespace GGJ2022
{
	public class HUD : MonoBehaviour
	{
		[SerializeField]
		TMPro.TextMeshProUGUI timeLeft;
		[SerializeField]
		TMPro.TextMeshProUGUI score;

		PlayerModel playerModel;
		void Start()
		{
			playerModel = ModelFactory.Get<PlayerModel>();
		}

		// Update is called once per frame
		void Update()
		{
			float displayTime = playerModel.gameDuration - playerModel.GetTimePassed(this);
			timeLeft.text = displayTime.ToString("0");
			score.text = playerModel.score.Value.ToString();
		}
	}
}

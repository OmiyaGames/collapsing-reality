using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames;
using OmiyaGames.MVC;

namespace GGJ2022
{
	public class WebCamModel : Model
	{
		public Trackable<string> DeviceName = new Trackable<string>();
	}
}

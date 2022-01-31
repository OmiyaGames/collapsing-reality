using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2022
{
	public class AskWebCamPermission : MonoBehaviour
	{
		IEnumerator Start()
		{
			yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
			if (Application.HasUserAuthorization(UserAuthorization.WebCam))
			{
				Debug.Log("Permission granted for using webcam");
				WebCam.SetupWebCam();
			}
			else
			{
				Debug.Log("webcam not found");
			}
		}
	}
}

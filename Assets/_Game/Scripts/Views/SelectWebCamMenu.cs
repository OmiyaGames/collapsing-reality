using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames.Menus;
using OmiyaGames.MVC;
using OmiyaGames.Global;
using OmiyaGames.Scenes;
using UnityEngine.UI;

namespace GGJ2022
{
	public class SelectWebCamMenu : IMenu
	{
		[SerializeField]
		Button startButton;
		[SerializeField]
		GameObject noWebCamFoundLabel;
		[SerializeField]
		WebCamButton camButton;

		WebCamModel webCamModel;
		
		public override Type MenuType => Type.DefaultManagedMenu;
		public override Selectable DefaultUi => startButton;

		void Start()
		{
			webCamModel = ModelFactory.Get<WebCamModel>();

			// Get a list of webcams
			List<UiEventNavigation> scrollableButtons = new List<UiEventNavigation>();
			if (WebCamTexture.devices.Length == 0)
			{
				noWebCamFoundLabel.SetActive(true);
			}
			else
			{
				noWebCamFoundLabel.SetActive(false);

				foreach(var device in WebCamTexture.devices)
				{
					// Clone the webcam button
					WebCamButton clone = Instantiate(camButton, camButton.transform.parent);
					string deviceName = device.name;
					clone.name = deviceName + " Button";
					clone.label.text = deviceName;
					clone.toggle.isOn = (deviceName == webCamModel.DeviceName.Value);
					clone.toggle.onValueChanged.AddListener(isOn =>
					{
						if(isOn)
						{
							webCamModel.DeviceName.Value = deviceName;
						}
					});

					// Add button to list
					scrollableButtons.Add(clone.navigator);
				}
			}

			// Disable original webcam button
			camButton.gameObject.SetActive(false);
			Navigator.UiElementsInScrollable = scrollableButtons.ToArray();
		}

		public void OnStartGameClicked()
		{
			Singleton.Get<SceneTransitionManager>().LoadNextLevel();
			Hide();
		}
	}
}

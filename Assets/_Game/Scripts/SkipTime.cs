using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkipTime : MonoBehaviour
{
	[SerializeField]
	float skipTime = 0.5f;

	public void OnSkip(InputValue value)
	{
		if(value.isPressed)
		{
			Physics.autoSimulation = false;

			// Skip forward in time
			float totalTimePassed = 0f;
			while(totalTimePassed < skipTime)
			{
				Physics.Simulate(Time.fixedDeltaTime);
				totalTimePassed += Time.fixedDeltaTime;
			}

			Physics.autoSimulation = true;
		}
	}
}

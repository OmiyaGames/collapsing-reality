using UnityEngine;
using OmiyaGames;
using OmiyaGames.MVC;

namespace GGJ2022
{
	public class QuantumTrigger : MonoBehaviour
	{
		const string GLITCH = "_Glitch";
		const float STABLE_THRESHOLD = 0.1f;

		[SerializeField]
		Renderer mesh;
		[SerializeField]
		float lerpSpeed = 5f;
		[SerializeField]
		Collider focusCollider;
		[SerializeField]
		Rigidbody body;

		[Header("Destroying")]
		[SerializeField]
		GameObject destroyObject;
		[SerializeField]
		float destroyAfterSeconds = 0.25f;

		[Header("Debug")]
		[SerializeField, ReadOnly]
		bool isFocused = false;
		[SerializeField, ReadOnly, Range(0f, 1f)]
		float currentGlitchiness = 1;

		float timeLastStable = -1f;
		PlayerModel playerModel;

		public float Glitchiness => currentGlitchiness;
		public GameObject GameObject => destroyObject;
		public Collider FocusCollider => focusCollider;
		public Rigidbody Body => Helpers.GetComponentCached(this, ref body);
		public bool IsFocused
		{
			get => isFocused;
			set => isFocused = value;
		}
		public Color Color
		{
			set => mesh.material.color = value;
		}

		void Start()
		{
			// Setup PlayerModel
			playerModel = ModelFactory.Get<PlayerModel>();

			// Set to maximum glitchiness
			mesh.material.SetFloat(GLITCH, currentGlitchiness);
		}

		void Update()
		{
			// Set the glitchiness value
			float targetGlitchiness = IsFocused ? 0 : 1;
			currentGlitchiness = Mathf.Lerp(currentGlitchiness, targetGlitchiness, (Time.deltaTime * lerpSpeed));
			mesh.material.SetFloat(GLITCH, currentGlitchiness);

			// Check how long this has been stable
			if (IsFocused == false)
			{
				timeLastStable = -1f;
			}
			else if (currentGlitchiness < STABLE_THRESHOLD)
			{
				if(timeLastStable < 0)
				{
					timeLastStable = Time.time;
				}
				else if((Time.time - timeLastStable) > destroyAfterSeconds)
				{
					playerModel.DestroyTrigger?.Invoke(this, this);
				}
			}
		}

		void OnDestroy()
		{
			// Clean up the material generated from this script
			Destroy(mesh.material);
		}
	}
}

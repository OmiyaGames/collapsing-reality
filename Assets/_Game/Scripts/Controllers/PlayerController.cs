using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using OmiyaGames.MVC;
using OmiyaGames.Global;

namespace GGJ2022
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField]
		float defaultSkipDurationSeconds = 0.5f;
		[SerializeField]
		float raycastDistance = 20f;
		[SerializeField]
		LayerMask raycastMask;

		[Header("Objective")]
		[SerializeField]
		int maxNumCollectables = 50;
		[SerializeField]
		QuantumTrigger collectable;
		[SerializeField]
		Transform minPositionRange;
		[SerializeField]
		Transform maxPositionRange;
		[SerializeField]
		int maxVelocity = 5;
		[SerializeField]
		int maxTorque = 5;
		[SerializeField]
		Vector2 scaleRange = new Vector2(1, 2);
		[SerializeField]
		OmiyaGames.HsvColor collectableColor;

		[Header("Time")]
		[SerializeField]
		float startTimer = 3;
		[SerializeField]
		float endGameAfterSeconds = 120;
		[SerializeField]
		AnimationCurve spawnCurve;

		PlayerModel playerModel;
		QuantumTrigger leftEyeTrigger = null, rightEyeTrigger = null;
		float timeStart = -1f, lastSpawned = -1f, spawnDelay = 1f;

		void Awake()
		{
			playerModel = ModelFactory.Create<PlayerModel>();
			playerModel.defaultSkipDurationSeconds = defaultSkipDurationSeconds;
			playerModel.raycastDistance = raycastDistance;
			playerModel.raycastMask = raycastMask;
			playerModel.triggerPool = new ObjectPool<QuantumTrigger>(CreateTrigger, GetTrigger, ReleaseTrigger, DestroyTrigger, maxSize: maxNumCollectables);
		}

		void Start()
		{
			playerModel.SkipTime += (source, duration) =>
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

			playerModel.RaycastFromEyes += (source) =>
			{
				RaycastHit hit;

				// Ray-trace from each eye
				leftEyeTrigger = GetTrigger(playerModel.leftEyeTransform, leftEyeTrigger);
				rightEyeTrigger = GetTrigger(playerModel.rightEyeTransform, rightEyeTrigger);

				QuantumTrigger GetTrigger(Transform eyeTransform, QuantumTrigger lastTrigger)
				{
					QuantumTrigger returnTrigger = null;

					// Make sure transforms are set
					if (eyeTransform != null)
					{
						// Reset the left eye trigger
						if (lastTrigger != null)
						{
							lastTrigger.IsFocused = false;
						}

						// Raycast from eye
						if (Physics.Raycast(eyeTransform.position, eyeTransform.forward, out hit, playerModel.raycastDistance, playerModel.raycastMask) && playerModel.colliderToTriggerMap.TryGetValue(hit.collider, out returnTrigger))
						{
							returnTrigger.IsFocused = true;
						}
					}
					return returnTrigger;
				}
			};

			playerModel.CreateTrigger += (source) =>
			{
				QuantumTrigger trigger = playerModel.triggerPool.Get();

				// Setup scale
				trigger.transform.localScale = Vector3.one * Random.Range(scaleRange.x, scaleRange.y);

				// Setup physics
				trigger.Body.MovePosition(new Vector3(
					Random.Range(minPositionRange.position.x, maxPositionRange.position.x),
					Random.Range(minPositionRange.position.y, maxPositionRange.position.y),
					Random.Range(minPositionRange.position.z, maxPositionRange.position.z)
					));
				trigger.Body.velocity = Random.insideUnitSphere * maxVelocity;
				trigger.Body.angularVelocity = Random.insideUnitSphere * maxTorque;

				// Setup color
				collectableColor.Hue = Random.value;
				trigger.Color = collectableColor.ToColor();

				// Setup trigger
				playerModel.colliderToTriggerMap.Add(new SerializableDictionary<Collider, QuantumTrigger>.Pair(trigger.FocusCollider, trigger));
				return trigger;
			};

			playerModel.DestroyTrigger += (source, trigger) =>
			{
				playerModel.triggerPool.Release(trigger);
				playerModel.colliderToTriggerMap.Remove(trigger.FocusCollider);
			};

			playerModel.GetTimePassed += (source) => timeStart > 0f ? (Time.time - timeStart) : 0f;
			playerModel.GetMode += (source) =>
			{
				float timePassed = playerModel.GetTimePassed(this);
				if (Mathf.Approximately(timePassed, 0f))
				{
					return PlayerModel.Mode.Starting;
				}
				else if (timePassed > endGameAfterSeconds)
				{
					return PlayerModel.Mode.Done;
				}
				else
				{
					return PlayerModel.Mode.Playing;
				}
			};

			// Start delay
			StartCoroutine(DelayStart());
		}

		void Update()
		{
			// Make sure we're playing
			PlayerModel.Mode mode = playerModel.GetMode(this);
			if (mode == PlayerModel.Mode.Playing)
			{
				// Raycast from eyes
				playerModel.RaycastFromEyes?.Invoke(this);

				// Check when to spawn a new collectable
				if ((lastSpawned < 0) || ((Time.time - lastSpawned) > spawnDelay))
				{
					// Make sure there isn't too many triggers in the scene
					if(playerModel.colliderToTriggerMap.Count >= maxNumCollectables)
					{
						return;
					}

					// Spawn a collectable
					playerModel.CreateTrigger(this);

					// Delay the next spawn
					spawnDelay = spawnCurve.Evaluate(playerModel.GetTimePassed(this));
					lastSpawned = Time.time;
				}
			}
			else if(mode == PlayerModel.Mode.Done)
			{
				OmiyaGames.SIn
			}
		}

		void OnDrawGizmos()
		{
			if((minPositionRange != null) && (maxPositionRange != null))
			{
				Gizmos.color = Color.yellow;
				Vector3 center = (minPositionRange.position + maxPositionRange.position) / 2f;
				Vector3 size = minPositionRange.position - maxPositionRange.position;
				size.x = Mathf.Abs(size.x);
				size.y = Mathf.Abs(size.y);
				size.z = Mathf.Abs(size.z);
				Gizmos.DrawWireCube(center, size);
			}
		}

		IEnumerator DelayStart()
		{
			yield return new WaitForSeconds(startTimer);
			timeStart = Time.time;
			lastSpawned = -1;
		}

		#region Collectables
		private void DestroyTrigger(QuantumTrigger obj)
		{
			Destroy(obj.GameObject);
		}

		private void ReleaseTrigger(QuantumTrigger obj)
		{
			obj.GameObject.SetActive(false);
		}

		private void GetTrigger(QuantumTrigger obj)
		{
			obj.GameObject.SetActive(true);
		}

		private QuantumTrigger CreateTrigger()
		{
			return Instantiate(collectable);
		}
		#endregion

	}
}

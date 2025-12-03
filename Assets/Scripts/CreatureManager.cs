using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreatureManager : MonoBehaviour
{
    // 1. Create a variable to hold the reference to your Generate script
    public Generate creatureGenerator;
	private Vector3[] accelerations, crawlerAccel;
	private Vector3[] velocities, crawlerVel;
	private GameObject[] boids, crawlers;
	public LayerMask obstacleLayerMask;
	private Quaternion baseRotation = Quaternion.Euler(0, 90f, 0);
	private float rotationSpeed = 10.0f;
	private float wCollision = 1000.0f, wCentering = 50.0f, wRepulsion = 200.0f, wVelMatching = 10.0f, wWander = 100.0f;
	public bool hasCentering = true, hasRepulsion = true, hasVelMatching = true, hasWander = true, trailsOn = true;
	public int boidCount = 50;
	private float centeringRadius = 50.0f, repulsionRadius = 10.0f;
	public int seed = 5;
	private float minVelocity = 5.0f, maxVelocity = 200.0f;
	private float maxAnimRate = 30.0f, minAnimeRate = 2.0f;
	private bool isScatter = false;
	void Start()
	{
		Random.InitState(seed);
		accelerations = new Vector3[100];
		velocities = new Vector3[100];
		boids = new GameObject[100];
		
		crawlerAccel = new Vector3[100];
		crawlerVel = new Vector3[100];
		crawlers = new GameObject[100];
		for (int i = 0; i < 100; i++)
		{
			accelerations[i] = Vector3.zero;
			velocities[i] = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized * Random.Range(minVelocity, maxVelocity);
			boids[i] = SpawnSingleCreature(true);
			boids[i].SetActive(false);

			crawlerAccel[i] = Vector3.zero;
			crawlerVel[i] = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)).normalized * Random.Range(minVelocity, maxVelocity);
			crawlers[i] = SpawnSingleCreature(false);
			crawlers[i].SetActive(false);
		}
	}

    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space) && !isScatter)
		{
			StartCoroutine(ApplyScatter(2.0f, 5.0f));
		}

		UpdateCreatures(boids, velocities, accelerations);
		UpdateCreatures(crawlers, crawlerVel, crawlerAccel, false);
    }

	void UpdateCreatures(
		GameObject[] creatureList,
		Vector3[] creatureVelocity,
		Vector3[] creatureAcceleration,
		bool canFly = true
	)
	{
		for (int i = 0; i < creatureList.Length; i++)
		{
			if (i < boidCount)
			{
				creatureList[i].SetActive(true);
				TrailRenderer trail = creatureList[i].GetComponent<TrailRenderer>();
				if (trailsOn)
				{
					trail.enabled = true;
				} else
				{
					trail.Clear();
				}
				// Calculate the forces
				// Weights between obstacles, collisions, centering and random direction
				// Weights are applied by the functions, internally
				Vector3 wanderForce = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));

				creatureAcceleration[i] = CalculateCollisionForce(creatureList, creatureVelocity, i)
					+ CalculateRadialForce(creatureList, creatureVelocity, i, centeringRadius, 7, false)
					+ CalculateRadialForce(creatureList, creatureVelocity, i, repulsionRadius, 50, true)
					+ wanderForce.normalized * wWander * Convert.ToInt32(hasWander);

				// Update velocities
				creatureVelocity[i] += creatureAcceleration[i] * Time.deltaTime;
				if (!canFly)
				{
					creatureVelocity[i].y = 0;
				}
				if (creatureVelocity[i].magnitude < minVelocity)
				{
					creatureVelocity[i] = creatureVelocity[i].normalized * minVelocity;
				} else if (creatureVelocity[i].magnitude > maxVelocity)
				{
					creatureVelocity[i] = creatureVelocity[i].normalized * maxVelocity;
				}
				// Change position of boids
				creatureList[i].transform.position += creatureVelocity[i] * Time.deltaTime;
				// Point in direction of travel
				Quaternion direction = Quaternion.LookRotation(creatureVelocity[i]);
				creatureList[i].transform.rotation = Quaternion.Slerp(creatureList[i].transform.rotation, direction * baseRotation, Time.deltaTime * rotationSpeed);

				// Update animation speeds
				float currentSpeed = creatureVelocity[i].magnitude;
				CreatureAnimations animRefs = creatureList[i].GetComponent<CreatureAnimations>();
				if (animRefs != null)
				{
					// Calculate the Normalized Speed (0 to 1)
					float normalizedSpeed = Mathf.Clamp01(currentSpeed / maxVelocity);

					float targetAnimRate = Mathf.Lerp(minAnimeRate, maxAnimRate, normalizedSpeed);
					
					// Wings
					if (animRefs.wingScripts != null)
					{
						foreach (var wing in animRefs.wingScripts)
							if (wing != null) wing.xSpeed = targetAnimRate;
					}

					// Legs
					if (animRefs.legScripts != null)
					{
						float legSpeed = targetAnimRate * 0.5f; 
						
						foreach (var leg in animRefs.legScripts)
							if (leg != null) leg.xSpeed = legSpeed;
					}
				}
			}
			else
			{
				creatureList[i].SetActive(false);
			}
		}
	}

	IEnumerator ApplyScatter(float duration, float multiplier)
	{
		isScatter = true;
		float originalRepulsionRadius = repulsionRadius;

		repulsionRadius = originalRepulsionRadius * multiplier;
		yield return new WaitForSeconds(duration);

		repulsionRadius = originalRepulsionRadius;
		isScatter = false;
	}

	public void UpdateBoidCount(float count)
	{
		boidCount = (int)count;
	}

	Vector3 CalculateCollisionForce(
		GameObject[] creatureList,
		Vector3[] creatureVelocity,
		int creatureIndex,
		float safetyFactor = 3.0f
	)
	{
		Vector3 boidPos = creatureList[creatureIndex].transform.position;
		Vector3 boidDirection = creatureVelocity[creatureIndex].normalized;
		float speed = creatureVelocity[creatureIndex].magnitude;
		float safeDistance = speed * safetyFactor;

		if (speed > 0.01f)
		{
			RaycastHit hit;
			if (Physics.Raycast(boidPos, boidDirection, out hit, safeDistance, obstacleLayerMask)) {
				// Debug.Log("Boid will collide!");
				// Apply repulsive force normal to obstacle
				float closenessRatio = 1.0f - hit.distance / safeDistance;
				float strength = closenessRatio * closenessRatio;
				return hit.normal * strength * wCollision;
			}
		}
		return new Vector3(0,0,0);
	}

	Vector3 CalculateRadialForce(
		GameObject[] creatureList,
		Vector3[] creatureVelocity,
		int creatureIndex, float radius, int k, bool isRepulsion, bool hasVelocityMatching = true)
	{
		List<Vector2> distances = new List<Vector2>();

		for (int i = 0; i < creatureList.Length; i++)
		{
			// Calculate the distance with all other, active boids
			if (i != creatureIndex && creatureList[i].activeSelf)
			{
				float currDist = (creatureList[i].transform.position - creatureList[creatureIndex].transform.position).magnitude;
				if (currDist < radius)
				{
					distances.Add(new Vector2(currDist, i));	
				}
			}
		}

		// Sort distances and use weighted sum of closest k boids
		distances.Sort((a, b) => a.x.CompareTo(b.x));
		Vector3 force = Vector3.zero;
		Vector3 averageVelocity = Vector3.zero;
		float totalWeight = 0.0f;

		k = Mathf.Min(k, distances.Count);
		for (int j = 0; j < k; j++)
		{
			float currWeight = Mathf.Max(radius - distances[j][0], 0);
			totalWeight += currWeight;

			int neighborIndex = (int)distances[j].y;
			if (isRepulsion)
			{
				force += currWeight * (creatureList[creatureIndex].transform.position - creatureList[neighborIndex].transform.position);
			} else
			{
				force += currWeight * (creatureList[neighborIndex].transform.position - creatureList[creatureIndex].transform.position);
				averageVelocity += currWeight * (creatureVelocity[neighborIndex] - creatureVelocity[creatureIndex]);
			}
		}

		if (totalWeight < 0.01f)
		{
			return Vector3.zero;
		} else
		{
			if (isRepulsion)
			{
				return (force / totalWeight).normalized * wRepulsion * Convert.ToInt32(hasRepulsion);
			}
			else
			{
				return (force / totalWeight).normalized * wCentering * Convert.ToInt32(hasCentering)
					+ averageVelocity.normalized * wVelMatching * Convert.ToInt32(hasVelMatching);
			}
		}
	}

    GameObject SpawnSingleCreature(bool hasWings = true)
    {
        // 2. Safety check to ensure the reference exists
        if (creatureGenerator != null)
        {
            // Generate a random position
            Vector3 randomPos = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
            
            // 3. Call the public method from Generate.cs
            Debug.Log("Creature Spawned from external script!");
			return creatureGenerator.CreateCreature(randomPos, Vector3.zero, hasWings);
        }
        else
        {
            Debug.LogError("CreatureGenerator reference is missing!");
			return null;
        }
    }
}
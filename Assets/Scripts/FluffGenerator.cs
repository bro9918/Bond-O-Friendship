﻿using UnityEngine;
using System.Collections;

public class FluffGenerator : MonoBehaviour {

	public GameObject fluffPrefab;
	public Material greenFluffMaterial;
	//public float fluffSound;
	public float spawnRate = 3.0f;
	public float minimumVelocity = 3.0f;
	public float maximumVelocity = 10.0f;

	private float spawnTimer;
	private int colorPicker;
	private GameObject fluff;
	private float velocity;
	private Vector3 targetAngle;

	// Use this for initialization
	void Start () {
		spawnTimer = spawnRate;
	}
	
	// Update is called once per frame
	void Update () {
		spawnTimer -= Time.deltaTime;

		if(spawnTimer <= 0)
		{
			GenerateFluff();
			spawnTimer = spawnRate;
		}
	}

	void GenerateFluff () {
		fluff = (GameObject)Instantiate(fluffPrefab);
		colorPicker = Random.Range(0, 2);
		if(colorPicker == 1)
		{
			MeshRenderer[] meshRenderers = fluff.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < meshRenderers.Length; i++)
			{
				meshRenderers[i].material = greenFluffMaterial;
			}
		}
		fluff.transform.position = transform.position;
		fluff.transform.parent = gameObject.transform;
		MovePulse movePulse = fluff.GetComponent<MovePulse>();
		movePulse.ReadyForPass();
		velocity = Random.Range(minimumVelocity, maximumVelocity);
		if(Random.Range(0,2) == 1)
			targetAngle = Quaternion.Euler(0, 0, Random.Range(60, 211)) * new Vector3(velocity,velocity,0);
		else
			targetAngle = Quaternion.Euler(0, 0, Random.Range(-120, 31)) * new Vector3(velocity,velocity,0);
		movePulse.target = transform.position + targetAngle;
	}
}

﻿using UnityEngine;
using System.Collections;

public class CameraSplitter : MonoBehaviour {
	private static CameraSplitter instance;
	public static CameraSplitter Instance
	{
		get
		{
 			if (instance == null)
			{
				instance = GameObject.FindGameObjectWithTag("CameraSystem").GetComponent<CameraSplitter>();
			}
			return instance;
		}
	}
	public bool split = false;
	private bool wasSplit = false;
	public float splitDistance;
	public float combineDistance;
	public GameObject combinedCameraSystem;
	public GameObject player1CameraSystem;
	public GameObject player2CameraSystem;
	public GameObject player1;
	public GameObject player2;
	private bool justAltered;

	void Start()
	{
		combinedCameraSystem.transform.position = (player1.transform.position + player2.transform.position) / 2;
		player1CameraSystem.transform.position = player1.transform.position;
		player2CameraSystem.transform.position = player2.transform.position;
		wasSplit = split;
		CheckSplit(true);
	}

	void Update()
	{
		CheckSplit(false);
	}

	private void CheckSplit(bool forceCheck)
	{
		Vector3 testPlayerOne;
		Vector3 testPlayerTwo;
		float splitUpperBound = 0.9f;
		float splitLowerBound = 0.1f;
		float combineUpperBound = 0.85f;
		float combineLowerBound = 0.15f;

		if (!split)
		{
			testPlayerOne = combinedCameraSystem.GetComponent<CameraFollow>().childMainCamera.WorldToViewportPoint(player1.transform.position);
			testPlayerTwo = combinedCameraSystem.GetComponent<CameraFollow>().childMainCamera.WorldToViewportPoint(player2.transform.position);
		}
		else
		{
			testPlayerOne = player2CameraSystem.GetComponent<CameraFollow>().childMainCamera.WorldToViewportPoint(player1.transform.position);
			testPlayerTwo = player1CameraSystem.GetComponent<CameraFollow>().childMainCamera.WorldToViewportPoint(player2.transform.position);
		}

		if ((testPlayerTwo.x > combineLowerBound && testPlayerTwo.x < combineUpperBound && testPlayerTwo.y > combineLowerBound && testPlayerTwo.y < combineUpperBound))
		{
			split = false;
		}
		else if (testPlayerOne.x < splitLowerBound || testPlayerOne.x > splitUpperBound || testPlayerOne.y < splitLowerBound || testPlayerOne.y > splitUpperBound || testPlayerTwo.x < splitLowerBound || testPlayerTwo.x > splitUpperBound || testPlayerTwo.y < splitLowerBound || testPlayerTwo.y > splitUpperBound)
		{
			split = true;
		}

		if (split != wasSplit || forceCheck)
		{
			for (int i = 0; i < combinedCameraSystem.transform.childCount; i++)
			{
				combinedCameraSystem.transform.GetChild(i).gameObject.SetActive(!split);
			}
			for (int i = 0; i < player1CameraSystem.transform.childCount; i++)
			{
				player1CameraSystem.transform.GetChild(i).gameObject.SetActive(split);
			}
			for (int i = 0; i < player2CameraSystem.transform.childCount; i++)
			{
				player2CameraSystem.transform.GetChild(i).gameObject.SetActive(split);
			}
		}

		wasSplit = split;
	}

	public Camera GetFollowingCamera(GameObject player)
	{
		if (split && (player == player1 || player == player2))
		{
			if (player == player1)
			{
				return player1CameraSystem.GetComponentInChildren<Camera>();
			}
			else
			{
				return player2CameraSystem.GetComponentInChildren<Camera>();
			}
		}
		else
		{
			return combinedCameraSystem.GetComponentInChildren<Camera>();
		}
	}
}

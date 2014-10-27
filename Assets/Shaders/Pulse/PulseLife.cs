﻿using UnityEngine;
using System.Collections;

public class PulseLife : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		Invoke("DestroyPulse", 2.0f);
	
	}

	void DestroyPulse()
	{
		Destroy(gameObject);
	}
	void OnTriggerEnter(Collider collide)
	{
		if(collide.gameObject.tag == "Object")
		{
			Destroy(gameObject);
		}

	}

}

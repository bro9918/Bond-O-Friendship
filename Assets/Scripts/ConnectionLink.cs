﻿using UnityEngine;
using System.Collections;

public class ConnectionLink : MonoBehaviour {
	public BoxCollider linkCollider = null;
	public Rigidbody body = null;
	public SpringJoint jointPrevious = null;
	public SpringJoint jointNext = null;
	public int orderLevel = 0;
}

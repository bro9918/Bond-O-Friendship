﻿using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
	public SimpleMover mover;
	public PartnerLink partnerLink;
	protected Collider tailTrigger;
	public enum Player{Player1, Player2};

	public enum JoyStick{Joy1, Joy2, Joy3, Joy4};

	public Player playerNumber;
	public JoyStick joystickNumber;

	public ParticleSystem absorbPrefab;

	public bool useKeyboard = false;

	public PlayerInput otherPlayerInput;

	public GameObject geometry;
	public float deadZone = .75f;

	private bool firePulse = true;
	private Vector3 velocityChange;
	public float basePulsePower = 10;
	public float timedPulsePower = 10;
	public float basePulseDrain = 0.1f;
	public float timedPulseDrain = 0.1f;


	public bool swapJoysticks = false;

	private ParticleSystem absorb;
	private Vector3 target;
	public float absorbStrength = 5;
	public Vector3 desiredLook;
	public bool joystickDetermined = false;

	private bool paused = false;

	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}

		var gamepads = Input.GetJoystickNames();
		useKeyboard = (gamepads.Length == 1 && playerNumber == Player.Player1) || gamepads.Length > 1 ? false : true;

		if(!useKeyboard && !joystickDetermined)
		{
			if(playerNumber == Player.Player1)
			{
				if(Input.GetButtonDown("Joy1Absorb"))
				{
					joystickNumber = JoyStick.Joy1;
					joystickDetermined = true;
				}
				if(Input.GetButtonDown("Joy2Absorb"))
				{
					joystickNumber = JoyStick.Joy2;
					joystickDetermined = true;
				}
				if(Input.GetButtonDown("Joy3Absorb"))
				{
					joystickNumber = JoyStick.Joy3;
					joystickDetermined = true;
				}
				if(Input.GetButtonDown("Joy4Absorb"))
				{
					joystickNumber = JoyStick.Joy4;
					joystickDetermined = true;
				}

				//Debug.Log(joystickNumber.ToString());
			}
			else if(otherPlayerInput != null && otherPlayerInput.joystickDetermined)
			{
				if(Input.GetButtonDown("Joy1Absorb") && otherPlayerInput.joystickNumber != JoyStick.Joy1)
				{
					joystickNumber = JoyStick.Joy1;
					joystickDetermined = true;
				}
				if(Input.GetButtonDown("Joy2Absorb") && otherPlayerInput.joystickNumber != JoyStick.Joy2)
				{
					joystickNumber = JoyStick.Joy2;
					joystickDetermined = true;
				}
				if(Input.GetButtonDown("Joy3Absorb")&& otherPlayerInput.joystickNumber != JoyStick.Joy3)
				{
					joystickNumber = JoyStick.Joy3;
					joystickDetermined = true;
				}
				if(Input.GetButtonDown("Joy4Absorb")&& otherPlayerInput.joystickNumber != JoyStick.Joy4)
				{
					joystickNumber = JoyStick.Joy4;
					joystickDetermined = true;
				}
			}
		}


		if(useKeyboard || joystickDetermined)
		{
			if(GetPause() || Input.GetKeyDown(KeyCode.Escape))
			{
				if(paused)
					Time.timeScale = 1;
				else
					Time.timeScale = 0;

				paused = !paused;
			}
		
			if(!paused)
			{
				velocityChange = !useKeyboard ? PlayerJoystickMovement() : Vector3.zero;
				// Movement
				if(useKeyboard)
				{
					if ((playerNumber == Player.Player1 && Input.GetKey("w")) || (playerNumber == Player.Player2 && Input.GetKey(KeyCode.UpArrow)))
					{
						velocityChange += Vector3.up;
					}
					if ((playerNumber == Player.Player1 && Input.GetKey("a")) || (playerNumber == Player.Player2 && Input.GetKey(KeyCode.LeftArrow)))
					{
						velocityChange -= Vector3.right;
					}
					if ((playerNumber == Player.Player1 && Input.GetKey("s")) || (playerNumber == Player.Player2 && Input.GetKey(KeyCode.DownArrow)))
					{
						velocityChange -= Vector3.up;
					}
					if ((playerNumber == Player.Player1 && Input.GetKey("d")) || (playerNumber == Player.Player2 && Input.GetKey(KeyCode.RightArrow)))
					{
						velocityChange += Vector3.right;
					}
				
				
				}

				// Turn towards velocity change.
				if (velocityChange.sqrMagnitude > 0)
				{
					mover.Accelerate(velocityChange);
				}
				else
				{
					mover.SlowDown();
				}
				transform.LookAt(transform.position + mover.velocity, transform.up);

				PlayerLookAt();
				partnerLink.absorbing = Absorbing();

				if(absorb != null)
				{
					absorb.transform.position = transform.position;
				}
			}
		}
	}

	private Vector3 PlayerJoystickMovement()
	{
		Vector2 leftStickInput = new Vector2(GetAxisMoveHorizontal(), GetAxisMoveVertical());
		return leftStickInput.sqrMagnitude > Mathf.Pow(deadZone, 2f) ? new Vector3(GetAxisMoveHorizontal(),GetAxisMoveVertical(),0) : Vector3.zero;
	}

	bool Absorbing()
	{
		if ((!useKeyboard && GetAbsorb()) || (useKeyboard && playerNumber == Player.Player1 && (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))) || (useKeyboard && playerNumber == Player.Player2 && (Input.GetKey(KeyCode.Keypad0) || Input.GetMouseButton(1))))
		{
			if(absorb == null)
			{
				absorb = (ParticleSystem)Instantiate(absorbPrefab);
				absorb.transform.position = transform.position;
				absorb.startColor = GetComponent<PartnerLink>().headRenderer.material.color / 2;
				absorb.startColor = new Color(absorb.startColor.r, absorb.startColor.g, absorb.startColor.b, 0.1f);
			}
			GameObject[] pulseArray = GameObject.FindGameObjectsWithTag("Pulse");
			foreach(GameObject livePulse in pulseArray)
			{
				MovePulse livePulseMove = livePulse.GetComponent<MovePulse>();
				if (livePulseMove != null && Vector3.SqrMagnitude(livePulseMove.transform.position - transform.position) < Mathf.Pow(absorbStrength, 2))
				{
					livePulseMove.target = transform.position;
					livePulseMove.moving = true;
					if (livePulseMove.swayAnimation != null)
					{
						livePulseMove.swayAnimation.enabled = false;
					}
				}
			}
			return true;
		}
		else if(absorb != null)
		{
			absorb.startColor = Color.Lerp(absorb.startColor, new Color(0, 0, 0, 0), 0.5f);
			Destroy(absorb.gameObject, 1.0f);
		}
		return false;
	}

	void PlayerLookAt()
	{
		Vector2 lookAt = FireDirection();		
		float minToFire = useKeyboard ? 0 : deadZone;

		if(lookAt.sqrMagnitude > Mathf.Pow(minToFire, 2f))
		{
			lookAt.Normalize();

			if(firePulse)
			{
				Vector3 target = transform.position + new Vector3(lookAt.x, lookAt.y, 0);
				Vector3 pulseDirection = new Vector3(lookAt.x, lookAt.y, 0);
				Vector3 velocityBoost = Vector3.zero;

				if (Vector3.Dot(mover.velocity, pulseDirection) > 0)
				{
					velocityBoost += mover.velocity;
				}
			
				if (CanFire(basePulseDrain))
				{
					pulseDirection *= basePulsePower;
					partnerLink.pulseShot.Shoot(transform.position + velocityBoost + pulseDirection, basePulseDrain);
				}
				firePulse = false;
			}
		}
		else
		{
			firePulse = true;
		}
	}

	bool CanFire(float costToFire)
	{
		return transform.localScale.x - costToFire >= partnerLink.minScale;
	}
	

	
	#region Helper Methods
	
	private float GetAxisMoveHorizontal(){if(!swapJoysticks)return Input.GetAxis(joystickNumber.ToString() + "MoveHorizontal"); else return Input.GetAxis(joystickNumber.ToString() +"ThrowHorizontal");}
	private float GetAxisMoveVertical(){if(!swapJoysticks)return Input.GetAxis(joystickNumber.ToString() +"MoveVertical"); else return Input.GetAxis(joystickNumber.ToString() +"ThrowVertical");}
	private float GetAxisAimHorizontal(){if(!swapJoysticks)return Input.GetAxis(joystickNumber.ToString() +"ThrowHorizontal"); else return Input.GetAxis(joystickNumber.ToString() + "MoveHorizontal");}
	private float GetAxisAimVertical(){if(!swapJoysticks)return Input.GetAxis(joystickNumber.ToString() +"ThrowVertical"); else return Input.GetAxis(joystickNumber.ToString() +"MoveVertical");}
	private bool GetAbsorb() { return Input.GetButton(joystickNumber.ToString() + "Absorb");}
	private bool GetPause() { return Input.GetButtonDown(joystickNumber.ToString() + "Pause");}


	private Vector2 FireDirection()
	{
		Vector2 lookAt = Vector2.zero; 
		if (!useKeyboard)
		{
			lookAt = new Vector2(GetAxisAimHorizontal(), GetAxisAimVertical());
		}
		else
		{
			if ((playerNumber == Player.Player1 && Input.GetMouseButtonUp(0)) || (playerNumber == Player.Player2 && Input.GetMouseButtonUp(1)))
			{
				Vector3 mousePos = Input.mousePosition;
				mousePos.z = transform.position.z;
				mousePos = CameraSplitter.Instance.GetFollowingCamera(gameObject).ScreenToWorldPoint(mousePos);
				lookAt = mousePos - transform.position;
				lookAt.Normalize();
			}
			else if ((playerNumber == Player.Player1 && Input.GetKeyUp(KeyCode.Space)) || (playerNumber == Player.Player2 && Input.GetKeyUp("[0]")))
			{
				lookAt = geometry.transform.forward;
			}
		}

		return lookAt;
	}

	#endregion



}

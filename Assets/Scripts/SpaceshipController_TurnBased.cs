using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

public class SpaceshipController_TurnBased : NetworkBehaviour {

    public float movementSpeed, turnSpeed, maxDistance;
    public bool canMove;
    public GameObject energyBarPrefab;
    public float energyBarPosition;

    float distanceMoved;
    float prevDistanceMoved;
    Vector3 lastPosition;
    Rigidbody2D localRigidBody;
    Joystick joystick;
    public GameObject energyBar;

    void Start()
    {
        energyBar = Instantiate(energyBarPrefab,
                                transform.position + Vector3.up * energyBarPosition,
                                Quaternion.identity);

        distanceMoved = 0f;
        prevDistanceMoved = 0f;
        lastPosition = transform.position;

        localRigidBody = GetComponent<Rigidbody2D>();
        joystick = FindObjectOfType<Joystick>();
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;
        
        distanceMoved += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;
        if (distanceMoved >= maxDistance)
        {
            canMove = false;
            distanceMoved = 0f;
        }
    }

    public void EnableMoving()
    {
        distanceMoved = 0f;
        prevDistanceMoved = 0f;
        canMove = true;
        CmdUpdateEnergy(1);
    }

    void FixedUpdate()
    {
        energyBar.transform.position = transform.position + Vector3.up * energyBarPosition;

        if (!isLocalPlayer)
            return;

        if (!canMove)
            return;

        // Move rotation
        bool isTurningLeft = CrossPlatformInputManager.GetButton("TurnLeft");
        float turnLeftValue = turnSpeed * (isTurningLeft ? 1 : 0);
        bool isTurningRight = CrossPlatformInputManager.GetButton("TurnRight");
        float turnRightValue = turnSpeed * (isTurningRight ? 1 : 0);
        localRigidBody.MoveRotation(localRigidBody.rotation + turnLeftValue - turnRightValue);

        if (Mathf.Abs(distanceMoved - prevDistanceMoved) / maxDistance > 0.03f)
        {
            CmdUpdateEnergy(1f - distanceMoved / maxDistance);
            prevDistanceMoved = distanceMoved;
        }

        // Move position
        Vector2 direction = new Vector2(joystick.Horizontal, joystick.Vertical).normalized;
        localRigidBody.velocity = direction * movementSpeed;
    }

    public void BoostEnergy()
    {
        distanceMoved = 0f;
        prevDistanceMoved = 0f;
        CmdUpdateEnergy(1);
    }

    [Command]
    void CmdUpdateEnergy(float amount)
    {
        RpcUpdateEnergy(amount);
    }

    [ClientRpc]
    void RpcUpdateEnergy(float amount)
    {
        energyBar.GetComponentInChildren<Image>().fillAmount = amount;
    }
}

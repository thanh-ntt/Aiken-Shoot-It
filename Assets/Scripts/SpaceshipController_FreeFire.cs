using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

public class SpaceshipController_FreeFire : NetworkBehaviour {

    public float movementSpeed, boostMultiplier;
    public bool canMove;
    public GameObject energyBarPrefab;
    public float energyBarPosition;
    public float energyConsumeRate;
    public float energyRegenRate;

    Rigidbody2D localRigidBody;
    Joystick joystick;
    Joystick directionJoystick;
    public GameObject energyBar;
    float remainingEnergy;
    float previousEnergy;
    Transform mainCamera;

    void Start()
    {
        canMove = true;
        localRigidBody = GetComponent<Rigidbody2D>();
        joystick = GameObject.FindWithTag("Joystick").GetComponent<Joystick>();
        directionJoystick = GameObject.FindWithTag("DirectionalStick").GetComponent<Joystick>();
        energyBar = Instantiate(energyBarPrefab,
                                transform.position + Vector3.up * energyBarPosition,
                                Quaternion.identity);
        remainingEnergy = 100f;
        mainCamera = Camera.main.transform;
    }

    void FixedUpdate()
    {
        energyBar.transform.position = transform.position + Vector3.up * energyBarPosition;
        if (!isLocalPlayer)
            return;

        if (remainingEnergy <= 100)
            remainingEnergy += Time.deltaTime * energyRegenRate;

        if (Mathf.Abs(remainingEnergy - previousEnergy) > 3f)
        {
            previousEnergy = remainingEnergy;
            CmdUpdateEnergy(remainingEnergy / 100f);
        }

        if (!canMove)
        {
            localRigidBody.velocity = Vector2.zero;
            return;
        }

        // Move rotation (boost if BoostButton is pressed)
        /*
        bool isTurningLeft = CrossPlatformInputManager.GetButton("TurnLeft");
        float turnLeftValue = turnSpeed * (isTurningLeft ? 1 : 0);
        bool isTurningRight = CrossPlatformInputManager.GetButton("TurnRight");
        float turnRightValue = turnSpeed * (isTurningRight ? 1 : 0);
        localRigidBody.MoveRotation(localRigidBody.rotation + turnLeftValue - turnRightValue);
        */

        Vector2 shootDirection = new Vector2(directionJoystick.Horizontal, directionJoystick.Vertical).normalized;
        MoveDirection(shootDirection);

        // Moving the player using joystick
        bool isBoosting = CrossPlatformInputManager.GetButton("Boost");

        // Increase current speed if isBoosting
        float curSpeed = movementSpeed;
        if (remainingEnergy > 0 && isBoosting)
        {
            curSpeed *= boostMultiplier;
            remainingEnergy -= Time.deltaTime * energyConsumeRate;
        }

        if (CrossPlatformInputManager.GetButton("MapButton"))
        {
            Vector3 position = mainCamera.transform.position;
            position.x = 0f;
            position.y = 0f;
            mainCamera.transform.position = position;
            mainCamera.GetComponent<Camera>().orthographicSize = 10.6f;
            return;
        }

        MoveCamera();

        // Move position
        Vector2 direction = new Vector2(joystick.Horizontal, joystick.Vertical).normalized;
        localRigidBody.velocity = direction * curSpeed;
    }

    void MoveCamera()
    {
        mainCamera.GetComponent<Camera>().orthographicSize = 5f;
        float interpolation = 1.5f * Time.deltaTime;
        Vector3 position = mainCamera.transform.position;
        position.y = Mathf.Lerp(mainCamera.transform.position.y, transform.position.y, interpolation);
        position.x = Mathf.Lerp(mainCamera.transform.position.x, transform.position.x, interpolation);
        mainCamera.transform.position = position;
    }

    public static float Angle(Vector2 p_vector2)
    {
        return (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1) + 40f;
    }

    void MoveDirection(Vector2 shootDirection)
    {
        if (shootDirection.Equals(Vector2.zero))
            localRigidBody.MoveRotation(localRigidBody.rotation);
        else
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,0,transform.rotation.z + Angle(shootDirection)) , Time.deltaTime*4);           
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

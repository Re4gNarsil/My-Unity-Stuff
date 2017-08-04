using UnityEngine;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public class GroundCharacter : MonoBehaviour
{

    [Header("Check with Input Manager if you want to change any of these")]
    public string firstXAxis = "Horizontal";
    public string firstYAxis = "Vertical";
    public float ourGroundSpeed = 60, ourTurnSpeed = 30;
    public bool canReverse = true, canTurnInPlace = true, autoCorrectRotation = true, redirectMomentum = true;
	public float ourHealth = 100, weaponDamage = 10, objectDamage = 50, healRegen = 30, amountSubmerged = .1f, waterPressure = 10f;
    [Header("0 equals none; 1 is complete")]
    public float totalFriction = .1f;

    private Vector3 ourForce;
    private float ourMass, ourDrag, ourMomentum;
    private Rigidbody rigidBody;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        ourMass = rigidBody.mass;
        ourDrag = rigidBody.drag;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

	// Update is called every .02 seconds
	void FixedUpdate()
	{
		if (tag == "Player")
		{
			if (autoCorrectRotation)
			{
				float xAngle = Mathf.DeltaAngle(transform.rotation.eulerAngles.x, 0);   //this helps keep objects upright if desired
				float zAngle = Mathf.DeltaAngle(transform.rotation.eulerAngles.z, 0);
				if ((transform.rotation.eulerAngles.x < -.1f) || (transform.rotation.eulerAngles.x > .1f)) { CorrectXAngle(xAngle); }
				if ((transform.rotation.eulerAngles.z < -.1f) || (transform.rotation.eulerAngles.z > .1f)) { CorrectZAngle(zAngle); }
			}

			//the horizontal input determines side rotation, while vertical determines speed; 

			float ourValueX = CrossPlatformInputManager.GetAxisRaw(firstXAxis);
			float ourValueY = CrossPlatformInputManager.GetAxisRaw(firstYAxis);

			//check to see what conditions need to be met in order to rotate our gameObject, and then see if they're fulfilled

			if (ourValueX != 0)
			{
				MoveSideways(ourValueX);
			}
			//only apply a force if we are actively moving; otherwise we just 'glide' along

			if (ourValueY != 0)
			{
				MoveForwards(ourValueY);
			}
			if (rigidBody.velocity != Vector3.zero) { ApplyFriction(); }
		}
	}

    void CorrectXAngle(float xAngle)
    {
        float xCorrect = (xAngle < 0) ? -.1f : .1f;
        Vector3 ourRotation = transform.rotation.eulerAngles;
        Vector3 newRotation = new Vector3(ourRotation.x + xCorrect, ourRotation.y, ourRotation.z);
        transform.rotation = Quaternion.Euler(newRotation);
    }

    void CorrectZAngle(float zAngle)
    {
        float zCorrect = (zAngle < 0) ? -.1f : .1f;
        Vector3 ourRotation = transform.rotation.eulerAngles;
        Vector3 newRotation = new Vector3(ourRotation.x, ourRotation.y, ourRotation.z + zCorrect);
        transform.rotation = Quaternion.Euler(newRotation);
    }

	public void MoveSideways(float valueX)
	{
		if ((canTurnInPlace == true) || (rigidBody.velocity != Vector3.zero))
		{
			Vector3 ourVelocity = rigidBody.velocity;
			ourMomentum = Mathf.Sqrt((ourVelocity.x * ourVelocity.x) + (ourVelocity.z * ourVelocity.z));

			bool goingForward = true;
			if (Mathf.Abs(transform.forward.x) > .5f) { if ((transform.forward.x * ourVelocity.x) < 0) goingForward = false; }
			else if (Mathf.Abs(transform.forward.z) > .5f) { if ((transform.forward.z * ourVelocity.z) < 0) goingForward = false; }

			if (canReverse == true) { RotateOurShip(valueX, goingForward); }
			else if (goingForward == true) { RotateOurShip(valueX, goingForward); }

		}
	}

	public void MoveForwards(float valueY)
	{
		Rigidbody myRigidBody = GetComponent<Rigidbody>();
		ourForce = transform.forward * (ourGroundSpeed * ourMass * valueY) / ((1 * (1 - amountSubmerged)) + (amountSubmerged * waterPressure));
		if (myRigidBody.velocity.magnitude * ourMass < ourForce.magnitude)
		{
			if (canReverse)
			{
				rigidBody.AddForce(ourForce, ForceMode.Force);
			}
			else
			{
				if (valueY > 0) { rigidBody.AddForce(ourForce, ForceMode.Force); }
			}
		}
	}


    void RotateOurShip(float ourValueX, bool goingForward)
    {
        float sign = (ourValueX > 0) ? 1 : -1;
        if (!canTurnInPlace) { ourValueX = ((ourMomentum / ourGroundSpeed) * sign); } //how quickly you turn will be based on your current velocity
        Vector3 newRotation = transform.rotation.eulerAngles;
        newRotation.y += ((ourValueX * Time.deltaTime * ourTurnSpeed) / ((1 * (1 - amountSubmerged)) + (amountSubmerged * waterPressure)));
        transform.rotation = Quaternion.Euler(newRotation);
		if (redirectMomentum) { RedirectForce(goingForward); }    //this should enable us to turn with the proper momentum
    }

    void RedirectForce(bool goingForward)
    {
        //if we make a turn we cancel our previous momentum

        Vector3 ourVelocity = rigidBody.velocity;
        Vector3 reverseForce = (-(ourVelocity * ourMass * ourGroundSpeed) / ((1 * (1 - amountSubmerged)) + (amountSubmerged * waterPressure)));
        rigidBody.AddForce(reverseForce, ForceMode.Force);

        //and now reapply it in our new direction

        float levelForce = Mathf.Sqrt((ourVelocity.x * ourVelocity.x) + (ourVelocity.z * ourVelocity.z));
        float finalForce = Mathf.Sqrt((levelForce * levelForce) + (ourVelocity.y * ourVelocity.y));
        if (!goingForward) { finalForce = (finalForce * -1); }
        Vector3 newForce =(transform.forward * (finalForce * ourMass * ourGroundSpeed) / ((1 * (1 - amountSubmerged)) + (amountSubmerged * waterPressure)));
        rigidBody.AddForce(newForce, ForceMode.Force);
    }

	void ApplyFriction()
	{
		rigidBody.AddForce(-(rigidBody.velocity * ourMass * totalFriction), ForceMode.Force);
	}

    //all publicly exposed methods are here

    public void DestroyGameObject()
    {
        Destroy(gameObject);
        GameObject otherCamera = FindObjectOfType<Camera>().gameObject;
        otherCamera.AddComponent<AudioListener>();
    }

    public void RotateAround(Vector3 newRotation)
    {
        transform.rotation = Quaternion.Euler(newRotation);
    }

    public void FreezeUnfreeze(bool locked)
    {
        autoCorrectRotation = locked;
    }
    
    public Vector2 GetMassDrag()
    {
        return new Vector2(ourMass, ourDrag);
    }

    public void ChangeOurSpeed(float newSpeed)
    {
        ourGroundSpeed = newSpeed;
    }

    public void ChangeOurTurnSpeed(float newSpeed)
    {
        ourTurnSpeed = newSpeed;
    }
}

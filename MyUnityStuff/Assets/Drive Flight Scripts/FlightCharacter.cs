using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class FlightCharacter : MonoBehaviour {

    [Header("Check with Input Manager if you want to change either of these")]
    public string firstXAxis = "Horizontal";
    public string firstYAxis = "Vertical";
    [Header("Controls Forward, and Backward, speed; needs to be set up in Input Manager")]
    [Header("Speed, Roll, and/or Reset can be changed to an input already in Input Manager")]
    public string valueSpeed = "Speed";
    [Header("Controls whether you roll left/right as opposed to turning left/right;")]
	[Header("needs to be set up in Input Manager")]
    public string Roll = "Roll";
    public bool canReverse = true, canTurnInPlace = true, redirectMomentum = true;
    [Header("These are not hooked up to anything currently")]
    public float ourHealth = 100;
    public float weaponDamage = 10, objectDamage = 50, healRegen = 30, amountSubmerged = 0, waterPressure = 10f, ourAirSpeed = 50, ourTurnSpeed = 10;

    private float ourMass, ourDrag;
    private Rigidbody rigidBody;
    private Collider myCollider;
    private WhereIllBe whereIllBe;

    // Use this for initialization
    void Start()
    {
        gameObject.tag = "Player";
        rigidBody = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        whereIllBe = GetComponent<WhereIllBe>();
        if (whereIllBe && myCollider) { myCollider.isTrigger = true; }
        ourMass = rigidBody.mass;
        ourDrag = rigidBody.drag;
    }

    // Update is called once every .02 seconds
    void FixedUpdate() {

		//'Horizontal' and 'Vertical' controls operate control rotation from side to side and up and down, while 
		//separate roll and speed inputs control the remaining flight controls; the Mouse X/Y controls operate
		//the players camera.

		float valueX = 0, valueY = 0, speed = 0;
		if (tag == "Player")
		{
			valueX = CrossPlatformInputManager.GetAxisRaw(firstXAxis);
			valueY = CrossPlatformInputManager.GetAxisRaw(firstYAxis);
			speed = CrossPlatformInputManager.GetAxisRaw(valueSpeed);
		}

		//check to see what conditions need to be met in order to rotate our gameObject, and then see if they're fulfilled
		//plus check to see if we're going forwards or backwards, as we'll need to know when redirecting where we go

		Vector3 ourVelocity = rigidBody.velocity;
		bool goingForward = true;
		if (Mathf.Abs(transform.forward.x) > .5f) { if ((transform.forward.x * ourVelocity.x) < 0) goingForward = false; }
		else if (Mathf.Abs(transform.forward.z) > .5f) { if ((transform.forward.z * ourVelocity.z) < 0) goingForward = false; }

		if ((canTurnInPlace == true) && (valueX != 0) || (valueY != 0)) { RotateOurShip(valueX, valueY, goingForward); }
        else if (((valueX != 0) || (valueY != 0)) && speed != 0)
        {
            if (canReverse == true) { RotateOurShip(valueX, valueY, goingForward); }
            else if (speed > 0) { RotateOurShip(valueX, valueY, goingForward); }
        }

        //only apply a force if we are actively moving; otherwise we just 'glide' along

        if (speed != 0)
        {
			Accelerate(speed);
        }
    }

	void RedirectForce(bool goingForward)
	{
		//if we make a turn we cancel our previous momentum

		Vector3 ourVelocity = rigidBody.velocity;
		Vector3 reverseForce = (-(ourVelocity * ourMass * ourAirSpeed) / ((1 * (1 - amountSubmerged)) + (amountSubmerged * waterPressure)));
		rigidBody.AddForce(reverseForce, ForceMode.Force);

		//and now reapply it in our new direction

		float levelForce = Mathf.Sqrt((ourVelocity.x * ourVelocity.x) + (ourVelocity.z * ourVelocity.z));
		float finalForce = Mathf.Sqrt((levelForce * levelForce) + (ourVelocity.y * ourVelocity.y));
		if (!goingForward) { finalForce = (finalForce * -1); }
		Vector3 newForce = (transform.forward * (finalForce * ourMass * ourAirSpeed) / ((1 * (1 - amountSubmerged)) + (amountSubmerged * waterPressure)));
		rigidBody.AddForce(newForce, ForceMode.Force);
	}

	//all publicly exposed methods are here

	//rotate our ship around or flip over depending on if a certain button is being held down

	public void RotateOurShip(float ourValueX, float ourValueY, bool goingForward)
	{
		Vector3 newRotation = transform.rotation.eulerAngles;
		float waterPressure = 1;                            //1 is normal, ideal conditions

		newRotation.x -= ((ourValueY * Time.deltaTime * ourTurnSpeed) / ((1 * (1 - amountSubmerged)) + (amountSubmerged * waterPressure)));
		if (CrossPlatformInputManager.GetButton(Roll)) { newRotation.z -= ((ourValueX * Time.deltaTime * ourTurnSpeed) / ((1 * (1 - amountSubmerged)) + (amountSubmerged * waterPressure))); }
		else { newRotation.y += ((ourValueX * Time.deltaTime * ourTurnSpeed) / ((1 * (1 - amountSubmerged)) + (amountSubmerged * waterPressure))); }
		transform.rotation = Quaternion.Euler(newRotation);
		if (redirectMomentum) { RedirectForce(goingForward); }    //this should enable us to turn with the proper momentum
	}
	
	public void Accelerate(float ourSpeed)
	{
		Vector3 ourForce = ((transform.forward * ourSpeed * ourMass * ourAirSpeed) / ((1 * (1 - amountSubmerged)) + (amountSubmerged * waterPressure)));
		if (canReverse)
		{
			rigidBody.AddForce(ourForce, ForceMode.Force);
		}
		else
		{
			if (ourSpeed > 0) { rigidBody.AddForce(ourForce, ForceMode.Force); }
		}
	}

	public Vector2 GetMassDrag()
    {
        return new Vector2(ourMass, ourDrag);
    }

    public void ChangeOurSpeed(float newSpeed)
    {
        ourAirSpeed = newSpeed;
    }

    public void ChangeOurTurnSpeed(float newSpeed)
    {
        ourTurnSpeed = newSpeed;
    }
}

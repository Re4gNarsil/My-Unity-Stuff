using UnityEngine;
using System.Collections;

public class WindForces : MonoBehaviour {

    [Header("groundDrag must be at least 1, but can go as high as desired")]
    public float groundFriction = 1;
    public float amountSubmerged = 0, gravity = 9.81f;
    public bool canDriftSideways = true;

    private float ourMass, ourDrag;
    private WindManager windManager;
    private Rigidbody rigidBody;

    // Use this for initialization
    void Start () {
        gameObject.tag = "Player";
        windManager = FindObjectOfType<WindManager>();
        if (!windManager) { Debug.Log("No WindManager Script Present"); }
        rigidBody = GetComponent<Rigidbody>();
        ourMass = rigidBody.mass;
        ourDrag = rigidBody.drag;
    }
	
	// Update is called once every .02 seconds
	void FixedUpdate () {
        if (windManager && rigidBody) {
            ApplyWindForce();
            ApplyWindResistance();
        }
    }

    float CalculateWindAdjustment()
    {
        float windDirection = windManager.GetWind().y;
        float ourRotation = transform.rotation.eulerAngles.y;
        float angleDifference = Mathf.DeltaAngle(windDirection, ourRotation);
        if (angleDifference > 180) { angleDifference -= 180; }
        else if (angleDifference < -180) { angleDifference += 180; }

        float resistance;
        if (angleDifference < 90) { resistance = ((180 - angleDifference) / 180); }
        else { resistance = -(angleDifference / 180); }
        return resistance;
    }

    void ApplyWindResistance()
    {
        Vector3 windDirection = windManager.GetWind();
        Vector3 newRotation = transform.rotation.eulerAngles;

        //this calculates how much to rotate your gameobject so it faces in the direction the wind is blowing

        float angleDifferenceX = Mathf.DeltaAngle(windDirection.x, newRotation.x);
        if (angleDifferenceX > 180) { angleDifferenceX -= 180; }
        else if (angleDifferenceX < -180) { angleDifferenceX += 180; }
        float signX = (angleDifferenceX < 0) ? 1 : -1;

        float angleDifferenceY = Mathf.DeltaAngle(windDirection.y, newRotation.y);
        if (angleDifferenceY > 180) { angleDifferenceY -= 180; }
        else if (angleDifferenceY < -180) { angleDifferenceY += 180; }
        float signY = (angleDifferenceY < 0) ? 1 : -1;

        float windPower = windManager.GetWind().w * 5; //increasing this to look more realist in my humble opinion
        float windInfluence = ((windPower * (1 + ourDrag) * (1 - amountSubmerged)) * Time.deltaTime / (ourMass * gravity * (1 + groundFriction)));
        float influenceX = (Mathf.Abs(angleDifferenceX) / 180);
        float influenceY = (Mathf.Abs(angleDifferenceY) / 180);
        if (canDriftSideways)
        {
            newRotation.x += (windInfluence * influenceX * signX);
            newRotation.y += (windInfluence * influenceY * signY);
            transform.rotation = Quaternion.Euler(newRotation);
        }
    }

    void ApplyWindForce()
    {
        if (canDriftSideways)
        {
            Vector4 windForce = windManager.AcquireWind();
            Vector3 windForceFinal = ((new Vector3(windForce.x, windForce.y, windForce.z) * windForce.w * (1 - amountSubmerged) * (1 + ourDrag)) / (ourMass * (1 + groundFriction)));
            rigidBody.AddForce(windForceFinal, ForceMode.Force);
        }
        else {

            //this is needed if we can only go forward or backwards usually, like a car, to determine how much we drift in one of those two directions

            float windForce = windManager.AcquireWind().w;
            float windResistance = CalculateWindAdjustment();

            Vector3 windForceFinal = ((transform.forward * windForce * windResistance * (1 - amountSubmerged) * (1 + ourDrag)) / (ourMass * (1 + groundFriction)));
            rigidBody.AddForce(windForceFinal, ForceMode.Force);
        }
    }

    //all publicly exposed methods are here

    public void WeCanDrift(bool canDrift)
    {
        canDriftSideways = canDrift;
    }

    public void Submerged(float amountWeSubmerged)
    {
        amountSubmerged = amountWeSubmerged;
    }
}

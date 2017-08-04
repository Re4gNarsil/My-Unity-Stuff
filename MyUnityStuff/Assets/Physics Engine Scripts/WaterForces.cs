using UnityEngine;
using System.Collections;

public class WaterForces : MonoBehaviour {

    [Header("groundDrag must be at least 1, but can go as high as desired")]
    public float groundDrag = 1;
    public float amountSubmerged = 0, gravity = 9.81f, waterPressure = 10f;

    private float UpwardForce = 10.31f, ourMass, ourDrag; // Default gravity is 9.81. If we want the boat to float the upward force has to be greater
    private bool isInWater = false;
    private WaterManager waterManager;
    private Rigidbody rigidBody;

    // Use this for initialization
    void Start()
    {
        gameObject.tag = "Player";
        waterManager = FindObjectOfType<WaterManager>();
        if (!waterManager) { Debug.Log("No WaterManager Script Present"); }
        rigidBody = GetComponent<Rigidbody>();
        ourMass = rigidBody.mass;
        ourDrag = rigidBody.drag;
    }

    // Update is called once every .02 seconds
    void FixedUpdate()
    {
        if (waterManager && rigidBody) {
            ApplyWaterForce();
            ApplyWaterResistance();
        }
        if (isInWater)
        {
            // apply upward force if we're in water

            Vector3 force = (transform.up * UpwardForce);
            rigidBody.AddRelativeForce(force, ForceMode.Acceleration);
        }
    }

    void OnTriggerEnter(Collider obj)
    {
        if (obj.tag == "Water") {
            isInWater = true;
            amountSubmerged = .1f; //just a little bit on initial entering
        }
    }

    void OnTriggerExit(Collider obj)
    {
        if (obj.tag == "Water") {
            isInWater = false;
            amountSubmerged = 0;
        }
    }

    void ApplyWaterResistance()
    {
        Vector3 waterDirection = waterManager.GetWater();
        Vector3 newRotation = transform.rotation.eulerAngles;

        //this calculates how much to rotate your gameobject so it faces in the direction the water is flowing

        float angleDifferenceX = Mathf.DeltaAngle(waterDirection.x, newRotation.x);
        if (angleDifferenceX > 180) { angleDifferenceX -= 180; }
        else if (angleDifferenceX < -180) { angleDifferenceX += 180; }
        float signX = (angleDifferenceX < 0) ? 1 : -1;

        float angleDifferenceY = Mathf.DeltaAngle(waterDirection.y, newRotation.y);
        if (angleDifferenceY > 180) { angleDifferenceY -= 180; }
        else if (angleDifferenceY < -180) { angleDifferenceY += 180; }
        float signY = (angleDifferenceY < 0) ? 1 : -1;

        float waterPower = waterManager.GetWater().w * 5; //increasing this to look more realist in my humble opinion
        float waterInfluence = ((waterPower * (1 + ourDrag) * (1 - amountSubmerged)) * Time.deltaTime / (ourMass * gravity * groundDrag));
        float influenceX = (Mathf.Abs(angleDifferenceX) / 180);
        float influenceY = (Mathf.Abs(angleDifferenceY) / 180);

        newRotation.x += (waterInfluence * influenceX * signX);
        newRotation.y += (waterInfluence * influenceY * signY);

        transform.rotation = Quaternion.Euler(newRotation);
    }

    void ApplyWaterForce()
    {
        Vector4 waterForce = waterManager.AcquireWater();
        Vector3 waterForceFinal = ((new Vector3(waterForce.x, waterForce.y, waterForce.z) * amountSubmerged * waterForce.w * (1 + ourDrag)) / (ourMass * groundDrag));
        rigidBody.AddForce(waterForceFinal, ForceMode.Force);
    }

    //all publicly exposed methods are here

    public void Submerged(float amountWeSubmerged)
    {
        amountSubmerged = amountWeSubmerged;
    }
}


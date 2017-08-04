using UnityEngine;
using System;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.ImageEffects;

public class CharacterCamera : MonoBehaviour
{

    [Header("Note that any cameras pre-attached will be deleted during runtime")]
    [Header("Check with Input Manager if you want to change any of these")]
    public string secondXAxis = "Mouse X";
    public string secondYAxis = "Mouse Y", thirdAxis = "Mouse ScrollWheel";
    [Header("This needs to be set up in Input manager")]
    public string resetCam = "Reset";
    [Header("how far we can zoom in/out, where 1 equals the usual amount")]
    public float maxDistanceOut = 2;
    [Header("the two speed controls here determine how fast the camera transitions")]
    public float autoRotateSpeed = 3;
    public float autoMoveSpeed = .5f, minDistanceIn = .2f, zoomSpeed = 10f, cameraSpeed = 120, focusDistance = 1;
	public bool invertXAxis = false;

    private Vector3 originalLocation, originalRotation, changedRotation, changedLocation;
    private GameObject currentTarget, myCamera;
    private bool disableRotation, disableZoom, lookingAtTarget, movingBack, rotatingBack;
    private float maxDistance, minDistance, locationEqualizer = 1, rotationEqualizer = 1;
    private Camera myCam;

    // Use this for initialization
    void Start()
    {
        myCam = gameObject.GetComponentInChildren<Camera>();
        myCamera = myCam.gameObject;
        originalLocation = myCamera.transform.localPosition;
        originalRotation = gameObject.transform.localEulerAngles;

        float straightDistance = Mathf.Sqrt((originalLocation.x * originalLocation.x) + (originalLocation.y * originalLocation.y));
        float totalDistance = Mathf.Sqrt((straightDistance * straightDistance) + (originalLocation.z * originalLocation.z));

        maxDistance = (totalDistance * maxDistanceOut);
        minDistance = (totalDistance * minDistanceIn);
        zoomSpeed = Mathf.Abs(zoomSpeed * totalDistance);
    }

    // Update is called once per frame
    void Update()
    {
        float camValueX = CrossPlatformInputManager.GetAxisRaw(secondXAxis);
        float camValueY = CrossPlatformInputManager.GetAxisRaw(secondYAxis);
        float camValueZ = CrossPlatformInputManager.GetAxisRaw(thirdAxis);
        if (CrossPlatformInputManager.GetButtonDown(resetCam)) { ResetCamera(); }

        if (myCam.enabled)
        {
            Vector3 rotationAmount = GetShortestRoute();
            Vector3 movementAmount = (myCamera.transform.localPosition - changedLocation);
            FindTimeNeeded(rotationAmount, movementAmount);

			//we have to make sure we are within our zoom boundaries before moving in or out
			//plus we only want to zoom if we're not focusing on an enemy currently

			if (movingBack) { ChangeLocation(movementAmount); }
			else if (camValueZ != 0)
			{
				float straightDistance = Mathf.Sqrt((myCamera.transform.localPosition.y * myCamera.transform.localPosition.y) + (myCamera.transform.localPosition.z * myCamera.transform.localPosition.z));
				float camDistance = Mathf.Sqrt((straightDistance * straightDistance) + (myCamera.transform.localPosition.x * myCamera.transform.localPosition.x));

				if (((camDistance < maxDistance) && (camValueZ < 0)) || ((camDistance > minDistance) && (camValueZ > 0))) { Zoom(camValueZ); }
			}

			if (!disableRotation)
            {
                //we only want to shift the camera manually if we're not looking towards an enemy presently

                if (rotatingBack) { ChangeRotation(rotationAmount); }
                else if ((camValueX != 0) || (camValueX != 0)) {
                    RotateCamera(camValueX, camValueY, 0);
                }
            } else
            {
                if (currentTarget) {
                    if (lookingAtTarget)
                    {
                        transform.LookAt(currentTarget.transform);
                    } else
                    {
                        LookAtTarget(rotationAmount);
                    }
                    if (movingBack) { ChangeLocation(movementAmount); }
                }
                else { ResetCamera(); }
            }
        }
    }

    void LookAtTarget(Vector3 angleDifference)
    {
        float lookDistance = ((angleDifference.x * angleDifference.x) + ((angleDifference.y * angleDifference.y)));
        if (lookDistance > autoRotateSpeed)
        {
            ChangeRotation(angleDifference);
        } else
        {
            lookingAtTarget = true;     //if we're looking right at the target we can just use the LookAt each frame going forward
        }
    }

    Vector3 GetShortestRoute()
    {
        Vector3 currentRotation = gameObject.transform.localEulerAngles;
        Vector3 angleDifference;

        angleDifference.x = Mathf.DeltaAngle(changedRotation.x, currentRotation.x);
        angleDifference.y = Mathf.DeltaAngle(changedRotation.y, currentRotation.y);
        angleDifference.z = Mathf.DeltaAngle(changedRotation.z, currentRotation.z);

        return angleDifference;
    }

    void ChangeRotation(Vector3 changeRotationAmount)
    {
        Vector3 ourRotation = gameObject.transform.localEulerAngles;
        float differenceXZ = Mathf.Abs((changeRotationAmount.x * changeRotationAmount.x) + (changeRotationAmount.z * changeRotationAmount.z));

        if ((Mathf.Abs((differenceXZ * differenceXZ) + (changeRotationAmount.y * changeRotationAmount.y))) > autoRotateSpeed) {

            ourRotation.x -= (autoRotateSpeed * rotationEqualizer * (changeRotationAmount.x / (Mathf.Abs(changeRotationAmount.x) + Mathf.Abs(changeRotationAmount.y) + Mathf.Abs(changeRotationAmount.z))));
            ourRotation.y -= (autoRotateSpeed * rotationEqualizer * (changeRotationAmount.y / (Mathf.Abs(changeRotationAmount.x) + Mathf.Abs(changeRotationAmount.y) + Mathf.Abs(changeRotationAmount.z))));
			ourRotation.z -= (autoRotateSpeed * rotationEqualizer * (changeRotationAmount.z / (Mathf.Abs(changeRotationAmount.x) + Mathf.Abs(changeRotationAmount.y) + Mathf.Abs(changeRotationAmount.z))));
            gameObject.transform.localRotation = Quaternion.Euler(ourRotation);

        } else {
            gameObject.transform.localRotation = Quaternion.Euler(changedRotation);
            rotatingBack = false;
        }
    }

    void ChangeLocation(Vector3 changeLocationAmount)
    {
        float distanceXZ = Mathf.Abs((changeLocationAmount.x * changeLocationAmount.x) + (changeLocationAmount.z * changeLocationAmount.z));
        float distanceTotal = Mathf.Abs((distanceXZ * distanceXZ) + (changeLocationAmount.y * changeLocationAmount.y));

        if (distanceTotal > autoMoveSpeed)
        {
            Vector3 newLocation = myCamera.transform.localPosition;

            newLocation.x -= (autoMoveSpeed * locationEqualizer * (changeLocationAmount.x / (Mathf.Abs(changeLocationAmount.x) + Mathf.Abs(changeLocationAmount.y) + Mathf.Abs(changeLocationAmount.z))));
            newLocation.y -= (autoMoveSpeed * locationEqualizer * (changeLocationAmount.y / (Mathf.Abs(changeLocationAmount.x) + Mathf.Abs(changeLocationAmount.y) + Mathf.Abs(changeLocationAmount.z))));
            newLocation.z -= (autoMoveSpeed * locationEqualizer * (changeLocationAmount.z / (Mathf.Abs(changeLocationAmount.x) + Mathf.Abs(changeLocationAmount.y) + Mathf.Abs(changeLocationAmount.z))));

            myCamera.transform.localPosition = newLocation;
        }
        else
        {
            myCamera.transform.localPosition = changedLocation;
            movingBack = false;
        }
    }

    void FindTimeNeeded(Vector3 rotationNeeded, Vector3 movementNeeded)
    {
        //this little addition, multiplication, and division helps us make sure the rotation and movement of our transitions finishes at the same time

        float rotationXZ = Mathf.Sqrt((rotationNeeded.x * rotationNeeded.x) + (rotationNeeded.z * rotationNeeded.z));
        float rotationXYZ = Mathf.Sqrt((rotationXZ * rotationXZ) + (rotationNeeded.y * rotationNeeded.y));
        float rotationTime = (rotationXYZ / autoRotateSpeed);

        float locationXZ = Mathf.Sqrt((movementNeeded.x * movementNeeded.x) + (movementNeeded.z * movementNeeded.z));
        float locationXYZ = Mathf.Sqrt((locationXZ * locationXZ) + (movementNeeded.y * movementNeeded.y));
        float locationTime = (locationXYZ / autoMoveSpeed);

        locationEqualizer = 1;
        rotationEqualizer = 1;
        if ((rotationTime) > 0 && (locationTime > 0))
        {
            if (rotationTime > locationTime) { locationEqualizer = (locationTime / rotationTime); }
            else                             { rotationEqualizer = (rotationTime / locationTime); }
        }
    }

    void Zoom(float valueZ)
    {
        Vector3 ourPosition = myCamera.transform.localPosition, newPosition = ourPosition;

        float signX = (ourPosition.x < 0) ? 1 : -1;
        float signY = (ourPosition.y < 0) ? 1 : -1;
        float signZ = (ourPosition.z < 0) ? 1 : -1;

        newPosition.y += (valueZ * Time.deltaTime * zoomSpeed * signY);
        newPosition.z += (valueZ * Time.deltaTime * zoomSpeed * signZ * Mathf.Abs(ourPosition.z / ourPosition.y));
        newPosition.x += (valueZ * Time.deltaTime * zoomSpeed * signX * Mathf.Abs(ourPosition.z / ourPosition.y) * Mathf.Abs(ourPosition.x / ourPosition.z));

        myCamera.transform.localPosition = newPosition;
    }

    void OnEnable()
    {
        if (myCamera)
        {
            myCamera.transform.localPosition = originalLocation;
            gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }

    void RotateCamera(float changeX, float changeY, float changeZ)
    {
		//used to have the camera itself move around the player, but now I just rotate the anchor
		int yDirection = (invertXAxis) ? 1 : -1; //change the up/down movement if we want it to be inverted

		Vector3 localRotation = transform.localEulerAngles;
		localRotation += new Vector3(changeY * Time.deltaTime * cameraSpeed * yDirection, changeX * Time.deltaTime * cameraSpeed, 0);
		transform.localRotation = Quaternion.Euler(localRotation);
    }

    Vector3 GetTargetDirection()
    {
        //I originally had a complex formula for finding the angle to rotate to, but then decided to LookAt our target first, save the rotation, then put it back

        Vector3 currentAngle = gameObject.transform.localEulerAngles;
        gameObject.transform.LookAt(currentTarget.transform);
        Vector3 desiredAngle = gameObject.transform.localEulerAngles;

        gameObject.transform.localRotation = Quaternion.Euler(currentAngle);
        return desiredAngle;
    }

    void ResetCamera()
    {
        rotatingBack = true;
        movingBack = true;
        lookingAtTarget = false;

        if (currentTarget) {
            disableRotation = false;
            currentTarget = null;
        }
        changedLocation = originalLocation;
        changedRotation = originalRotation;
    }

    //all publicly exposed methods are here

    public void DisableEnableCamera(bool active)
    {
        myCam.enabled = active;
    }

    public void DisableMovement(bool disabled, GameObject newTarget)  //used when you're locked onto an object, and want to keep looking right at it
    {
        disableRotation = disabled;
        if (disableRotation) {
            currentTarget = newTarget;
            lookingAtTarget = false;
            movingBack = true;      //this needs to be here if we only zoom in initially when targeting a foe; otherwise we can check localPosition each update

            changedLocation = originalLocation;
			if (disableZoom) { changedLocation = (changedLocation * focusDistance); }
			changedRotation = GetTargetDirection();
        }
        else {
            ResetCamera();
        }
    }

	public void EnableMovment()
	{
		disableRotation = false;
		disableZoom = false;
	}

    public void DetachParent()  //sometimes we may want to have the camera remain in place, regardless of our movement
    {
        myCamera.transform.parent = null;
    }

    public void ChangeCameraSpeed(float newSpeed)
    {
        cameraSpeed = newSpeed;
    }

    public void ChangeZoomSpeed(float newSpeed)
    {
        zoomSpeed = newSpeed;
    }

    public void ChangeCameraLocation(Vector3 newRotation)
    {
        changedLocation = newRotation;       //whenever you give a command from another script to change location or rotation the camera will transition smoothly
        movingBack = true;
    }

    public void ChangeCameraRotation(Vector3 newLocation)
    {
        changedRotation = newLocation;
        rotatingBack = true;
    }
}
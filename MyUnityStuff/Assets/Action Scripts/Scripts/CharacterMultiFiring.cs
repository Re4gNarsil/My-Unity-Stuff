using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CharacterMultiFiring : MonoBehaviour {

	[Header("first array is for Fire1; second is for Fire2")]
	[Header("Initial weapon velocity and offset is based on these transforms")]
	public GameObject[] weaponSpawnPoints;
	public GameObject[] otherWeaponSpawnPoints;
	[Header("Make sure you have a rigidbody on your projectile(s)")]
	public GameObject myProjectile;
	public GameObject otherProjectile;
	[Header("Make sure you have a tag that begins with 'Weapon', for other scripts")]
	public string weaponTag = "Weapon";
	[Header("Make sure you have these configured with Input Manager")]
	public string fire = "Fire1";
	public string fireSecond = "Fire2";
	public float fireSpeed = 75;
	public float otherFireSpeed = 75;
	public float fireDelay = .1f, otherFireDelay = .1f;
	[Header("leave this as is if your projectile faces upwards; otherwise change as needed")]
	public Vector3 weaponRotation = new Vector3(90, 0, 0);

	private Rigidbody fireBody;
	private Rigidbody otherFireBody;
	private float delayTime, delayTimeSecond;

	void Start()
	{
		//delayTime = fireDelay;
	}

	void Update()
	{
		if (delayTime > 0) { delayTime -= Time.deltaTime; }
		if (delayTimeSecond > 0) { delayTimeSecond -= Time.deltaTime; }
		if (CrossPlatformInputManager.GetButtonDown(fire) && myProjectile) { FireShot(); }
		if (CrossPlatformInputManager.GetButtonDown(fireSecond) && otherProjectile) { FireOtherShot(); }
	}

	public void FireShot()
	{
		if (delayTime <= 0)
		{
			if (weaponSpawnPoints.Length > 0)
			{
				int randomNum = Random.Range(0, weaponSpawnPoints.Length);

				Vector3 firePosition = weaponSpawnPoints[randomNum].transform.position;
				Vector3 fireRotation = weaponSpawnPoints[randomNum].transform.rotation.eulerAngles;
				fireRotation = (fireRotation + weaponRotation);
				GameObject newProjectile = Instantiate(myProjectile, firePosition, Quaternion.Euler(fireRotation)) as GameObject;
				fireBody = newProjectile.GetComponent<Rigidbody>();
				fireBody.velocity = ((weaponSpawnPoints[randomNum].transform.forward) * fireSpeed * fireBody.mass);
				newProjectile.tag = weaponTag;
				newProjectile.name = "Weapon";
			}
			delayTime = fireDelay;
		}
	}

	public void FireOtherShot()
	{
		if (delayTimeSecond <= 0)
		{
			if (otherWeaponSpawnPoints.Length > 0)
			{
				int randomNum = Random.Range(0, otherWeaponSpawnPoints.Length);

				Vector3 firePosition = otherWeaponSpawnPoints[randomNum].transform.position;
				Vector3 fireRotation = otherWeaponSpawnPoints[randomNum].transform.rotation.eulerAngles;
				fireRotation = (fireRotation + weaponRotation);
				GameObject newProjectile = Instantiate(myProjectile, firePosition, Quaternion.Euler(fireRotation)) as GameObject;
				fireBody = newProjectile.GetComponent<Rigidbody>();
				fireBody.velocity = ((otherWeaponSpawnPoints[randomNum].transform.forward) * fireSpeed * fireBody.mass);
				newProjectile.tag = weaponTag;
				newProjectile.name = "Weapon";
			}
			delayTimeSecond = fireDelay;
		}
	}

	//all publicly exposed methods are here

	public void ChangeFireSpeed(float newSpeed)
	{
		fireSpeed = newSpeed;
	}
}

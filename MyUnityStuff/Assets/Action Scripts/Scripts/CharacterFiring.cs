using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class CharacterFiring : MonoBehaviour {

    [Header("Make sure you have a rigidbody on your projectile(s)")]
    public GameObject myProjectile;
	public GameObject otherProjectile;
	[Header("Make sure you have a tag that begins with 'Weapon', for other scripts")]
    public string weaponTag = "Weapon";
    [Header("Make sure you have this configured with Input Manager")]
    public string fire = "Fire1";
	public string fireSecond = "Fire2";
	public float fireSpeed = 75;
	public float otherFireSpeed = 75;
	public float fireDelay = .1f, otherFireDelay = .1f;
	[Header("leave this as is if your projectile faces upwards; otherwise change as needed")]
    public Vector3 weaponRotation = new Vector3(90, 0, 0);
    [Header("the offset moves it according it's transform")]
    public Vector3 weaponOffset = new Vector3(0, 0, 1);
	public Vector3 otherWeaponOffset = new Vector3(0, 0, 1);
	[Header("this rotates the weapon trajectory according to it's transform")]
    public Vector3 fireDirection = new Vector3(0, 0, 0);
	public Vector3 otherFireDirection = new Vector3(0, 0, 0);

	private Rigidbody fireBody, otherFireBody;
	private GameObject myWeapon, otherWeapon;
	private float timeDelay, otherTimeDelay;

    // Use this for initialization
    void Start()
    {
        //we effective create an invisible 'turret' from which our projectiles emerge

        myWeapon = new GameObject();
        myWeapon.transform.parent = gameObject.transform;
        myWeapon.transform.localPosition = weaponOffset;
        myWeapon.transform.localRotation = Quaternion.Euler(transform.forward + fireDirection);
        myWeapon.name = "Turret";
		timeDelay = fireDelay;
		otherTimeDelay = otherFireDelay;
    }

    // Update is called once per frame
    void Update () {
		if (timeDelay > 0) { timeDelay -= Time.deltaTime; }
		if (otherTimeDelay > 0) { otherTimeDelay -= Time.deltaTime; }
		if (CrossPlatformInputManager.GetButtonDown(fire) && myProjectile) { FireShot(); }
		if (CrossPlatformInputManager.GetButtonDown(fireSecond) && otherProjectile) { FireOtherShot(); }
	}

    void FireShot()
    {
		if (timeDelay <= 0)
		{
			Vector3 firePosition = myWeapon.transform.position;
			Vector3 fireRotation = myWeapon.transform.rotation.eulerAngles;
			fireRotation = (fireRotation + weaponRotation);
			GameObject newProjectile = Instantiate(myProjectile, firePosition, Quaternion.Euler(fireRotation)) as GameObject;
			fireBody = newProjectile.GetComponent<Rigidbody>();
			fireBody.velocity = ((myWeapon.transform.forward) * fireSpeed * fireBody.mass);
			newProjectile.tag = weaponTag;
			newProjectile.name = "Weapon";
		}
    }

	void FireOtherShot()
	{
		if (otherTimeDelay <= 0)
		{
			Vector3 firePosition = otherWeapon.transform.position;
			Vector3 fireRotation = otherWeapon.transform.rotation.eulerAngles;
			fireRotation = (fireRotation + weaponRotation);
			GameObject newProjectile = Instantiate(otherProjectile, firePosition, Quaternion.Euler(fireRotation)) as GameObject;
			otherFireBody = newProjectile.GetComponent<Rigidbody>();
			otherFireBody.velocity = ((otherWeapon.transform.forward) * otherFireSpeed * otherFireBody.mass);
			newProjectile.tag = weaponTag;
			newProjectile.name = "Weapon";
		}
	}

	//all publicly exposed methods are here

	public void ChangeFireSpeed(float newSpeed)
    {
        fireSpeed = newSpeed;
    }
}

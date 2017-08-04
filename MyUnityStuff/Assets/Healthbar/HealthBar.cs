using UnityEngine;
using UnityEditorInternal;

public class HealthBar : MonoBehaviour {

    [Header("Make sure to add the tag 'Health' in the inspector")]
	[Header("The scale, length, and offset needs to be adjusted as needed")]
	public float ObjectHealth = 100;
    public float localScale = .2f, offsetUp = 0, overallLength = 1;
    public bool regenHealth = false;

    private GameObject myHealth, currentCamera;
    private Renderer healthRenderer;
    private float originalSize, currentHealth, initialLength, currentTime;

    // Use this for initialization
    void Start()
    {

        myHealth = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(myHealth.GetComponent<Collider>());
        myHealth.transform.parent = gameObject.transform;

        foreach (Transform child in transform) //seems to be necessary given when you destroy and re-create a unity gameobject it retains things created during runtime
        {
            if (child.gameObject.tag == "Health") { Destroy(child.gameObject); }
            else if (child.gameObject.tag == "MainCamera") { currentCamera = child.gameObject; }
        }

        myHealth.tag = "Health";
        myHealth.name = "Health";
        healthRenderer = myHealth.GetComponent<Renderer>();

        SetSizeAndPosition();
        healthRenderer.material.color = new Color(.1f, .5f, .9f);

        currentHealth = ObjectHealth;
        initialLength = myHealth.transform.localScale.x;
    }

    void SetSizeAndPosition()
    {
        Vector3 newLocalScale = new Vector3(localScale * overallLength, localScale, localScale);
        float averageSizeXZ = Mathf.Sqrt((transform.lossyScale.x * transform.lossyScale.x) + (transform.lossyScale.z * transform.lossyScale.z));
        float averageSize = Mathf.Sqrt((averageSizeXZ * averageSizeXZ) + (transform.lossyScale.y * transform.lossyScale.y));

        newLocalScale = (newLocalScale * averageSize);
        float newLocalYPos = ((transform.localScale.y) + (transform.localScale.y * localScale) + offsetUp);

        myHealth.transform.localScale = newLocalScale;
        myHealth.transform.position = new Vector3(transform.position.x, newLocalYPos, transform.position.z);
        myHealth.transform.rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update () {
        if (currentCamera) { myHealth.transform.LookAt(currentCamera.transform); }
        else { currentCamera = GameObject.FindGameObjectWithTag("MainCamera"); }
        if ((regenHealth) && currentHealth < ObjectHealth)
        {
            if (currentTime < 3) { currentTime += Time.deltaTime; }
            else {
                float health = ((ObjectHealth / 10) * Time.deltaTime);
                if (currentHealth > (ObjectHealth - health)) { DealDamage(-(ObjectHealth - currentHealth)); }
                else { DealDamage(-(health)); }
            }
        }
	}

    public void DealDamage(float damage) {//positive if it's damage; negative if you're regaining health
        currentHealth -= damage;
        if (currentHealth <= 0) { print("GAME OVER"); } //Game Over stuff goes here
        if (damage > 0) { currentTime = 0; }

        //even if we exceed maximum health our size will increase

        float newLength = (initialLength * (currentHealth / ObjectHealth));
        myHealth.transform.localScale = new Vector3(newLength, myHealth.transform.localScale.y, myHealth.transform.localScale.z);

        if (currentHealth < ObjectHealth)
        {
            //change our length and alter our color with each hit if we are under maximum health

            if (currentHealth > (ObjectHealth / 2))
            {
                float newRedColor = (healthRenderer.material.color.r + (1.4f / (ObjectHealth / damage)));
                float newBlueColor = (healthRenderer.material.color.b - (1.8f / (ObjectHealth / damage)));
                float newGreenColor = (healthRenderer.material.color.g + (1f / (ObjectHealth / damage)));
                healthRenderer.material.color = new Color(newRedColor, newGreenColor, newBlueColor);
            }
            else {
                float newRedColor = (healthRenderer.material.color.r + (.2f / (ObjectHealth / damage)));
                float newGreenColor = (healthRenderer.material.color.g - (1.8f / (ObjectHealth / damage)));
                float newBlueColor = (healthRenderer.material.color.b - (.2f / (ObjectHealth / damage)));
                healthRenderer.material.color = new Color(newRedColor, newGreenColor, newBlueColor);
            }
        }
    }

    public void ChangeCamera(GameObject camera) {
        currentCamera = camera;
    }

}
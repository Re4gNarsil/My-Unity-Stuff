using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class UIDamage : MonoBehaviour {

	public float amountOfBlur = 5;
	public float initialBlurAmount = 10;
	public float opaquenessAmount = .5f;
    public float playerHealth = 100;
	public float healthRegenDelay = 3;
	public float healthRegenTime = 3;
	public float initialPainTime = 1;
	public bool initialPain = true;
	public bool autoHeal = false;
	public bool removeEffects = true;

	public bool damaged = false;

	private BlurOptimized[] blurs;
    private RawImage outerImage;
    private RawImage innerImage;
    private RectTransform outerRect;
    private RectTransform innerRect;
    private GameObject mainCamera;
	private float actualHealth;
	private float currentHealth;
	private float currentDelay;

	// Use this for initialization
	void Start () {
        outerImage = GameObject.Find("OuterImage").GetComponent<RawImage>();
        innerImage = GameObject.Find("InnerImage").GetComponent<RawImage>();
        outerRect = GameObject.Find("OuterImage").GetComponent<RectTransform>();
        innerRect = GameObject.Find("InnerImage").GetComponent<RectTransform>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		blurs = mainCamera.GetComponents<BlurOptimized>();

		actualHealth = playerHealth;
		currentHealth = actualHealth;
		blurs[0].enabled = false;
		blurs[1].enabled = false;
}
	
	// Update is called once per frame
	void Update () {
		if (damaged)
		{
			damaged = false;
			TakeDamage(25);
		}

		//if we can auto heal or just want our ui to return to normal we need to change the display slowly over time

		if (autoHeal && actualHealth < playerHealth)
		{
			if (currentDelay <= 0)
			{
				actualHealth += (playerHealth * Time.deltaTime) / healthRegenTime;
				currentHealth = actualHealth;
			} else
			{
				currentDelay -= Time.deltaTime;
			}
		}
		else if (removeEffects && currentHealth < playerHealth)
		{
			if (currentDelay <= 0)
			{
				currentHealth += (playerHealth * Time.deltaTime) / healthRegenTime;
			}
			else
			{
				currentDelay -= Time.deltaTime;
			}
		}
        float SizeY = 720 + (360 * Mathf.Clamp(currentHealth / playerHealth, 0, 1));
        float SizeX = 1280 + (640 * Mathf.Clamp(currentHealth / playerHealth, 0, 1));

        // Change the size of the damage indicators as our health is depleted
        outerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, SizeY);
        outerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, SizeX);
        outerRect.localPosition = new Vector3(-(SizeX / 2), -(SizeY / 2), 0);

        innerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, SizeY);
        innerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, SizeX);
        innerRect.localPosition = new Vector3(-(SizeX / 2), -(SizeY / 2), 0);

        // Change opacity based on damage taken; the inner one will fade back to normal after a little bit
        outerImage.color = new Color(outerImage.color.r, outerImage.color.g, outerImage.color.b, Mathf.Clamp(1 - (currentHealth / playerHealth), 0, 1) * opaquenessAmount);
		blurs[0].blurSize = Mathf.Clamp(1 - (currentHealth / playerHealth), 0, 1) * amountOfBlur;
		if (currentHealth >= playerHealth) { blurs[0].enabled = false; }
		if (initialPain)
		{
			float newAlpha = Mathf.Clamp(innerImage.color.a - (Time.deltaTime / initialPainTime), 0, 1);
			innerImage.color = new Color(innerImage.color.r, innerImage.color.g, innerImage.color.b, newAlpha);

			blurs[1].blurSize = newAlpha * initialBlurAmount;
			if (newAlpha == 0) { blurs[1].enabled = false; }
		}

        mainCamera.GetComponent<ColorCorrectionCurves>().saturation = Mathf.Clamp(currentHealth / playerHealth, 0, 1);
	}

    public void TakeDamage(float damageAmount)
    {
		actualHealth = Mathf.Clamp(actualHealth - damageAmount, 0, playerHealth * 2);
		blurs[0].enabled = true;
		currentHealth = actualHealth;
		currentDelay = healthRegenDelay;

		if (damageAmount > 0 && initialPain)
		{
            innerImage.color = new Color(innerImage.color.r, innerImage.color.g, innerImage.color.b, Mathf.Clamp(1 - (actualHealth * 2 / playerHealth), 0, 1) * opaquenessAmount);
			blurs[1].enabled = true;
		}
	}
}

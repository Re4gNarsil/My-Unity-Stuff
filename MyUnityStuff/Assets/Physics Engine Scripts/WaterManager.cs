using UnityEngine;
using System.Collections;

public class WaterManager : MonoBehaviour {

    [Header("Maximum water force plus how quickly it changes")]
    public float maxWaterForce = 25000;
    public float maxWaterChange = 125;

    private Vector3 waterDirection;
    private float waterForce;

    // Use this for initialization
    void Start()
    {
        float waterAngle = Random.Range(0, 359);
        waterForce = Random.Range((maxWaterChange), (maxWaterForce));
        waterDirection = new Vector3(0, waterAngle, 0);
        transform.rotation = Quaternion.Euler(waterDirection);
    }

    // Update is called once per frame
    void Update()
    {
        float changeDirection = (Random.Range(-60, 60) * Time.deltaTime);
        float changeForce = (Random.Range(-maxWaterForce, maxWaterChange) * Time.deltaTime);
        waterDirection.y += changeDirection;
        transform.rotation = Quaternion.Euler(waterDirection);

        waterForce += changeForce;
        if (waterForce < (maxWaterChange * 10)) { waterForce = maxWaterForce; }
        if (waterForce > maxWaterForce) { waterForce = maxWaterForce; }
    }

    public Vector4 GetWater()
    {
        return new Vector4(waterDirection.x, waterDirection.y, waterDirection.z, waterForce);
    }

    public Vector4 AcquireWater()
    {
        return new Vector4(transform.forward.x, transform.forward.y, transform.forward.z, waterForce);
    }
}
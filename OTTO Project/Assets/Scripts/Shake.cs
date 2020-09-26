using UnityEngine;
public class Shake : MonoBehaviour
{
    public float shakeDuration, shakeStartPower;
    private float shakeTimeRemaining, shakePower;
    Vector3 tempTransform, randDirection;

    public static Shake instance;

    float temp;
    private bool isShaking = false;

    void Awake()
    {
        instance = this;
        tempTransform = transform.position;
    }

    public void StartShake()
    {
        shakeTimeRemaining = shakeDuration;
        shakePower = shakeStartPower;
        isShaking = true;
    }
    
    public void Update()
    {
        if (isShaking)
        {
            if (shakeTimeRemaining > 0)
            {
                shakeTimeRemaining -= Time.deltaTime;
                temp = shakeTimeRemaining * 0.5f;
                randDirection = 4 * new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
                transform.position = tempTransform + new Vector3((temp * shakePower) * randDirection.x, (temp * shakePower) * randDirection.y, (temp * shakePower) * randDirection.z);
                shakePower = Mathf.Lerp(shakePower, 0f, shakeTimeRemaining * 10f * Time.deltaTime);
            }
            else
            {
                transform.position = tempTransform;
                isShaking = false;
            }
        }
    }
}
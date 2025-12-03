using UnityEngine;

public class RotateObj : MonoBehaviour
{
    public float xAngle, yAngle, zAngle = 0.0f;
    public float xSpeed, ySpeed, zSpeed = 0.0f;
    private float cycle = 0.0f;
    
    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
{
    cycle += Time.deltaTime * xSpeed;

    float newXAngle = Mathf.Sin(cycle) * xAngle / 2.0f + xAngle;
    float newYAngle = Mathf.Sin(cycle) * yAngle / 2.0f + yAngle;
    float newZAngle = Mathf.Sin(cycle) * zAngle / 2.0f + zAngle;
    
    transform.localRotation = startRotation * Quaternion.Euler(newXAngle, newYAngle, newZAngle);
}
}
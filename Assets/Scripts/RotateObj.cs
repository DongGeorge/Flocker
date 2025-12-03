using UnityEngine;

public class RotateObj : MonoBehaviour
{
    public float xAngle, yAngle, zAngle = 0.0f;
    public float xSpeed, ySpeed, zSpeed = 0.0f;
    
    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
    {
        // Calculate the flap amount using a sine wave
        float newXAngle = Mathf.Sin(Time.time * xSpeed) * xAngle / 2.0f + xAngle;
        float newYAngle = Mathf.Sin(Time.time * ySpeed) * yAngle / 2.0f + yAngle;
        float newZAngle = Mathf.Sin(Time.time * zSpeed) * zAngle / 2.0f + zAngle;
        
        transform.localRotation = startRotation * Quaternion.Euler(newXAngle, newYAngle, newZAngle);
    }
}
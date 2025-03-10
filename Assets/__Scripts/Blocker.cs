using UnityEngine;

public class Blocker : MonoBehaviour
{
    [Header("Inscribed")]
    // Speed at which the blocker moves in meters/second
    public float speed = 12f;

    // The minimum and maximum height the blocker will move between
    public float minY = -5f;
    public float maxY = 30f;

    // A smooth transition using a sine wave for continuous motion
    private float direction = 1f; // 1 for moving up, -1 for moving down

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Move the blocker vertically using sine for smooth up and down movement
        float newY = Mathf.PingPong(Time.time * speed, maxY - minY) + minY;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}

using UnityEngine;

public class player : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
            if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(new Vector3(0, speed), ForceMode2D.Impulse);
            }

        float horizontal = 0f;
        if (Input.GetKey(KeyCode.A)) horizontal -= speed;
        if (Input.GetKey(KeyCode.D)) horizontal += speed;

        transform.Translate(new Vector3(horizontal * Time.deltaTime, 0, 0));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Color
{
    Red, Green, Blue,Null
}
public enum BallState
{
    Idle,Moving
}
public class Ball : MonoBehaviour
{
    public Color color;
    public Rigidbody2D rb;
    BallState state;
    private Transform tf;

    public Transform TF
    {
        get
        {
            if (tf == null)
            {
                tf = transform;
            }
            return tf;
        }
    }
    public BallState State { get => state; set => state = value; }

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.J))
        //{
        //    rb.AddForce(new Vector2(100,100));
        //}
    }
    public void StopMoving()
    {
        state = BallState.Idle;
        rb.velocity = Vector2.zero;
    }
    public void AddForce(Vector2 force)
    {
        state = BallState.Moving;
        Debug.Log(force);
        rb.AddForce(force);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }
        if (collision.CompareTag("Top")|| collision.CompareTag("Ball"))
        {
            StopMoving();
        }
    }
}

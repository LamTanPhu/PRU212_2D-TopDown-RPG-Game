using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;

    public Rigidbody2D rgb;

    public Animator animator;

    private Vector2 Movedirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
/*    void Start()
    {
        
    }*/

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
        Animate();
    }
     void FixedUpdate()
    {
        Move();
    }
    void ProcessInputs() 
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Movedirection = new Vector2(moveX, moveY).normalized;
    }
     void Move()
    {

        rgb.linearVelocity= Movedirection*moveSpeed;
    }

    void Animate()
    {
        animator.SetFloat("AnimMoveX", Movedirection.x);
        animator.SetFloat("AnimMoveY", Movedirection.y);
        animator.SetFloat("AnimMagnitude", Movedirection.magnitude);
    }    
}

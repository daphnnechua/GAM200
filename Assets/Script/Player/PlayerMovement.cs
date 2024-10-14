using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : PlayerScript, InputReceiver
{
    private GameController gameController;
    Rigidbody2D rb;

    Animator anim;

    public float movementSpeed = 1f;
    private Vector2 oriPos;

    [SerializeField] private bool isFacingUp;
    [SerializeField] private bool isFacingRight;
    [SerializeField] private bool isFacingLeft;
    [SerializeField] private bool isFacingDown;

    private float radius =5;
    private float semiCircleAngle = 90f;
    void Start()
    {
        anim = GetComponent<Animator>(); 
    }

    void Update()
    {
        PlayerAnimation();
    }

    public override void Initialize(GameController gameController)
    {
        this.gameController = gameController;
        oriPos = Vector2.zero;
    }

    public void Move(Vector2 newPos)
    {   
        oriPos = newPos;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        oriPos.Normalize();

        Vector2 movePos = rb.position + oriPos*movementSpeed*Time.fixedDeltaTime; //player movement calculation
        rb.MovePosition(movePos); //move player
        UpdateFacingDirection();
    }

    private void UpdateFacingDirection()
    {
        if(Input.GetAxis("Vertical")>0)
        {
            isFacingUp = true;

            isFacingDown =false;
            isFacingLeft = false;
            isFacingRight = false;
        }
        if(Input.GetAxis("Vertical")<0)
        {
            isFacingDown = true;

            isFacingUp = false;
            isFacingLeft = false;
            isFacingRight = false;;
        }
        if(Input.GetAxis("Horizontal") >0)
        {
            isFacingRight = true;

            isFacingDown = false;
            isFacingLeft = false;
            isFacingUp = false;
        }
        if(Input.GetAxis("Horizontal") <0)
        {
            isFacingLeft = true;

            isFacingDown = false;
            isFacingRight = false;
            isFacingUp = false;

        }
    }
    public Vector2 FacingDirection()
    {
        if (isFacingUp)
        {
            return Vector2.up; 
        }
        if (isFacingDown)
        {
            return Vector2.down;
        }
        if (isFacingRight)
        {
            return Vector2.right;
        }
        if(isFacingLeft)
        {
            return Vector2.left;
        }    
        return Vector2.down;
    }
    public bool IsObjectInteractable(Transform objectTransform)
    {
        Vector2 objectDir = (objectTransform.position - transform.position).normalized;

        Vector2 playerDir = FacingDirection();

        float angle = Vector2.Angle(playerDir, objectDir);

        if (angle <= semiCircleAngle / 2 && Vector2.Distance(transform.position, objectTransform.position) <= radius)
        {
            return true; 
        }

        return false; 
    }

    //testing direction
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 position = transform.position;
        Vector3 playerDir = FacingDirection();

        DrawSemicircle(position, playerDir, radius, semiCircleAngle);
    }
    private void DrawSemicircle(Vector3 position, Vector2 direction, float radius, float angle)
    {
        int lines = 20; 
        float progression = angle / lines; 

        Vector3 previousPoint = position; 

        for (int i = 0; i <= lines; i++)
        {
            float currentAngle = -angle / 2 + progression * i; 
            Vector3 point = position + Quaternion.Euler(0, 0, currentAngle) * direction * radius; 

            Gizmos.DrawLine(previousPoint, point); 
            previousPoint = point; 
        }

        Gizmos.DrawLine(previousPoint, position);
    }

    //setting up player animation
    private void PlayerAnimation()
    {
        float movementInput = Input.GetAxis("Vertical") + Input.GetAxis("Horizontal");
        if(movementInput<0)
        {
            movementInput*=-1; //always positive
        }

        if(isFacingUp)
        {
            anim.SetFloat("InputMovement", movementInput);
            anim.SetBool("FacingUp", true);

            anim.SetBool("FacingDown", false);
            anim.SetBool("FacingLeft", false);
            anim.SetBool("FacingRight", false);
        }
        if(isFacingDown)
        {
            anim.SetFloat("InputMovement", movementInput);
            anim.SetBool("FacingDown", true);

            anim.SetBool("FacingUp", false);
            anim.SetBool("FacingLeft", false);
            anim.SetBool("FacingRight", false);
        }
        if(isFacingRight)
        {
            anim.SetFloat("InputMovement", movementInput);
            anim.SetBool("FacingRight", true);

            anim.SetBool("FacingUp", false);
            anim.SetBool("FacingLeft", false);
            anim.SetBool("FacingDown", false);
        }
        if(isFacingLeft)
        {
            anim.SetFloat("InputMovement", movementInput);
            anim.SetBool("FacingLeft", true);

            anim.SetBool("FacingUp", false);
            anim.SetBool("FacingRight", false);
            anim.SetBool("FacingDown", false);

        }
    }
}


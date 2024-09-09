using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : PlayerScript, InputReceiver
{
    private GameController gameController;
    Rigidbody2D rb;

    public float movementSpeed = 5f;
    private Vector2 oriPos;


    void Start()
    {

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

    }
}

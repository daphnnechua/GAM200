using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private GameObject player;
    private InputReceiver inputReceiver;

    public void SetInputReceiver(InputReceiver receiver)
    {
        inputReceiver = receiver;
    }


    void FixedUpdate()
    {
        if(inputReceiver == null)
        {
            return;
        }

        float hori = Input.GetAxis("Horizontal");
        float verti = Input.GetAxis("Vertical");
        Vector2 movePos = new Vector2(hori, verti); 

        // Debug.Log(Input.GetAxis("Horizontal"));

        
        inputReceiver.Move(movePos);

        // Debug.Log($"hori: {hori}, vert: {verti}");
    }



}

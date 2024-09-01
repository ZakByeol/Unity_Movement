using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_GroundCheck : MonoBehaviour
{
    // cs 파일, Player_Movement 을 가져와서 public 함수와 Box Collider 를 사용해 점프 체크
    public Player_Movement player_Controller;

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player_Controller.gameObject)
        {
            return;
        }
        player_Controller.SetJumpBool(false);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player_Controller.gameObject)
        {
            return;
        }
        player_Controller.SetJumpBool(true);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == player_Controller.gameObject)
        {
            return;
        }
        player_Controller.SetJumpBool(false);
    }
}

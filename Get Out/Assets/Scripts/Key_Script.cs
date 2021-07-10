using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key_Script : MonoBehaviour
{
    public Door_Script door;
    public bool canMove;

    private void Update()
    {
        transform.Rotate(Vector3.up * 50 * Time.deltaTime);

        if(canMove)
        {
            transform.position =  Vector3.MoveTowards(transform.position, newPlayer.player_instance.keyOnCanvas.position, 10f * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            door.gotKey = true;
            canMove = true;
        }

        if(other.gameObject.tag == "KeyOnCanvas")
        {
            newPlayer.player_instance.key.enabled = true;
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSphere : MonoBehaviour
{
    public Player_Script player;
    public int count = 0;

    public List<Collider> overlaps = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            //player.enemies.Add(other.gameObject);
            overlaps.Add(other.gameObject.GetComponent<Collider>());
            count++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            //player.enemies.Remove(other.gameObject);
            overlaps.Remove(other.gameObject.GetComponent<Collider>());
            count--;
        }
    }
}

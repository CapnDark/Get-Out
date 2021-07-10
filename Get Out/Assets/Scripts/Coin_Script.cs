using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin_Script : MonoBehaviour
{
    public GameObject coinFx;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            coinFx.transform.parent = null;
            coinFx.SetActive(true);
            Destroy(coinFx, 1f);
            Destroy(gameObject);

            newPlayer.player_instance.coinCount++;
        }
    }
}

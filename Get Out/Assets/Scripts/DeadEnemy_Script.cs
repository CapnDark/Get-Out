using DitzeGames.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnemy_Script : MonoBehaviour
{
    private int number = 0;
    public GameObject enemy;

    Vector3 temp;
    int count = 0;

    private void Update()
    {
        //temp = transform.position;
        //temp.y = 0.6f;
        //transform.position = temp;
        if(enemy != null)
        {
            transform.LookAt(enemy.transform);
        }

        transform.Translate(transform.forward * 15 * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy" && count == 0)
        {
            count++;

            GameManager.manager.bodyHit_SFX.Play();
            CameraEffects.ShakeOnce();

            //StartCoroutine(GameManager.manager.DisableSFX());

            GameObject blood = Instantiate(newPlayer.player_instance.bloodPrefab, collision.contacts[0].point, Quaternion.identity);
            Destroy(blood, 1f);
            Destroy(collision.gameObject);
            newPlayer.player_instance.enemies.Remove(collision.gameObject);

            GameObject g = Instantiate(newPlayer.player_instance.enemyRagdoll, collision.gameObject.transform.position, collision.gameObject.transform.rotation);
            g.transform.Find("mixamorig:Hips").GetComponent<Rigidbody>().AddForce(transform.forward * 5000);

            Destroy(g, 4f);

            GameObject k = Instantiate(newPlayer.player_instance.enemyRagdoll, gameObject.transform.position, gameObject.transform.rotation);
            Destroy(gameObject);
            Destroy(k, 1f);
        }

        if (number == 0)
        {
            //number++;
            //Destroy(gameObject);
            //GameObject g = Instantiate(newPlayer.player_instance.enemyRagdoll, transform.position, transform.rotation);
            //Destroy(g, 1f);
        }

    }
}

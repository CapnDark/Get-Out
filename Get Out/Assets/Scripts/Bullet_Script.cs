using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using LionStudios;

public class Bullet_Script : MonoBehaviour
{
    public float speed;
    public bool playerBullet;

    private Vector3 temp;
    public GameObject enemy;
    public GameObject enemyRagdoll;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerBullet && enemy != null)
        {
            transform.LookAt(enemy.transform);
        }
        else
        {
            //transform.LookAt(Player_Script.instance.gameObject.transform);
            //transform.LookAt(newPlayer_Script.p_instance.gameObject.transform);
            //transform.LookAt(Player._instance.gameObject.transform);
            transform.LookAt(newPlayer.player_instance.gameObject.transform);
        }

        temp = transform.position;
        temp.y = 0.8f;
        transform.position = temp;

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("Player Hit");
            newPlayer.player_instance.playerHealth--;
            PlayerPrefs.SetInt("playerHealth", newPlayer.player_instance.playerHealth);

            //if(newPlayer.player_instance.playerHealth == 0)
            //{
            //    //Instantiate player Ragdoll;
            //    Analytics.Events.LevelFailed(GameManager.manager.lvlNo, PlayerPrefs.GetInt("coinCount"));
            //    Instantiate(GameManager.manager.playerRagdoll, newPlayer.player_instance.transform.position, newPlayer.player_instance.transform.rotation);
            //    Destroy(newPlayer.player_instance.gameObject);
            //    GameManager.manager.restartBttn.SetActive(true);
            //}
        }

        if(other.gameObject.tag == "Enemy" && playerBullet)
        {
            //Debug.Log(other.transform.name);
            //Player_Script.instance.enemies.Remove(this.gameObject);
            //newPlayer_Script.p_instance.enemies.Remove(other.gameObject);
            newPlayer.player_instance.enemies.Remove(other.gameObject);

            //Player._instance.sphere.overlaps.Remove(other.gameObject.GetComponent<Collider>());
            //Player._instance.sphere.count--;
            //Player._instance.enemies.Remove(other.GetComponent<newEnemy_AI>());
            GameObject g = Instantiate(enemyRagdoll, other.transform.position, other.transform.rotation);
            other.GetComponent<newEnemy_AI>().blood.SetActive(true);
            other.GetComponent<newEnemy_AI>().blood.transform.SetParent(null);
            Destroy(other.GetComponent<newEnemy_AI>().blood, 2f);
            Destroy(g, 4f);
            Destroy(other.gameObject);
        }

        if(playerBullet)
        {
            //Debug.Log(other.transform.name);
        }

        if(other.gameObject.tag != "PlayerSphere")
        {
            Destroy(gameObject);
        }
    }
}

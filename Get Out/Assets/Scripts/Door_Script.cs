using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzeGames.Effects;

public class Door_Script : MonoBehaviour
{
    public float doorSpeed;
    private bool doorOpen = false;
    public bool gotKey = false;

    public bool isLastDoor;
    public bool playerReached;
    public Vector3 endPos;

    public bool canKick = false;
    public bool kickable;
    public int force;
    public Transform pos;
    public Transform doorParent;
    public Collider col1;
    public Collider col2;

    public GameObject cube;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!kickable && doorOpen && transform.position.y < 2.8f)
        {
            transform.Translate(Vector3.up * doorSpeed * Time.deltaTime);
        }

        if(canKick && kickable)
        {
            rb.AddForceAtPosition(-transform.forward * force * Time.deltaTime, pos.position);
            //doorParent.Rotate(Vector3.left * force * Time.deltaTime);
        }

        if(playerReached)
        {
            newPlayer.player_instance.transform.position = Vector3.MoveTowards(newPlayer.player_instance.transform.position, endPos, 2f * Time.deltaTime);
        }

        //if (doorParent != null && doorParent.eulerAngles.x < 90)
        //{
        //    canKick = false;
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isLastDoor && gotKey && other.gameObject.tag == "Player")
        {
            GameManager.manager.confetti.SetActive(true);
            playerReached = true;
            newPlayer.player_instance.anim.SetTrigger("End");
            newPlayer.player_instance.cam.enabled = false;
        }

        if(other.gameObject.tag == "Player" && gotKey)
        {
            if(kickable)
            {
                CameraEffects.ShakeOnce();
            }
            doorOpen = true;
            cube.SetActive(false);
            canKick = true;

            if(col1 != null && col2 != null)
            {
                col1.enabled = false;
                col2.enabled = true;
                Destroy(gameObject, 1f);
            }

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy" && canKick)
        {
            Debug.Log("Sound");
            GameManager.manager.bodyHit_SFX.Play();
            //StartCoroutine(GameManager.manager.DisableSFX());

            GameObject blood = Instantiate(newPlayer.player_instance.bloodPrefab, collision.contacts[0].point, Quaternion.identity);
            Destroy(blood, 1f);

            newPlayer.player_instance.enemies.Remove(collision.gameObject);
            GameObject g = Instantiate(newPlayer.player_instance.enemyRagdoll, collision.gameObject.transform.position, collision.gameObject.transform.rotation);
            Destroy(collision.gameObject);
            Destroy(g, 2f);

            Destroy(this.gameObject, 1f);
        }
    }
}

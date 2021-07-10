using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hostage_Script : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private bool canRun;

    public GameObject enemy;
    public bool isSecure = false;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(newPlayer.player_instance.canMove == true)
        {
            canRun = true;
            agent.isStopped = false;
        }
        else
        {
            canRun = false;
            agent.isStopped = true;
        }

        if (enemy == null)
        {
            isSecure = true;
        }

        if(isSecure && canRun)
        {
            anim.SetBool("canRun", true);
            agent.SetDestination(newPlayer.player_instance.followPoint.position);
        }
        else
        {
            anim.SetBool("canRun", false);
        }
    }
}

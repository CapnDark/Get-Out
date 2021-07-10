using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class newEnemy_AI : MonoBehaviour, System.IComparable<newEnemy_AI>
{
    Animator anim;

    private NavMeshAgent agent;
    private Vector3 nxtPoint;
    private int noOfChild;
    private int rnd;
    private int rndRot;
    private float rotY;

    private bool endOfList;
    private bool isinFOV;
    private bool rotRight = true;

    private Vector3 lastSeen_Pos;
    private GameObject target;
    private Transform viewPoint;
    private float memory_Timer = 0f;

    public float rotSpeed;
    public float maxAngle;
    public float maxRadius;
    public float memory_Duration;

    public float distanceToPlayer;

    public float rotAngle;
    public bool rotEnemy;

    public bool canMove = true;
    public bool canFollow;
    public bool canShoot;

    public GameObject player;
    public GameObject wayPoints;
    public GameObject viewCone;
    public GameObject bulletPrefab;
    public GameObject muzzleSpark;
    public GameObject blood;

    public Transform raycastPoint;

    public Transform headObj;
    public Transform gunPoint;
    //public Transform hitPoint;

    public Material whiteMat;
    public Material redMat;

    public LayerMask layer;
    Vector3 temp;

    //public void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, maxRadius);

    //    Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, transform.up) * transform.forward * maxRadius;
    //    Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, transform.up) * transform.forward * maxRadius;

    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawRay(transform.position, fovLine1);
    //    Gizmos.DrawRay(transform.position, fovLine2);

    //    if (!isinFOV)
    //        Gizmos.color = Color.white;
    //    else
    //        Gizmos.color = Color.red;
    //    Gizmos.DrawRay(transform.position, (player.transform.position - transform.position).normalized * maxRadius);

    //    Gizmos.color = Color.black;
    //    Gizmos.DrawRay(transform.position, transform.forward * maxRadius);
    //}


    public void OnDrawGizmos()
    {
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(viewPoint.position, maxRadius);

        //Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, headObj.up) * viewPoint.forward * maxRadius;
        //Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, headObj.up) * viewPoint.forward * maxRadius;

        //Gizmos.color = Color.blue;
        //Gizmos.DrawRay(viewPoint.position, fovLine1);
        //Gizmos.DrawRay(viewPoint.position, fovLine2);

        //if (!isinFOV)
        //    Gizmos.color = Color.white;
        //else
        //    Gizmos.color = Color.red;
        //Gizmos.DrawRay(viewPoint.position, (player.transform.position - viewPoint.position).normalized * maxRadius);

        //Gizmos.color = Color.black;
        //Gizmos.DrawRay(viewPoint.position, viewPoint.forward * maxRadius);
    }

    public bool inFOV(Transform checkingObj, Transform player, float maxAngle, float maxRadius)
    {
        Collider[] overlaps = new Collider[5];
        int count = Physics.OverlapSphereNonAlloc(checkingObj.position, maxRadius, overlaps, layer);

        for (int i = 0; i < count + 1; i++)
        {
            if (overlaps[i] != null)
            {
                if (overlaps[i].transform == player)
                {
                    Vector3 dirBetween = (player.position - checkingObj.position).normalized;
                    dirBetween.y *= 0f;

                    float angle = Vector3.Angle(checkingObj.forward, dirBetween);
                    //Debug.Log(angle + checkingObj.name);

                    if (angle <= maxAngle)
                    {
                        Ray ray = new Ray(checkingObj.position, player.position - checkingObj.position);
                        //Debug.DrawRay(checkingObj.position, player.position - checkingObj.position, Color.green);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, maxRadius))
                        {
                            if (hit.transform == player)
                            {
                                target = player.gameObject;
                                lastSeen_Pos = player.transform.position;

                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    void Awake()
    {
        viewPoint = headObj;

        noOfChild = 0;
        agent = GetComponent<NavMeshAgent>();
        if(wayPoints != null)
        {
            nxtPoint = wayPoints.transform.GetChild(noOfChild).transform.position;
        }
        viewCone.GetComponent<MeshRenderer>().material = whiteMat;
    }

    void Start()
    {
        anim = GetComponent<Animator>();

        //if (!rotEnemy)
        //{
        //    //StartCoroutine(RandomDelay1());
        //    //InvokeRepeating("RandomDelay", 2f, 2f);
        //}
    }

    void Update()
    {
        if(player != null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        }

        //temp = transform.position;
        //temp.y = 0.05f;
        //transform.position = temp;

        //Delay();
        if (rotEnemy)
        {
            viewPoint = headObj;
            anim.SetBool("Idle", true);
            canMove = false;
            canFollow = false;
            //RotateEnemy();
        }

        if (canShoot)
        {
            viewPoint = this.transform;
            anim.SetBool("canShoot", true);
        }
        else
        {
            viewPoint = headObj;
            anim.SetBool("canShoot", false);
        }

        if (canMove && !canFollow && !rotEnemy)
        {
            //anim.SetBool("canMove", true);
            Patroling();
        }
        else if(!canMove && !rotEnemy)
        {
            //anim.SetBool("Idle", true);
            agent.isStopped = true;
            transform.LookAt(player.transform);
        }
        else if(canMove)
        {
            agent.isStopped = false;
        }

        isinFOV = inFOV(viewPoint, player.transform, maxAngle, maxRadius);

        if(isinFOV)
        {
            viewCone.GetComponent<MeshRenderer>().material = redMat;
            //anim.SetBool("canShoot", true);

            canMove = false;
            canFollow = true;
            canShoot = true;
        }
        else
        {
            viewCone.GetComponent<MeshRenderer>().material = whiteMat;
            //anim.SetBool("canShoot", false);

            target = null;

            if(!rotEnemy)
            {
                canMove = true;
            }
            canShoot = false;
        }

        if (canFollow /*&& !canShoot */&& !rotEnemy)
        {
            //anim.SetBool("canMove", true);
            canMove = true;
            Follow();
        }

        if (newPlayer.player_instance.playerHealth < 1)
        {
            canShoot = false;
            agent.isStopped = true;
        }
    }

    void Patroling()
    {
        agent.SetDestination(nxtPoint);

        if(Vector3.Distance(transform.position, nxtPoint) <= 0.5f)
        {
            if(!endOfList)
            {
                noOfChild++;

                if (noOfChild > wayPoints.transform.childCount-1)
                    endOfList = true;
                else
                    nxtPoint = wayPoints.transform.GetChild(noOfChild).transform.position;
            }
            else
            {
                noOfChild--;

                if (noOfChild < 0)
                    endOfList = false;
                else
                    nxtPoint = wayPoints.transform.GetChild(noOfChild).transform.position;
            }
        }
    }

    void Follow()
    {
        Vector3 targ = target ? target.transform.position : lastSeen_Pos;

        if(target == null && memory_Duration > 0.1f)
        {
            memory_Timer += Time.deltaTime;
            {
                if(memory_Timer < memory_Duration)
                {
                    // Follow to last know Location.

                    //canFollow = true;
                    lastSeen_Pos = player.transform.position;
                    targ = lastSeen_Pos;
                }
                else
                {
                    canFollow = false;
                }
            }
        }

        if(target != null)
        {
            // Shoot.
            //canShoot = true;
            //canFollow = true;
            canMove = false;

            memory_Timer = 0f;
            lastSeen_Pos = player.transform.position;
            targ = lastSeen_Pos;
        }

        if(canFollow && canMove)
        {
            agent.SetDestination(targ);
        }
    }

    void RotateEnemy()
    {
        if (rotY > rotAngle)
        {
            rotRight = false;
        }
        else if (rotRight)
        {
            rotY += Time.deltaTime * 30f;
            transform.rotation = Quaternion.Euler(0f, rotY, 0f);
        }


        if (rotY < -rotAngle)
        {
            rotRight = true;
        }
        else if (!rotRight)
        {
            rotY -= Time.deltaTime * 30f;
            transform.rotation = Quaternion.Euler(0f, rotY, 0f);
        }
    }

    void EnemyShoot()
    {
        GameObject g = Instantiate(bulletPrefab, gunPoint.position, gunPoint.rotation);
        GameObject spark = Instantiate(muzzleSpark, gunPoint.position, gunPoint.rotation);
        Destroy(spark, 1f);
        Destroy(g, 2f);
    }

    void EndIdle()
    {
        Debug.Log("EndIdle");
        //anim.SetBool("Idle", false);

        canMove = true;
        agent.isStopped = false;
    }

    //IEnumerator RandomDelay1()
    //{
    //    Debug.Log("Delay");
    //    if(canMove && !canFollow)
    //    {
    //        yield return new WaitForSeconds(2f);
    //        rnd = Random.Range(0, 101);
    //        if (rnd < 30)
    //        {
    //            anim.SetBool("Idle", true);
    //            //anim.SetBool("canMove", false);

    //            canMove = false;
    //            agent.isStopped = true;
    //        }
    //    }

    //    yield return new WaitForSeconds(1f);
    //    StartCoroutine(RandomDelay1());
    //}

    void RandomDelay()
    {
        //Debug.Log("Delay");
        //anim.SetBool("Idle", false);

        //if (canMove && !canFollow)
        //{
        //    //yield return new WaitForSeconds(2f);
        //    rnd = Random.Range(0, 101);
        //    if (rnd < 30)
        //    {
        //        anim.SetBool("Idle", true);
        //        //anim.SetBool("canMove", false);

        //        canMove = false;
        //        agent.isStopped = true;
        //    }
        //    else
        //    {
        //        anim.SetBool("Idle", false);
        //        //anim.SetBool("canMove", true);

        //        canMove = true;
        //        agent.isStopped = false;
        //    }
        //}

        //InvokeRepeating("RandomDelay", 2f, 2f);
    }

    public int CompareTo(newEnemy_AI other)
    {
        if (distanceToPlayer - other.distanceToPlayer > 0)
        {
            return 1;
        }
        else if (distanceToPlayer - other.distanceToPlayer == 0)
        {
            return 0;
        }
        else
        {
            return -1;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Player_Script : MonoBehaviour
{
    public static Player_Script instance;

    public VariableJoystick joystick;
    public LayerMask layer;
    public GameObject muzzleSpark;

    public GameObject bulletPrefab;
    public Transform playerShootPoint;
    public Transform playerHead;

    public float speed;
    public float maxAngle;
    public float maxRadius;

    private bool canMove;
    private Rigidbody rb;
    private Animator anim;

    public bool isinFOV;
    private bool obstacle;

    public List<GameObject> enemies = new List<GameObject>();
    public GameObject enemyLocked;

    private Vector3 startPos;
    private Vector3 endPoos;
    private float dist;
    public float minDist;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            endPoos = Input.mousePosition;
            dist = Vector3.Distance(startPos, endPoos);

            canMove = true;
            anim.SetBool("canMove", true);
        }
        else
        {
            canMove = false;
            anim.SetBool("canMove", false);
        }

        if (canMove && dist>minDist && enemyLocked == null)
        {
            Vector3 direction = Vector3.forward * joystick.Vertical + Vector3.right * joystick.Horizontal;
            //transform.Translate(direction * speed * Time.deltaTime);
            rb.velocity = direction * speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        isinFOV = inFOV(transform, maxAngle, maxRadius);

        if (isinFOV)
        {
            anim.SetBool("playerShoot", true);
            transform.LookAt(enemyLocked.transform);
            CheckEnemy();
        }
        else
        {
            anim.SetBool("playerShoot", false);
            //enemyLocked = null;
        }

    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRadius);

        Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle/2, transform.up) * transform.forward * maxRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle/2, transform.up) * transform.forward * maxRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        if (!isinFOV)
            Gizmos.color = Color.white;
        else
            Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, (enemyLocked.transform.position - transform.position).normalized * maxRadius);

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * maxRadius);
    }

    public bool inFOV(Transform checkingObj, /*Transform player,*/ float maxAngle, float maxRadius)
    {
        Collider[] overlaps = new Collider[10];
        int count = Physics.OverlapSphereNonAlloc(checkingObj.position, maxRadius, overlaps, layer);

        for (int i = 0; i < count + 1; i++)
        {
            if (overlaps[i] != null)
            {
                if(!enemies.Contains(overlaps[i].gameObject))
                {
                    enemies.Add(overlaps[i].gameObject);
                }
                
                for(int x = 0; x<enemies.Count; x++)
                {
                    if(enemies[x] == null)
                    {
                        enemies.RemoveAt(x);
                    }
                    Vector3 dirBetween = (enemies[x].transform.position - checkingObj.position).normalized;
                    dirBetween.y *= 0f;

                    float angle = Vector3.Angle(checkingObj.forward, dirBetween);
                    //Debug.Log(angle);
                    Debug.Log("Inside");


                    if (angle <= maxAngle)
                    {

                        Ray ray = new Ray(checkingObj.position, enemies[x].transform.position - checkingObj.position);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, maxRadius))
                        {
                            //Debug.Log(hit.transform.name);
                            if (hit.transform == enemies[x].transform)
                            {
                                //PLayer Shoot
                                enemyLocked = enemies[x];
                                return true;

                            }
                        }
                    }
                }
            }
        }

        enemyLocked = null;
        return false;
    }

    void CheckEnemy()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.forward, Color.green);

        if(Physics.Raycast(ray, out hit, maxRadius))
        {
            if(hit.transform.tag == "Obstacles")
            {
                isinFOV = false;
            }
        }
    }

    void PlayerShoot()
    {
        GameObject g = Instantiate(bulletPrefab, playerShootPoint.position, playerShootPoint.rotation);
        g.GetComponent<Bullet_Script>().enemy = enemyLocked;
        GameObject spark = Instantiate(muzzleSpark, playerShootPoint.position, Quaternion.identity);
        Destroy(g, 2f);
        Destroy(spark, 1f);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}

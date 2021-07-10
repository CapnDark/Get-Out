using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class newPlayer_Script : MonoBehaviour
{
    public static newPlayer_Script p_instance;
    public VariableJoystick joystick;
    public LayerMask layer;

    public GameObject muzzleSpark;
    public GameObject bulletPrefab;

    public Transform playerShootPoint;
    public Transform raycastPoint;

    public float speed;
    public float maxAngle;
    public float maxRadius;

    private bool canMove;
    private bool canShoot;
    public bool isinFOV;

    private Animator anim;
    private Rigidbody rb;

    private Vector3 startPos;
    private Vector3 endPos;
    private float dist;
    public float minDist;

    public GameObject enemyLocked;

    public List<GameObject> enemies = new List<GameObject>();

    private void Awake()
    {
        if (p_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            p_instance = this;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
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
        Gizmos.DrawRay(transform.position, (enemyLocked.transform.position - transform.position).normalized * maxRadius);

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * maxRadius);
    }

    public bool inFOV(Transform checkingObj, Transform enemy, float maxAngle, float maxRadius)
    {
        Collider[] overlaps = new Collider[5];
        int count = Physics.OverlapSphereNonAlloc(checkingObj.position, maxRadius, overlaps, layer);

        //Debug.Log(count);
        if(count != 0)
        {
            for (int i = 0; i < count + 1; i++)
            {
                if (overlaps[i] != null)
                {
                    if (overlaps[i].transform == enemy)
                    {
                        Vector3 dirBetween = (enemy.position - checkingObj.position).normalized;
                        dirBetween.y *= 0f;

                        float angle = Vector3.Angle(checkingObj.forward, dirBetween);
                        Debug.Log(angle);
                        //Debug.Log(angle + checkingObj.name);

                        if (angle <= maxAngle)
                        {
                            Ray ray = new Ray(checkingObj.position, enemy.position - checkingObj.position);
                            Debug.DrawRay(checkingObj.position, (enemy.position - checkingObj.position).normalized * maxRadius, Color.green);
                            RaycastHit hit;

                            if (Physics.Raycast(ray, out hit, maxRadius))
                            {
                                Debug.Log("Raycast");
                                if (hit.transform == enemy)
                                {
                                    //Debug.Log("Inside");
                                    enemyLocked = enemy.gameObject;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    void CheckEnemy()
    {
        Collider[] overlaps = new Collider[5];
        int count = Physics.OverlapSphereNonAlloc(transform.position, maxRadius, overlaps, layer);

        if(count <= 0)
        {
            Debug.Log("No Enemy");
            enemyLocked = null;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            endPos = Input.mousePosition;
            dist = Vector3.Distance(startPos, endPos);

            canMove = true;
            anim.SetBool("canMove", true);
        }
        else
        {
            canMove = false;
            anim.SetBool("canMove", false);
        }

        if (canMove && dist > minDist && !isinFOV)
        {
            Vector3 direction = Vector3.forward * joystick.Vertical + Vector3.right * joystick.Horizontal;
            //transform.Translate(direction * speed * Time.deltaTime);
            rb.velocity = direction * speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        if(enemies.Count > 0)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemyLocked == null)
                {
                    isinFOV = inFOV(transform, enemies[i].transform, maxAngle, maxRadius);
                }
            }
        }
        else
        {
            isinFOV = false;
        }
        CheckEnemy();

        if (isinFOV)
        {
            anim.SetBool("playerShoot", true);
            transform.LookAt(enemyLocked.transform);
            //CheckEnemy();
        }
        else
        {
            anim.SetBool("playerShoot", false);
            enemyLocked = null;
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

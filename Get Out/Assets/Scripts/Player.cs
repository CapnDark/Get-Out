using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player _instance;
    public PlayerSphere sphere;
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

    public List<newEnemy_AI> enemies = new List<newEnemy_AI>();

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        StartCoroutine(SortDelay());
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, maxRadius);

        Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle / 2, transform.up) * transform.forward * maxRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle / 2, transform.up) * transform.forward * maxRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        if (!isinFOV)
            Gizmos.color = Color.white;
        else
            Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, (enemyLocked.transform.position - transform.position).normalized * maxRadius);

        Gizmos.color = Color.black;
        //Gizmos.DrawRay(transform.position, transform.forward * maxRadius);
    }

    public bool inFOV(Transform checkingObj, Transform enemy, float maxAngle, float maxRadius)
    {
        //Collider[] overlaps = new Collider[enemies.Count];
        //int count = Physics.OverlapSphereNonAlloc(checkingObj.position, maxRadius, overlaps, layer);

        //Debug.Log(sphere.count);
        if(sphere.count != 0)
        {
            for (int i = 0; i < sphere.count; i++)
            {
                if (sphere.overlaps[i] != null)
                {
                    if (sphere.overlaps[i].transform == enemy)
                    {
                        Vector3 dirBetween = (enemy.position - checkingObj.position).normalized;
                        dirBetween.y *= 0f;

                        float angle = Vector3.Angle(checkingObj.forward, dirBetween);
                        //Debug.Log(angle);
                        //Debug.Log(angle + checkingObj.name);

                        if (angle <= maxAngle)
                        {
                            Debug.Log("Inside");
                            transform.LookAt(enemy.transform);

                            //Ray ray = new Ray(raycastPoint.position, enemy.GetComponent<newEnemy_AI>().raycastPoint.position - raycastPoint.position);
                            //Debug.DrawRay(raycastPoint.position, (enemy.GetComponent<newEnemy_AI>().raycastPoint.position - raycastPoint.position).normalized * maxRadius, Color.green);

                            Ray ray = new Ray(raycastPoint.position, transform.forward);
                            Debug.DrawRay(raycastPoint.position, transform.forward .normalized * maxRadius, Color.green);
                            RaycastHit hit;

                            if (Physics.Raycast(ray, out hit, maxRadius))
                            {
                                //Debug.Log("Ray");
                                if (hit.transform.tag == "Enemy")
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

    //void RaycastObj()
    //{
    //    Ray ray = new Ray(raycastPoint.position, transform.forward);
    //    Debug.DrawRay(raycastPoint.position, transform.forward.normalized * maxRadius, Color.red);
    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        Debug.Log("Ray");
    //        Debug.Log(hit.transform.name);
    //        if (hit.transform.tag == "Enemy")
    //        {
    //            Debug.Log("Enemy");
    //            //Debug.Log("Inside");
    //            //enemyLocked = enemy.gameObject;
    //        }
    //    }
    //}

    void CheckEnemy()
    {
        Collider[] overlaps = new Collider[5];
        int count = Physics.OverlapSphereNonAlloc(transform.position, maxRadius, overlaps, layer);

        if (count <= 0)
        {
            Debug.Log("No Enemy");
            enemyLocked = null;
        }
    }

    void Update()
    {
        //RaycastObj();

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

        if (enemyLocked != null)
        {
            anim.SetBool("playerShoot", true);
            //transform.LookAt(enemyLocked.transform);
            //CheckEnemy();
        }
        else
        {
            anim.SetBool("playerShoot", false);
            //enemyLocked = null;
        }

        if (canMove && dist > minDist && !isinFOV)
        {
            Vector3 direction = Vector3.forward * joystick.Vertical + Vector3.right * joystick.Horizontal;
            //transform.Translate(direction * speed * Time.deltaTime);
            rb.velocity = direction * speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        if (enemies.Count > 0)
        {
            isinFOV = inFOV(transform, enemies[0].transform, maxAngle, maxRadius);
        }
        else
        {
            isinFOV = false;
            enemyLocked = null;
        }
        //CheckEnemy();


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

    IEnumerator SortDelay()
    {
        yield return new WaitForSeconds(0.1f);
        enemies.Sort();
        StartCoroutine(SortDelay());
    }
}

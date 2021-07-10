using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using LionStudios;

public class newPlayer : MonoBehaviour
{
    public static newPlayer player_instance;
    public Camera_Follow cam;
    public VariableJoystick joystick;
    public LayerMask layer;

    public GameObject muzzleSpark;
    public GameObject bulletPrefab;

    public Transform playerShootPoint;
    public Transform raycastPoint;

    private float speed = 180f; //180, 450
    public float maxAngle;
    public float maxRadius;

    public bool canMove;
    private bool canThrow;
    private bool canPick = true;
    private bool isinFOV;

    public Animator anim;
    private Rigidbody rb;

    private Vector3 startPos;
    private Vector3 endPos;
    private float dist;
    public float minDist;

    private GameObject enemyLocked;
    private Transform enemy_;
    public Transform followPoint;
    public GameObject enemyOnFloor;

    public List<GameObject> enemies = new List<GameObject>();
    public GameObject enemyInHand;
    public GameObject enemyRagdoll;
    public GameObject deathPose;
    public GameObject bloodPrefab;

    public Transform keyOnCanvas;
    public Image key;
    public Text coinTxt;

    public int coinCount;
    public int playerHealth;

    //private bool isSearch = true;
    Vector3 temp;

    private void Awake()
    {
        Debug.Log("Player Script");
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            PlayerPrefs.SetInt("coinCount", 0);
            PlayerPrefs.SetInt("playerHealth", 3);
        }


        //Debug.Log("Event LvlRestart");
        //Analytics.Events.LevelStarted(SceneManager.GetActiveScene().buildIndex, PlayerPrefs.GetInt("coinCount"));

        //playerHealth = PlayerPrefs.GetInt("playerHealth");
        coinCount = PlayerPrefs.GetInt("coinCount");
        if (player_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            player_instance = this;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        GameManager.manager.adjust.Adjust_LvlStart();
        GameManager.manager.adjust.LogAchieveLevelEvent(SceneManager.GetActiveScene().name);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRadius);

        Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, transform.up) * transform.forward * maxRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, transform.up) * transform.forward * maxRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        //if (!isinFOV)
        //    Gizmos.color = Color.white;
        //else
        //    Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, (enemyLocked.transform.position - transform.position).normalized * maxRadius);

        //Gizmos.color = Color.black;
        //Gizmos.DrawRay(transform.position, transform.forward * maxRadius);
    }

    //public bool inFOV(Transform checkingObj, Transform enemy, float maxAngle, float maxRadius)
    //{
    //    Collider[] overlaps = new Collider[5];
    //    int count = Physics.OverlapSphereNonAlloc(checkingObj.position, maxRadius, overlaps, layer);

    //    Debug.Log(count);
    //    if(count != 0)
    //    {
    //        for (int i = 0; i < count + 1; i++)
    //        {
    //            if (overlaps[i] != null)
    //            {
    //                if (overlaps[i].transform == enemy)
    //                {
    //                    Vector3 dirBetween = (enemy.position - checkingObj.position).normalized;
    //                    dirBetween.y *= 0f;

    //                    float angle = Vector3.Angle(checkingObj.forward, dirBetween);
    //                    //Debug.Log(angle + checkingObj.name);

    //                    if (angle <= maxAngle)
    //                    {
    //                        Ray ray = new Ray(checkingObj.position, enemy.position - checkingObj.position);
    //                        Debug.DrawRay(checkingObj.position, (enemy.position - checkingObj.position).normalized * maxRadius, Color.green);
    //                        RaycastHit hit;

    //                        if (Physics.Raycast(ray, out hit, maxRadius))
    //                        {
    //                            if (hit.transform == enemy)
    //                            {
    //                                //Debug.Log("Inside");
    //                                enemyLocked = enemy.gameObject;
    //                                return true;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    Debug.Log("FOV return false");
    //    return false;
    //}

    public bool inFOV(Transform checkingObj, Transform enemy, float maxAngle, float maxRadius)
    {
        Collider[] overlaps = new Collider[5];
        int count = Physics.OverlapSphereNonAlloc(checkingObj.position, maxRadius, overlaps, layer);

        //Transform enemy_;
        Collider[] colliders = Physics.OverlapSphere(checkingObj.position, maxRadius, layer);

        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<newEnemy_AI>())
            {
                enemy_ = collider.transform;
                Vector3 dirBetween = (enemy_.position - checkingObj.position).normalized;
                dirBetween.y *= 0f;

                float angle = Vector3.Angle(checkingObj.forward, dirBetween);
                //Debug.Log(angle);

                if (angle <= maxAngle)
                {
                    Ray ray = new Ray(raycastPoint.position, enemy_.position - raycastPoint.position);
                    Debug.DrawRay(raycastPoint.position, (enemy_.position - raycastPoint.position).normalized * maxRadius, Color.green);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, maxRadius))
                    {
                        //Debug.Log(hit.transform.name);
                        if (hit.transform == enemy_)
                        {
                            enemyLocked = enemy_.gameObject;
                            //isSearch = false;

                            return true;
                        }
                    }
                }
            }
        }

        //Debug.Log("FOV return false");
        return false;
    }

    void CheckEnemy()
    {
        Collider[] overlaps = new Collider[5];
        int count = Physics.OverlapSphereNonAlloc(transform.position, maxRadius, overlaps, layer);

        if (count == 0)
        {
            Debug.Log("No Enemy");
            enemyLocked = null;
        }
    }

    void Update()
    {
        if (newPlayer.player_instance.playerHealth < 1)
        {
            //Instantiate player Ragdoll;
            Debug.Log("Event LvlFailed");
            GameManager.manager.adjust.Adjust_LvlFail();

            //Analytics.Events.LevelFailed(GameManager.manager.lvlNo, PlayerPrefs.GetInt("coinCount"));
            Instantiate(GameManager.manager.playerRagdoll, newPlayer.player_instance.transform.position, newPlayer.player_instance.transform.rotation);
            Destroy(newPlayer.player_instance.gameObject);
            GameManager.manager.restartBttn.SetActive(true);
        }

        Vector3 tmp = transform.position;
        tmp.x = Mathf.Clamp(tmp.x, -3.6f, 3.6f);
        tmp.z = Mathf.Clamp(tmp.z, 25.75f, 55f);
        transform.position = tmp;

        coinTxt.text = coinCount.ToString();

        if(enemies.Count == 0)
        {
            isinFOV = false;
        }

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

        if (/*isSearch &&*/ enemies.Count != 0)
        {
            isinFOV = inFOV(transform, enemies[0].transform, maxAngle, maxRadius);
        }
        else
        {
            canPick = false;
        }

        //CheckEnemy();

        if (isinFOV)
        {
            transform.LookAt(enemy_.transform);
            if(canThrow)
            {
                anim.SetBool("canThrow", true);
            }
            else
            {
                anim.SetBool("playerShoot", true);
            }
        }
        else
        {
            anim.SetBool("playerShoot", false);
            //enemyLocked = null;
        }
    }

    void PlayerShoot()
    {
        GameObject g = Instantiate(bulletPrefab, playerShootPoint.position, playerShootPoint.rotation);
        g.GetComponent<Bullet_Script>().enemy = enemyLocked;
        GameObject spark = Instantiate(muzzleSpark, playerShootPoint.position, Quaternion.identity);
        Destroy(g, 2f);
        Destroy(spark, 1f);

        enemy_ = null;

        //isSearch = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void EnmeyInHand_Enable()
    {
        canThrow = true;
        enemyInHand.SetActive(true);
        Destroy(enemyOnFloor);
    }

    public void EnemyThrow()
    {
        enemyInHand.SetActive(false);
        GameObject g = Instantiate(deathPose, enemyInHand.transform.position, enemyInHand.transform.rotation);
        g.GetComponent<DeadEnemy_Script>().enemy = enemyLocked;
        //g.GetComponent<Rigidbody>().AddForce(transform.forward * 25000 * Time.deltaTime);
        //Destroy(g, 1f);
    }

    public void EndEnemyThrow()
    {
        anim.SetBool("canThrow", false);
        anim.SetBool("pickUp 0", false);

        canThrow = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "EnemyRagdoll" && canPick)
        {
            //canThrow = true;
            enemyOnFloor = collision.gameObject.transform.root.gameObject;
            //anim.SetTrigger("pickUp");
            anim.SetBool("pickUp 0", true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Finish")
        {
            PlayerPrefs.SetInt("playerHealth", playerHealth);
            PlayerPrefs.SetInt("coinCount", coinCount);
            Debug.Log("Event LvlComplete");
            GameManager.manager.adjust.Adjust_LvlComplete();
            //Analytics.Events.LevelComplete(GameManager.manager.lvlNo, PlayerPrefs.GetInt("coinCount"));

            if(GameManager.manager.lvlNo == 4)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                SceneManager.LoadScene(GameManager.manager.lvlNo + 1);
            }
        }
    }
}

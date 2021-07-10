using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using LionStudios;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    public AdjustScript adjust;
    public GameObject playerRagdoll;

    public GameObject restartBttn;
    public int lvlNo;
    public AudioSource bodyHit_SFX;
    public GameObject confetti;

    private void Awake()
    {
        Debug.Log("GameManager Script");
        lvlNo = SceneManager.GetActiveScene().buildIndex;
        if (manager != null)
        {
            Destroy(gameObject);
        }
        else
        {
            manager = this;
        }
    }

    public void Restart()
    {
        PlayerPrefs.SetInt("playerHealth", 3);
        Debug.Log("Restart Event");
        adjust.Adjust_LvlRestart();
        //Analytics.Events.LevelRestart(lvlNo, PlayerPrefs.GetInt("coinCount"));
        SceneManager.LoadScene(lvlNo);
    }

    public IEnumerator DisableSFX()
    {
        yield return new WaitForSeconds(0.2f);
        bodyHit_SFX.enabled = false;
    }
}

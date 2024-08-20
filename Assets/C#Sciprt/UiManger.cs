using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UiManger : MonoBehaviour
{
    private static UiManger instance;
   public static UiManger ui_instance
    {
        get
        {
            if(instance == null)
            instance = FindObjectOfType<UiManger>();
            return instance;
        }
    }
   
    public Transform hudUi;
    private Text ammo;
    public Text scoreText;
    public GameObject GameoverUi;
    public Text WaveText;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        ammo = hudUi.GetChild(0).GetChild(0).GetComponent<Text>();
       
    }
    public void AmmoTextUpdate(int magAmmo, int AmmoRemain)
    {
        ammo.text = $"{magAmmo}/{AmmoRemain}";
    }
    public void UpdateScoreText(int newScore)
    {
        scoreText.text = "Score :" + newScore;
    }
    public void UpdateWaveText(int waves, int count)
    {
       WaveText.text = "Wave"+ waves +" EnemyLeft :" + count;
    }
    public void SetActiveGameOverUI(bool active)
    {
        GameoverUi.SetActive(active);
    }
    public void GameRestart()
    {                                // 자기 자신 씬 이름으로 다시 재생
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        GameoverUi.gameObject.SetActive(false);
        scoreText.text = "Score : 0";
    }
}

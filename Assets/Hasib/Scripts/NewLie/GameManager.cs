using Cinemachine;
using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    [SerializeField]
    GameObject canvas;
    public bool IsPlayerOn ;
    [SerializeField]
    GameObject lieGameManager;
    [SerializeField]
    GameObject playerCam;
    [SerializeField]
    GameObject player;
    bool inUIMode;
    [SerializeField]
    CinemachineVirtualCamera hoodlumCam;

    public ThirdPersonController controller;
    Animator animator;

    [SerializeField]
    StarterAssetsInputs starterAssetsInputs;
    [SerializeField]
    GameObject Restart;

    [SerializeField]
    GameObject uiPanelPass;
    [SerializeField]
    GameObject uiPanelFail;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        animator = controller.GetComponent<Animator>();
        IsPlayerOn = true;
        // Stop current animation and play default state (e.g., "Idle" or base locomotion state)
       
    }
    public void EnableLieGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        canvas.SetActive(true);
        lieGameManager.SetActive(true);
        //playerCam.SetActive(false);
        //player.SetActive(false);

       DisablePlayer();
        
        hoodlumCam.Priority = 13;


    }

    public void ReloadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    public void DisableLieGame(bool result)
    {
        canvas.SetActive(false);
        lieGameManager.SetActive(false);
        //playerCam.SetActive(true);
        //player.SetActive(true);
        controller.enabled = true;
        //hoodlumCam.Priority = 0;
        EnablePlayer();
        

        if (result)
        {
            StartCoroutine(ShowPanelWithDelay(uiPanelPass));
        }
        else
        {
            StartCoroutine(ShowPanelWithDelay(uiPanelFail));
        }

    }
    IEnumerator ShowPanelWithDelay(GameObject panel)
    {
        yield return new WaitForSeconds(1f);
        panel.SetActive(true);
        Restart.SetActive(true);
    }

    public void EnablePlayer()
    {
        


        
        IsPlayerOn = false;
        controller.enabled = true;
    }

    public void DisablePlayer() {
      
        animator.SetFloat("Speed", 0f);
        animator.SetFloat("MotionSpeed", 0f);


        controller.enabled = false; // disable the controller entirely
        IsPlayerOn = true;
    }

   public  bool IsSprinting()
    {
        return starterAssetsInputs.sprint;
    }
}

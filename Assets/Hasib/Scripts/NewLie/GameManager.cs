using Cinemachine;
using StarterAssets;
using UnityEngine;
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
    public void DisableLieGame()
    {
        canvas.SetActive(false);
        lieGameManager.SetActive(false);
        //playerCam.SetActive(true);
        //player.SetActive(true);
        controller.enabled = true;
        hoodlumCam.Priority = 0;
        EnablePlayer();
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

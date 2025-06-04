using Cinemachine;
using StarterAssets;
using UnityEngine;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject canvas;

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
    private void Start()
    {
        animator = controller.GetComponent<Animator>();

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

        animator.SetFloat("Speed", 0f);
        animator.SetFloat("MotionSpeed", 0f);


        controller.enabled = false; // disable the controller entirely
        
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
    }
}

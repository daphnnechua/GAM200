using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManualController : MonoBehaviour
{
    [SerializeField] private GameObject tutorialManual;
    [SerializeField] private Button closeButton;
    public bool isInteracting = false;

    private GameObject player;

    private GameController gameController;
    private TabController tabController;

    private MasterController masterController;

    // Start is called before the first frame update
    void Start()
    {
        masterController = FindObjectOfType<MasterController>();

        player = GameObject.FindWithTag("Player");

        gameController = FindObjectOfType<GameController>();

        closeButton.onClick.AddListener(() => CloseManual());

        tutorialManual.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            player.GetComponent<PlayerMovement>().canMove = false;

            masterController.canPause = false;
            isInteracting = true;
            tutorialManual.SetActive(true);
            tabController = FindObjectOfType<TabController>();
            tabController.UpdateTabVisuals(0);
            
        }
        if(Input.GetKeyDown(KeyCode.Escape) && isInteracting)
        {
            CloseManual();
        }

    }

    private void CloseManual()
    {
        tutorialManual.SetActive(false);
        isInteracting = false;
        masterController.canPause = true;

        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        player.GetComponent<PlayerMovement>().canMove = true;
    }
}
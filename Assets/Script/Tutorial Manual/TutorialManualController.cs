using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManualController : MonoBehaviour
{
    [SerializeField] private GameObject tutorialManual;
    [SerializeField] private Button closeButton;

    [SerializeField] private Button tutorialManualButton;

    [SerializeField] private GameObject droneMenu;
    [SerializeField] private GameObject dialogueInterface;
    public bool isInteracting = false;

    private GameObject player;

    private GameController gameController;
    private TabController tabController;

    private MasterController masterController;
    
    [SerializeField] private List<PageToggle> journalSections;

    [SerializeField] private List<AudioClip> clickButtonSound;

    // Start is called before the first frame update
    void Start()
    {
        masterController = FindObjectOfType<MasterController>();

        player = GameObject.FindWithTag("Player");

        gameController = FindObjectOfType<GameController>();

        closeButton.onClick.AddListener(() => CloseManual());

        tutorialManualButton.onClick.AddListener(() => OpenManual());

        tutorialManual.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H) && !masterController.pauseMenuOpen)
        {
            if(!droneMenu.activeInHierarchy && !dialogueInterface.activeInHierarchy)
            {
                OpenManual();
            }
            
            
        }
        if(Input.GetKeyDown(KeyCode.Escape) && isInteracting)
        {
            tutorialManual.SetActive(false);
            isInteracting = false;
            masterController.canPause = true;

            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            player.GetComponent<PlayerMovement>().canMove = true;
        }

    }

    private void OpenManual()
    {
        if(!droneMenu.activeInHierarchy && !dialogueInterface.activeInHierarchy)
        {
            int random = Random.Range(0, clickButtonSound.Count);
            SoundFXManager.instance.PlaySound(clickButtonSound[random], transform, 1f);

            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            player.GetComponent<PlayerMovement>().canMove = false;

            masterController.canPause = false;
            isInteracting = true;
            tutorialManual.SetActive(true);
            tabController = FindObjectOfType<TabController>();
            tabController.UpdateTabVisuals(0);

            foreach(var e in journalSections)
            {
                e.pageIndex = 0;
                e.toUpdatePageToggleButtons = true;
            }
        }
    }

    private void CloseManual()
    {
        int random = Random.Range(0, clickButtonSound.Count);
        SoundFXManager.instance.PlaySound(clickButtonSound[random], transform, 1f);

        tutorialManual.SetActive(false);
        isInteracting = false;
        masterController.canPause = true;

        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        player.GetComponent<PlayerMovement>().canMove = true;
    }
}

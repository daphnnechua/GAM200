using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneStation : MonoBehaviour
{
    private bool canAccessDrone;
    private RestockingController restockingController;
    private GameController gameController;
    private MaintenanceManager maintenanceManager;
    [SerializeField] private GameObject droneMenu;
    [SerializeField] private Button closeButton;

    private GameObject player;
    public bool isinteracting;
    private bool hasBeenInitialized  = false;
    private TabController tabController;
    // Start is called before the first frame update
    void Start()
    {
        droneMenu = GameObject.FindWithTag("DroneMenu");
        closeButton = GameObject.FindWithTag("CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(()=> CloseMenu());

        restockingController = FindObjectOfType<RestockingController>();
        tabController = FindObjectOfType<TabController>();
        maintenanceManager = FindObjectOfType<MaintenanceManager>();
        gameController = FindObjectOfType<GameController>();


        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(gameController.gameStart && !hasBeenInitialized)
        {
            maintenanceManager.Initialize();
            restockingController.InitializeRestockingController();
            droneMenu.SetActive(false);

            hasBeenInitialized = true;
            
            // Debug.Log("initializing...");
        }

        if(Input.GetKeyDown(KeyCode.F) && canAccessDrone)
        {
            droneMenu.SetActive(true);
            tabController.UpdateTabVisuals(0);
            isinteracting = true;
            
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll; //do not move while menu is open

        }
        if(Input.GetKeyDown(KeyCode.Escape) && isinteracting)
        {
            CloseMenu();
        }    
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            canAccessDrone = true;
        }
    }
    void OnTriggerExit2D (Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            canAccessDrone = false;
        }
    }

    private void CloseMenu()
    {
        restockingController.ClearDisplayButtons();
        isinteracting = false;
        droneMenu.SetActive(false);
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneStation : MonoBehaviour
{
    private RestockingController restockingController;
    private GameController gameController;
    private MaintenanceManager maintenanceManager;
    [SerializeField] public GameObject droneMenu;
    [SerializeField] private Button closeButton;

    [SerializeField] private GameObject tutorialManual;

    [SerializeField] private GameObject dialogueInterface;

    private GameObject player;
    public bool isinteracting;
    private TabController tabController;

    // Start is called before the first frame update
    void Start()
    {
        droneMenu = GameObject.FindWithTag("DroneMenu");
        
        closeButton.onClick.AddListener(()=> CloseMenu());

        restockingController = FindObjectOfType<RestockingController>();
        
        maintenanceManager = FindObjectOfType<MaintenanceManager>();
        gameController = FindObjectOfType<GameController>();


        maintenanceManager.Initialize();
        restockingController.InitializeRestockingController();
        droneMenu.SetActive(false);


        player = GameObject.FindWithTag("Player");

        // Debug.Log(player);
    }

    // Update is called once per frame
    void Update()
    {

        if(!gameController.levelEnded && Input.GetKeyDown(KeyCode.J) && player.GetComponent<PickUpObjs>().IsDroneStation())
        {
            if(!tutorialManual.activeInHierarchy && !dialogueInterface.activeInHierarchy)
            {
                MasterController masterController = FindObjectOfType<MasterController>();
                masterController.canPause = false;

                OpenDroneMenu();
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape) && isinteracting)
        {
            CloseMenu();

            MasterController masterController = FindObjectOfType<MasterController>();
            masterController.canPause = true;
        }    

    }

    private void OpenDroneMenu()
    {
        droneMenu.SetActive(true);
        isinteracting = true;
        tabController = FindObjectOfType<TabController>();
        tabController.UpdateTabVisuals(0);
            
            
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll; //do not move while menu is open

    }

    private void CloseMenu()
    {
        restockingController.ClearDisplayButtons();
        isinteracting = false;
        droneMenu.SetActive(false);
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }


}

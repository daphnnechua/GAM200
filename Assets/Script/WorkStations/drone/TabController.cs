using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    [SerializeField] private Button tutorialNext;
    [SerializeField] private Button tutorialPrevious;
    public List<TextMeshProUGUI> tabText = new List<TextMeshProUGUI>();

    public List<Image> tabImage = new List<Image>();
    public List<GameObject> pages = new List<GameObject>();

    public List<Button> buttons= new List<Button>();

    public bool isRestockingPage = false; //assuming first of the list is restocking page

    private DroneStation droneStation;

    private TutorialManualController tutorialManualController;

    [SerializeField] private List<AudioClip> clickButtonSound;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int index =i;
            buttons[i].onClick.AddListener(() => UpdateTabVisuals(index));
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateTabVisuals(int index)
    {

        droneStation = FindObjectOfType<DroneStation>();
        tutorialManualController = FindObjectOfType<TutorialManualController>();

        if(droneStation.isinteracting)
        {
            int random = Random.Range(0, clickButtonSound.Count);
            SoundFXManager.instance.PlaySound(clickButtonSound[random], transform, 1f);

            isRestockingPage = (index == 0);

            // Set all pages inactive and update button images
            for (int i = 0; i < pages.Count; i++)
            {
                pages[i].SetActive(i == index);
                if(i ==index)
                {
                    tabText[i].color = new Color(0, 147, 255);
                }
                else 
                {
                    tabText[i].color = Color.white;
                }
            }

            MaintenanceManager maintenanceManager = FindObjectOfType<MaintenanceManager>();
            if(maintenanceManager!=null)
            {
                maintenanceManager.UpdateMaintenanceButtonImage();
            }
        }
        else if(tutorialManualController.isInteracting)
        {
            int random = Random.Range(0, clickButtonSound.Count);
            SoundFXManager.instance.PlaySound(clickButtonSound[random], transform, 1f);
            
            for (int i = 0; i < pages.Count; i++)
            {
                pages[i].SetActive(i == index);
                // string filePath = "";
                if(i ==index)
                {
                    // filePath = "UI/active_button1";
                    tabImage[i].gameObject.transform.SetAsLastSibling();
                    tabImage[i].color = Color.white;

                    PageToggle pageToggle = pages[i].GetComponentInChildren<PageToggle>();
                    if(pageToggle!=null)
                    {
                        pageToggle.pageIndex = 0;
                        pageToggle.UpdatePageToggleButtons();
                    }
                }
                else 
                {
                    // filePath = "UI/inactive_button1";
                    tabImage[i].color = Color.gray;
                }
                // SetButtonImage(filePath, tabImage[i]);
            }
            
        }
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }
}

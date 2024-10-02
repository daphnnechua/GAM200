using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public List<Image> tabImage = new List<Image>();
    public List<GameObject> pages = new List<GameObject>();

    public List<Button> buttons= new List<Button>();

    public bool isRestockingPage = false; //assuming first of the list is restocking page

    public bool isToggled = false;

    // Start is called before the first frame update
    void Start()
    {
        UpdateTabVisuals(0); //always open at first tab
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
        isRestockingPage = (index == 0);

        // Set all pages inactive and update button images
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(i == index);
            string filePath = "";
            if(i ==index)
            {
                filePath = "UI/active_button1";
            }
            else 
            {
                filePath = "UI/inactive_button1";
            }
            SetButtonImage(filePath, tabImage[i]);
        }

        MaintenanceManager maintenanceManager = FindObjectOfType<MaintenanceManager>();
        maintenanceManager.UpdateMaintenanceButtonImage();
        
    }
    private void SetButtonImage(string filePath, Image image)
    {
        AssetManager.LoadSprite(filePath, (Sprite sp) =>
        {
            image.sprite = sp;
        });
    }
}

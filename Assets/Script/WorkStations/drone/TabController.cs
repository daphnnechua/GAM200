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
        if(index == 0)
        {
            isRestockingPage = true;
            
        }
        else
        {
            isRestockingPage = false;
        }

        for(int i =0; i<pages.Count ;i++)
        {
            pages[i].SetActive(false);
            SetButtonImage("UI/inactive_button1", tabImage[i]);
        }
        pages[index].SetActive(true);
        SetButtonImage("UI/active_button1", tabImage[index]);


    }

    private void SetButtonImage(string filePath, Image image)
    {
        AssetManager.LoadSprite(filePath, (Sprite sp) =>
        {
            image.sprite = sp;
        });
    }
}

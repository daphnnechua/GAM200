using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PageToggle : MonoBehaviour
{
    [SerializeField] private Button tutorialNext;
    [SerializeField] private Button tutorialPrevious;
    public List<GameObject> pages = new List<GameObject>();

    private TutorialManualController tutorialManualController;

    public int pageIndex = 0;

    private bool toUpdatePageToggleButtons = true;

    // Start is called before the first frame update
    void Start()
    {
        tutorialManualController = FindObjectOfType<TutorialManualController>();

        tutorialNext.onClick.AddListener(() => UpdateNextPage());
        tutorialPrevious.onClick.AddListener(() => UpdatePrevPage());
    }

    // Update is called once per frame
    void Update()
    {
        if(toUpdatePageToggleButtons)
        {
            UpdatePageToggleButtons();
            toUpdatePageToggleButtons = false;     
        }
        
    }

    private void UpdateNextPage()
    {
        pageIndex++;

        foreach(var page in pages)
        {
            page.SetActive(false);
        }

        toUpdatePageToggleButtons = true;

        pages[pageIndex].SetActive(true);
        
        
    }

    private void UpdatePrevPage()
    {
        pageIndex --;

        foreach(var page in pages)
        {
            page.SetActive(false);
        }

        toUpdatePageToggleButtons = true;

        pages[pageIndex].SetActive(true);
        
        
    }

    public void UpdatePageToggleButtons()
    {
        tutorialPrevious.gameObject.SetActive(pageIndex>0);
        tutorialNext.gameObject.SetActive(pageIndex < pages.Count - 1 );
    }
}
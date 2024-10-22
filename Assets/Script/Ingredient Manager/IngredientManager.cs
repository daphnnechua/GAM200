using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IngredientManager : MonoBehaviour
{
    public IngredientSO ingredientSO;

    public float prepProgress = 0;
    public bool startedPrep = false;

    private bool hasUndergonePrep = false; 

    public GameObject progressBar;

    void Update()
    {
        if(progressBar!=null)
        {
            progressBar.transform.position = ProgressBarPos();
        }
    }

    public void SetImage(string spritePath)
    {
        AssetManager.LoadSprite(spritePath, (Sprite sp) =>
        {
            this.GetComponent<SpriteRenderer>().sprite = sp;
        });


    }

    public void SpawnProgressBar()
    {
        if(progressBar == null && !hasUndergonePrep)
        {
            AssetManager.LoadPrefab("UI/Progress bar", (GameObject prefab) =>
            {
                progressBar= Instantiate(prefab, GameObject.Find("Canvas").transform);
                progressBar.transform.SetAsFirstSibling();
                // Debug.Log($"new progress bar for {ingredientSO.ingredientName} created");

                Slider slider = progressBar.GetComponent<Slider>();

                slider.value = prepProgress;

            });
            hasUndergonePrep = true;
        }
    }

    private Vector3 ProgressBarPos()
    {
        return transform.position + new Vector3(0, 0.75f, 0);
    }

    public void UpdateCuttingProgressBar(CuttingStation cuttingStation, float timer)
    {
        if(cuttingStation.ingredientOnStation && progressBar!=null)
        {
            Slider slider = progressBar.GetComponent<Slider>();
            slider.value = prepProgress/timer;
        }
    }

    public void DestroyProgressBar(float timer)
    {
        if(prepProgress>=timer)
        {
            Destroy(progressBar);
        }
    }


}

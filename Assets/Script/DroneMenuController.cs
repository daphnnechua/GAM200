using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DroneMenuController : MonoBehaviour
{

    #region handle restocking
    private IEnumerator RestockTimer(RestockingController restockingController, List<string> ingredientIDList, GameObject timer)
    {
        float totalRestockTime = 2f * ingredientIDList.Count;
        float timeLeft = totalRestockTime;

        GameObject timerInstance = Instantiate(timer, GameObject.Find("Canvas").transform);
        timerInstance.transform.position = restockingController.TimerPos();
        timerInstance.transform.SetAsFirstSibling();
        Slider slider= timerInstance.GetComponent<Slider>();
        slider.value = 1;

        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            slider.value = timeLeft/totalRestockTime;

            yield return null;
        }

        restockingController.RestockIngredients();
        Destroy(timerInstance);
        restockingController.droneAvailable = true;    
    }

    public void SendDroneOut(RestockingController restockingController, List<string> ingredientIDList, GameObject timer)
    {
        StartCoroutine(RestockTimer(restockingController, ingredientIDList, timer));
    }

    #endregion handle restocking
    #region text ui for restocking
    private IEnumerator RetrievingText(RestockingController restockingController, TextMeshProUGUI text)
    {
        string retrieveText = "Retrieving";
        int dotCount = 0;

        while (!restockingController.droneAvailable)
        {
            text.text = retrieveText + new string('.', dotCount);
            dotCount = (dotCount + 1) % 4; 
            yield return new WaitForSeconds(0.5f); 
        }
        
    }

    public Coroutine UITextForRestock(RestockingController restockingController, TextMeshProUGUI text)
    {
        return StartCoroutine(RetrievingText(restockingController, text));
    }

    #endregion text ui for restocking


}

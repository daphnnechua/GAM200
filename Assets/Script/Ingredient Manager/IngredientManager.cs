using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientManager : MonoBehaviour
{
    public IngredientSO ingredientSO;

    public void SetImage(string spritePath)
    {
        AssetManager.LoadSprite(spritePath, (Sprite sp) =>
        {
            this.GetComponent<SpriteRenderer>().sprite = sp;
        });


    }

}

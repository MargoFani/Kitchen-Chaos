using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }
    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRrecipeSOList;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipeMax = 4;

    private void Awake()
    {
        Instance = this;
        waitingRrecipeSOList = new List<RecipeSO>();
    }
    private void Update()
    {
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;
            if (waitingRrecipeSOList.Count < waitingRecipeMax)
            {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[Random.Range(0, recipeListSO.recipeSOList.Count)];
                Debug.Log("generated recipe: " + waitingRecipeSO.recipeName);
                waitingRrecipeSOList.Add(waitingRecipeSO);
            }
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        bool plateContentsMatchesRecipe = false; 
        for(int i = 0; i< waitingRrecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRrecipeSOList[i];

            if(waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                plateContentsMatchesRecipe = plateKitchenObject.GetKitchenObjectSOList().All(s => waitingRecipeSO.kitchenObjectSOList.Contains(s));
            }

            if (plateContentsMatchesRecipe)
            {
                //player delivered right recipe
                Debug.Log("player delivered right recipe");
                waitingRrecipeSOList.RemoveAt(i);
                return;
            }
        }
        // no matches found
        Debug.Log("player delivered wrong recipe");
    }
}

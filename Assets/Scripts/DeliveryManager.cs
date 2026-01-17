using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeListChanged;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    public static DeliveryManager Instance { get; private set; }
    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRrecipeSOList;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipeMax = 4;
    private int successfulRecipesAmount;

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
            if (GameManager.Instance.IsGamePlaying() && waitingRrecipeSOList.Count < waitingRecipeMax)
            {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];
                waitingRrecipeSOList.Add(waitingRecipeSO);
                OnRecipeListChanged?.Invoke(this, EventArgs.Empty);
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
                successfulRecipesAmount++;
                waitingRrecipeSOList.RemoveAt(i);
                OnRecipeListChanged?.Invoke(this, EventArgs.Empty);

                OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                return;
            }
        }
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRrecipeSOList;
    }

    public int GetSuccessRecipesAmount()
    {
        return successfulRecipesAmount;
    }
}

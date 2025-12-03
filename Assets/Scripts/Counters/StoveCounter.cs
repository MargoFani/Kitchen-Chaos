using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    private State StoveState {
        get { return stoveState; }
        set { 
            stoveState = value;
            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs() { state = stoveState});
            Debug.Log("stoveState " + stoveState);
        } 
    }
    private State stoveState; 
    private float fryingTimer;
    private float burningTimer;
    private float FryingTimer {
        get
        {
            return fryingTimer;
        }
        set
        {
            fryingTimer = value;
            if (fryingRecipeSO != null)
            {
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                });
            }
        } 
    }
    //второй таймер выглядит лишним, он повторяет тоже самое. лучше использовать один таймер
    private float BurningTimer
    {
        get
        {
            return burningTimer;
        }
        set
        {
            burningTimer = value;
            if (burningRecipeSO != null)
            {
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = burningTimer / burningRecipeSO.burningTimerMax
                });
            }
        }
    }

    FryingRecipeSO fryingRecipeSO;
    BurningRecipeSO burningRecipeSO;
    private void Start()
    {
        StoveState = State.Idle;
    }
    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (StoveState)
            {
                case State.Idle:
                    break;
                case State.Frying:

                    FryingTimer += Time.deltaTime;

                    if (FryingTimer > fryingRecipeSO.fryingTimerMax)
                    {
                        //fried
                        GetKitchenObject().DestrySelf();
                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
                        StoveState = State.Fried;
                        burningRecipeSO = GetBurningRecipeSO(GetKitchenObject().GetKitchenObjectSO());
                        FryingTimer = 0f;
                        BurningTimer = 0f;             

                    }
                    break;

                case State.Fried:

                    BurningTimer += Time.deltaTime;

                    if (BurningTimer > burningRecipeSO.burningTimerMax)
                    {
                        //fried
                        GetKitchenObject().DestrySelf();

                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        StoveState = State.Burned;                     

                    }

                    break;
                case State.Burned:

                    SetProgressToZero();

                    break;
            }
        }

    }
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject() && HasRecipeInput(player.GetKitchenObject().GetKitchenObjectSO()))
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);

                fryingRecipeSO = GetFryingRecipeSO(GetKitchenObject().GetKitchenObjectSO());
                StoveState = State.Frying;
                FryingTimer = 0f;
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {

                    if (plateKitchenObject.TryAddIngridient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestrySelf();
                        SetProgressToZero();
                        StoveState = State.Idle;
                    }

                }
            }
            else
            {
                SetProgressToZero();
                GetKitchenObject().SetKitchenObjectParent(player);
                StoveState = State.Idle;
            }
        }
    }
    private bool HasRecipeInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSO(inputKitchenObjectSO);
        return fryingRecipeSO != null;
    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSO(inputKitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
        }
        else return null;
    }

    private FryingRecipeSO GetFryingRecipeSO(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }

    private BurningRecipeSO GetBurningRecipeSO(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == inputKitchenObjectSO)
            {
                return burningRecipeSO;
            }
        }
        return null;
    }
    
    private void SetProgressToZero()
    {
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = 0f
        });
    }
}

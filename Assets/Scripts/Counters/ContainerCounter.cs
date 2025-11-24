using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter, IKitchenObjectParent
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public event EventHandler OnPlyerGrabbedObject;
    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            KitchenObject.SpawnKitchenObject(this.kitchenObjectSO, player);

            OnPlyerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
    }
}

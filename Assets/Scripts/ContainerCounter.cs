using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter, IKitchenObjectParent
{
    [SerializeField] private KitchenObjectsSO kitchenObjectSO;

    public event EventHandler OnPlyerGrabbedObject;
    public override void Interact(Player player)
    {
        Transform kitchenObjectTransform = Instantiate(this.kitchenObjectSO.prefab);
        kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(player);

        OnPlyerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }
}

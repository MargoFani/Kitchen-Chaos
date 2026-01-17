using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{
    private const string POPUP_TRIGGER = "Popup";

    [SerializeField] private Image backroundImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image iconImage;

    [SerializeField] private Color sucsessColor;
    [SerializeField] private Color faildColor;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite faildSprite;

    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        gameObject.SetActive(false);
    }

    private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
        animator.SetTrigger(POPUP_TRIGGER);
        backroundImage.color = faildColor;
        iconImage.sprite = faildSprite;
        messageText.text = "Faild!";

    }

    private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
        animator.SetTrigger(POPUP_TRIGGER);
        backroundImage.color = sucsessColor;
        iconImage.sprite = successSprite;
        messageText.text = "Success!";
    }
}

using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class UIController : MonoBehaviour
{
    [Serializable] struct UI_Input
    {
        public Slider slider;
        public TextMeshProUGUI num;
    }

    public static UIController instance;

    [SerializeField] RectTransform propertyUI;
    [SerializeField] RectTransform propertyBtn;
    [SerializeField] float uiSpeed;

    [Header("Control")]
    [SerializeField] Image controlBtnImg;
    [SerializeField] TextMeshProUGUI controlBtnTMP;
    [SerializeField] Color idleColor;
    [SerializeField] Color patrolColor;
    [SerializeField] Color fleeColor;

    [Header("Pedestrian")]
    [SerializeField] UI_Input pedestrian_NormalSpeed;
    [SerializeField] UI_Input pedestrian_FleeSpeed;
    [SerializeField] UI_Input pedestrian_FleeDuration;
    [SerializeField] UI_Input pedestrian_ViewDistance;

    [Header("Dog")]
    [SerializeField] UI_Input dog_NormalSpeed;
    [SerializeField] UI_Input dog_RunSpeed;
    [SerializeField] UI_Input dog_StaminaReduce;
    [SerializeField] UI_Input dog_StaminaRecover;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void propertyOpen()
    {
        propertyBtn.gameObject.SetActive(false);
        propertyUI.gameObject.SetActive(true);

        propertyUI.localScale = Vector3.zero;
        propertyUI.DOScale(1f, uiSpeed);
    }

    public void PropertyClose()
    {
        propertyUI.DOScale(0, uiSpeed).OnComplete(() => {
            propertyUI.gameObject.SetActive(false);
            propertyBtn.gameObject.SetActive(true);
        });
    }

    //Pedestrian Control
    public void Idle()
    {
        GameController.instance.Idle();
    }

    public void ChangedControlBtn(PedestrianNPC.State state)
    {
        switch (state)
        {
            case PedestrianNPC.State.Idle:
                controlBtnImg.color = idleColor;
                controlBtnTMP.text = "Idle";
                break;

            case PedestrianNPC.State.Patrolling:
                controlBtnImg.color = patrolColor;
                controlBtnTMP.text = "Patrol";
                break;

            case PedestrianNPC.State.Fleeing:
                controlBtnImg.color = fleeColor;
                controlBtnTMP.text = "Flee";
                break;
        }
    }


    //Pedestrian NPC
    public void OnPedestrianChanged_NormalSpeed()
    {
        pedestrian_NormalSpeed.num.text = pedestrian_NormalSpeed.slider.value.ToString();
        GameController.instance.OnPedestrianChanged_NormalSpeed(pedestrian_NormalSpeed.slider.value);
    }

    public void OnPedestrianChanged_FleeSpeed()
    {
        pedestrian_FleeSpeed.num.text = pedestrian_FleeSpeed.slider.value.ToString();
        GameController.instance.OnPedestrianChanged_FleeSpeed(pedestrian_FleeSpeed.slider.value);
    }

    public void OnPedestrianChanged_FleeDuration()
    {
        pedestrian_FleeDuration.num.text = pedestrian_FleeDuration.slider.value.ToString();
        GameController.instance.OnPedestrianChanged_FleeDuration(pedestrian_FleeDuration.slider.value);
    }

    public void OnPedestrianChanged_ViewDistance()
    {
        pedestrian_ViewDistance.num.text = pedestrian_ViewDistance.slider.value.ToString();
        GameController.instance.OnPedestrianChanged_ViewDistance(pedestrian_ViewDistance.slider.value);
    }

    //Dog NPC
    public void OnDogChanged_NormalSpeed()
    {
        dog_NormalSpeed.num.text = dog_NormalSpeed.slider.value.ToString();
        GameController.instance.OnDogChanged_NormalSpeed(dog_NormalSpeed.slider.value);
    }

    public void OnDogChanged_RunSpeed()
    {
        dog_RunSpeed.num.text = dog_RunSpeed.slider.value.ToString();
        GameController.instance.OnDogChanged_RunSpeed(dog_RunSpeed.slider.value);
    }

    public void OnDogChanged_StaminaReduce()
    {
        float value = (float)Math.Round(dog_StaminaReduce.slider.value, 1);
        dog_StaminaReduce.num.text = value.ToString();
        dog_StaminaReduce.slider.value = value;
        GameController.instance.OnDogChanged_StaminaReduce(value);
    }

    public void OnDogChanged_StaminaRecover()
    {
        float value = (float)Math.Round(dog_StaminaRecover.slider.value, 1);
        dog_StaminaRecover.num.text = value.ToString();
        dog_StaminaRecover.slider.value= value;
        GameController.instance.OnDogChanged_StaminaRecover(value);
    }
}

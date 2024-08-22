using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [SerializeField] private PedestrianNPC pedestrianNPC;
    [SerializeField] private DogNPC[] dogNPCs;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    //Idle
    public void Idle()
    {
        pedestrianNPC.ChangedIdle();
    }

    //Set variable
    public void OnPedestrianChanged_NormalSpeed(float speed)
    {
        pedestrianNPC.Set_NormalSpeed(speed);
    }

    public void OnPedestrianChanged_FleeSpeed(float speed)
    {
        pedestrianNPC.Set_FleeingSpeed(speed);
    }

    public void OnPedestrianChanged_FleeDuration(float duration)
    {
        pedestrianNPC.Set_FleeingDuration(duration);
    }

    public void OnPedestrianChanged_ViewDistance(float viewDistance)
    {
        pedestrianNPC.Set_ViewDistance(viewDistance);
    }

    public void OnDogChanged_NormalSpeed(float speed)
    {
        foreach (DogNPC dog in dogNPCs)
        {
            dog.Set_NormalSpeed(speed);
        }
    }

    public void OnDogChanged_RunSpeed(float speed)
    {
        foreach (DogNPC dog in dogNPCs)
        {
            dog.Set_RunSpeed(speed);
        }
    }

    public void OnDogChanged_StaminaReduce(float staminaReduce)
    {
        foreach (DogNPC dog in dogNPCs)
        {
            dog.Set_StaminaReduce(staminaReduce);
        }
    }

    public void OnDogChanged_StaminaRecover(float staminaRecover)
    {
        foreach (DogNPC dog in dogNPCs)
        {
            dog.Set_StaminaRecover(staminaRecover);
        }
    }
}

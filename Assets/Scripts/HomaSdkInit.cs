using HomaGames.HomaBelly;
using UnityEngine;

public class HomaSdkInit : MonoBehaviour
{
    public void Awake()
    {
        if (!HomaBelly.Instance.IsInitialized)
        {
            // Listen event for initialization
            Events.onInitialized += OnHomaBellyInitialized;
        }
        else
        {
            OnHomaBellyInitialized();
        }
    }

    private void OnDisable()
    {
        Events.onInitialized -= OnHomaBellyInitialized;
    }

    private void OnHomaBellyInitialized()
    {
        // Homa Belly initialized, call any Homa Belly method
    }
}

using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    void Awake()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("💥 All PlayerPrefs cleared on start!");
    }
}

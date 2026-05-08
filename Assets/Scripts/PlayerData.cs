using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;
    public string playerName;
    public string sessionToken;
    public int bestScore;
    

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

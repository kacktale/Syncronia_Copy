using UnityEngine;

[DefaultExecutionOrder(-100)]
public class UserData : MonoBehaviour
{
    public static UserData instance;
    public string userName;
    public float userSpeed;
    public int userLV;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

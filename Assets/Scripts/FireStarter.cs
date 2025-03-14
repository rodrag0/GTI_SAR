using UnityEngine;

public class FireStarter : MonoBehaviour
{
    
    public Fire Fire;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && Fire != null)
        {
            Fire.Ignite();
        }
    }
}

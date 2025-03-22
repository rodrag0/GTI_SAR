using UnityEngine;

public class fireContact : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if(other.tag == "fire")
        {
             Debug.Log(other.name + "'  temperature could be fire");
        }
       
    }
}


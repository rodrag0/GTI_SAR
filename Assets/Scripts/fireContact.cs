using UnityEngine;
using TMPro;
public class fireContact : MonoBehaviour
{
    public TMP_Text fireText;
    void OnTriggerStay(Collider other)
    {
        if(other.tag == "fire")
        {
            fireText.text = "Temperature: Fire";
        }else
        {
            fireText.text = "Temperature: Normal";
        }
       
    }
}


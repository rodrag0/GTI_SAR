using System.Collections;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [Header("Fire Settings")]
    [Tooltip("Should this cube start on fire?")]
    public bool isOnFire = false;
    [Tooltip("Particle system prefab to display fire")]
    public ParticleSystem fireEffectPrefab;
    [Tooltip("Delay (in seconds) before spreading fire to nearby cubes")]
    public float propagationDelay = 2f;
    [Tooltip("Radius within which to search for cubes to ignite")]
    public float propagationRadius = 2f;

    // Used to ensure fire is only spread once from this cube
    private bool hasSpread = false;
    private ParticleSystem fireInstance;

    void Start()
    {
        // If the cube should start on fire immediately, ignite it.
        if (isOnFire)
        {
            Ignite();
        }
    }

    // Public method to ignite this cube.
    public void Ignite()
    {
        // If already on fire, do nothing.
        if (isOnFire)
            return;

        isOnFire = true;
        StartFireEffect();
        StartCoroutine(SpreadFire());
    }

    // Instantiate and play the fire effect at the cube's position.
    private void StartFireEffect()
    {
        if (fireEffectPrefab != null)
        {
            // Instantiate the fire effect as a child so it follows the cube.
            fireInstance = Instantiate(fireEffectPrefab, transform.position, Quaternion.identity, transform);
            fireInstance.Play();
        }
    }

    // After a delay, check for nearby cubes to ignite.
    private IEnumerator SpreadFire()
    {
        yield return new WaitForSeconds(propagationDelay);

        // Ensure fire is only spread once.
        if (hasSpread)
            yield break;
        hasSpread = true;

        // Look for nearby colliders within the specified radius.
        Collider[] colliders = Physics.OverlapSphere(transform.position, propagationRadius);
        foreach (Collider nearby in colliders)
        {
            // Skip this cube.
            if (nearby.gameObject == gameObject)
                continue;

            // Get the Fire component from the nearby cube.
            Fire otherCube = nearby.GetComponent<Fire>();

            if (otherCube != null)
            {
                Debug.Log("GameObject Name: " + otherCube.name);
                if(!otherCube.isOnFire){
                    otherCube.Ignite();
                }
                
               
            }
        }
    }

    // Optional: Visualize the propagation radius in the Scene view.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, propagationRadius);
    }
}

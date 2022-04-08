using UnityEngine;

public class AttackParticleController : MonoBehaviour
{
    [SerializeField] [Tooltip("Needs either a Unit or Building component")]
    private Unit unitStats;
    [SerializeField] [Tooltip("Needs either a Unit or Building component")]
    private Building buildingStats;

    [SerializeField]
    private Transform[] particleParents;
    [SerializeField]
    private GameObject[] particlesToSpawn;
    [SerializeField]
    private float[] particleAttackDelayStartTimings;

    private int arrayLength = 0;
    private bool[] particlesTriggered;

    private void Awake()
    {
        // check that all arrays are the same length
        arrayLength = particleParents.Length;

        if (particlesToSpawn.Length == arrayLength &&
            particleAttackDelayStartTimings.Length == arrayLength)
        {
            particlesTriggered = new bool[arrayLength];
        }
        else
        {
            // log error and remove script to avoid bugs
            Debug.LogError("Particle data arrays are not equal in length. Removing functionality. Gameobject ID: " + gameObject.GetInstanceID().ToString());
            Destroy(this);
        }

        for (int i = 0; i < arrayLength; i++)
        {
            particlesTriggered[i] = false;
        }
    }

    private void Update()
    {
        for (int i = 0; i < arrayLength; i++)
        {
            if (unitStats != null)
            {
                // keep track of whether the attack animation has been triggered yet
                if (particlesTriggered[i] == false)
                {

                    // set attack animation trigger when it reaches the correct time in the attack timer
                    if (unitStats.Stats.CurrAttackDelay >= particleAttackDelayStartTimings[i])
                    {
                        Instantiate(particlesToSpawn[i], particleParents[i]);

                        particlesTriggered[i] = true;
                    }
                }
                else
                {
                    // reset attack triggered boolean when attack has finished
                    if (unitStats.Stats.CurrAttackDelay <= unitStats.Stats.AttackChargeLimiter)
                    {
                        particlesTriggered[i] = false;
                    }
                }
            }

            if (buildingStats != null)
            {
                // keep track of whether the attack animation has been triggered yet
                if (particlesTriggered[i] == false)
                {

                    // set attack animation trigger when it reaches the correct time in the attack timer
                    if (buildingStats.Stats.CurrAttackDelay >= particleAttackDelayStartTimings[i])
                    {
                        Instantiate(particlesToSpawn[i], particleParents[i]);

                        particlesTriggered[i] = true;
                    }
                }
                else
                {
                    // reset attack triggered boolean when attack has finished
                    if (buildingStats.Stats.CurrAttackDelay <= buildingStats.Stats.AttackChargeLimiter)
                    {
                        particlesTriggered[i] = false;
                    }
                }
            }
        }
    }
}
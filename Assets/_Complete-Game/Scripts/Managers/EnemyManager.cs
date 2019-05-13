// ************************************************************************************************** //

// This script is slightly modified to delay enemy from spawning until the player baseline BPM is calculated.
// Enemy is delyed from spawning for 60 seconds. 

// Modified areas are clearly marked.

// ************************************************************************************************** //

using UnityEngine;
using System.Diagnostics;
using System.IO;

namespace CompleteProject
{
    public class EnemyManager : MonoBehaviour
    {
        public PlayerHealth playerHealth;       // Reference to the player's heatlh.
        public GameObject enemy;                // The enemy prefab to be spawned.
        public float spawnTime = 3f;            // How long between each spawn.
        public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.

        // MODIFICATION CODE: START //
        public float delay = 3f;    // Used for delaying EnemyManager from spawning enemies for one minute, when game starts!
        // MODIFICATION CODE: END   //

        void Start ()
        {
            // MODIFICATION CODE: START //

            // If the savedBaseline.txt is empty, it means that we need to calculate a baseline heart rate for the player //
            if (new FileInfo("C:\\Users\\Farmer\\Documents\\Unity Projects\\Survival Game\\savedBaseline.txt").Length == 0)
            {
                delay = 60f; // Since the file is empty, reset delay to 60 seconds
            }
            
            // Stay idle for 1 minute so that the player's baseline heart rate can be calculated without being attacked by enemy
            // Call the Spawn function after a delay of 60 seconds and then continue to call after spawnTime amount of time
            InvokeRepeating("Spawn", delay, spawnTime);

            // MODIFICATION CODE: END //
        }


        void Spawn ()
        {
            // If the player has no health left...
            if(playerHealth.currentHealth <= 0f)
            {
                // ... exit the function.
                return;
            }

            // Find a random index between zero and one less than the number of spawn points.
            int spawnPointIndex = Random.Range (0, spawnPoints.Length);

            // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
            Instantiate (enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    [SerializeField] private Light sun;
    [SerializeField] private float secondsInFullDay = 120f;

    [Range(0, 1)] [SerializeField] public float currentTimeOfDay = 0;
    [Range(1, 365)] [SerializeField] private float currentDay = 1;
    [SerializeField] private TextMesh currentDayName;
    private float timeMultiplier = 1f;
    private float sunInitialIntensity;

    // Spawning:
    private float nextSpawn = 0f;
    public float spawnRate = 10f;
    private float nextMapSpawn = 0f;
    public float mapSpawnRate = 10f;
    public float TempleSpawnAreaGive = 4f;
    public float spawnAreaGive = 4f;
    public Transform Enemy;
    public Transform Enemy2;

    void Start()
    {
        sunInitialIntensity = sun.intensity;

    }

    void Update()
    {
        UpdateSun();

        currentTimeOfDay += (Time.deltaTime / secondsInFullDay) * timeMultiplier; // Over time divide by set seconds and * by mulitplier - Add realistic effect

        if (currentTimeOfDay >= 1)
        {
            currentTimeOfDay = 0; // Restart
            currentDay++;
            currentDayName.text = "Days Survived: " + (currentDay - 1);
            if (spawnRate > 2)
            {
                spawnRate--;
            }
            if (mapSpawnRate > 2)
            {
                mapSpawnRate--;
            }
        }

        // initial spawns in bone temple:
        if (currentTimeOfDay >= 0.8 || currentTimeOfDay <= 0.1)
        {
            GameObject[] enemySpawns = GameObject.FindGameObjectsWithTag("EnemySpawnPoint"); // Find and hold each GameObject under EnemySpawnPoint tag in array

            foreach (GameObject spawnPoint in enemySpawns)                                    // for each spawn point in the game
            {
                // counter for how many enemies are made
                int i = 1;

                float noOfEnemies = (Random.Range(1, currentDay));                          // Spawn random amount of enemies but more enemies the further into the game you go

               
                   
                    // loop through spawn points
                    for (int j = 0; j < noOfEnemies; j++)
                    {
                    if (Time.time > nextSpawn)
                    {
                        i++;

                        Debug.Log("Enemy spawner activated");

                        // create enemy and give it a unique name
                        Transform enemy;
                        enemy = Instantiate(Enemy, new Vector3(spawnPoint.transform.position.x + Random.Range(-TempleSpawnAreaGive, TempleSpawnAreaGive),
                                                           spawnPoint.transform.position.y,
                                                           spawnPoint.transform.position.z + Random.Range(-TempleSpawnAreaGive, TempleSpawnAreaGive)),
                        spawnPoint.transform.rotation) as Transform;
                        enemy.name = ("Enemy " + i);
                        GameObject inst = enemy.gameObject;
                        inst.SetActive(true);
                        inst.transform.Find("NPCHealthContainer").gameObject.SetActive(false);
                        nextSpawn = Time.time + spawnRate;
                    }
                    }

                } 

            // map spawn 1 after day 2
            if (currentDay >= 3)
            {
                GameObject[] mapSpawn1 = GameObject.FindGameObjectsWithTag("EnemySpawnPoint2"); // Find and hold each GameObject under EnemySpawnPoint tag in array

                foreach (GameObject spawnPoint in mapSpawn1)                                    // for each spawn point in the game
                {
                    // counter for how many enemies are made
                    int i = 40;

                        // loop through spawn points
                        float noOfEnemies2 = (Random.Range(1, 3) * currentDay);                          // Spawn random amount of enemies but more enemies the further into the game you go

                        for (int j = 0; j < noOfEnemies2; j++)
                        {
                            Debug.Log("Roaming Enemy spawner activated");

                            if (Time.time > nextMapSpawn)
                                {
                                
                                i++;
                                // create enemy and give it a unique name
                                Transform enemy;
                                enemy = Instantiate(Enemy2, new Vector3(spawnPoint.transform.position.x + Random.Range(-spawnAreaGive, spawnAreaGive),
                                                                   spawnPoint.transform.position.y,
                                                                   spawnPoint.transform.position.z + Random.Range(-spawnAreaGive, spawnAreaGive)),
                                spawnPoint.transform.rotation) as Transform;
                                enemy.name = ("Roaming Enemy " + i);
                                GameObject inst = enemy.gameObject;
                                inst.SetActive(true);
                                inst.transform.Find("NPCHealthContainer").gameObject.SetActive(false);
                                nextMapSpawn = Time.time + mapSpawnRate;
                            }
                        }
                    }
                }
            }

           
            }
            

    void UpdateSun()
    {
        sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90, 170, 0); // Transform local rotation of sun based on the current time of day and include horizon setting

        float intensityMultiplier = 1;

        if (currentTimeOfDay <= 0.23f || currentTimeOfDay >= 0.75f) //...if time of day is just before sunrise or just after sunset...
        {
            intensityMultiplier = 0;
        }
        else if (currentTimeOfDay <= 0.25f) { 
            intensityMultiplier = Mathf.Clamp01((currentTimeOfDay - 0.23f) * (1 / 0.02f)); //clamp a value between 0 - 1 and multiply by a value allowing it to fade
        }
        else if (currentTimeOfDay >= 0.73f)
        {
            intensityMultiplier = Mathf.Clamp01(1 - ((currentTimeOfDay - 0.73f) * (1 / 0.02f))); //if close to sunset, set mulitplier and fade intensity out
        }

        sun.intensity = sunInitialIntensity * intensityMultiplier; // Take the initial intensity of the sun and multiply it by the values given each time UpdateSun is called
    }
}

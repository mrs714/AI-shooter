using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    // Private variables, available in the inspector
    [SerializeField] int numberOfEnemies = 10;
    [SerializeField] int numberOfObjectives = 10;
    [SerializeField] int numberOfZonesPerSide = 3;
    [SerializeField] int startingX = -100;
    [SerializeField] int startingY = -100;
    [SerializeField] int zoneSize = 30;
    [SerializeField] int typeOfTraining = 0; // 0 = runner, 1 = shooter, 2 = both, -1 = basic
    [SerializeField] float secondsPerGeneration = 15f;

    // Prefabs to be set in the inspector
    public GameObject enemyPrefab;
    public GameObject objectivePrefab;
    public GameObject playerPrefab;
    public GameObject wallPrefab;

    // List of players, enemies, walls, objectives, and nn
    List<GameObject> players = new List<GameObject>();
    List<GameObject> enemies = new List<GameObject>();
    List<GameObject> walls = new List<GameObject>();
    List<GameObject> objectives = new List<GameObject>();
    NeuralNetwork[] neuralNetworks;
    //list that contains a list of all the objects of each zone, listed by type, which will be provided to the players
    List<List<List<GameObject>>> zoneObjects = new List<List<List<GameObject>>>(); //first, we have the zones, then the type of object, then the objects themselves: enemies, objectives
  
    // Additional variables - info
    float timer = 0f;
    int generation = 0;
    int maxAwardedScore = int.MinValue;


    void Start()
    {
        // We start with a blank (random) neural network
        // We can select the type of training we want to do
        // 0 = runner, 1 = shooter, 2 = both
        if (typeOfTraining == 0)
        {
            neuralNetworks = createNeuralNetworksRunner();
            spawnRunners(neuralNetworks);
        }
        else if (typeOfTraining == 1)
        {
            neuralNetworks = createNeuralNetworksShooter();
            spawnShooters(neuralNetworks);
        }
        else if (typeOfTraining == 2)
        {
            neuralNetworks = createNeuralNetworksBoth();
            spawnBoth(neuralNetworks);
        }
        else if (typeOfTraining == -1)
        {
            numberOfEnemies = 1;
            numberOfObjectives = 1;
            neuralNetworks = createNeuralNetworksBasic();
            spawnBasic(neuralNetworks);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if enough time has passed since the last generation
        timer += Time.deltaTime;
        if (timer > secondsPerGeneration)
        {
            // Take the best players and mutate their NN
            evolve();

            // Delete all the objects in the scene
            deleteObjects();
            
            // Respawn them
            if (typeOfTraining == 0)
            {
                spawnRunners(neuralNetworks);
            }
            else if (typeOfTraining == 1)
            {
                spawnShooters(neuralNetworks);
            }
            else if (typeOfTraining == 2)
            {
                spawnBoth(neuralNetworks);
            }
            else if (typeOfTraining == -1)
            {
                spawnBasic(neuralNetworks);
            }

            // Reset timer
            timer = 0f;

            //print info generation
            generation++;
            Debug.Log("Generation: " + generation + ". Max Score: " + maxAwardedScore);
        }
    }

    // Creates the neural networks for the runner players
    NeuralNetwork[] createNeuralNetworksRunner()
    {
        // First, the ammount of parameters is calculated
        int neuronsOnFirstLayer = 9; //3 for the position of the player, 3 for the position of the objective, 3 for the position of the enemy

        // Then, we create the neural networks
        NeuralNetwork[] neuralNetworks = new NeuralNetwork[numberOfZonesPerSide * numberOfZonesPerSide];
        for (int i = 0; i < neuralNetworks.Length; i++)
        {
            neuralNetworks[i] = new NeuralNetwork(neuronsOnFirstLayer,2,5,10);
        }

        return neuralNetworks;
    }

    // Creates the neural networks for the shooter players
    NeuralNetwork[] createNeuralNetworksShooter()
    {
        return null;
    }

    // Creates the neural networks for the shooter and runner players
    NeuralNetwork[] createNeuralNetworksBoth()
    {
        return null;
    }

    // Creates the neural networks for the basic players
    NeuralNetwork[] createNeuralNetworksBasic()
    {
        // First, the ammount of parameters is calculated
        int neuronsOnFirstLayer = 9; //3 for the position of the player, 3 for the position of the objective, 3 for the position of the enemy

        // Then, we create the neural networks
        NeuralNetwork[] neuralNetworks = new NeuralNetwork[numberOfZonesPerSide * numberOfZonesPerSide];
        for (int i = 0; i < neuralNetworks.Length; i++)
        {
            neuralNetworks[i] = new NeuralNetwork(neuronsOnFirstLayer,2,3,5);
        }

        return neuralNetworks;
    }

    // Selects the best players, and copies their neural network to the list of neural networks
    void evolve(){
        // Searches the players with the best score, copies their neural networks, and evolves them
        
        // Sorts the players by score
        players.Sort((x, y) => x.GetComponentInChildren<PlayerController>().points.CompareTo(y.GetComponentInChildren<PlayerController>().points));
        maxAwardedScore = players[players.Count - 1].GetComponentInChildren<PlayerController>().points;

        // If there are n*n scenarios, each one with one player, we select the best n players to copy their neural network n times
        for (int i = 0; i < numberOfZonesPerSide; i++)
        {
            for (int j = 0; j < numberOfZonesPerSide; j++)
            {
                // Copies the neural network of the best player to the list of neural networks
                int index = (i * numberOfZonesPerSide) + j;
                neuralNetworks[index] = players[players.Count - 1 - index].GetComponentInChildren<PlayerController>()._neuralNetwork;
            }
        }

        // Then, we can mutate the neural networks
        for (int i = 0; i < neuralNetworks.Length; i++)
        {
            neuralNetworks[i].Mutate();
        }
    }

    //delete all the objects in the scene
    void deleteObjects(){
        //delete players
        for (int i = 0; i < players.Count; i++)
        {
            Destroy(players[i]);
        }
        players.Clear();
        //delete enemies
        for (int i = 0; i < enemies.Count; i++)
        {
            Destroy(enemies[i]);
        }
        enemies.Clear();
        //delete objectives
        for (int i = 0; i < objectives.Count; i++)
        {
            Destroy(objectives[i]);
        }
        objectives.Clear();
        //delete walls
        for (int i = 0; i < walls.Count; i++)
        {
            Destroy(walls[i]);
        }
        walls.Clear();

        //delete zoneObjects
        zoneObjects = new List<List<List<GameObject>>>();
    }





    // Spawning functions
    // Variables used for spawning
    int zoneStartingX; // Starting point of each zone
    int zoneStartingY; 
    int zoneDispersionX; // End point of each zone
    int zoneDispersionY;
    int dispersionX; // Same as zoneSize, can be overwritten to account for collisions with barriers
    int dispersionY;

    // Spawns the barriers of the zone, and updates the limits for spawning
    void spawnBarriers(int i, int j)
    {
        // calculate starting point of each zone
        zoneStartingX = startingX + (i * zoneSize);
        zoneStartingY = startingY + (j * zoneSize);

        // calculate dispersion of each zone
        zoneDispersionX = zoneStartingX + zoneSize;
        zoneDispersionY = zoneStartingY + zoneSize;
        dispersionX = Mathf.Abs(zoneDispersionX - zoneStartingX);
        dispersionY = Mathf.Abs(zoneDispersionY - zoneStartingY);

        // update limits for spawning, so that walls don't spawn inside the barriers
        zoneStartingX += 1;
        zoneStartingY += 1;
        zoneDispersionX -= 1;
        zoneDispersionY -= 1;
        dispersionX -= 1;
        dispersionY -= 1;
        
        //spawn the barriers for each zone. The inital object is a cube, which will be scaled to the correct size and instanced four times
        //first cube:
        GameObject firstBarrier = Instantiate(wallPrefab, new Vector3(zoneStartingX + dispersionX / 2, -1, zoneStartingY), Quaternion.identity);
        firstBarrier.transform.localScale = new Vector3(zoneSize, 1, 1);
        //second cube:
        GameObject secondBarrier = Instantiate(wallPrefab, new Vector3(zoneStartingX, -1, zoneStartingY + dispersionY / 2), Quaternion.identity);
        secondBarrier.transform.localScale = new Vector3(1, 1, zoneSize);
        //third cube:
        GameObject thirdBarrier = Instantiate(wallPrefab, new Vector3(zoneStartingX + dispersionX / 2, -1, zoneStartingY + dispersionY), Quaternion.identity);
        thirdBarrier.transform.localScale = new Vector3(zoneSize, 1, 1);
        //fourth cube:
        GameObject fourthBarrier = Instantiate(wallPrefab, new Vector3(zoneStartingX + dispersionX, -1, zoneStartingY + dispersionY / 2), Quaternion.identity);
        fourthBarrier.transform.localScale = new Vector3(1, 1, zoneSize);
        //add walls to the list
        walls.Add(firstBarrier);
        walls.Add(secondBarrier);
        walls.Add(thirdBarrier);
        walls.Add(fourthBarrier);

        // update limits for spawning, so that enemies and objectives don't spawn inside the barriers
        zoneStartingX += 5;
        zoneStartingY += 5;
        zoneDispersionX -= 5;
        zoneDispersionY -= 5;
        dispersionX -= 5;
        dispersionY -= 5;
    }

    // Spawns the enemies with the given values for dispersion and number of enemies
    void spawnEnemies(int minimumX, int maximumX, int minimumY, int maximumY, string type)
    {
        for (int i = 0; i < numberOfEnemies; i++)
            {
                // instantiate the enemy prefab (object, position, rotation)
                GameObject enemyObject = Instantiate(enemyPrefab, new Vector3(Random.Range(minimumX, maximumX), 0, Random.Range(minimumY, maximumY)), Quaternion.identity);
                enemyObject.GetComponent<EnemyController>().id = i;
                enemyObject.GetComponent<EnemyController>().type = type;
                //add enemies to the list
                enemies.Add(enemyObject);
            }
    }

    // Spawns the objectives with the given values for dispersion and number of objectives
    void spawnObjectives(int minimumX, int maximumX, int minimumY, int maximumY)
    {
        for (int i = 0; i < numberOfObjectives; i++)
            {
                // instantiate the objective prefab (object, position, rotation)
                GameObject objectiveObject = Instantiate(objectivePrefab, new Vector3(Random.Range(minimumX, maximumX), 0, Random.Range(minimumY, maximumY)), Quaternion.identity);
                //add objectives to the list
                objectives.Add(objectiveObject);
            }
    }

    // Spawns the players with the given values for dispersion


    void spawnRunners(NeuralNetwork[] neuralNetworks){
        // Calculate starting point of each zone with 2 loops:
        for (int i = 0; i < numberOfZonesPerSide; i++)
        {
            for (int j = 0; j < numberOfZonesPerSide; j++)
            {
                
                spawnBarriers(i, j);

                spawnEnemies(zoneStartingX, zoneDispersionX, zoneStartingY, zoneDispersionY, "Light");
                
                spawnObjectives(zoneStartingX, zoneDispersionX, zoneStartingY, zoneDispersionY);
                
                // spawn player
                GameObject playerObject = Instantiate(playerPrefab, new Vector3(Random.Range(zoneStartingX, zoneDispersionX), 0, Random.Range(zoneStartingY, zoneDispersionY)), Quaternion.identity);
                playerObject.GetComponentInChildren<PlayerController>()._neuralNetwork = neuralNetworks[i * numberOfZonesPerSide + j];
                players.Add(playerObject);
                
                zoneObjects.Add(new List<List<GameObject>>());
                zoneObjects[zoneObjects.Count - 1].Add(new List<GameObject>()); //enemies
                zoneObjects[zoneObjects.Count - 1].Add(new List<GameObject>()); //objectives

                //add all elements to the list of elements of the zone
                for (int k = 0; k < numberOfEnemies; k++)
                {
                    zoneObjects[zoneObjects.Count - 1][0].Add(enemies[k]);
                }
                for (int k = 0; k < numberOfObjectives; k++)
                {
                    zoneObjects[zoneObjects.Count - 1][1].Add(objectives[k]);
                }

                playerObject.GetComponentInChildren<PlayerController>().zoneObjects = zoneObjects[(i * numberOfZonesPerSide) + j];
            }
        }
    }

    void spawnShooters(NeuralNetwork[] neuralNetworks)
    {
        
    }

    void spawnBoth(NeuralNetwork[] neuralNetworks)
    {
        
    }

    // Same as the spawn Runners, but only one reward and enemy is spawned, the reward its always at the center
    void spawnBasic(NeuralNetwork[] neuralNetworks)
    {
        // calculate starting point of each zone with 2 loops:
        for (int i = 0; i < numberOfZonesPerSide; i++)
        {
            for (int j = 0; j < numberOfZonesPerSide; j++)
            {
                spawnBarriers(i, j);

                spawnEnemies(zoneStartingX, zoneDispersionX, zoneStartingY, zoneDispersionY, "Light");
                
                spawnObjectives(zoneStartingX + (dispersionX / 2), zoneStartingX + (dispersionX / 2), zoneStartingY + (dispersionY / 4), zoneStartingY + (dispersionY / 4));             

                // spawn player
                GameObject playerObject = Instantiate(playerPrefab, new Vector3(zoneStartingX + dispersionX / 2, 0, zoneStartingY), Quaternion.identity);
                playerObject.GetComponentInChildren<PlayerController>()._neuralNetwork = neuralNetworks[i * numberOfZonesPerSide + j];
                players.Add(playerObject);
                
                zoneObjects.Add(new List<List<GameObject>>());
                zoneObjects[zoneObjects.Count - 1].Add(new List<GameObject>()); //enemies
                zoneObjects[zoneObjects.Count - 1].Add(new List<GameObject>()); //objectives

                //add all elements to the list of elements of the zone
                for (int k = 0; k < numberOfEnemies; k++)
                {
                    zoneObjects[zoneObjects.Count - 1][0].Add(enemies[k]);
                }
                for (int k = 0; k < numberOfObjectives; k++)
                {
                    zoneObjects[zoneObjects.Count - 1][1].Add(objectives[k]);
                }

                playerObject.GetComponentInChildren<PlayerController>().zoneObjects = zoneObjects[(i * numberOfZonesPerSide) + j];
            }
        }
    }
}

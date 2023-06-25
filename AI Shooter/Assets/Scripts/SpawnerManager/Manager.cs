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
    [SerializeField] int typeOfTraining = 0; // 0 = runner, 1 = shooter, 2 = both
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
        int neuronsOnFirstLayer = 6; //2 for the position of the player, 2 for the position of the objective, 2 for the position of the enemy

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

    // Selects the best players, and copies their neural network to the list of neural networks
    void evolve(){
        // Searches the players with the best score, copies their neural networks, and evolves them
        
        // Sorts the players by score
        players.Sort((x, y) => x.GetComponentInChildren<PlayerController>().points.CompareTo(y.GetComponentInChildren<PlayerController>().points));

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

    void spawnRunners(NeuralNetwork[] neuralNetworks){
        // calculate starting point of each zone with 2 loops:
        for (int i = 0; i < numberOfZonesPerSide; i++)
        {
            for (int j = 0; j < numberOfZonesPerSide; j++)
            {
                // calculate starting point of each zone
                int zoneStartingX = startingX + (i * zoneSize);
                int zoneStartingY = startingY + (j * zoneSize);

                // calculate dispersion of each zone
                int zoneDispersionX = zoneStartingX + zoneSize;
                int zoneDispersionY = zoneStartingY + zoneSize;
                int dispersionX = Mathf.Abs(zoneDispersionX - zoneStartingX);
                int dispersionY = Mathf.Abs(zoneDispersionY - zoneStartingY);

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

                // spawn all enemies
                for (int k = 0; k < numberOfEnemies; k++)
                {
                    // instantiate the enemy prefab (object, position, rotation)
                    GameObject enemyObject = Instantiate(enemyPrefab, new Vector3(Random.Range(zoneStartingX, zoneDispersionX), 0, Random.Range(zoneStartingY, zoneDispersionY)), Quaternion.identity);
                    enemyObject.GetComponent<EnemyController>().id = k;
                    enemyObject.GetComponent<EnemyController>().type = "Light";
                    //add enemies to the list
                    enemies.Add(enemyObject);

                }
                // spawn all objectives
                for (int k = 0; k < numberOfObjectives; k++)
                {
                    // instantiate the objective prefab (object, position, rotation)
                    GameObject objectiveObject = Instantiate(objectivePrefab, new Vector3(Random.Range(zoneStartingX, zoneDispersionX), 0, Random.Range(zoneStartingY, zoneDispersionY)), Quaternion.identity);
                    //add objectives to the list
                    objectives.Add(objectiveObject);
                }
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
}

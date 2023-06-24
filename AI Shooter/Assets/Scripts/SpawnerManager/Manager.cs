using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
        // Start is called before the first frame update

    [SerializeField] int numberOfEnemies = 10;
    [SerializeField] int numberOfObjectives = 10;
    [SerializeField] int numberOfZonesPerSide = 3;
    [SerializeField] int startingX = -100;
    [SerializeField] int startingY = -100;
    [SerializeField] int zoneSize = 30;
    public GameObject enemyPrefab;
    public GameObject objectivePrefab;
    public GameObject playerPrefab;
    public GameObject wallPrefab;

    //list of players, enemies, walls, objectives, and nn
    List<GameObject> players = new List<GameObject>();
    List<GameObject> enemies = new List<GameObject>();
    List<GameObject> walls = new List<GameObject>();
    List<GameObject> objectives = new List<GameObject>();
    NeuralNetwork[] neuralNetworks;
    //list that contains a list of all the objects of each zone, listed by type, which will be provided to the players
    List<List<List<GameObject>>> zoneObjects = new List<List<List<GameObject>>>(); //first, we have the zones, then the type of object, then the objects themselves: enemies, objectives
  

    //timer, counter
    float timer = 0f;
    int generation = 0;
    int maxAwardedScore = 0;


    void Start()
    {
        //the first time, we spawn with blank nn

        //first, we get the parameters
        int neuronsOnFirstLayer = (2 * objectives.Count) + (2 * enemies.Count) + 2; //2 for the position of the player

        //then, we create the neural networks
        neuralNetworks = new NeuralNetwork[numberOfZonesPerSide * numberOfZonesPerSide];
        for (int i = 0; i < neuralNetworks.Length; i++)
        {
            neuralNetworks[i] = new NeuralNetwork(neuronsOnFirstLayer,2,5,10);
        }

        spawn(neuralNetworks);

        //once everything is setup, we can wait and then get the best players, mutate their neural network and spawn them again
    }

    // Selects the best players, and copies their neural network to the list of neural networks

    void getBestPlayers(){
        //searches for the player with the highest score
        int maxScore = 0;
        int maxScoreIndex = 0;
        //lenght of the player list
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponentInChildren<PlayerController>().points > maxScore)
            {
                maxScore = players[i].GetComponentInChildren<PlayerController>().points;
                maxScoreIndex = i;
            }
        }
        maxAwardedScore = maxScore; 
        //once we have the best player, we copy its neural network to the list of neural networks
        for (int i = 0; i < neuralNetworks.Length; i++)
        {
            neuralNetworks[i] = players[maxScoreIndex].GetComponentInChildren<PlayerController>()._neuralNetwork;
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 15f)
        {
            //get the best x players players
            getBestPlayers();

            //delete all the objects in the scene
            deleteObjects();
            
            //mutate the neural networks
            for (int i = 0; i < neuralNetworks.Length; i++)
            {
                neuralNetworks[i].Mutate();
            }

            //spawn them
            spawn(neuralNetworks);
            //reset timer
            timer = 0f;

            //print info generation
            generation++;
            Debug.Log("Generation: " + generation + ". Max Score: " + maxAwardedScore);
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

    void spawn(NeuralNetwork[] neuralNetworks){
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
}

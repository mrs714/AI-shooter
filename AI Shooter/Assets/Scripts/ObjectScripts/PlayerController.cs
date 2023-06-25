using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Public variables
    public float speed = 1000f;
    public int points = 0; // Points are used to reward the player for good actions and punish for bad ones
    public NeuralNetwork _neuralNetwork; // The neural network is assigned from the manager
    public List<List<GameObject>> zoneObjects; // List that contains a list with a list of all the objects of each zone, sorted by type, provided to the players; enemies, objectives
        
    // Private variables
    Rigidbody _rigidbody = null;
    Vector3 previousPosition = Vector3.zero;
    
    // Assign the rigidbody
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        
    }

    // Handles input and rewards for distance
    private void FixedUpdate() 
    {
        // Checks
        if (_neuralNetwork == null || _rigidbody == null || zoneObjects == null)
        {
            return;
        }

        // Input
        float[] input = createInput();

        // Move the player
        move(input);

        // Rewards and punishments
        rewardPlayer();

    }

    // Collision handler
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "Enemy")
        {
            addReward(-1000);
        }
        else if (collision.gameObject.tag == "Reward")
        {
            addReward(20000);
        }
    }

    // Sustained collision handler
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "Enemy")
        {
            //reward
            addReward(-1);
        }
    }

    // Other methods
    public void addReward(int reward)
    {
        points += reward;
    }

    // Find closer element of the type provided
    public GameObject findClosestElement(string type)
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        List<GameObject> objects = new List<GameObject>(zoneObjects[0]);
        foreach (GameObject element in objects)
        {
            if (element.tag == type)
            {
                Vector3 diff = element.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = element;
                    distance = curDistance;
                }
            }
        }
        return closest;
    }

    // Create input array
    public float[] createInput()
    {
        float[] input = new float[9]; // Position of the player, position of the closest enemy, position of the closest objective (x and z)

        // Position of the player
        input[0] = transform.position.x;
        input[1] = transform.position.y;
        input[2] = transform.position.z;

        // Position of the closest enemy
        GameObject closestEnemy = findClosestElement("Enemy");
        if (closestEnemy != null)
        {
            // Calculate the direction vector to the enemy
            Vector3 direction = closestEnemy.transform.position - transform.position;
            input[3] = direction.x;
            input[4] = direction.y;
            input[5] = direction.z;
        }
        else
        {
            input[3] = 1000;
            input[4] = 1000;
            input[5] = 1000;
        }
        
        // Position of the closest objective
        GameObject closestObjective = findClosestElement("Reward");
        if (closestObjective != null)
        {
            // Calculate the direction vector to the objective
            Vector3 direction = closestObjective.transform.position - transform.position;
            input[6] = direction.x;
            input[7] = direction.y;
            input[8] = direction.z;
        }
        else
        {
            input[6] = 1000;
            input[7] = 1000;
            input[8] = 1000;
        }

        return input;
    }

    // Move the player
    public void move(float[] input)
    {
        float[] output = _neuralNetwork.Calculate(input);
        Vector3 movement = new Vector3(output[0], 0, output[1]);
        _rigidbody.AddRelativeForce(movement * speed, ForceMode.Force);
    }

    // Reward the player 
    public void rewardPlayer()
    {
        float distance = 0;
        for (int i = 0; i < zoneObjects[1].Count; i++)
        {
            distance += Vector3.Distance(transform.position, zoneObjects[1][i].transform.position);
        }
        addReward(- (int)(distance));
        addReward((int)Mathf.Abs((Vector3.Distance(transform.position, previousPosition) / 100)));
    }
}

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

        // Paint the player
        paintPlayer();

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
            // Put camera on the objective
            Camera.main.transform.position = new Vector3(collision.gameObject.transform.position.x, 100, collision.gameObject.transform.position.z);
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
        objects.AddRange(zoneObjects[1]);
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
        float[] input = new float[4]; // Position of the closest objective (x and z)
        
        // Position of the closest objective
        GameObject closestObjective = findClosestElement("Reward");
        if (closestObjective != null)
        {
            // Calculate the direction vector to the objective
            Vector3 direction = closestObjective.transform.position - transform.position;
            input[0] = direction.x;
            input[1] = direction.z;
        }
        else
        {
            input[0] = 1000;
            input[1] = 1000;
            Debug.Log("No objective found");
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
            distance += Mathf.Abs(Vector3.Distance(transform.position, zoneObjects[1][i].transform.position));
        }
        addReward(- (int)(distance / 10));
        addReward((int)Mathf.Abs((Vector3.Distance(transform.position, previousPosition) / 15)));
    }

    // Paint the player depending on the points (better --> greener, worse --> redder)
    public void paintPlayer()
    {
        float r = 0;
        float g = 0;
        float b = 0;
        if (points < 0)
        {
            g = 1;
            r = 1 - (points / 10000);
        }
        else
        {
            r = 1;
            g = 1 + (points / 10000);
        }
        GetComponentInChildren<Renderer>().material.color = new Color(r, g, b);
        // Print color
        // Debug.Log("R: " + r + " G: " + g + " B: " + b);
        // Print points
        // Debug.Log("Points: " + points);
    }
}

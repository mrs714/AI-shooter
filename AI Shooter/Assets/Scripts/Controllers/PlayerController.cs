using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 500f;
    public int points = 0;
    public NeuralNetwork _neuralNetwork; //the neural network is assigned from the manager

    Rigidbody _rigidbody = null;
    
    //list that contains a list with a list of all the objects of each zone, sorted by type, provided to the players; enemies, objectives
    public List<List<GameObject>> zoneObjects;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    private void FixedUpdate() 
    {
        //check that the nn exists
        if (_neuralNetwork == null)
        {
            return;
        }

        //check that the list of objects exists
        if (zoneObjects == null)
        {
            return;
        }
        
        //get input from the list of objects
        float[] input = new float[zoneObjects[0].Count * 2 + zoneObjects[1].Count * 2 + 2];

        //first, we get the position of the player, and assign to the input array
        input[0] = transform.position.x;
        input[1] = transform.position.z;

        //then, we get the position of the elements. First the enemies
        for (int i = 0; i < zoneObjects[0].Count; i++)
        {
            input[2 + i] = zoneObjects[0][i].transform.position.x;
            input[2 + i + 1] = zoneObjects[0][i].transform.position.z;
        }

        //then, the objectives  
        for (int i = 0; i < zoneObjects[1].Count; i++)
        {
            input[2 + zoneObjects[0].Count * 2 + i] = zoneObjects[1][i].transform.position.x;
            input[2 + zoneObjects[0].Count * 2 + i + 1] = zoneObjects[1][i].transform.position.z;
        }
      
        //calculate output
        float[] output = _neuralNetwork.Calculate(input);

        //move
        Vector3 movement = new Vector3(output[0], 0, output[1]);
        _rigidbody.AddRelativeForce(movement * speed, ForceMode.Force);

        //reward closeness to the objectives
        float distance = 0;
        for (int i = 0; i < zoneObjects[1].Count; i++)
        {
            distance += Vector3.Distance(transform.position, zoneObjects[1][i].transform.position);
        }
        addReward(100 - (int)distance);

    }

    //collision handler
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "Enemy")
        {
            //reward
            addReward(-100);
        }
        else if (collision.gameObject.tag == "Reward")
        {
            //reward
            addReward(1000);
        }
    }
    //sustained collision handler
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "Enemy")
        {
            //reward
            addReward(-1);
        }
    }

    public void addReward(int reward)
    {
        points += reward;
    }
}

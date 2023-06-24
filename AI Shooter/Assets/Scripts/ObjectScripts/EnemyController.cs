using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //properties:
    public int id;
    public Vector3 position;
    public string type;
    int health;
    int damage;


    //constructor with id, position and type:
    public EnemyController(int id, Vector3 position, string type)
    {
        this.id = id;
        this.type = type; //light, medium, heavy
    }

    // Start is called before the first frame update
    void Start()
    {
        if (type == "Light")
        {
            health = 100;
            damage = 10;
        }
        else if (type == "Medium")
        {
            health = 200;
            damage = 20;
        }
        else if (type == "Heavy")
        {
            health = 300;
            damage = 30;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

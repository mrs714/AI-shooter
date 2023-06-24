using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    //handles collision with the player
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Untagged")
        {
            //destroy goal
            Destroy(this.gameObject);
            print("Goal reached");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    void OnTriggerEnter(Collider col){
        if(col.tag == "MLAgent"){
            col.gameObject.GetComponent<MazeSolver>().goal = true;
        }
    }
}

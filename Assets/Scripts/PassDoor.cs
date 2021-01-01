using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassDoor : MonoBehaviour
{
    public GameObject player;
    public int room_id;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Object Entered the trigger");
        print("P---Object Entered the trigger "+ other.transform.name);
        if(other.transform.name == "player")
        {
            other.gameObject.GetComponent<Gun>().room_id = room_id;

            if(gameObject.name == "escapeDoor")
            {
               // win_reset();
               player.GetComponent<Gun>().win_reset();
            }

        }      
    }


    void OnTriggerStay(Collider other)
    {
        print("P---Object stay in the trigger");
    }

    void OnTriggerExit(Collider other)
    {
        print("P---Object exit the trigger");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.Find("player");
    }
}

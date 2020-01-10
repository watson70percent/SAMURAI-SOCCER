using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugiAI : MonoBehaviour
{
    int menber;
    GameObject goal;
    GameObject[] friends;
    Vector3[] friendVecs;
    Vector3[] friendEffect; 

    // Start is called before the first frame update
    void Awake()
    {
       


        goal = GameObject.FindGameObjectWithTag("Goal");
        var temp = GameObject.FindGameObjectsWithTag("Finish");


        menber = temp.Length - 1;
        friends = new GameObject[menber];
        friendVecs = new Vector3[menber];
        friendEffect = new Vector3[menber];
        int count = 0;
        for(int i = 0; i < menber+1; i++)
        {
            print(i + ":" + (temp[i] != gameObject));
            if(temp[i] != gameObject)
            {
                friends[count] = temp[i];
                print(count + "" + i);
                print(friends[0].transform.position);
                count++;

            }
            
        }
        //print(friends[0].transform.position.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 goalVec = goal.transform.position - transform.position;
        var potential = goalVec / goalVec.magnitude / goalVec.magnitude / goalVec.magnitude;


        for(int i = 0; i < menber; i++)
        {

            friendVecs[i] = friends[i].transform.position - transform.position;
        }


        for (int i=0;i<menber;i++)
        {
            var distance = friendVecs[i].magnitude;
            friendEffect[i] = -friendVecs[i] / distance / distance / distance;
        }

        var direction = Vector3.zero;

        for(int i = 0; i < menber; i++)
        {
            direction += friendEffect[i];
        }
        direction += potential;

    
        transform.Translate(direction.normalized*Time.deltaTime);
    }

}

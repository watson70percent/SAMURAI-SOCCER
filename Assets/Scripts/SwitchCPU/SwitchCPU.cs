using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCPU : MonoBehaviour
{

    public WeightSet[] gameObjectsAndWeight;
    // Start is called before the first frame update
    void Start()
    {


        float weightSum = 0;
        foreach (WeightSet set in gameObjectsAndWeight)
        {
            weightSum += set.weight;
        }

        if (weightSum != 0)
        {
            float random = Random.value * weightSum;
            bool haveRearchedZero = false;
            for (int i = 0; i < gameObjectsAndWeight.Length; i++)
            {
                random -= gameObjectsAndWeight[i].weight;

                if (random <= 0 && !haveRearchedZero)
                {
                    haveRearchedZero = true;
                    gameObjectsAndWeight[i].gameObject.SetActive(true);
                }
                else
                {
                    gameObjectsAndWeight[i].gameObject.SetActive(false);
                }
            }

            if (!haveRearchedZero) { gameObjectsAndWeight[0].gameObject.SetActive(true); }

        }
        else
        {
            for (int i = 0; i < gameObjectsAndWeight.Length; i++)
            {
                if (i == 0) { gameObjectsAndWeight[i].gameObject.SetActive(true); }
                else
                {
                    gameObjectsAndWeight[i].gameObject.SetActive(false);
                }
            }
        }
    }

    [System.Serializable]
    public struct WeightSet
    {
        public GameObject gameObject;
        public float weight;
    }
}

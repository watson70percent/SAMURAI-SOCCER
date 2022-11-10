using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{

    Image image;
    // Start is called before the first frame update
    void Start()
    {
        transform.parent = GameObject.Find("Canvas").transform;
        this.transform.position = new Vector2(Screen.width / 2, Screen.height / 2);
        image = GetComponent<Image>();
    }


    float time = 0;
    Color col = new Color(0, 0, 0, 0);
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        col.a = time;
        image.color = col;
        if (time > 1)
        {
            SceneManager.LoadScene("Result");
        }
    }
}

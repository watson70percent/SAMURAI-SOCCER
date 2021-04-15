﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PanjanRoll : MonoBehaviour
{
    Transform player;
    public float moveSpeed;
    public float rotSpeed;
    bool exploded;
    GameObject rot;
    public GameObject fire;
    public int partMax;

    GameManager gameManager;
    bool isEnd;
    private string ResultSceneName = "Result";

    // Start is called before the first frame update
    void Start()
    {
        rot = transform.GetChild(0).gameObject;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

        Move();
    }

    void Move()
    {
        if (gameManager.CurrentGameState == GameState.Standby && !exploded) {
            selfExplode();
        }
            if (!exploded) { 
        // 制限なしの回転を求め...
        var rotation = Quaternion.LookRotation(player.transform.position +Vector3.up*2 - transform.position);

        // その回転角を_maxAngleまでに制限した回転を作り、それをrotationにセットする
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotSpeed * Time.deltaTime);

        //transform.rotation = RotateTowards(this.transform.rotation, player.rotation, 10*Time.deltaTime);
        //    LookAt(player, Vector3.up);
        rot.transform.Rotate(moveSpeed, 0, 0);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.transform.root.name);
        if (!exploded)
        {
            if (other.transform.root.tag == "Slash" || other.gameObject.tag == "Player")
            {
                Explode();
                exploded = true;
            }
        }
    }

    void Explode()
    {
        GetComponent<AudioSource>().Play();
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.AddForce(0, 2000, 0);
        this.GetComponent<CapsuleCollider>().enabled = false;
        int index = 0;
        foreach (Transform part in rot.transform)
        {
            if (index < 5) Instantiate(fire, part.position,Quaternion.identity,part);
            //part.gameObject.AddComponent<Rigidbody>().AddForce(Random.Range(-1000, 1000), Random.Range(0, 1000), Random.Range(-1000, 1000));
            part.gameObject.AddComponent<PanjanExplode>();
            //part.gameObject.AddComponent<Rigidbody>().AddForce((player.transform.position + Vector3.up * 2 - part.position).normalized*1000);
            part.gameObject.AddComponent<Rigidbody>().AddForce(transform.forward * 1000);
            index++;
        }
        //for (int i = 0; i < partMax; i++)
        //{
        //    Instantiate(part, transform.position, Quaternion.identity, null).GetComponent<Rigidbody>().AddForce(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100));
        //}
        StartCoroutine(waitDestroy());
    }

    void selfExplode()
    {
        GetComponent<AudioSource>().Play();
        exploded = true;
        foreach (Transform part in rot.transform)
        {
            //part.gameObject.AddComponent<Rigidbody>().AddForce(Random.Range(-1000, 1000), Random.Range(0, 1000), Random.Range(-1000, 1000));
            //part.gameObject.AddComponent<PanjanExplode>();
            //part.gameObject.AddComponent<Rigidbody>().AddForce((player.transform.position + Vector3.up * 2 - part.position).normalized*1000);
            part.gameObject.AddComponent<Rigidbody>().AddForce(transform.up * 1000);
        }
        StartCoroutine(waitDestroy());
    }

    IEnumerator waitDestroy()
    {
        yield return new WaitForSeconds(4.0f);
        Destroy(gameObject);
    }
}

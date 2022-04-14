using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;


public class Spawner : MonoBehaviour
{
    // Variables
    [SerializeField] private float spreadRadius = 6.0f;
    [SerializeField] private float spreadHeight = 5f;
    [SerializeField] private List<Rule> rules;
    
    [Serializable]
    public struct Rule
    {
        [SerializeField] public GameObject instance;
        [SerializeField] public int amount;
    }
    
    // Properties
    public Vector3 randomPosition => new Vector3(
        Random.Range(transform.position.x - spreadRadius, transform.position.x + spreadRadius),
        Random.Range(transform.position.y - spreadHeight/2f, transform.position.y + spreadHeight/2f),
        Random.Range(transform.position.z - spreadRadius, transform.position.z + spreadRadius)
    );
    
    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnDrawGizmosSelected()
    {   
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, new Vector3(spreadRadius*2f, spreadHeight, spreadRadius*2));
    }

    private void Spawn()
    {
        foreach (var rule in rules)
            for (var i = 0; i < rule.amount; i++)
                PhotonNetwork.Instantiate(rule.instance.name, randomPosition, Quaternion.identity);
    }
}

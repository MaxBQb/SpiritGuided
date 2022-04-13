using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Variables
    [SerializeField] private float radius = 6.0f;
    [SerializeField] private float heightSpread = 5f;
    [SerializeReference] private GameObject instance;
    
    // Properties
    public Vector3 randomPosition => new Vector3(
        Random.Range(transform.position.x - radius, transform.position.x + radius),
        Random.Range(transform.position.y - heightSpread/2f, transform.position.y + heightSpread/2f),
        Random.Range(transform.position.z - radius, transform.position.z + radius)
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
        Gizmos.DrawWireCube(transform.position, new Vector3(radius*2f, heightSpread, radius*2));
    }

    void Spawn()
    {
        PhotonNetwork.Instantiate(instance.name, randomPosition, Quaternion.identity);
    }
}

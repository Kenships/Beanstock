using System.Collections;
using UnityEngine;

public class RespawnHolder : MonoBehaviour
{
    public GameObject enemy;
    const float respawnTime = 2;
    private Vector3 respawnPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(doRespawn());
    }

    IEnumerator doRespawn(){
        enemy.SetActive(false);
        yield return new WaitForSeconds(respawnTime);
        enemy.SetActive(true);
        enemy.transform.position = transform.position;
        Destroy(gameObject);
    }
}
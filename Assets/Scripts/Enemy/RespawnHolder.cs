using System.Collections;
using UnityEngine;

public class RespawnHolder : MonoBehaviour
{
    public GameObject enemy;
    const float respawnTime = 3.5f;
    private Vector3 respawnPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DoRespawn());
    }

    IEnumerator DoRespawn(){
        enemy.SetActive(false);
        yield return new WaitForSeconds(respawnTime);
        enemy.SetActive(true);
        enemy.transform.position = transform.position;
        Destroy(gameObject);
    }
}

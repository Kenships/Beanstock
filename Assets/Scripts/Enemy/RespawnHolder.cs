using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class RespawnHolder : MonoBehaviour
    {
        public GameObject RespawnObject
        {
            get => _respawnObject;
            set
            {
                _respawnObject = value;
                StartCoroutine(DoRespawn());
            }
        }

        private GameObject _respawnObject;
        private const float RespawnTime = 3.5f;

        public void Respawn(GameObject gameObject)
        {
            _respawnObject = gameObject;
            StartCoroutine(DoRespawn());
        }

        IEnumerator DoRespawn(){
            RespawnObject.SetActive(false);
            yield return new WaitForSeconds(RespawnTime);
            RespawnObject.SetActive(true);
            RespawnObject.transform.position = transform.position;
            ObjectPoolManager.RecycleObject(gameObject);
        }
    }
}

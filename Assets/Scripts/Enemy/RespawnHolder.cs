using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class RespawnHolder : MonoBehaviour
    {
        public bool dontRespawn;
        [SerializeField] private GameObject respawnPoof;
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
        public void Respawn(GameObject respawnObject, float time = RespawnTime)
        {
            _respawnObject = respawnObject;
            StartCoroutine(DoRespawn(time));
        }
        public void InstantRespawn(GameObject respawnObject)
        {
            _respawnObject = respawnObject;
            respawnObject.transform.position = transform.position;
            ObjectPoolManager.RecycleObject(gameObject);
        }

        IEnumerator DoRespawn(float respawnTime = RespawnTime){
            if(dontRespawn) Destroy(gameObject);
            _respawnObject.SetActive(false);
            Instantiate(respawnPoof, _respawnObject.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(respawnTime);
            _respawnObject.SetActive(true);
            _respawnObject.transform.position = transform.position;
            Instantiate(respawnPoof, transform.position, Quaternion.identity);
            ObjectPoolManager.RecycleObject(gameObject);
        }
    }
}

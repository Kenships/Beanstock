using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    private static readonly List<PooledObjectInfo> PooledObjects = new();

    private GameObject _poolObjectHolder;
    
    private static GameObject _particleHolder;
    private static GameObject _gameObjectHolder;
    private static GameObject _soundHolder;

    private void Awake()
    {
        InitializeObjectHolders();
    }

    private void InitializeObjectHolders()
    {
        _poolObjectHolder = new GameObject("Pooled Objects");
        
        _gameObjectHolder = new GameObject("GameObjects");
        _gameObjectHolder.transform.SetParent(_poolObjectHolder.transform);
        
        _soundHolder = new GameObject("Sounds");
        _soundHolder.transform.SetParent(_gameObjectHolder.transform);
        
        _particleHolder = new GameObject("Particle Effects");
        _particleHolder.transform.SetParent(_poolObjectHolder.transform);
    }

    public static GameObject SpawnObject(GameObject spawnedObject, Vector3 position, Quaternion rotation, PoolType poolType = PoolType.GameObject)
    {
        PooledObjectInfo pool = PooledObjects.Find(obj => obj.LookupString == spawnedObject.name);

        if (pool == null)
        {
            pool = new PooledObjectInfo() { LookupString = spawnedObject.name };
            PooledObjects.Add(pool);
        }

        GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

        if (spawnableObj == null)
        {
            spawnableObj = Instantiate(spawnedObject, position, rotation);
            GameObject holder = GetObjectHolder(poolType);
            if (holder != null)
            {
                spawnableObj.transform.SetParent(holder.transform);
            }
        }
        else
        {
            spawnableObj.transform.position = position;
            spawnableObj.transform.rotation = rotation;
            pool.InactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }
        
        return spawnableObj;
    }
    
    public static GameObject SpawnObject(GameObject spawnedObject, Transform parent)
    {
        PooledObjectInfo pool = PooledObjects.Find(obj => obj.LookupString == spawnedObject.name);

        if (pool == null)
        {
            pool = new PooledObjectInfo() { LookupString = spawnedObject.name };
            PooledObjects.Add(pool);
        }

        GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

        if (spawnableObj == null)
        {
            spawnableObj = Instantiate(spawnedObject, parent);
        }
        else
        {
            spawnableObj.transform.SetParent(parent);
            pool.InactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }
        
        return spawnableObj;
    }

    public static GameObject RecycleObject(GameObject spawnedObject)
    {
        string clippedName = spawnedObject.name.Replace("(Clone)", "");
        PooledObjectInfo pool = PooledObjects.Find(obj => obj.LookupString == clippedName);

        if (pool == null)
        {
            pool = new PooledObjectInfo() { LookupString = spawnedObject.name};
            PooledObjects.Add(pool);
        }
        
        spawnedObject.SetActive(false);
        pool.InactiveObjects.Add(spawnedObject);
        
        return spawnedObject;
    }

    private static GameObject GetObjectHolder(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.GameObject:
                return _gameObjectHolder;
            case PoolType.Particles:
                return _particleHolder;
            case PoolType.Sounds:
                return _soundHolder;
            default:
                return null;
        }
    }
}

public enum PoolType
{
    GameObject, Particles, Sounds, None
}
public class PooledObjectInfo
{
    public string LookupString;
    public readonly List<GameObject> InactiveObjects = new();
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPool
{
    // Setup vars
    public string _poolName;
    public GameObject _prefab;
    public int _count;
    public bool _disableOnSpawn = true;
    GameObject pool;

    // Initialize queue for pooled items
    public Queue<GameObject> items = new Queue<GameObject>();

    public ObjectPool(string poolName, GameObject prefab, int count)
    {
        _poolName = poolName;
        _prefab = prefab;
        _count = count;

        Awake();
    }

    public ObjectPool(string poolName, GameObject prefab, int count, bool disableOnSpawn)
    {
        _poolName = poolName;
        _prefab = prefab;
        _count = count;
        _disableOnSpawn = disableOnSpawn;

        Awake();
    }

    private void Awake()
    {
        pool = new GameObject(_poolName);

        for (int i = 0; i < _count; i++)
        {
            GameObject _item = Object.Instantiate(_prefab, pool.transform);
            items.Enqueue(_item);
            if (_disableOnSpawn)
            {
                _item.SetActive(false);
            }
        }
    }

    public GameObject Instantiate(Vector3 position, Quaternion rotation)
    {
        if (items.Count > 0)
        {
            GameObject _item = items.Dequeue();

            _item.SetActive(true);
            _item.transform.position = position;
            _item.transform.rotation = rotation;

            items.Enqueue(_item);

            if (_item.GetComponent<Rigidbody>() != null)
            {
                _item.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }

            return _item;
        }
        Debug.LogWarning("ObjectPool '" + _poolName + "' is empty.");
        return null;
    }

    //public void Destroy(GameObject gameObject)
    //{
    //    if (items.Contains(gameObject))
    //    {
    //        gameObject.SetActive(false);
    //        items.Enqueue(gameObject);
    //    } 
    //}

    // Destroy pool GameObject
    public void DestroyPool()
    {
        GameObject.Destroy(pool);
    }
}

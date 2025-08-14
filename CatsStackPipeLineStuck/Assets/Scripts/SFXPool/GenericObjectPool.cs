using System.Collections.Generic;
using UnityEngine;

public interface IPool
{
    void ReturnToPool(Component obj);
}

public class GenericObjectPool<T> : MonoBehaviour, IPool where T : MonoBehaviour
{
    [SerializeField] private uint initPoolSize = 8;
    public uint InitPoolSize => initPoolSize;

    [SerializeField] private T objectToPool;
    [SerializeField] protected List<T> objectsToPool;

    private void Start() => SetupPool();

    private void SetupPool()
    {
        if (objectToPool == null)
        {
            Debug.LogError("[Pool] ObjectToPool prefab is not assigned.", this);
            return;
        }

        objectsToPool = new List<T>((int)initPoolSize);

        for (int i = 0; i < initPoolSize; i++)
        {
            var instance = Instantiate(objectToPool, transform);
            instance.gameObject.SetActive(false);

            // Attach pool to the instance if it has PooledObject
            if (instance.TryGetComponent<PooledObject>(out var po))
                po.AttachPool(this); // pass as IPool

            objectsToPool.Add(instance);
        }
    }

    /// <summary>Gets an inactive instance or grows the pool.</summary>
    public T GetPooledObject()
    {
        if (objectToPool == null)
        {
            Debug.LogError("[Pool] ObjectToPool prefab is not assigned.", this);
            return null;
        }

        // Reuse an inactive instance
        for (int i = 0; i < objectsToPool.Count; i++)
        {
            var inst = objectsToPool[i];
            if (!inst.gameObject.activeSelf)
            {
                inst.gameObject.SetActive(true);
                return inst;
            }
        }

        // Grow pool
        var newInstance = Instantiate(objectToPool, transform);
        if (newInstance.TryGetComponent<PooledObject>(out var po))
            po.AttachPool(this);

        newInstance.gameObject.SetActive(true);
        objectsToPool.Add(newInstance);
        return newInstance;
    }

    /// <summary>Typed return for this pool’s T.</summary>
    public void ReturnToPool(T pooledObject)
    {
        if (!pooledObject) return;

        pooledObject.transform.SetParent(transform);
        pooledObject.gameObject.SetActive(false);

        // Ensure it stays tracked exactly once
        if (!objectsToPool.Contains(pooledObject))
            objectsToPool.Add(pooledObject);
    }

    /// <summary>Non-generic return so PooledObject can call back without knowing T.</summary>
    void IPool.ReturnToPool(Component obj)
    {
        var cast = obj as T;
        if (cast) ReturnToPool(cast);
    }
}

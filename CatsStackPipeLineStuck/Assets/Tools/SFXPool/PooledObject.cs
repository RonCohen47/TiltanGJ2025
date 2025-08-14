using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public IPool Pool { get; private set; }
    public virtual void AttachPool(IPool attachedPool)
    {
        Pool = attachedPool;
        if (Pool == null) Debug.LogError("Pool not set on PooledObject.", this);
    }
}

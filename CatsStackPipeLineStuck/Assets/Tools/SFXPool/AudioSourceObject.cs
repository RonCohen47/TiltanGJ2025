using System.Collections;
using UnityEngine;

//example how to use.
public class AudioSourceObject : PooledObject
{
    public AudioSource Source;
    private bool _autoReturn;
    private void Update()
    {
        // Auto return only for non-looping sounds after they finish
        if (_autoReturn && !Source.loop && !Source.isPlaying && gameObject.activeSelf && Pool != null)
        {
            Pool.ReturnToPool(this);
        }
    }

    public void ReturnNow()
    {
        if (Source.isPlaying) Source.Stop();
        if (Pool != null) Pool.ReturnToPool(this);
    }
    public override void AttachPool(IPool attachedPool)
    {
        base.AttachPool(attachedPool);
    }
}

using UnityEngine;

[CreateAssetMenu(menuName = "Audio/SFX Event")]
public class SfxEvent : ScriptableObject
{
    public int id;              // key you call by
    //public int priorityID = 0;
    public AudioClip[] clips;      // optional random selection
    public bool loop;          // << use this for music/ambience too
    public SoundCategory soundCategory;
    public AudioClip PickClip()
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }
}

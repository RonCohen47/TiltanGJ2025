using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SFX
{
    public SoundCategory category;
    public AudioClip audioClip;
}
public enum SoundCategory
{
    Music,
    SFX,
    Ambience
}
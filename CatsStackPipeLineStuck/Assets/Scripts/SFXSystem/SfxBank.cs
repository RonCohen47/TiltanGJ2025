using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/SFX Bank")]
public class SfxBank : ScriptableObject
{
    public List<SfxEvent> events = new();
}

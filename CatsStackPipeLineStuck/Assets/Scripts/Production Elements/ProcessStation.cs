using UnityEngine;

public class ProcessStation : Station
{
    [SerializeField] private StationType _stationType;
    [SerializeField] private float _interactionDur = 0;

    public enum StationType
    {
        AI,
        Concept,
        Asset,
        Polish
    }   
}

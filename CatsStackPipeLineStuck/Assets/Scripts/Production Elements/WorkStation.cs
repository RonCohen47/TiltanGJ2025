using UnityEngine;

public class WorkStation : Station
{
    [SerializeField] private StationType _stationType;
    [SerializeField] private ProductionStage _productionStage;
    [SerializeField] private float _interactionDur = 0;
    


    public enum StationType
    {
        Dev,
        Art,
        Sound
    }
    public enum ProductionStage
    {
        Brief,
        basic,
        Polish
    }
}

using UnityEngine;

public class WorkStation : MonoBehaviour
{
    [SerializeField] private StationType _stationType;
    [SerializeField] private ProductionStage _productionStage;
    [SerializeField] private float _interactionDur = 0;
    

    public void OutputAssignment(PlayerCarry player)
    {

    }

    public void InputAssignment()
    {

    }
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

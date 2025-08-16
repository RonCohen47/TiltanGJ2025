using static ProcessStation;
using UnityEngine;

public class BriefStation : Station
{
    [SerializeField] private AssignmentType _stationType;

    public void SpawnBrief()
    {
        Instantiate(currentAssignment.Sprite, transform.position, Quaternion.identity);
    }
}
using static ProcessStation;
using UnityEngine;

public class BriefStation : Station
{
    [SerializeField] private AssignmentType _stationType;
    [SerializeField] private GameObject _trowablePrefab;
    private void Start()
    {
        SpawnBrief();
    }
    public void SpawnBrief()
    {
        GameObject brief = Instantiate(_trowablePrefab, transform.position, Quaternion.identity);
        brief.GetComponent<SpriteRenderer>().sprite = currentAssignment.Sprite;
        brief.GetComponent<ThrowableAssignment>().Data = currentAssignment;
    }
}
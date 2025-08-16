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
        RenderBriefFirstTime(brief);
        ThrowableAssignment = brief.GetComponent<ThrowableAssignment>();
        ThrowableAssignment.Station = this; // Set the station reference in the throwable assignment
        IsOccupied = true;
    }
    private void RenderBriefFirstTime(GameObject brief)
    {
        SpriteRenderer briefIconRenderer = brief.transform.GetChild(0).GetComponent<SpriteRenderer>();//icon and color
        switch (_stationType)
        {
            case AssignmentType.Dev: // Default
                briefIconRenderer.color = new Color(0.5f, 0f, 0.5f, 1f);//purple
                briefIconRenderer.sprite = currentAssignment.Sprite;
                Debug.Log(currentAssignment.Sprite);
                break;
            case AssignmentType.Art: // Carrying
                briefIconRenderer.color = Color.green;
                briefIconRenderer.sprite = currentAssignment.Sprite;
                Debug.Log(currentAssignment.Sprite);
                break;
            case AssignmentType.Sound: // Processing
                briefIconRenderer.color = Color.yellow;
                briefIconRenderer.sprite = currentAssignment.Sprite;
                Debug.Log(currentAssignment.Sprite);
                break;
        }
    }
}
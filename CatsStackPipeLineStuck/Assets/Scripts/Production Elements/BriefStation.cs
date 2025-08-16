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
        SpriteRenderer briefBorderRenderer = brief.transform.GetChild(1).GetComponent<SpriteRenderer>();//border
        switch (_stationType)
        {
            case AssignmentType.Dev: // Default
                briefIconRenderer.color = new Color(0.5f, 0f, 0.5f, 1f);//purple
                briefBorderRenderer.sprite = currentAssignment.Sprite;
                break;
            case AssignmentType.Art: // Carrying
                briefBorderRenderer.color = Color.green;
                briefBorderRenderer.sprite = currentAssignment.Sprite;
                break;
            case AssignmentType.Sound: // Processing
                briefIconRenderer.color = Color.yellow;
                briefBorderRenderer.sprite = currentAssignment.Sprite;
                break;
        }
    }
}
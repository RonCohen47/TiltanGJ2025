using UnityEngine;
using UnityEngine.UI;

public class ProcessSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;

    /// <summary>
    /// Sets the slider's value based on the current progress and total time.
    /// </summary>
    /// <param name="currentTime">The current progress time.</param>
    /// <param name="totalTime">The total time to complete the task.</param>
    public void SetProgress(float currentTime, float totalTime)
    {
        if (slider == null || totalTime <= 0f)
            return;

        slider.minValue = 0f;
        slider.maxValue = totalTime;
        slider.value = Mathf.Clamp(currentTime, 0f, totalTime);
    }
    /// <summary>
    /// Shows the slider by enabling its GameObject.
    /// </summary>
    public void Show()
    {
        if (slider != null)
            slider.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the slider by disabling its GameObject.
    /// </summary>
    public void Hide()
    {
        if (slider != null)
            slider.gameObject.SetActive(false);
    }

}

using Mono.Cecil.Cil;
using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Feature : MonoBehaviour
{
    [SerializeField] private FeatureSO _featureSORef;
    [SerializeField] public List<Image> _FeatureUI;
    [SerializeField] public List<Image> _backgroundImages;
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Color _startColor = Color.green;
    [SerializeField] private Color _midColor = Color.yellow;
    [SerializeField] private Color _endColor = Color.red;
    [SerializeField] private Color _artColor;
    [SerializeField] private Color _devColor;
    [SerializeField] private Color _soundColor;

    private float _deadlineTime;
    private float _timeLeft;

    void Start()
    {

        int index = 0;
        foreach (var image in _FeatureUI)
        {
            if (_featureSORef != null)
            {
                if (index >= 0 && index < _featureSORef.requiredAssignments.Count)
                {
                    image.sprite = _featureSORef.requiredAssignments[index].Sprite;
                    switch (_featureSORef.requiredAssignments[index].assignmentType)
                    {
                        case AssignmentType.Dev: image.color = _devColor; break;
                        case AssignmentType.Art: image.color = _artColor; break;
                        case AssignmentType.Sound: image.color = _soundColor; break;
                    }
                }
                else
                {
                    Debug.LogWarning($"Index {index} is out of range for requiredAssignments.");
                }
            }
            else
            {
                Debug.LogWarning("FeatureSO reference is not set.");
            }
            index++;
        }
        index = 0;

        foreach(var image in _backgroundImages)
        {
            if (_featureSORef != null)
            {
                if (index >= 0 && index < _featureSORef.requiredAssignments.Count)
                {
                    if (_featureSORef.requiredAssignments[index].IsPolished)
                    {
                        switch (_featureSORef.requiredAssignments[index].polishType)
                        {
                            case AssignmentType.Dev: image.color = _devColor; break;
                            case AssignmentType.Art: image.color = _artColor; break;
                            case AssignmentType.Sound: image.color = _soundColor; break;
                        }
                    }
                    else image.color = Color.white; break;
                }
                else
                {
                    Debug.LogWarning($"Index {index} is out of range for requiredAssignments.");
                }
            }
            else
            {
                Debug.LogWarning("FeatureSO reference is not set.");
            }
            index++;
        }

        if (_featureSORef != null)
        {
            _deadlineTime = _featureSORef.dueTimeSeconds;
            _timeLeft = _deadlineTime;
            _slider.maxValue = 1.0f;
            _slider.value = 1.0f;
            _fillImage.color = _startColor;
        }
    }

    void Update()
    {
        if (_featureSORef == null) return;

        // Simulate time passing for demonstration; replace with your own timer logic
        _timeLeft -= Time.deltaTime;
        _timeLeft = Mathf.Clamp(_timeLeft, 0, _deadlineTime);

        float normalized = _deadlineTime > 0 ? _timeLeft / _deadlineTime : 0f;
        _slider.value = normalized;

        // Color interpolation: green -> yellow -> red
        if (normalized > 0.5f)
        {
            // Green to Yellow
            float t = (normalized - 0.5f) * 2f;
            _fillImage.color = Color.Lerp(_midColor, _startColor, t);
        }
        else
        {
            // Yellow to Red
            float t = normalized * 2f;
            _fillImage.color = Color.Lerp(_endColor, _midColor, t);
        }
    }



}

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Feature : MonoBehaviour
{
    [SerializeField] private FeatureSO _featureSORef;
    [SerializeField] public List<Image> _FeatureUI;

    void Start()
    {
        int index = 0;
        foreach (var image in _FeatureUI)
        {
            if (_featureSORef != null)
            {
                // Fix: Use the index to retrieve the corresponding AssignmentSO and assign its sprite
                if (index >= 0 && index < _featureSORef.requiredAssignments.Count)
                {
                    image.sprite = _featureSORef.requiredAssignments[index].Sprite;
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
    }

    // Update is called once per frame
    void Update()
    {

    }
}

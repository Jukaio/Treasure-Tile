using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Health health;
    private Slider slider;

    public void SetContext(Health that)
    {
        health = that;
    }
    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        slider.value =  (float)health.Current / (float)health.Maximum;
    }
}

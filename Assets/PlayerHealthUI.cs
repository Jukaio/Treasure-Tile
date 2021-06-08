using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerController player = null;
    [SerializeField] private Text value = null;
    void Start()
    {
        value.text = player.CurrentHealth.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

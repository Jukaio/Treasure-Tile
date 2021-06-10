using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Health player = null;
    [SerializeField] private Text value = null;
    void Start()
    {
        value.text = player.Current.ToString();
    }

    void FixedUpdate()
    {
        value.text = player.Current.ToString();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] private RectTransform main = null;
    [SerializeField] private Health enemy = null;
    [SerializeField] private GameObject tracker = null;
    private RectTransform transformation;

    void Awake()
    {
        transformation = GetComponent<RectTransform>();
    }
    public void Register(Health element)
    {
        enemy = element;
    }
    void Update()
    {
        var auto = RectTransformUtility.WorldToScreenPoint(Camera.main, enemy.transform.position);
        var active = RectTransformUtility.ScreenPointToLocalPointInRectangle(main, auto, Camera.main, out var position);
        tracker.SetActive(active);
        transformation.anchoredPosition = new Vector3(position.x, position.y);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] private RectTransform main = null;
    [SerializeField] private Health enemy = null;
    [SerializeField] private HealthBarUI tracker = null;
    private RectTransform transformation;


    void Awake()
    {
        transformation = GetComponent<RectTransform>();
    }
    public void SetCanvas(GameObject canvas)
    {
        main = canvas.GetComponent<RectTransform>();
        //transform.parent = main.gameObject.transform;
        var scale = transform.localScale;
        var position = transform.position;
        var rotation = transform.rotation;
        transform.SetParent(main.gameObject.transform);
        transform.localPosition = position;
        transform.localRotation = rotation;
        transform.localScale = scale;
        //transformation.position = Vector3.zero;
    }
    public void Register(Health element)
    {
        enemy = element;
        tracker.SetContext(enemy);
    }
    void Update()
    {
        var auto = RectTransformUtility.WorldToScreenPoint(Camera.main, enemy.transform.position);
        var active = RectTransformUtility.ScreenPointToLocalPointInRectangle(main, auto, Camera.main, out var position);
        tracker.gameObject.SetActive(active);
        transformation.anchoredPosition = new Vector3(position.x, position.y);
    }
}

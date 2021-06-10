using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject enemy_ui;
    private List<GameObject> enemy_ui_pool = new List<GameObject>();

    void Start()
    {
        for(int i = 0; i < 10; i++) {
            enemy_ui_pool.Add(Instantiate<GameObject>(enemy_ui));
            enemy_ui_pool[i].GetComponent<EnemyHealthUI>().Register(null);
            enemy_ui_pool[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

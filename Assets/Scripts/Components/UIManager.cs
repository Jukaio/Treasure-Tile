using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour // all it manages right now is enemy health bars
{
    [SerializeField] private GameObject original;
    [SerializeField] private int size;
    private Stack<GameObject> pool = new Stack<GameObject>();
    //private List<GameObject> used = new List<GameObject>();
    private Dictionary<EnemyController, GameObject> user = new Dictionary<EnemyController, GameObject>();
    public GameObject Request(EnemyController that)
    {
        if(pool.Count > 0) {
            var to_return = pool.Pop();
            user.Add(that, to_return);
            //used.Add(to_return);
            return to_return;
        }
        return null; // Empty
    }
    public void Return(EnemyController that)
    {
        if(user.TryGetValue(that, out var value)) {
            pool.Push(value);
            user.Remove(that);
            value.SetActive(false);
        }
    }

    void Awake()
    {
        for(int i = 0; i < size; i++) {
            var that = Instantiate<GameObject>(original);
            that.GetComponent<EnemyHealthUI>().Register(null);
            that.SetActive(false);
            pool.Push(that);
        }
    }

}

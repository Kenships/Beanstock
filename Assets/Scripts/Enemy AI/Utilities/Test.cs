using UnityEngine;

public class Test : MonoBehaviour
{
    private GridSystem grid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grid = new GridSystem(4,2,2f, new UnityEngine.Vector3(20, 0));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            grid.SetValue(Utils.GetMouseWorldPosition(), 56);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(grid.GetValue(Utils.GetMouseWorldPosition()));
        }
    }



}
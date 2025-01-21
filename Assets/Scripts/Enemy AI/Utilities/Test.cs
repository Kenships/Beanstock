using UnityEngine;

public class Test : MonoBehaviour
{
    private GridSystemGenerics<HeatMapGridObject> grid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grid = new GridSystemGenerics<HeatMapGridObject>(4,2,2f, new UnityEngine.Vector3(20, 0), true, () => new HeatMapGridObject());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //grid.SetValue(Utils.GetMouseWorldPosition(), true);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(grid.GetValue(Utils.GetMouseWorldPosition()));
        }
    }

    public class HeatMapGridObject
    {
        public int value;

        public void AddValue(int addValue)
        {
            value += addValue;
        }
    }



}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InputManager : MonoBehaviour
{
    public Transform cam;
    public float panSpeed = 20f;
    public Vector2 plusPanLimit;
    public Vector2 minusPanLimit;

    private void Start()
    {
        plusPanLimit = new Vector2(GridManager.Instance.width, GridManager.Instance.height);
    }

    void Update()
    {
        Vector3 pos = cam.transform.position;

        if(Input.GetKey("w"))
        {
            pos.y += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            pos.y -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, minusPanLimit.x, plusPanLimit.x);
        pos.y = Mathf.Clamp(pos.y, minusPanLimit.y, plusPanLimit.y);

        cam.transform.position = pos;

        if (Input.GetKey(KeyCode.Q))
        {
            MenuManager.instance.showSelectedUnit(null);
            MenuManager.instance.showFullTileInfo(null);
        }
    }
}

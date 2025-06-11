using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private Vector2 plusPanLimit;
    [SerializeField] private Vector2 minusPanLimit;

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

        if (Input.GetKey("q"))
        {
            MenuManager.Instance.showSelectedUnit(null);
            MenuManager.Instance.showFullTileInfo(null);
        }

        if(Input.GetKey("k"))
        {
            SceneManager.LoadScene("Level 3");
        }
    }
}

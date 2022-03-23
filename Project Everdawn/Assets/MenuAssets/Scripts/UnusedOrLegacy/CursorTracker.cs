using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorTracker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Hides cursor
        //Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Get input position and change object position to input position
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = pos;
    }
}

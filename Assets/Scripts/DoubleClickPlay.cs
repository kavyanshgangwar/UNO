using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleClickPlay : MonoBehaviour
{
    private void OnMouseDrag()
    {
        if (UIManager.Instance.choosingColor) return;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        transform.Translate(mousePosition);
    }

    private void OnMouseUp()
    {
        if(transform.position.y > 0)
        {
            Player.Instance.PlayCard(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public LayerMask targetMask; //적이 hair에 놓이면 색을 바꾸기 위함
    public SpriteRenderer dot;
    public Color dotHightlightColor;
    
    Color originalDotColor;

    private void Start()
    {
        Cursor.visible = false;
        originalDotColor = dot.color;
    }
    void Update()
    {
        transform.Rotate(Vector3.forward * 40 * Time.deltaTime);
    }

    public void DetectTargets(Ray ray)
    {
        if(Physics.Raycast(ray,100f,targetMask))
        {
            dot.color = dotHightlightColor;
        }
        else
        {
            dot.color = originalDotColor;
        }
    }
}

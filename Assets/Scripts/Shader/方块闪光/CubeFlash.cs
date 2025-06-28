using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFlash : MonoBehaviour
{
    public Material mat;
    public float speed = 2f;
    public float maxStrength = 0.4f;

    void Update()
    {
        float s = Mathf.Sin(Time.time * speed);
        float strength = Mathf.Lerp(0f, maxStrength, (s + 1f) * 0.5f);
        mat.SetFloat("_ExpandStrength", strength);

       //Vector2 screenUV = Camera.main.WorldToViewportPoint(transform.position);
       //mat.SetVector("_ExpandCenter", new Vector4(screenUV.x, screenUV.y, 0, 0));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarBackgroundControl : MonoBehaviour {
    private SpriteRenderer spriteRenderer;
    private Material material;
    private Shader starShader;

    public float paralax = 0.0f;

    // Start is called before the first frame update
    void Start() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
        starShader = spriteRenderer.material.shader;
    }

    // Update is called once per frame
    void Update() {
        material.SetVector("_Offset", transform.position * paralax);
        float height = Camera.main.orthographicSize * 2f;
        float width = height * Camera.main.aspect;
        transform.localScale = new Vector3(width, height);
    }
}

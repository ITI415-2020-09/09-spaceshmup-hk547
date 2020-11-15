using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ProjectileEnemy : MonoBehaviour
{
    private BoundsCheck bndCheck;

    private Renderer render;

    [Header("Set Dynamically")]
    public Rigidbody rigid;


    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        render = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (bndCheck.offUp)
        { 
            Destroy(gameObject);
        }
    }
}
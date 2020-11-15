using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    public float speed = 10f;
    private Vector3 p0, p1; // The two points to interpolate
    private float timeStart; // Birth time for this
    private float duration = 4; // Duration of movement

    [Header("Set in Inspector")]
    // This is an unusual but handy use of Vector2s. x holds a min value
    // and y a max value for a Random.Range() that will be called later
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 8f; // Seconds the PowerUp exists
    public float fadeTime = 6f; // Seconds it will then fade

    [Header("Set Dynamically")]
    public WeaponType type; // The type of the PowerUp 
    public GameObject cube; // Reference to the Cube child
    public TextMesh letter; // Reference to the TextMesh
    public Vector3 rotPerSecond; // Euler rotation speed
    public float birthTime;

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Renderer cubeRend;

    private void Awake()
    {
        // Find the Cube reference
        cube = transform.Find("Cube").gameObject;
        // Find the TextMesh and other components
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeRend = cube.GetComponent<Renderer>();

        // Set a random elocity
        Vector3 vel = Random.onUnitSphere; // Get Random XYZ velocity
        // Random.onUnitSphere gives you a vector point that is somewhere on
        // the surface of the sphere with a radius of 1m around the origin
        vel.z = 0; // Flatten the vel to the XY plane
        vel.Normalize(); // Normalizing a Vector3 makes it length 1m
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        // Set the rotation of this GameObject to R: [0,0,0]
        transform.rotation = Quaternion.identity;
        //Quaternion.identity is equal to no rotation

        // Set up the rotPerSecond for the Cube child using rotMinMax x & y
        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y));

        birthTime = Time.time;
    }

    private void Start()
    {
        p0 = p1 = pos;

        InitMovement();
    }

    void InitMovement()
    {
        p0 = p1; // Set p0 to the old p1
        // Assign a new on-screen location to p1
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        // Reset the time
        timeStart = Time.time;
    }
    public Vector3 pos
    {
        get
        {
            return (this.transform.position);
        }
        set
        {
            this.transform.position = value;
        }
    }

    private void Update()
    {
        Move();

        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        // Fade out the PowerUp over time
        // Given the default values, a PowerUp will exist for 10 seconds
        // and then fade out over 4 seconds.
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        // For lifeTime seconds, u will be <= 0. Then it will transition to
        // 1 over the course of fadeTime seconds.

        // If u >= 1, destroy this PowerUp
        if (u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }

        // Use u to determine the alpha value of the Cube & Letter
        if (u > 0)
        {
            Color c = cubeRend.material.color;
            c.a = 1f - u;
            cubeRend.material.color = c;
            // Fade the letter too, just not as much
            c = letter.color;
            c.a = 1f - (u * 0.5f);
            letter.color = c;
        }

        if (!bndCheck.isOnScreen)
        {
            // If the PowerUp has drifted entirely off screen, destroy it
            Destroy(gameObject);
        }
    }

    public virtual void Move()
    {
        float u = (Time.time - timeStart) / duration;

        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2); // Apply Ease Out easing to u
        pos = ((1 - u) * p0) + (u * p1);// Simple linear interpolation
    }

    public void SetType(WeaponType wt)
    {
        // Grab the WeaponDefinition from Main
        WeaponDefinition def = Main.GetWeaponDefinition(wt);
        // Set the color of the Cube child
        cubeRend.material.color = def.color;
        //letter.color = def.color; // We could colorize the letter too
        letter.text = def.letter; // Set the letter that is shown
        type = wt; // Finally actually set the type
    }

    public void AbsorbedBy(GameObject target)
    {
        // This function is called by the Hero class when a PowerUp is collected
        // We could tween into the target and shrink in size.
        // But for now just destroy this.gameObject
        Destroy(this.gameObject);
    }
}

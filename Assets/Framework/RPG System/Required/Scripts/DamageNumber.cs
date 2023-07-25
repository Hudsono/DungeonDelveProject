using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Animations;

/// <summary>
/// Controls in-world UI representing damage numbers.
/// </summary>
public class DamageNumber : MonoBehaviour
{
    /// <summary>
    /// The DamageType used to create this number.
    /// </summary>
    public DamageType damageType;

    /// <summary>
    /// Damage incurred.
    /// </summary>
    public int damage;
    
    /// <summary>
    /// Rigidbody used to control the UI's physics effect.
    /// </summary>
    public Rigidbody rb;

    /// <summary>
    /// Text for the damage number.
    /// </summary>
    public TextMeshProUGUI textUI;

    /// <summary>
    /// Component to turn this into a billboard.
    /// </summary>
    public AimConstraint aimConstraint;

    // Used to keep track of the object's lifetime in seconds; to delete it.
    private float timeAlive = 0;

    // Maximum time which all DamageNumbers are alive for, in seconds.
    private static float maxTimeAlive = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        // Add upwards force upon spawn.
        rb.AddForce(new Vector3(0, 5, 0), ForceMode.Impulse);

        // Set the damage to the supplied number.
        textUI.text = damage.ToString();

        // Set the aim constraint.
        ConstraintSource source = new ConstraintSource();
        source.weight = 1;
        source.sourceTransform = GameManager.instance.cameraUIAim.transform;
        aimConstraint.SetSource(0, source);
    }

    // Update is called once per frame
    void Update()
    {
        // Evaluate time alive, destroy after
        timeAlive += Time.deltaTime;
        if (timeAlive > maxTimeAlive)
            Destroy(gameObject);

        // Enforce artificially-high gravity for effect.
        if (rb != null)
        {
            rb.AddForce(new Vector3(0, -2000 * Time.deltaTime, 0));
        }
    }

    //public void Initialise(DamageType damageType, int damage)
    //{
    //    this.damageType = damageType;
    //    this.damage = damage;
    //    gameObject.transform.position = other.gameObject.transform.position;
    //
    //    TMPro.VertexGradient grad;
    //    grad.topLeft = meter.meterColour;
    //    grad.topRight = meter.meterColour;
    //    grad.bottomLeft = new Color(meter.meterColour.r * 0.4f, meter.meterColour.g * 0.4f, meter.meterColour.b * 0.4f, meter.meterColour.a);
    //    grad.bottomRight = new Color(meter.meterColour.r * 0.4f, meter.meterColour.g * 0.4f, meter.meterColour.b * 0.4f, meter.meterColour.a);
    //    textUI.colorGradient = grad;
    //}
}

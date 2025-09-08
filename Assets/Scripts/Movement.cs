using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Thrust Settings")]
    [SerializeField] float thrustStrength = 100f;
    [SerializeField] float rotationStrength = 100f;
    [SerializeField] float boostMultiplier = 2f;          // Speed multiplier when boosting
    [SerializeField] float boostFuelMultiplier = 3f;      // Fuel drain multiplier when boosting

    [Header("Effects")]
    [SerializeField] AudioClip mainEngineSFX;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem rightThrustParticles;
    [SerializeField] ParticleSystem leftThrustParticles;

    [Header("References")]
    [SerializeField] FuelSystem fuelSystem;

    Rigidbody rb;
    AudioSource audioSource;

    InputAction thrust;
    InputAction rotation;
    InputAction boost;

    private void Awake()
    {
        thrust = new InputAction(binding: "<Keyboard>/space");

        rotation = new InputAction(type: InputActionType.Value);
        rotation.AddBinding("<Keyboard>/a");
        rotation.AddBinding("<Keyboard>/d");

        boost = new InputAction(binding: "<Keyboard>/leftShift");
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        thrust.Enable();
        rotation.Enable();
        boost.Enable();
    }

    private void FixedUpdate()
    {
        ProcessThrust();
        ProcessRotation();
    }

    private void ProcessThrust()
    {
        if (fuelSystem != null && !fuelSystem.CanUseSpace)
        {
            StopThrusting();
            return;
        }

        if (thrust.IsPressed())
            StartThrusting();
        else
            StopThrusting();
    }

    private void StartThrusting()
    {
        bool isBoosting = boost.IsPressed();
        float currentThrust = thrustStrength * (isBoosting ? boostMultiplier : 1f);

        rb.AddRelativeForce(Vector3.up * currentThrust * Time.fixedDeltaTime);

        if (fuelSystem != null)
            fuelSystem.ConsumeFuel(isBoosting ? boostFuelMultiplier : 1f);

        if (!audioSource.isPlaying) audioSource.PlayOneShot(mainEngineSFX);
        if (!mainEngineParticles.isPlaying) mainEngineParticles.Play();
    }

    private void StopThrusting()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void ProcessRotation()
    {
        float value = 0f;

        if (Keyboard.current.aKey.isPressed) value = -1f;
        if (Keyboard.current.dKey.isPressed) value = 1f;

        if (value < 0)
            RotateRight();
        else if (value > 0)
            RotateLeft();
        else
            StopRotating();
    }

    private void RotateRight()
    {
        ApplyRotation(rotationStrength);
        if (!rightThrustParticles.isPlaying)
        {
            leftThrustParticles.Stop();
            rightThrustParticles.Play();
        }
    }

    private void RotateLeft()
    {
        ApplyRotation(-rotationStrength);
        if (!leftThrustParticles.isPlaying)
        {
            rightThrustParticles.Stop();
            leftThrustParticles.Play();
        }
    }

    private void StopRotating()
    {
        rightThrustParticles.Stop();
        leftThrustParticles.Stop();
    }

    private void ApplyRotation(float rotationThisFrame)
    {
        rb.freezeRotation = true;
        transform.Rotate(Vector3.forward * rotationThisFrame * Time.fixedDeltaTime);
        rb.freezeRotation = false;
    }
}
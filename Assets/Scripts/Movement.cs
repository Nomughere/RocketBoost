using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{   
    [SerializeField] float thrustStrength = 100f;
    [SerializeField] float rotationStrength = 100f;
    [SerializeField] AudioClip mainEngineSFX;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem rightThrustParticles;
    [SerializeField] ParticleSystem leftThrustParticles;

    Rigidbody rb;
    AudioSource audioSource;

    InputAction thrust;
    InputAction rotation;

private void Awake()
{
    // SPACE = forward thrust
    thrust = new InputAction(binding: "<Keyboard>/space");

    // A = rotate right, D = rotate left (as you asked before)
    rotation = new InputAction(type: InputActionType.Value);
    rotation.AddBinding("<Keyboard>/a"); // rotate right
    rotation.AddBinding("<Keyboard>/d"); // rotate left
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
    }

    private void FixedUpdate()
    {
        ProcessThrust();
        ProcessRotation();
    }

    private void ProcessThrust()
    {
        if (thrust.IsPressed())
            StartThrusting();
        else
            StopThrusting();
    }

    private void StartThrusting()
    {
        rb.AddRelativeForce(Vector3.up * thrustStrength * Time.fixedDeltaTime);
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

    // Now pressing A will rotate right, D will rotate left
    if (Keyboard.current.aKey.isPressed) value = -1f; // right
    if (Keyboard.current.dKey.isPressed) value = 1f;  // left

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
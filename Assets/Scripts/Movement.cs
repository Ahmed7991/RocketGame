using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // PARAMETERS - for tuning, typically set in the editor
    // CACHE - e.g. references for readability or speed
    // STATE - private instance (member) variables

    [Tooltip("Force applied when thrusting. Increase to make the rocket faster.")]
    [SerializeField] float mainThrust = 10f; // Default value in code. Editor value overrides this!
    [Tooltip("Speed of rotation in degrees per second.")]
    [SerializeField] float rotationThrust = 100f;
    [SerializeField] AudioClip mainEngine;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem leftThrusterParticles;
    [SerializeField] ParticleSystem rightThrusterParticles;

    [SerializeField] KeyCode thrustKey = KeyCode.Space;
    [SerializeField] KeyCode rotateLeftKey = KeyCode.A;
    [SerializeField] KeyCode rotateRightKey = KeyCode.D;

    Rigidbody rb;
    AudioSource audioSource;
    bool isThrusting = false;
    float rotationInput = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource)
        {
            audioSource.clip = mainEngine;
            audioSource.loop = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ProcessThrustInput();
        ProcessRotationInput();
    }

    void FixedUpdate()
    {
        ApplyThrust();
        ApplyRotationPhysics();
    }

    void ProcessThrustInput()
    {
        if (Input.GetKey(thrustKey))
        {
            isThrusting = true;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            if (!mainEngineParticles.isPlaying)
            {
                mainEngineParticles.Play();
            }
        }
        else
        {
            isThrusting = false;
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    void ProcessRotationInput()
    {
        if (Input.GetKey(rotateLeftKey))
        {
            rotationInput = 1f;
            if (!rightThrusterParticles.isPlaying)
            {
                rightThrusterParticles.Play();
            }
        }
        else if (Input.GetKey(rotateRightKey))
        {
            rotationInput = -1f;
            if (!leftThrusterParticles.isPlaying)
            {
                leftThrusterParticles.Play();
            }
        }
        else
        {
            rotationInput = 0f;
            rightThrusterParticles.Stop();
            leftThrusterParticles.Stop();
        }
    }

    void ApplyThrust()
    {
        if (isThrusting)
        {
            rb.AddRelativeForce(Vector3.up * mainThrust);
        }
    }

    void ApplyRotationPhysics()
    {
        if (Mathf.Abs(rotationInput) > Mathf.Epsilon)
        {
            ApplyRotation(rotationInput * rotationThrust);
        }
    }

    void ApplyRotation(float rotationThisFrame)
    {
        rb.freezeRotation = true;  // freezing rotation so we can manually rotate
        transform.Rotate(Vector3.forward * rotationThisFrame * Time.fixedDeltaTime);
        rb.freezeRotation = false;  // unfreezing rotation so the physics system can take over
    }
}

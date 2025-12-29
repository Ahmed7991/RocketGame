using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip crash;

    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem crashParticles;

    [SerializeField] KeyCode nextLevelKey = KeyCode.L;
    [SerializeField] KeyCode collisionToggleKey = KeyCode.C;

    AudioSource audioSource;

    bool isTransitioning = false;
    bool collisionDisabled = false;

    const string FriendlyTag = "Friendly";
    const string FinishTag = "Finish";

    void Start() 
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    void Update() 
    {
        RespondToDebugKeys();    
    }

    void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(nextLevelKey))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(collisionToggleKey))
        {
            collisionDisabled = !collisionDisabled;  // toggle collision
        } 
    }

    void OnCollisionEnter(Collision other) 
    {
        if (isTransitioning || collisionDisabled) { return; }
        
        switch (other.gameObject.tag)
        {
            case FriendlyTag:
                Debug.Log("This thing is friendly");
                break;
            case FinishTag:
                StartSuccessSequence();
                break;
            default:
                StartCrashSequence();
                break;
        }
    }

    void StartSuccessSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        GetComponent<Movement>().enabled = false;
        StartCoroutine(LoadLevelRoutine(LoadNextLevel));
    }

    void StartCrashSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(crash);
        crashParticles.Play();
        GetComponent<Movement>().enabled = false;
        StartCoroutine(LoadLevelRoutine(ReloadLevel));
    }

    IEnumerator LoadLevelRoutine(Action levelLoadAction)
    {
        yield return new WaitForSeconds(levelLoadDelay);
        levelLoadAction();
    }

    void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    // fields set in the Unity Inspector pane
    [Header("Inscribed")]
    public GameObject ProjectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;
    public AudioClip releaseSound; // Ensure this is assigned in the Inspector
    public Projectile projectilePrefab;

    // fields set dynamically
    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    private AudioSource audioSource;

    [SerializeField] private LineRenderer rubber;
    [SerializeField] private Transform firstPoint;
    [SerializeField] private Transform secondPoint;

    public static int shotsFired;
    public static int maxShots = 20;

    void Awake()
    {
        // Find and initialize the launchPoint
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
        
        // Initialize the audio source
        audioSource = GetComponent<AudioSource>();
        
        // Ensure the AudioSource doesn't play automatically at start
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop(); // Prevent playing any sound on startup
        }
        
        // Debug message to check if audioSource is found
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource component not found on Slingshot.");
        }
    }

    void OnMouseEnter()
    {
        launchPoint.SetActive(true);
    }

    void OnMouseExit()
    {
        launchPoint.SetActive(false);
    }

    void Start()
    {
        rubber.SetPosition(0, firstPoint.position);
        rubber.SetPosition(2, secondPoint.position);
    }

    void OnMouseDown()
    {
        aimingMode = true;
        projectile = Instantiate(ProjectilePrefab) as GameObject;
        projectile.transform.position = launchPos;
        projectile.GetComponent<Rigidbody>().isKinematic = true;

        
    }

    void Update()
    {
        if (!aimingMode) return;

        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        Vector3 mouseDelta = mousePos3D - launchPos;

        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        rubber.SetPosition(1, projPos);

        if (Input.GetMouseButtonUp(0))
        {
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.linearVelocity = -mouseDelta * velocityMult;

            // Debug message before playing sound
            Debug.Log("Projectile released. Playing sound...");

            // Play the sound when the projectile is released
            if (releaseSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(releaseSound); // Play the sound effect
            }
            else
            {
                if (releaseSound == null)
                {
                    Debug.LogWarning("Release sound not assigned.");
                }
                if (audioSource == null)
                {
                    Debug.LogWarning("AudioSource component is missing.");
                }
            }

            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);
            FollowCam.POI = projectile;
            Instantiate(projLinePrefab, projectile.transform);
            projectile = null;
            shotsFired++;
            MissionDemolition.SHOT_FIRED();

            if ((shotsFired == maxShots))
            {
                shotsFired = 0;
                GameOver();
            }
        }
    }

    private void GameOver()
    {
        // Load game over scene
        SceneManager.LoadScene("GameOver");
    }
}
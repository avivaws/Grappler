using UnityEngine;
using System;
using System.Diagnostics;
using System.Collections;

public class GrapplingGun : MonoBehaviour
{
    [Header("Scripts Ref:")]
    public GrapplingRope grappleRope;

    [Header("Layers Settings:")]
    [SerializeField] private bool grappleToAll = false;
    [SerializeField] private int grappableLayerNumber = 9;

    [Header("Main Camera:")]
    public Camera m_camera;

    [Header("Transform Ref:")]
    public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;
    public Transform predictionPoint;

    [Header("RendererRef")]
    public SpriteRenderer shootingRadiusRenderer;
    public SpriteRenderer predictionPointRenderer;


    [Header("Physics Ref:")]
    public SpringJoint2D m_springJoint2D;
    public Rigidbody2D m_rigidbody;

    [Header("Rotation:")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 60)] [SerializeField] private float rotationSpeed = 4;

    [Header("Distance:")]
    [SerializeField] private bool hasMaxDistance = false;
    [SerializeField] private float maxDistnace = 20;
    private bool playerLaunch = false;
    private bool outOfDistance = false;

    [Header("Audio:")]
    public AudioManager audioManager;


    private enum LaunchType
    {
        Transform_Launch,
        Physics_Launch
    }

    [Header("Launching:")]
    [SerializeField] private bool launchToPoint = true;
    [SerializeField] private LaunchType launchType = LaunchType.Physics_Launch;
    [SerializeField] private float launchSpeed = 1;

    [Header("No Launch To Point")]
    [SerializeField] private bool autoConfigureDistance = false;
    [SerializeField] private float targetDistance = 3;
    [SerializeField] private float targetFrequncy = 1;

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;

    [Header("particle manager")]
    public ParticleSystem particleHookSys;
    public ParticleSystem paticleFireSys;

    private Coroutine fakeFireCoroutine;

    private void Start()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;
        shootingRadiusRenderer.enabled= ButtonHandler.radiusShow;
        predictionPointRenderer.enabled= ButtonHandler.predictionPointShow;
        paticleFireSys.Stop();

    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Z)) {
            if (grappleRope.isGrappling)
            {
                Launch();
            }
            else
            {
                playerLaunch = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SetGrapplePoint();
            paticleFireSys.Play();
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            if (grappleRope.enabled)
            {
                RotateGun(grapplePoint, false);
                //if (stopwatch.ElapsedMilliseconds >= 300&&outOfDistance)
                //{
                //    audioManager.PlaySFX(audioManager.ropeFail);
                //    grappleRope.enabled = false;
                //    m_springJoint2D.enabled = false;
                //    m_rigidbody.gravityScale = 1;
                //    outOfDistance = false;
                //    stopwatch.Stop();
                //    stopwatch.Reset();
                //}
            }
            else
            {
                Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
                RotateGun(mousePos, true);
            }

            if (launchToPoint && grappleRope.isGrappling)
            {
                if (launchType == LaunchType.Transform_Launch)
                {
                    Vector2 firePointDistnace = firePoint.position - gunHolder.localPosition;
                    Vector2 targetPos = grapplePoint - firePointDistnace;
                    gunHolder.position = Vector2.Lerp(gunHolder.position, targetPos, Time.deltaTime * launchSpeed);
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            grappleRope.enabled = false;
            m_springJoint2D.enabled = false;
            m_rigidbody.gravityScale = 1;
            outOfDistance = false;
            paticleFireSys.Stop();
            //stopwatch.Stop();
        }
        else
        {
            updatePredictionPoint();
            Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            RotateGun(mousePos, true);
        }
    }

    void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
    {
        Vector3 distanceVector = lookPoint - gunPivot.position;

        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        if (rotateOverTime && allowRotationOverTime)
        {
            gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
        }
        else
        {
            gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void SetGrapplePoint()
    {
        audioManager.PlaySFX(audioManager.ropeSent);
        Vector2 distanceVector = m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
        if (Physics2D.Raycast(firePoint.position, distanceVector.normalized))
        {
            RaycastHit2D _hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized);
            if (_hit.transform.gameObject.layer == grappableLayerNumber || grappleToAll)
            {
                if (Vector2.Distance(_hit.point, firePoint.position) <= maxDistnace || !hasMaxDistance)
                {
                    grapplePoint = _hit.point;
                    grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
                    predictionPoint.position = _hit.point;
                    grappleRope.enabled = true;
                }
                else if(Vector2.Distance(_hit.point, firePoint.position) > maxDistnace)
                {
                    FakeGrapple();
                }
            }
        }
        else
        {
            FakeGrapple();
        }
        playerLaunch = false;
    }

    public void Grapple()
    {
        audioManager.PlaySFX(audioManager.ropeHook);
        particleHookSys.transform.position= grapplePoint;
        paticleFireSys.Stop();
        particleHookSys.Play();
        m_springJoint2D.autoConfigureDistance = false;
        if (!launchToPoint && !autoConfigureDistance)
        {
            m_springJoint2D.distance = targetDistance;
            m_springJoint2D.frequency = targetFrequncy;
        }
        if (!launchToPoint&&!playerLaunch)
        {
            if (autoConfigureDistance)
            {
                m_springJoint2D.autoConfigureDistance = true;
                m_springJoint2D.frequency = 0;
            }

            m_springJoint2D.connectedAnchor = grapplePoint;
            m_springJoint2D.enabled = true;
        }
        else
        {
            Launch();
        }
    }

    private void updatePredictionPoint()
    {
        //print("im here");
        Vector2 distanceVector = m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
        if (Physics2D.Raycast(firePoint.position, distanceVector.normalized))
        {
            RaycastHit2D _hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized);
            if (_hit.transform.gameObject.layer == grappableLayerNumber || grappleToAll)
            {
                if (Vector2.Distance(_hit.point, firePoint.position) <= maxDistnace || !hasMaxDistance)
                {
                    predictionPoint.position = new Vector3(_hit.point.x, _hit.point.y, -1);
                }
                else
                {
                    predictionPoint.position=new Vector3(predictionPoint.position.x, predictionPoint.position.y, -100);
                }

            }
            else
            {
                predictionPoint.position = new Vector3(predictionPoint.position.x, predictionPoint.position.y, -100);
            }

        }
        else
        {
            predictionPoint.position = new Vector3(predictionPoint.position.x, predictionPoint.position.y, -100);
        }

    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null && hasMaxDistance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistnace);
        }
    }

    private void FakeGrapple()
    {
        grapplePoint = m_camera.ScreenToWorldPoint(Input.mousePosition);
        grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
        grappleRope.enabled = true;
        outOfDistance = true;
        predictionPoint.position = new Vector3(100, 100, -100);
        fakeFireCoroutine = StartCoroutine(CancelRopeAfterDelay(0.35f));
        //stopwatch.Start();
    }

    private IEnumerator CancelRopeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        paticleFireSys.Stop();
        cancelRope();


    }

    private void cancelRope()
    {
        if (outOfDistance)
        {
            audioManager.PlaySFX(audioManager.ropeFail);
            grappleRope.enabled = false;
            m_springJoint2D.enabled = false;
            m_rigidbody.gravityScale = 1;
            outOfDistance = false;
        }
    }

    private void Launch()
    {
        switch (launchType)
        {
            
            case LaunchType.Physics_Launch:
                audioManager.PlaySFX(audioManager.ropeJump);
                m_springJoint2D.connectedAnchor = grapplePoint;

                Vector2 distanceVector = firePoint.position - gunHolder.position;

                m_springJoint2D.distance = distanceVector.magnitude;
                m_springJoint2D.frequency = launchSpeed;
                m_springJoint2D.enabled = true;
                break;
            case LaunchType.Transform_Launch:
                m_rigidbody.gravityScale = 0.4f;
                m_rigidbody.velocity = Vector2.zero;
                break;       
        }
        playerLaunch = false;
    }

}

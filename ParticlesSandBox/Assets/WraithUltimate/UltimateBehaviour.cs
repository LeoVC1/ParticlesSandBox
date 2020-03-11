using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class UltimateBehaviour : MonoBehaviour
{
    [Header("Settings:")]
    [SerializeField]
    private float maxDistance;
    [SerializeField]
    private float deltaThreshold = 1;
    [ReadOnly]
    [SerializeField]
    private float remainingEnergyPercentual;
    private float pathDistance;

    [Header("References:")]

    //Game Events
    [SerializeField]
    private GameEventBool isTeleporting;

    //Paths controllers
    [SerializeField]
    private List<Transform> observedTrasforms = new List<Transform>();
    private Path myTransformPath;
    private List<Path> paths = new List<Path>();

    //Coroutines
    private Coroutine definePathCoroutine;
    private Coroutine runPathCoroutine;

    //Portal 
    [SerializeField]
    private GameObject portalPrefab;
    private GameObject entrancePortal;
    private GameObject exitPortal;
    private bool insidePortal;

    private void Start()
    {
        myTransformPath = new Path(transform);

        foreach (Transform t in observedTrasforms)
        {
            paths.Add(new Path(t));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && definePathCoroutine == null)
        {
            AddPathFrames();

            definePathCoroutine = StartCoroutine(DefinePath());
        }
    }

    private void AddPathFrames()
    {
        myTransformPath.AddFrame();

        foreach (Path path in paths)
        {
            path.AddFrame();
        }
    }

    private void SetPathFrames(int pathIndex)
    {
        myTransformPath.SetFrame(pathIndex);

        foreach (Path path in paths)
        {
            path.SetFrame(pathIndex);
        }

        insidePortal = true;
    }

    IEnumerator DefinePath()
    {
        while(pathDistance < maxDistance)
        {
            float deltaPosition = Vector3.Distance(myTransformPath.GetLastFrame(), transform.position);

            if (deltaPosition > deltaThreshold)
            {
                AddPathFrames();

                pathDistance += deltaPosition;
                remainingEnergyPercentual = 100 - (pathDistance * 100 / maxDistance);

                remainingEnergyPercentual = Mathf.Clamp(remainingEnergyPercentual, 0, 100); //Display remaining energy as percentual
            }

            insidePortal = true;

            yield return null;
        }

        entrancePortal = Instantiate(portalPrefab, myTransformPath.GetFirstFrame(), Quaternion.identity);
        exitPortal = Instantiate(portalPrefab, myTransformPath.GetLastFrame(), Quaternion.identity);

        pathDistance = 0;

        definePathCoroutine = null;
    }

    IEnumerator RunPath(bool reversePath)
    {
        isTeleporting.Raise(true);

        for (int i = reversePath ? myTransformPath.frames.Count - 1 : 0; reversePath ? i >= 0 : i < myTransformPath.frames.Count; i = reversePath ? i - 1 : i + 1)
        {
            SetPathFrames(i);
            insidePortal = true;
            yield return null;
        }

        isTeleporting.Raise(false);

        runPathCoroutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (insidePortal)
        {
            return;
        }

        if (other.gameObject == entrancePortal)
        {
            if (runPathCoroutine == null)
            {
                runPathCoroutine = StartCoroutine(RunPath(false));
            }
        }
        else if (other.gameObject == exitPortal)
        {
            if (runPathCoroutine == null)
            {
                runPathCoroutine = StartCoroutine(RunPath(true));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == entrancePortal || other.gameObject == exitPortal)
        {
            insidePortal = false;
        }
    }
}

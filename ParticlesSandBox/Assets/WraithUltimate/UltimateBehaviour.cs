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
    private float durationTime;
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

            if (deltaPosition > 0.2f)
            {
                AddPathFrames();

                pathDistance += deltaPosition;
                remainingEnergyPercentual = 100 - (pathDistance * 100 / maxDistance);

                remainingEnergyPercentual = Mathf.Clamp(remainingEnergyPercentual, 0, 100); //Display remaining energy as percentual
            }

            yield return null;
        }

        entrancePortal = Instantiate(portalPrefab, myTransformPath.GetFirstFrame(), Quaternion.identity);
        exitPortal = Instantiate(portalPrefab, myTransformPath.GetLastFrame(), Quaternion.identity);

        pathDistance = 0;

        definePathCoroutine = null;

        Debug.Log(myTransformPath.frames.Count);
        Debug.Log(paths[0].frames.Count);
    }

    IEnumerator RunPath()
    {
        isTeleporting.Raise(true);

        for (int i = 0; i < myTransformPath.frames.Count - 1; i++)
        {
            SetPathFrames(i);
            yield return new WaitForSeconds(durationTime / myTransformPath.frames.Count);
        }

        isTeleporting.Raise(false);

        runPathCoroutine = null;
    }

    IEnumerator RunReversePath()
    {
        isTeleporting.Raise(true);

        for (int i = myTransformPath.frames.Count - 1; i >= 0; i--)
        {
            SetPathFrames(i);
            yield return new WaitForSeconds(durationTime / myTransformPath.frames.Count);
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
                runPathCoroutine = StartCoroutine(RunPath());
            }
        }
        else if (other.gameObject == exitPortal)
        {
            if (runPathCoroutine == null)
            {
                runPathCoroutine = StartCoroutine(RunReversePath());
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

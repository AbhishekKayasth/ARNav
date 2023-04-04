using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class SetNavigationTarget : MonoBehaviour
{
    public TMP_Text remainingDst;
    public LineRenderer line; // To render the path
    [HideInInspector]
    public Vector3 targetPosition { get; set; } = Vector3.zero; // current target position

    //[SerializeField]
    //private GameObject arrow;
    [SerializeField]
    private float moveOnDistance;

    //private List<Transform> navigationTargetObjects;
    private NavMeshPath path; // Current calculated path
    private float currentDistance;
    private Vector3[] pathOffset;
    private Vector3 nextNavigationPoint = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();
        targetPosition = Vector3.zero;
        //navigationTargetObjects = FindObjectOfType<ApplicationManager>().navigationTargetObjects;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPosition != Vector3.zero)
        {
            NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path);
            line.positionCount = path.corners.Length;
            line.SetPositions(path.corners);
            line.enabled = true;
            //AddOffsetToPath();
            //SelectNextNavigationPoint();
            //arrow.transform.LookAt(nextNavigationPoint);
            remainingDst.gameObject.SetActive(true);
            float remainingDist = GetPathLength(path);
            Debug.Log(remainingDist);
            if (remainingDist > 1.5)
                remainingDst.text = remainingDist.ToString() + " m";
            else
                remainingDst.text = "Destination Reached";
        }
        else
        {
            line.enabled = false;
            remainingDst.gameObject.SetActive(false);
        }
    }

    public static float GetPathLength(NavMeshPath path)
    {
        float lng = 0.0f;

        if ((path.status != NavMeshPathStatus.PathInvalid))
        {
            for (int i = 1; i < path.corners.Length; ++i)
            {
                lng += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
        }
        return lng;
    }

    /*
    private void AddOffsetToPath()
    {
        pathOffset = new Vector3[path.corners.Length];
        for (int i = 0; i < path.corners.Length; i++)
        {
            pathOffset[i] = new Vector3(path.corners[i].x, transform.position.y, path.corners[i].z);
        }
    }

    private void SelectNextNavigationPoint()
    {
        nextNavigationPoint = SelectNextNavigationPointWithinDistance();
    }


    private Vector3 SelectNextNavigationPointWithinDistance()
    {
        for (int i = 0; i < pathOffset.Length; i++)
        {
            currentDistance = Vector3.Distance(transform.position, pathOffset[i]);
            if (currentDistance > moveOnDistance)
            {
                return pathOffset[i];
            }
        }
        return targetPosition;
    } */
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class TestNav : MonoBehaviour
{
    [SerializeField] private GameObject points;
    
    private List<GameObject> pointList = new List<GameObject>();
    private List<GameObject> movePoints = new List<GameObject>();
    public List<Vector3> turningPointList = new List<Vector3>();
    public Canvas gameOver;
    public Text scoretxt;
    private NavMeshAgent navMeshAgent;
    private NavMeshPath navMeshPath;

    public Camera playerCam;
   

    public int targetPoint;
    public GameObject turningPoint;
    private void Awake()
    {
        targetPoint = Random.Range(1, 15);
    }
    void Start()
    {        
        navMeshAgent = GetComponent<NavMeshAgent>();

        int pointCount = points.transform.childCount;

        for (int i = 0; i < pointCount; i++)
        {
            pointList.Add(points.transform.GetChild(i).gameObject);
        }

        pointList[targetPoint].GetComponent<MeshRenderer>().material.color = Color.yellow;

        navMeshPath = new NavMeshPath();

    }

    public void OnTriggerEnter(Collider other)
    {
        int targetNum = GameObject.Find("Player").GetComponent<TestNav>().targetPoint+1;
        if(other.gameObject.name.Contains(targetNum.ToString())&&other.gameObject.tag == "Point")
        {
            Time.timeScale = 0;
            scoretxt.text = "Score : " + GameManager.instance.catCatchScore.ToString();
            gameOver.gameObject.SetActive(true);
        }

        DeleteTurningPoints();

        var point = other;

        Debug.Log("Point " + other.name + " Collision! \n" + targetNum.ToString());

        navMeshAgent.CalculatePath(pointList[targetPoint].transform.position, navMeshPath);
        
        foreach (var p in navMeshPath.corners)
        {
            turningPointList.Add(p);            
        }
        MakeTurningPoints();
    }
    //public void OnTriggerExit(Collider other)
    //{
    //    DeleteTurningPoints();

    //    var point = other;

    //    Debug.Log("Point " + other.name + " Collision!");

    //    navMeshAgent.CalculatePath(pointList[targetPoint].transform.position, navMeshPath);

    //    foreach (var p in navMeshPath.corners)
    //    {
    //        //Debug.Log("targetPoint: " + (targetPoint + 1) + "\ncorner: " + p);
    //        turningPointList.Add(p);
    //    }
    //    MakeTurningPoints();
    //    ExplainDirection();
    //}

    private void MakeTurningPoints()
    {
        if(turningPointList.Count == 0)
        {
            return;
        }
        foreach(var p in turningPointList)
        {
            movePoints.Add(Instantiate(turningPoint, p, transform.rotation));
        }
        turningPointList.Clear();
    }
    private void DeleteTurningPoints()
    {
        if(movePoints.Count == 0)
        {
            return;
        }
        for(int i =0;i<movePoints.Count;i++)
        {
            Destroy(movePoints[i]);
        }
        movePoints.Clear();
    }

    private void ExplainDirection()
    {
        Vector2 forward = new Vector2(transform.forward.x, transform.forward.z);
        Vector2 wantvec = new Vector2(movePoints[2].transform.position.x- movePoints[1].transform.position.x, 
            movePoints[2].transform.position.z - movePoints[1].transform.position.z);
        
        float angle = GetAngle2(forward, wantvec.normalized);
        Debug.Log($"Angle : {angle} " + $"\nplayer forward : {transform.forward}, forward vec : {forward.x} ,{forward.y} "+ $"\nWantVec : {wantvec.x} , {wantvec.y}");
        //Debug.Log($"move1 : {movePoints[1].transform.position} " + $"\nmove0 : {movePoints[0].transform.position}"+ $"\nplayer : {transform.position}");
    }


    public float CalculateAngle(Vector3 from, Vector3 to)
    {
        //return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
        return Vector3.SignedAngle(Vector3.up, to - from, transform.forward) + 90;
    }
    public  float GetAngle(Vector2 pos1, Vector2 pos2)
    {
        Vector2 dir = (pos2 - pos1).normalized;
        float angle = Mathf.Atan2(dir.x, dir.y);
        float degree = (angle * 180) / Mathf.PI;
        if (degree < 0)
            degree += 360;
        float reDegree = 360 - degree;
        return reDegree;
    }

    public float GetAngle2(Vector2 point1, Vector2 point2)
    {
        Vector2 offset = point2 - point1;
        Debug.Log($"Offset vec : {offset.x},{offset.y}");
        float deg = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        return deg;
    }

}

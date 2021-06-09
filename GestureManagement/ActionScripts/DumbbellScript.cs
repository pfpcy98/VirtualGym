using UnityEngine;
using System.Collections.Generic;
using Windows.Kinect;

public class DumbbellScript : ActionScript
{

    private Vector3 m_startPos_spine;
    private Vector3 m_startPos_shoulder;

    [SerializeField]
    private GameObject m_markerLeft;
    [SerializeField]
    private GameObject m_markerRight;

    private GameObject m_marker;
    private MarkerScript m_markerScript;

    private void FixedUpdate()
    {
        if (m_marker != null)
        {
            if (FeedbackManager.m_returnObject.m_motionStartFlag && !FeedbackManager.m_returnObject.m_motionOnProgressFlag)
            {
                if (m_graspCtrl.m_isCaughtByLeftHand && m_markerScript.m_isOnCollision && m_markerScript.m_collidedJoint == JointType.HandLeft)
                {
                    OnStartAction();
                }
                else if (m_graspCtrl.m_isCaughtByRightHand && m_markerScript.m_isOnCollision && m_markerScript.m_collidedJoint == JointType.HandRight)
                {
                    OnStartAction();
                }
                else
                {

                }
            }
            else if (FeedbackManager.m_returnObject.m_motionStartFlag && FeedbackManager.m_returnObject.m_motionOnProgressFlag)
            {
                if (m_graspCtrl.m_isCaughtByLeftHand && m_markerScript.m_isOnCollision && m_markerScript.m_collidedJoint == JointType.HandLeft)
                {
                    OnProgressAction();
                }
                else if (m_graspCtrl.m_isCaughtByRightHand && m_markerScript.m_isOnCollision && m_markerScript.m_collidedJoint == JointType.HandRight)
                {
                    OnProgressAction();
                }
                else
                {

                }
            }
            else
            {

            }
        }
    }

    public override void OnStart()
    {

    }

    public override bool OnStart(List<JointData> data)
    {
        if(m_graspCtrl == null) { return false; }

        if (m_graspCtrl.m_isCaughtByLeftHand)
        {
            foreach(JointData jointData in data)
            {
                if(jointData.m_jointType == JointType.SpineMid)
                {
                    m_startPos_spine = jointData.m_position;
                }
                else if(jointData.m_jointType == JointType.ShoulderLeft)
                {
                    m_startPos_shoulder = jointData.m_position;
                }
            }

            m_marker = (GameObject)Instantiate(m_markerLeft, new Vector3(m_startPos_shoulder.x + 0.3f, m_startPos_spine.y - 0.8f, m_startPos_spine.z - 0.3f), Quaternion.Euler(0, 90, -45));
        }
        else if(m_graspCtrl.m_isCaughtByRightHand)
        {
            foreach (JointData jointData in data)
            {
                if (jointData.m_jointType == JointType.SpineMid)
                {
                    m_startPos_spine = jointData.m_position;
                }
                else if (jointData.m_jointType == JointType.ShoulderRight)
                {
                    m_startPos_shoulder = jointData.m_position;
                }
            }

            m_marker = (GameObject)Instantiate(m_markerRight, new Vector3(m_startPos_shoulder.x - 0.3f, m_startPos_spine.y - 0.8f, m_startPos_spine.z - 0.3f), Quaternion.Euler(0, 90, -45));
        }
        
        m_marker.GetComponent<Transform>().localScale = new Vector3(3, 3, 3);
        m_markerScript = m_marker.GetComponent<MarkerScript>();
        return true;
    }

    public override void OnAchieveProgress()
    {
        if(m_graspCtrl == null) { return; }
    }

    public override void OnOpenHand()
    {
        if (m_graspCtrl == null) { return; }
        if (m_marker != null)
        {
            Destroy(m_marker);
            m_marker = null;
            m_markerScript = null;
        }
        FeedbackManager.m_returnObject.m_motionStartFlag = false;
        FeedbackManager.m_returnObject.m_motionOnProgressFlag = false;
    }

    private void OnStartAction()
    {
        m_marker.transform.RotateAround(m_marker.GetComponent<Transform>().Find("Pivot").position, Vector3.right, m_speed * Time.deltaTime);
        if(m_marker.transform.rotation.z >= 0.2f)
        {
            FeedbackManager.m_returnObject.OnAchieveProgress();
        }
    }

    private void OnProgressAction()
    {
        m_marker.transform.RotateAround(m_marker.GetComponent<Transform>().Find("Pivot").position, Vector3.right, -m_speed * Time.deltaTime);
        //m_marker.transform.Rotate(m_downVector * Time.deltaTime);
        if (m_marker.transform.rotation.z <= -0.3f)
        {
            FeedbackManager.m_returnObject.OnStart();
        }
    }
}

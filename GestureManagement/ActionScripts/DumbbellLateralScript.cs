using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;

public class DumbbellLateralScript : ActionScript
{

    private Vector3 m_startPos_Spine;
    private Vector3 m_startPos_Shoulder_Left;
    private Vector3 m_startPos_Shoulder_Right;

    [SerializeField]
    private GameObject m_dumbbellLeftObject;
    [SerializeField]
    private GameObject m_dumbbellRightObject;
    
    [SerializeField]
    private GameObject m_markerLeft;
    [SerializeField]
    private GameObject m_markerRight;

    private GameObject m_leftMarker;
    private MarkerScript m_leftMarkerScript;

    private GameObject m_rightMarker;
    private MarkerScript m_rightMarkerScript;

    private void FixedUpdate()
    {
        if(m_leftMarker != null && m_rightMarker != null)
        {
            if(FeedbackManager.m_returnObject.m_motionStartFlag && !FeedbackManager.m_returnObject.m_motionOnProgressFlag)
            {
                if(m_dumbbellLeftObject.GetComponent<GraspCtrl>().m_isCaughtByLeftHand && m_leftMarkerScript.m_isOnCollision && m_leftMarkerScript.m_collidedJoint == JointType.HandLeft &&
                    m_dumbbellRightObject.GetComponent<GraspCtrl>().m_isCaughtByRightHand && m_rightMarkerScript.m_isOnCollision && m_rightMarkerScript.m_collidedJoint == JointType.HandRight)
                {
                    m_leftMarker.transform.RotateAround(m_leftMarker.GetComponent<Transform>().Find("Pivot").position, Vector3.forward, m_speed * Time.deltaTime);
                    m_rightMarker.transform.RotateAround(m_rightMarker.GetComponent<Transform>().Find("Pivot").position, Vector3.forward, -m_speed * Time.deltaTime);

                    if (m_leftMarker.transform.rotation.z >= -0.05f && m_rightMarker.transform.rotation.z <= -0.95f)
                    {
                        FeedbackManager.m_returnObject.OnAchieveProgress();
                    }
                }
            }
            else if(FeedbackManager.m_returnObject.m_motionStartFlag && FeedbackManager.m_returnObject.m_motionOnProgressFlag)
            {
                if (m_dumbbellLeftObject.GetComponent<GraspCtrl>().m_isCaughtByLeftHand && m_leftMarkerScript.m_isOnCollision && m_leftMarkerScript.m_collidedJoint == JointType.HandLeft &&
                    m_dumbbellRightObject.GetComponent<GraspCtrl>().m_isCaughtByRightHand && m_rightMarkerScript.m_isOnCollision && m_rightMarkerScript.m_collidedJoint == JointType.HandRight)
                {
                    m_leftMarker.transform.RotateAround(m_leftMarker.GetComponent<Transform>().Find("Pivot").position, Vector3.forward, -m_speed * Time.deltaTime);
                    m_rightMarker.transform.RotateAround(m_rightMarker.GetComponent<Transform>().Find("Pivot").position, Vector3.forward, m_speed * Time.deltaTime);

                    if (m_leftMarker.transform.rotation.z <= -0.7f && m_rightMarker.transform.rotation.z >= -0.7f)
                    {
                        FeedbackManager.m_returnObject.OnStart();
                    }
                }
            }
        }
    }

    public override bool OnStart(List<JointData> data)
    {
        if(m_dumbbellLeftObject.GetComponent<GraspCtrl>() != null
            && m_dumbbellRightObject.GetComponent<GraspCtrl>() != null)
        {
            if(m_dumbbellLeftObject.GetComponent<GraspCtrl>().m_isCaughtByLeftHand &&
                m_dumbbellRightObject.GetComponent<GraspCtrl>().m_isCaughtByRightHand)
            {
                foreach(JointData jointData in data)
                {
                    if(jointData.m_jointType == JointType.SpineMid)
                    {
                        m_startPos_Spine = jointData.m_position;
                    }
                    else if(jointData.m_jointType == JointType.ShoulderLeft)
                    {
                        m_startPos_Shoulder_Left = jointData.m_position;
                    }
                    else if(jointData.m_jointType == JointType.ShoulderRight)
                    {
                        m_startPos_Shoulder_Right = jointData.m_position;
                    }
                }

                m_leftMarker = (GameObject)Instantiate(m_markerLeft, new Vector3(m_startPos_Shoulder_Left.x + 0.5f, m_startPos_Spine.y - 0.5f, m_startPos_Spine.z), Quaternion.Euler(0, 0, -90));
                m_rightMarker = (GameObject)Instantiate(m_markerRight, new Vector3(m_startPos_Shoulder_Right.x - 0.5f, m_startPos_Spine.y - 0.5f, m_startPos_Spine.z), Quaternion.Euler(0, 0, -90));
                m_leftMarkerScript = m_leftMarker.GetComponent<MarkerScript>();
                m_rightMarkerScript = m_rightMarker.GetComponent<MarkerScript>();

                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    public override void OnOpenHand()
    {
        if(m_leftMarker != null)
        {
            Destroy(m_leftMarker);
            m_leftMarker = null;
            m_leftMarkerScript = null;
        }
        
        if(m_rightMarker != null)
        {
            Destroy(m_rightMarker);
            m_rightMarker = null;
            m_rightMarkerScript = null;
        }

        FeedbackManager.m_returnObject.m_motionStartFlag = false;
        FeedbackManager.m_returnObject.m_motionOnProgressFlag = false;
    }
}

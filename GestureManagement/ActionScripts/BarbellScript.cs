using UnityEngine;
using System.Collections.Generic;
using Windows.Kinect;

public class BarbellScript : ActionScript
{

    private Vector3 m_startPos_Head;
    private Vector3 m_startPos_Spine;
    private Vector3 m_startPos_Shoulder_Left;
    private Vector3 m_startPos_Shoulder_Right;

    [SerializeField] private GameObject m_markerObject;
    private GameObject m_marker;
    private MarkerScript m_markerScript;

	// Update is called once per frame
	void FixedUpdate ()
    {
	    if(m_marker != null)
        {
            if(FeedbackManager.m_returnObject.m_motionStartFlag && !FeedbackManager.m_returnObject.m_motionOnProgressFlag)
            {
                if(m_graspCtrl.m_isCaughtByLeftHand && m_markerScript.m_isOnCollision &&
                    m_graspCtrl.m_isCaughtByRightHand && m_markerScript.m_isOnCollision)
                {
                    OnStartAction();
                }
            }
            else if (FeedbackManager.m_returnObject.m_motionStartFlag && FeedbackManager.m_returnObject.m_motionOnProgressFlag)
            {
                if (m_graspCtrl.m_isCaughtByLeftHand && m_markerScript.m_isOnCollision &&
                     m_graspCtrl.m_isCaughtByRightHand && m_markerScript.m_isOnCollision)
                {
                    OnProgressAction();
                }
            }
        }
	}

    public override bool OnStart(List<JointData> data)
    {
        if(m_graspCtrl == null) { return false; }

        foreach(JointData jointData in data)
        {
            if (jointData.m_jointType == JointType.Head)
            {
                m_startPos_Head = jointData.m_position;
            }
            else if (jointData.m_jointType == JointType.ShoulderLeft)
            {
                m_startPos_Shoulder_Left = jointData.m_position;
            }

            else if (jointData.m_jointType == JointType.ShoulderRight)
            {
                m_startPos_Shoulder_Right = jointData.m_position;
            }

            else if (jointData.m_jointType == JointType.SpineMid)
            {
                m_startPos_Spine = jointData.m_position;
            }
        }

        m_marker = (GameObject)Instantiate(m_markerObject, new Vector3(Vector3.Lerp(m_startPos_Shoulder_Left, m_startPos_Shoulder_Right, 0.5f).x, m_startPos_Head.y, m_startPos_Spine.z), Quaternion.Euler(0, 0, 0));
        m_markerScript = m_marker.GetComponent<MarkerScript>();

        return true;
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
        m_marker.transform.Translate(Vector3.up * m_speed * Time.deltaTime);
        if(m_marker.transform.position.y >= m_startPos_Head.y + 1.0f)
        {
            FeedbackManager.m_returnObject.OnAchieveProgress();
        }
    }

    private void OnProgressAction()
    {
        m_marker.transform.Translate(Vector3.down * m_speed * Time.deltaTime);
        if(m_marker.transform.position.y <= m_startPos_Head.y)
        {
            FeedbackManager.m_returnObject.OnStart();
        }
    }
}

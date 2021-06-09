using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;

public class TimeStamp
{

    public Vector3 m_positon;
    public float m_time;

    public TimeStamp()
    {
        m_positon = new Vector3();
        m_time = 0.0f;
    }
}

public class Hand
{
    
    public GameObject m_handObject;
    public bool m_isGrasped { get; private set; }

    private int m_capturedFrame;
    private const int m_req_CapturedFrame = 10;

    private List<Vector3> m_position;

    private int m_openedFrame;
    private const int m_requiredFrame = 60;

    public Hand(GameObject handObject)
    {
        m_handObject = handObject;

        m_position = new List<Vector3>();

        m_openedFrame = 0;
        m_capturedFrame = 0;

        m_isGrasped = false;
    }

    public void Grasp()
    {
        m_isGrasped = true;
        m_openedFrame = 0;
    }

    public void GraspInterpolation()
    {
        if (m_isGrasped)
        {
            m_openedFrame++;
            if (m_openedFrame >= m_requiredFrame)
            {
                m_isGrasped = false;
                m_openedFrame = 0;
            }
        }
    }
    
}

public class FeedbackManager : MonoBehaviour, IActionScript
{

    #region System variable

    // Singleton
    static public FeedbackManager m_returnObject { get; private set; }

    #region Kinect Hardware variable

    private KinectSensor m_kinectSensor;

    private BodyFrameReader m_bodyFrameReader;
    private int m_bodyCount;
    private Body[] m_bodies;
    private List<int> m_catchedBodyList = null;

    private int m_bodyIndex;

    #endregion

    // Target track
    public enum Axis { X, Y, Z };

    [SerializeField]
    private JointType[] m_trackTarget = null;
    private List<TimeStamp>[] m_targetPosition = null;
    /*
    [SerializeField]
    private Axis[] m_referenceAxis = null;
    private bool[] m_isNegative = null;
    */
    
    [SerializeField]
    private Transform m_head;
    
    [SerializeField]
    private GameObject m_leftHandObject;
    public Hand m_leftHand { get; private set; }
    
    [SerializeField]
    private GameObject m_rightHandObject;
    public Hand m_rightHand { get; private set; }

    /*
    // Velocity calculation
    private TimeStamp[] m_originStamp;
    private TimeStamp m_currentStamp;
    private TimeStamp[] m_prevStamp;
    private const int m_requiredFrame = 15;
    private float[] m_avgMoveSpeed;
    [SerializeField]
    private float m_standardVelocity;
    */

    // Gesture detection
    /*
    private List<GestureDetector> m_gestureDetectorList = null;
    [SerializeField]
    private string m_gestureDBFile;
    [SerializeField]
    private string m_onProgressGestureName;
    */
    [SerializeField]
    private GameObject m_gestureDBObject;
    private ActionScript m_gestureDB;
    [SerializeField] private UIManager m_UIManager = null;

    public bool m_motionStartFlag { get; set; }
    public bool m_motionOnProgressFlag { get; set; }

    /*
    // Feedback
    private bool[] m_isOverSpeed;
    private const int m_requiredCheckCount = 3;
    private int[] m_checkedCount;
    */

    #endregion
    
    void Start ()
    {
        m_returnObject = this;
        m_kinectSensor = KinectSensor.GetDefault();

        if (m_kinectSensor != null)
        {
            m_bodyCount = m_kinectSensor.BodyFrameSource.BodyCount;
            m_bodyFrameReader = m_kinectSensor.BodyFrameSource.OpenReader();
            m_bodyFrameReader.FrameArrived += Reader_FrameArrived;
            m_bodies = new Body[m_bodyCount];
            m_catchedBodyList = new List<int>();
            
            /*
            if(m_trackTarget.Length > 0)
            {
                m_originStamp = new TimeStamp[m_trackTarget.Length];
                m_prevStamp = new TimeStamp[m_trackTarget.Length];
                m_targetPosition = new List<TimeStamp>[m_trackTarget.Length];
                for(int i = 0; i < m_targetPosition.Length; i++)
                {
                    m_targetPosition[i] = new List<TimeStamp>();
                }
                m_avgMoveSpeed = new float[m_trackTarget.Length];

                /*
                m_isNegative = new bool[m_trackTarget.Length];
                for(int i = 0; i < m_isNegative.Length; i++)
                {
                    m_isNegative[i] = false;
                }
                */

                /*
                m_isOverSpeed = new bool[m_trackTarget.Length];
                for(int i = 0; i < m_isOverSpeed.Length; i++)
                {
                    m_isOverSpeed[i] = false;
                }

                m_checkedCount = new int[m_trackTarget.Length];
                for(int i = 0; i < m_checkedCount.Length; i++)
                {
                    m_checkedCount[i] = 0;
                }
            }
            */

            /*
            m_gestureDetectorList = new List<GestureDetector>();
            for(int i = 0; i < m_bodyCount; i++)
            {
                m_gestureDetectorList.Add(new GestureDetector(m_kinectSensor, m_gestureDBFile));
            }
            */

            if(m_gestureDBObject.GetComponent<ActionScript>() != null)
            {
                m_gestureDB = m_gestureDBObject.GetComponent<ActionScript>();
            }

            m_leftHand = new Hand(m_leftHandObject);
            m_rightHand = new Hand(m_rightHandObject);

            m_motionStartFlag = false;
            m_motionOnProgressFlag = false;

            m_kinectSensor.Open();
        }
    }

    void FixedUpdate()
    {
        bool isNewBodyData = false;
        using (BodyFrame bodyFrame = m_bodyFrameReader.AcquireLatestFrame())
        {
            if (bodyFrame != null)
            {
                bodyFrame.GetAndRefreshBodyData(m_bodies);
                isNewBodyData = true;
            }
        }

        if (isNewBodyData)
        {
            Body body = m_bodies[m_bodyIndex];
            if (body != null)
            {
                /*
                // Gesture Detecting
                ulong trackingId = body.TrackingId;

                if(trackingId != m_gestureDetectorList[m_bodyIndex].TrackingId)
                {
                    m_gestureDetectorList[m_bodyIndex].TrackingId = trackingId;

                    m_gestureDetectorList[m_bodyIndex].IsPaused = (trackingId == 0);
                    m_gestureDetectorList[m_bodyIndex].OnGestureDetected += CreateOnGestureHandler();
                }
                */

                // Joint(main) tracking
                m_leftHand.m_handObject.transform.position = GetVector3FromJoint(body.Joints[JointType.HandLeft]);
                if (body.HandLeftState == HandState.Closed)
                {
                    m_leftHand.Grasp();
                    m_leftHand.m_handObject.GetComponent<MeshRenderer>().material = Resources.Load("IsGrasped") as Material;
                }
                else
                {
                    if (m_leftHand.m_isGrasped) { m_leftHand.GraspInterpolation(); }
                    else
                    {
                        m_leftHand.m_handObject.GetComponent<MeshRenderer>().material = Resources.Load("Default") as Material;
                    }
                }

                m_rightHand.m_handObject.transform.position = GetVector3FromJoint(body.Joints[JointType.HandRight]);
                if (body.HandRightState == HandState.Closed)
                {
                    m_rightHand.Grasp();
                    m_rightHand.m_handObject.GetComponent<MeshRenderer>().material = Resources.Load("IsGrasped") as Material;
                }
                else
                {
                    if (m_rightHand.m_isGrasped) { m_rightHand.GraspInterpolation(); }
                    else
                    {
                        m_rightHand.m_handObject.GetComponent<MeshRenderer>().material = Resources.Load("Default") as Material;
                    }
                }

                m_head.position = new Vector3(GetVector3FromJoint(body.Joints[JointType.Head]).x,
                    4.5f + GetVector3FromJoint(body.Joints[JointType.Head]).y * 0.1f,
                    GetVector3FromJoint(body.Joints[JointType.Head]).z);

                /*
                // Joint(target) tracking
                int jointIndex = 0;
                foreach (JointType joint in m_trackTarget)
                {
                    m_currentStamp = new TimeStamp();
                    m_currentStamp.m_positon = GetVector3FromJoint(body.Joints[joint]);
                    m_currentStamp.m_time = Time.time;

                    if (m_targetPosition[jointIndex].Count < m_requiredFrame + 1) { m_targetPosition[jointIndex].Add(m_currentStamp); }
                    else
                    {
                        // full frame
                        m_originStamp[jointIndex] = m_targetPosition[jointIndex][m_targetPosition[jointIndex].Count - m_requiredFrame - 1];
                        m_prevStamp[jointIndex] = m_targetPosition[jointIndex][m_targetPosition[jointIndex].Count - 1];

                        m_currentStamp.m_positon.x = m_prevStamp[jointIndex].m_positon.x + (m_currentStamp.m_positon.x - m_originStamp[jointIndex].m_positon.x) / m_requiredFrame;
                        m_currentStamp.m_positon.y = m_prevStamp[jointIndex].m_positon.y + (m_currentStamp.m_positon.y - m_originStamp[jointIndex].m_positon.y) / m_requiredFrame;
                        m_currentStamp.m_positon.z = m_prevStamp[jointIndex].m_positon.z + (m_currentStamp.m_positon.z - m_originStamp[jointIndex].m_positon.z) / m_requiredFrame;

                        switch(m_referenceAxis[jointIndex])
                        {
                            case Axis.X:
                                m_avgMoveSpeed[jointIndex] = (m_currentStamp.m_positon.x - m_originStamp[jointIndex].m_positon.x) / (m_currentStamp.m_time - m_originStamp[jointIndex].m_time);
                                break;
                            case Axis.Y:
                                m_avgMoveSpeed[jointIndex] = (m_currentStamp.m_positon.y - m_originStamp[jointIndex].m_positon.y) / (m_currentStamp.m_time - m_originStamp[jointIndex].m_time);
                                break;
                            case Axis.Z:
                                m_avgMoveSpeed[jointIndex] = (m_currentStamp.m_positon.z - m_originStamp[jointIndex].m_positon.z) / (m_currentStamp.m_time - m_originStamp[jointIndex].m_time);
                                break;
                        }


                        if (!m_isNegative[jointIndex] && (m_avgMoveSpeed[jointIndex] >= m_standardVelocity))
                        {
                            // Positive reference(start to on progress) & Move speed is over set speed.

                            m_isOverSpeed[jointIndex] = true;
                            if (joint == JointType.HandLeft) { m_leftHand.m_object.GetComponent<TrailRenderer>().material.color = new Color(255, 0, 0); }
                            if (joint == JointType.HandRight) { m_rightHand.m_object.GetComponent<TrailRenderer>().material.color = new Color(255, 0, 0); }
                        }
                        else if (m_isNegative[jointIndex] && (m_avgMoveSpeed[jointIndex] <= -(m_standardVelocity)))
                        {
                            // Negative reference(on progress to end) & Move speed is over set speed.

                            m_isOverSpeed[jointIndex] = true;
                            if (joint == JointType.HandLeft) { m_leftHand.m_object.GetComponent<TrailRenderer>().material.color = new Color(255, 0, 0); }
                            if (joint == JointType.HandRight) { m_rightHand.m_object.GetComponent<TrailRenderer>().material.color = new Color(255, 0, 0); }
                        }
                        else
                        {
                            // Good Status!

                            if (m_isOverSpeed[jointIndex])
                            {
                                m_checkedCount[jointIndex]++;
                                if (m_checkedCount[jointIndex] >= m_requiredCheckCount)
                                {
                                    m_isOverSpeed[jointIndex] = false;
                                    m_checkedCount[jointIndex] = 0;

                                    if (joint == JointType.HandLeft) { m_leftHand.m_object.GetComponent<TrailRenderer>().material.color = new Color(0, 255, 0); }
                                    if (joint == JointType.HandRight) { m_rightHand.m_object.GetComponent<TrailRenderer>().material.color = new Color(0, 255, 0); }
                                }
                            }
                        }

                        m_targetPosition[jointIndex].Clear();
                    }

                    jointIndex++;
                }
                */

            }
        }
	}
    
    private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
    {
        bool dataReceived = false;

        using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
        {
            if(bodyFrame != null)
            {
                if(m_bodies == null) { m_bodies = new Body[bodyFrame.BodyCount]; }
                bodyFrame.GetAndRefreshBodyData(m_bodies);
                dataReceived = true;
            }
        }

        if (dataReceived)
        {
            for(int i = 0; i < m_bodies.Length; i++)
            {
                if(m_bodies[i].IsTracked)
                {
                    m_catchedBodyList.Add(i);
                }
            }

            if (m_catchedBodyList.Count >= 2)
            {
                int i = 0;
                float coord_Z = m_bodies[0].Joints[JointType.SpineBase].Position.Z;
                m_bodyIndex = m_catchedBodyList[0];

                foreach (int j in m_catchedBodyList)
                {
                    if (coord_Z < m_bodies[i].Joints[JointType.SpineBase].Position.Z)
                    {
                        coord_Z = m_bodies[i].Joints[JointType.SpineBase].Position.Z;
                        m_bodyIndex = j;
                    }
                    i++;
                }
            }
            else if(m_catchedBodyList.Count <= 0) { }
            else { m_bodyIndex = m_catchedBodyList[0]; }

            m_catchedBodyList.Clear();
        }
    }

    /*
    private EventHandler<GestureEventArgs> CreateOnGestureHandler()
    {
        return (object sender, GestureEventArgs e) => OnGestureDetected(sender, e);
    }
    */

    /*
    private void OnGestureDetected(object sender, GestureEventArgs e)
    {
        bool isDetected = e.m_isBodyTrackingIdValid && e.m_isGestureDetected;
        
        if(e.m_gestureID == m_startGestureName && isDetected && e.m_detectionConfidence > 0.8f)
        {
            if (!m_motionStartFlag) { m_motionStartFlag = true; }

            // Success
            if (m_motionStartFlag && m_motionOnProgressFlag)
            {
                m_motionStartFlag = false;
                m_motionOnProgressFlag = false;
                for (int i = 0; i < m_trackTarget.Length; i++) { m_isNegative[i] = false; }

                GameManager.GetObject().ScoreEvent();
            }
        }

        if(e.m_gestureID == m_onProgressGestureName && isDetected && e.m_detectionConfidence > 0.8f)
        {
            if (m_motionStartFlag && !m_motionOnProgressFlag)
            {
                m_motionOnProgressFlag = true;

                for(int i = 0; i < m_trackTarget.Length; i++) { m_isNegative[i] = true; }
            }
        }
    }
    */

    public void OnStart()
    {
        if (m_motionStartFlag && m_motionOnProgressFlag)
        {
            if(m_UIManager.OnExcirciseSuccess())
            {

            }
            m_motionOnProgressFlag = false;
            m_motionStartFlag = true;
        }
        else if (!m_motionStartFlag)
        {
            List<JointData> data = new List<JointData>();
            for (int i = 0; i < m_trackTarget.Length; i++)
            {
                data.Add(new JointData(m_trackTarget[i], GetVector3FromJoint(m_bodies[m_bodyIndex].Joints[m_trackTarget[i]])));
            }

            if (m_gestureDB != null)
            {
                if (m_gestureDB.OnStart(data))
                {
                    m_motionStartFlag = true;
                }
            }
        }
    }

    public bool OnStart(List<JointData> data)
    {
        //
        return false;
    }

    public void OnAchieveProgress()
    {
        m_motionOnProgressFlag = true;
    }

    public void OnOpenHand()
    {
        m_gestureDB.OnOpenHand();
    }

    private Vector3 GetVector3FromJoint(Windows.Kinect.Joint joint)
    {
        return new Vector3(-(joint.Position.X * 3) - 5, joint.Position.Y * 3 + 3, joint.Position.Z * 3);
    }

    private Quaternion GetQuaternionFromVector3(Vector3 a, Vector3 b, bool flag)
    {
        float dx = b.x - a.x;
        float dy = b.y - a.y;
        float dz = Math.Abs(b.z - a.z);

        float rot_x;
        float rot_y;
        float rot_z;

        if (flag)
        {
            // left
            rot_x = 45;
            rot_y = 180 - (float)(Math.Atan2(dz, dx) * (180 / Math.PI));
            rot_z = 90 + (float)(Math.Atan2(dz, dy) * (180 / Math.PI));
        }
        else
        {
            // right
            rot_x = 45;
            rot_y = -(float)(Math.Atan2(dz, dx) * (180 / Math.PI));
            rot_z = -90 - (float)(Math.Atan2(dz, dy) * (180 / Math.PI));
        }

        return Quaternion.Euler(rot_x, rot_y, rot_z);
    }
}

using UnityEngine;
using System.Collections.Generic;
using Windows.Kinect;

interface IActionScript
{
    void OnStart();
    bool OnStart(List<JointData> data);
    void OnAchieveProgress();
    void OnOpenHand();
}

public class JointData
{
    public JointType m_jointType { get; private set; }
    public Vector3 m_position { get; private set; }

    public JointData(JointType jointType, Vector3 position)
    {
        m_jointType = jointType;
        m_position = position;
    }
}

public class ActionScript : MonoBehaviour, IActionScript
{
    
    protected GraspCtrl m_graspCtrl;
    
    [SerializeField]
    protected float m_speed;

    protected Vector3 m_upVector;
    protected Vector3 m_downVector;

    private void Start()
    {
        m_graspCtrl = GetComponent<GraspCtrl>();

        m_upVector = new Vector3(0, 0, m_speed);
        m_downVector = new Vector3(0, 0, -(m_speed));
    }

    virtual public void OnStart()
    {

    }

    virtual public bool OnStart(List<JointData> data)
    {
        return false;
    }

    virtual public void OnAchieveProgress()
    {

    }

    virtual public void OnOpenHand()
    {

    }
}
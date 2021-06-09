using UnityEngine;
using Windows.Kinect;

public class MarkerScript : MonoBehaviour
{
    
    public bool m_isCaughtByLeftHand { get; private set; }
    public bool m_isCaughtByRightHand { get; private set; }

    public JointType m_collidedJoint { get; private set; }
    public bool m_isOnCollision { get; private set; }

    private void Start()
    {
        m_isOnCollision = false;   
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Collider>().tag == "Tool")
        {
            m_isOnCollision = true;
        }
        else if(other.GetComponent<Collider>().tag == "LeftHand")
        {
            m_collidedJoint = JointType.HandLeft;
            m_isOnCollision = true;
        }
        else if(other.GetComponent<Collider>().tag == "RightHand")
        {
            m_collidedJoint = JointType.HandRight;
            m_isOnCollision = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        m_isOnCollision = false;
    }
}

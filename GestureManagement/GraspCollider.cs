using UnityEngine;
using System.Collections;

public class GraspCollider : MonoBehaviour {

    public bool m_isCollidedByLeftHand { get; private set; }
    public bool m_isCollidedByRightHand { get; private set; }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Collider>().tag == "LeftHand")
        {
            if (FeedbackManager.m_returnObject.m_leftHand.m_isGrasped)
            {
                m_isCollidedByLeftHand = true;
            }
            else
            {
                m_isCollidedByLeftHand = false;
            }
        }
        if (other.GetComponent<Collider>().tag == "RightHand")
        {
            if (FeedbackManager.m_returnObject.m_rightHand.m_isGrasped)
            {
                m_isCollidedByRightHand = true;
            }
            else
            {
                m_isCollidedByRightHand = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Collider>().tag == "LeftHand")
        {
            m_isCollidedByLeftHand = false;
        }
        if (other.GetComponent<Collider>().tag == "RightHand")
        {
            m_isCollidedByRightHand = false;
        }
    }
}

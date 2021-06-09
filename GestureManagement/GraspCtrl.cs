using UnityEngine;
using System.Collections;

public class GraspCtrl : MonoBehaviour
{

    private Transform m_transform;
    private Vector3 m_initialTransform;

    [SerializeField]
    private bool m_isNeedBothHands;
    [SerializeField] private GraspCollider m_leftCollider;
    [SerializeField] private GraspCollider m_rightCollider;

    public bool m_isCaughtByLeftHand { get; private set; }
    public bool m_isCaughtByRightHand { get; private set; }
    private bool m_isCaught;

	// Use this for initialization
	private void Start ()
    {
        m_transform = GetComponent<Transform>();
        m_initialTransform = m_transform.position;
        m_isCaught = false;
	}
	
	private void FixedUpdate ()
    {
        m_isCaughtByLeftHand = m_leftCollider.m_isCollidedByLeftHand;
        m_isCaughtByRightHand = m_rightCollider.m_isCollidedByRightHand;

        if(m_isNeedBothHands)
        {
            if(m_isCaughtByLeftHand && m_isCaughtByRightHand) { m_isCaught = true; }
            else { m_isCaught = false; }
        }
        else
        {
            if(m_isCaughtByLeftHand || m_isCaughtByRightHand) { m_isCaught = true; }
            else { m_isCaught = false; }
        }
        
        if(m_isCaught)
        {
            if(m_isNeedBothHands)
            {
                if(FeedbackManager.m_returnObject.m_motionStartFlag == false)
                {
                    FeedbackManager.m_returnObject.OnStart();
                }
                m_transform.position = Vector3.Lerp(FeedbackManager.m_returnObject.m_leftHand.m_handObject.transform.position, FeedbackManager.m_returnObject.m_rightHand.m_handObject.transform.position, 0.5f);
            }
            else
            {
                if(m_isCaughtByLeftHand)
                {
                    if (FeedbackManager.m_returnObject.m_motionStartFlag == false)
                    {
                        FeedbackManager.m_returnObject.OnStart();
                    }
                    m_transform.position = FeedbackManager.m_returnObject.m_leftHand.m_handObject.transform.position;
                }
                else if(m_isCaughtByRightHand)
                {
                    if (FeedbackManager.m_returnObject.m_motionStartFlag == false)
                    {
                        FeedbackManager.m_returnObject.OnStart();
                    }
                    m_transform.position = FeedbackManager.m_returnObject.m_rightHand.m_handObject.transform.position; 
                }
            }
        }
        else
        {
            m_transform.position = m_initialTransform;
            FeedbackManager.m_returnObject.OnOpenHand();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Diagnostics;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text       m_curTimeText        = null;
    [SerializeField] private Text       m_excirciseTimeText  = null;
    [SerializeField] private Text       m_spendCalorieText   = null;

    [SerializeField] private GameObject m_successUI               = null;
    [SerializeField] private Text       m_UI_tryCountText         = null;
    [SerializeField] private Text       m_UI_usedCalorieText      = null;
    [SerializeField] private Text       m_UI_totalUsedCalorieText = null;

    [SerializeField] private int        m_calorieAmount      = 0;
    [SerializeField] private int        m_requiredTryCount   = 0;
    [SerializeField] private bool       m_isLobbyScene       = false;

    private static int m_excirciseTime_Hours   = 0;
    private static int m_excirciseTime_Minutes = 0;
    private static int m_excirciseTime_Seconds = 0;
    private static int m_totalSpendCalorie     = 0;
    
    private Stopwatch m_stopwatch    = new Stopwatch();
    private int       m_spendCalorie = 0;

    private void Start()
    {
        m_successUI.SetActive(false);
        if (!m_isLobbyScene)
        {
            m_stopwatch.Start();
        }
    }

    private void FixedUpdate ()
    {
        ////값 수정 부분
        ////1초가 지났을 경우 && 로비씬이 아닐때
        //if (!m_isLobbyScene)
        //{
        //    m_excirseTime += 1.0f;
        //    m_spendCalorie += m_calorieAmount;
        //}

        //문자열 수정 부분
        m_curTimeText.text = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString("D2");
        if (m_isLobbyScene)
        {
            m_excirciseTimeText.text = "총 운동 시간 : " + m_excirciseTime_Hours.ToString() + "시간 " + m_excirciseTime_Minutes.ToString() + "분 " + m_excirciseTime_Seconds.ToString("D2") + "초";
            m_spendCalorieText.text = "현재 소모 칼로리 : " + m_totalSpendCalorie.ToString() + "cal";
        }
        else
        {
            m_excirciseTimeText.text = "현재 운동 시간 :\n " + m_stopwatch.Elapsed.Hours.ToString() + "시간 " + m_stopwatch.Elapsed.Minutes.ToString() + "분 " + m_stopwatch.Elapsed.Seconds.ToString("D2") + "초";
            m_spendCalorieText.text = "현재 소모 칼로리 :\n" + m_spendCalorie.ToString() + "cal";
        }

    }

    private void OnDestroy()
    {
        if(!m_isLobbyScene) { OnExitScene(); }
    }

    public bool OnExcirciseSuccess()
    {
        if(m_isLobbyScene) { return false; }

        m_spendCalorie += m_calorieAmount;

        if(m_spendCalorie >= m_calorieAmount * m_requiredTryCount)
        {
            OnExitScene();

            m_successUI.SetActive(true);
            m_UI_tryCountText.text = (m_spendCalorie / m_calorieAmount).ToString() + "회";
            m_UI_usedCalorieText.text = m_spendCalorie.ToString() + "cal";
            m_UI_totalUsedCalorieText.text = m_totalSpendCalorie.ToString() + "cal";

            return true;
        }

        return false;
    }

    public void OnExitScene()
    {
        m_totalSpendCalorie += m_spendCalorie;

        if (m_stopwatch.IsRunning)
        {
            m_stopwatch.Stop();

            m_excirciseTime_Hours += m_stopwatch.Elapsed.Hours;
            m_excirciseTime_Minutes += m_stopwatch.Elapsed.Minutes;
            m_excirciseTime_Seconds += m_stopwatch.Elapsed.Seconds;

            if (m_excirciseTime_Seconds >= 60)
            {
                m_excirciseTime_Minutes += m_excirciseTime_Seconds / 60;
                m_excirciseTime_Seconds = m_excirciseTime_Seconds % 60;
            }

            if (m_excirciseTime_Minutes >= 60)
            {
                m_excirciseTime_Hours += m_excirciseTime_Minutes / 60;
                m_excirciseTime_Minutes = m_excirciseTime_Minutes % 60;
            }
        }
    }
}

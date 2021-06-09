using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Diagnostics;

public class GameManager
{

    static private GameManager returnObject = null;

    public enum ActionType { DUMBBELL, DUMBBELL_LATERAL, BARBELL };
    public ActionType actionType { get; private set; }

    [SerializeField]
    private Text m_scoreText;
    public Stopwatch m_stopwatch { get; private set; }

    public int m_score { get; private set; }

    #region Singleton method
    public GameManager()
    {
        returnObject = this;

        m_stopwatch = new Stopwatch();
        m_score = 0;
    }

    public static GameManager GetObject()
    {
        if(returnObject == null) { returnObject = new GameManager(); }
        return returnObject;
    }
    #endregion

    public void ScoreEvent()
    {
        m_score++;
        m_scoreText.text = "Score : " + m_score.ToString();
    }
}

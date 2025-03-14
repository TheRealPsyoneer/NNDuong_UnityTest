using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCondition : MonoBehaviour
{
    public event Action ConditionCompleteEvent = delegate { };

    protected Text m_txt;

    protected bool m_conditionCompleted = false;

    //
    public bool ConditionCompleted { get => m_conditionCompleted; }
    [SerializeField] EventSO levelLoseEvent;
    [SerializeField] EventSO levelWinEvent;

    private void OnEnable()
    {
        levelLoseEvent.ThingHappened += OnConditionFail;
        levelWinEvent.ThingHappened += OnConditionComplete;
    }

    private void OnDisable()
    {
        levelLoseEvent.ThingHappened -= OnConditionFail;
        levelWinEvent.ThingHappened -= OnConditionComplete;
    }
    
    private void Awake()
    {
        levelLoseEvent = Resources.Load<EventSO>(Constants.LEVEL_LOSE_EVENT);
        levelWinEvent = Resources.Load<EventSO>(Constants.LEVEL_WIN_EVENT);
    }
    //
    public virtual void Setup(float value, Text txt)
    {
        m_txt = txt;
    }

    public virtual void Setup(float value, Text txt, GameManager mngr)
    {
        m_txt = txt;
    }

    public virtual void Setup(float value, Text txt, BoardController board)
    {
        m_txt = txt;
    }

    protected virtual void UpdateText() { }

    protected void OnConditionComplete()
    {
        m_conditionCompleted = true;
        GameManager.Instance.isGameOver = true;

        ConditionCompleteEvent();
    }

    protected void OnConditionFail()
    {
        m_conditionCompleted = false;
        GameManager.Instance.isGameOver = true;

        ConditionCompleteEvent();
    }

    protected virtual void OnDestroy()
    {

    }
}

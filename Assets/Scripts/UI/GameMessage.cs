using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;
using SamuraiSoccer.Event;

public class GameMessage : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_text;

    private float m_time = 100;

    private void Start()
    {
        InGameEvent.Goal.Where(t => t == GoalEventType.NormalTeammateGoal || t == GoalEventType.NormalOpponentGoal).Subscribe(_ => TextContent = "Goal!").AddTo(this);
        InGameEvent.Penalty.Subscribe(_ => TextContent = "反則!").AddTo(this);
    }
    public string TextContent 
    { 
        set 
        { 
            m_text.text = value; 
            m_time = 0; 
        } 
    }

    private void Update()
    {
        m_text.color = new Color(0, 0, 0, (4 - m_time) / 2);
        m_time += m_time > 100 ? 0 : Time.deltaTime;
    }
}

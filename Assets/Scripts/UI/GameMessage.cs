using UnityEngine;
using UniRx;
using TMPro;
using SamuraiSoccer.Event;
using UnityEngine.UI;

public class GameMessage : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_text;

    [SerializeField]
    private Image m_backgroundImage;

    [SerializeField]
    private Color m_goalColor;

    [SerializeField]
    private Color m_penaltyColor;

    private float m_time = 100;

    private void Start()
    {
        InGameEvent.Goal.Where(t => t == GoalEventType.NormalTeammateGoal || t == GoalEventType.NormalOpponentGoal).Subscribe(_ => 
        {
            m_backgroundImage.color = m_goalColor;
            TextContent = "Goal!";           
        }).AddTo(this);
        InGameEvent.Penalty.Subscribe(_ =>
        {
            m_backgroundImage.color = m_penaltyColor;
            TextContent = "反則!";            
        }).AddTo(this);
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
        m_backgroundImage.color = new Color(m_backgroundImage.color.r, m_backgroundImage.color.g, m_backgroundImage.color.b, (4 - m_time) / 2);
        m_time += m_time > 100 ? 0 : Time.deltaTime;
    }
}

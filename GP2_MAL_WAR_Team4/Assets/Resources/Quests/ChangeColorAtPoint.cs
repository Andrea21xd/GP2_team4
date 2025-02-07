using UnityEngine;

public class QuestPointHighlighter : MonoBehaviour
{
    [SerializeField] private Color startQuestColor = Color.green;
    [SerializeField] private Color inProgressColor = Color.gray;
    [SerializeField] private Color finishQuestColor = Color.green;

    private MeshRenderer meshRenderer;
    private QuestPoint[] questPoints;

    private void Start()
    {
        questPoints = GetComponents<QuestPoint>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (questPoints == null || questPoints.Length == 0) return;

        if (CurrentQuestExists()) {
            meshRenderer.material.color = startQuestColor;
        } else
        {
            meshRenderer.material.color = inProgressColor;
        }
    }


    private bool CurrentQuestExists()
    {
        bool questExists = false;

        foreach(var questPoint in questPoints)
        {
            if (questPoint.GetStartPoint() && questPoint.GetCurrentQuestState() == QuestState.CAN_START) { questExists = true; }
            if (questPoint.GetFinishPoint() && questPoint.GetCurrentQuestState() == QuestState.CAN_FINISH) { questExists = true; }
        }

        return questExists;
    }
}

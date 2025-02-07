using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "ScriptableObjects/QuestInfoSO")]
public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }

    [Header("General")]
    public string displayName;

    [Header("General")]
    public string objective;

    [Header("Requirements")]
    public int health;
    public int speed;
    public int luck;
    public int baseDamage;
    public QuestInfoSO[] questPrerequisites;

    [Header("Steps")]
    public GameObject[] questStepPrefabs;

    [Header("Rewards")]
    public int healthReward;
    public int speedReward;
    public int luckReward;
    public int baseDamageReward;

    public bool playerChoosesReward;


    private void OnValidate()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(id))
        {
            id = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this); 
        }
#endif
    }
}

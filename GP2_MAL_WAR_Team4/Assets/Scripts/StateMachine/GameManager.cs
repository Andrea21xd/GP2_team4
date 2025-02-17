using UnityEngine;

public class GameManager : StateMachine
{
    public static GameManager Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
           // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        SwitchState<StartState>();
    }

    void Update()
    {
        UpdateStateMachine();
    }

    private void FixedUpdate()
    {
        FixedUpdateStateMachine();
    }

    public GameState GetGameState<T>() where T : GameState
    {
        foreach (State state in states)
        {
            if (state.GetType() == typeof(T))
            {
                return (GameState)state;
            }
        }
        Debug.LogWarning("GameState does not exist in GameManager");
        return null;
    }

}

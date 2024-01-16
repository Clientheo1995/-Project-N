public static class EventManager
{
    //서버
    public delegate void ServerConnected();
    public delegate void ServerDisconnected();

    //UI
    public delegate void StageRestEffect(int index);
    public static event StageRestEffect EStageRestEffect;
    public static void OnEventStageRestEffect(int index)
    {
        EStageRestEffect?.Invoke(index);
    }

    public delegate void OnPanel(EnUIPanel panel);
    public static event OnPanel EOnPanel;
    public static void OnEventOnPanel(EnUIPanel panel)
    {
        EOnPanel?.Invoke(panel);
    }

    public delegate void OffPanel(EnUIPanel panel);
    public static event OffPanel EOffPanel;
    public static void    OnEventOffPanel(EnUIPanel panel)
    {
        EOffPanel?.Invoke(panel);
    }

    public delegate void OnRestPanel(EnRestPanelOrder restIndex);
    public static event OnRestPanel EOnRestPanel;
    public static void OnEventOnRestPanel(EnRestPanelOrder restIndex)
    {
        EOnRestPanel?.Invoke(restIndex);
    }

    //게임 프로세스
    public delegate void MakeInstance();
    public static event MakeInstance EMakeInstance;

    public delegate void RemoveInstance();
    public static event RemoveInstance ERemoveInstance;

    public delegate void SkillEffect(EnSkillConditionType condition, float value);
    public static event SkillEffect ESkillEffect;

    public delegate void MonsterFreeze();
    public static event MonsterFreeze EMonsterFreeze;

    public delegate void Initialize();
    public static event Initialize EInitialize;
    public static void OnEventInitialize()
    {
        EInitialize?.Invoke();
    }

    public delegate void SetNextStage(int stageIndex, MapNode node, bool isBoss);
    public static event SetNextStage ESetNextStage;
    public static void OnEventSetNextStage(int stageIndex, MapNode node, bool isBoss)
    {
        ESetNextStage?.Invoke(stageIndex, node, isBoss);
    }

    public delegate void SetStageData(int stageIndex, MapNode node, bool isBoss);
    public static event SetStageData ESetStageData;
    public static void OnEventSetStageData(int stageIndex, MapNode node, bool isBoss)
    {
        ESetStageData?.Invoke(stageIndex, node, isBoss);
    }

    public delegate void FirstGameOver();
    public static event FirstGameOver EFirstGameOver;
    public static void OnEventFirstGameOver()
    {
        EFirstGameOver?.Invoke();
    }

    public delegate void ResetGameOverStack();
    public static event ResetGameOverStack EResetGameOverStack;
    public static void OnEventResetGameOverStack()
    {
        EResetGameOverStack?.Invoke();
    }

    public delegate void InstanceValueReset();
    public static event InstanceValueReset EInstanceValueReset;
    public static void OnEventInstanceValueReset()
    {
        EInstanceValueReset?.Invoke();
    }

    public delegate void MonsterDie();
    public static event MonsterDie EMonsterDie;
    public static void OnEventMonsterDie()
    {
        EMonsterDie?.Invoke();
    }

}
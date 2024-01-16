using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstanceController : MonoBehaviour
{
    [SerializeField] public Character FirstCrypture;
    [SerializeField] Text ScoreText;
    [SerializeField] Text GOScore;
    [SerializeField] GameObject stageSelect;
    [SerializeField] int MapIndex;

    List<Character> m_listCrypture;
    List<Monster> m_listMonster;
    Vector2 m_vt2Direction;
    float m_fFollowSpeed;

    GameObject m_goMonster;

    [SerializeField] List<GameObject> Maps;
    List<Vector3> m_listSpawnPoints;
    GameObject Map;

    bool m_bIsBoss;
    bool m_bIsFirstGameOver = true;
    int m_nGoNextStageCut;

    List<SkillData> m_listRestSkill;

    IEnumerator WaitForData()
    {
        yield return DataManager.Instance.DATALOADCOMPLETE;
        yield return DataManager.Instance.GameStart;
        GameInitialize();
    }

    void Awake()
    {
        EventManager.EInitialize += GameInitialize;
        EventManager.EStageRestEffect += StageRest;
        EventManager.EFirstGameOver += CheckFIrstGameOver;
        EventManager.EResetGameOverStack += ResetGameOverStack;
        EventManager.EInstanceValueReset += ValueReset;
        EventManager.EMonsterDie += MonsterDie;
        GameInitialize();
    }

    void OnDestroy()
    {
        EventManager.EInitialize -= GameInitialize;
        EventManager.EStageRestEffect -= StageRest;
        EventManager.EFirstGameOver -= CheckFIrstGameOver;
        EventManager.EResetGameOverStack -= ResetGameOverStack;
        EventManager.EInstanceValueReset -= ValueReset;
        EventManager.EMonsterDie -= MonsterDie;
    }

    void Start()
    {
        m_goMonster = Resources.Load<GameObject>("Prefabs/Monster");
    }

    void Update()
    {
        if (DataManager.Instance.GameStart)
        {
            if (DataManager.Instance.CurrentKillCount >= m_nGoNextStageCut)
            {
                Clear();
            }
        }
    }

    void ValueReset()
    {
        DataManager.Instance.CurrentScore = 0f;
        DataManager.Instance.CurrentKillCount = 0;
        DataManager.Instance.CurrentCombo = 0;
        m_bIsFirstGameOver = true;
        m_bIsBoss = false;
        m_nGoNextStageCut = 0;
    }

    public void GameInitialize()
    {
        
        if (m_listMonster == null)
            m_listMonster = new List<Monster>();
        else if (m_listMonster != null)
            m_listMonster.Clear();

        FirstCrypture.gameObject.SetActive(true);
        FirstCrypture.transform.position = Vector3.zero;
        FirstCrypture.SetData(this, DataManager.Instance.User.RosterCryptures[0], null);
        //CryptureList.Add(FirstCrypture);
        m_fFollowSpeed = FirstCrypture.m_Speed * 3;//임시
    }

    void StageRest(int index)
    {
        switch (index)
        {
            case 0://낮잠 최대체력의 20퍼마큼 모든 크립쳐 회복
                FirstCrypture.m_Hp += FirstCrypture.m_MaxHp * 0.2f;
                SoundManager.Instance.SetSound(AudioChannel.UI, "heal");
                EventManager.OnEventOnRestPanel(EnRestPanelOrder.Length);
                break;
            case 1://드르러엉
                {
                    int result = Random.Range(0, 2);
                    SoundManager.Instance.SetSound(AudioChannel.UI, "heal");

                    if (result == 0)//50퍼확률로 최대체력의40퍼만큼 모든크립쳐 회복
                    {
                        FirstCrypture.m_Hp += FirstCrypture.m_MaxHp * 0.4f;
                        EventManager.OnEventOnRestPanel(EnRestPanelOrder.Length);
                    }
                    else// 50퍼확률로 최대체력의 10퍼만큼 회복 + 일반전투 진입
                    {
                        FirstCrypture.m_Hp += FirstCrypture.m_MaxHp * 0.1f;
                        EventManager.OnEventOnPanel(EnUIPanel.Instance);
                    }
                }
                break;
            case 2://주변탐색 50퍼확률로 최대 체력 10퍼만큼 모든크립처 회복 or 50퍼확률로 스킬 획득 실패 최대체력 10퍼만큼 모든 크립처 회복
                {
                    //스킬 획득
                    int result = Random.Range(0, 2);
                    if (result == 0)
                    {
                        FirstCrypture.m_Hp += FirstCrypture.m_MaxHp * 0.1f;
                        DataManager.Instance.RestSkillList.Clear();

                        while (DataManager.Instance.RestSkillList.Count < 3)
                        {
                            int skillIndex = Random.Range(1, 9);
                            if (DataManager.Instance.RestSkillList.Contains(DataManager.Instance.SkillInfo[skillIndex + 3000]))
                                continue;
                            DataManager.Instance.RestSkillList.Add(DataManager.Instance.SkillInfo[skillIndex + 3000]);
                        }

                        SoundManager.Instance.SetSound(AudioChannel.UI, "DM-CGS-45");
                        EventManager.OnEventOnRestPanel(EnRestPanelOrder.restSelect);
                    }
                    else
                    {
                        FirstCrypture.m_Hp += FirstCrypture.m_MaxHp * 0.05f;
                        SoundManager.Instance.SetSound(AudioChannel.UI, "DM-CGS-02");
                        //미획득
                        EventManager.OnEventOnRestPanel(EnRestPanelOrder.Length);
                    }
                }
                break;
        }
    }

    void ResetGameOverStack()
    {
        m_bIsFirstGameOver = true;
    }

    void CheckFIrstGameOver()
    {
        m_bIsFirstGameOver = false;
    }

    void MonsterDie()
    {
        DataManager.Instance.CurrentKillCount += 1;
    }

    public void SetStageData(int stageIndex, MapNode node, bool isBoss)
    {
        this.m_bIsBoss = isBoss;
        ValueReset();
        DataManager.Instance.ThisStage = DataManager.Instance.StageInfo[stageIndex];
        m_nGoNextStageCut = DataManager.Instance.RecentLayer * DataManager.Instance.ThisStage.monster_stair_clear_plus + DataManager.Instance.ThisStage.monster_first_stair_clear_ea;

        DataManager.Instance.ThisNode = node;
        DataManager.Instance.KILLALL = false;
    }

    void AddMonster(int index)
    {
        Monster newMonster = Instantiate(m_goMonster, SetSpawnPoint(), Quaternion.identity, transform).GetComponent<Monster>();
        newMonster.SetData(this, index);
        m_listMonster.Add(newMonster);
    }

    public void AddScore(int monster_score)
    {
        DataManager.Instance.CurrentScore += monster_score;
        ScoreText.text = $"Score: {DataManager.Instance.CurrentScore}";
        GOScore.text = $"Score: {DataManager.Instance.CurrentScore}";
    }

    Vector3 SetSpawnPoint()
    {
        if (m_listSpawnPoints == null)
        {
            m_listSpawnPoints = new List<Vector3>();
        }
        else
        {
            m_listSpawnPoints.Clear();
        }

        Transform spawnPoints = Map.transform.Find("SpawnPoint");
        return spawnPoints.GetChild(Random.Range(0, spawnPoints.childCount)).position;
    }

    void Clear()
    {
        m_bIsBoss = false;
        DataManager.Instance.GameStart = false;
        EventManager.OnEventOnPanel(EnUIPanel.GameClear);
        DataManager.Instance.KILLALL = true;
        FirstCrypture.transform.position = Vector3.zero;
    }

    public void Restart()
    {
        GameInitialize();
    }

    public void GameOver()
    {
        EventManager.OnEventOnPanel(EnUIPanel.GameOver);
    }

    public void OpenStageSelect()
    {
        stageSelect.SetActive(true);
    }

    public void BossClear()
    {
        Clear();
        m_bIsBoss = false;
    }
}

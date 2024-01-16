using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public List<int> startmembers = new List<int>();
    UserData m_UserData;

    public bool DATALOADCOMPLETE = false;
    public bool GameStart = false;
    [SerializeField]
    public static float MinDistance = 10f;

    public List<MapNode> WholeNodes;
    public List<Transform> Layers;

    public Stage ThisStage;
    public int RecentLayer = -1;
    public bool KILLALL = false;
    public MapNode ThisNode;
    public List<SkillData> RestSkillList = new List<SkillData>();

    //Table
    public Dictionary<string, string> StringTable = new Dictionary<string, string>();

    public Dictionary<int, CharacterData> CryptureInfo = new Dictionary<int, CharacterData>();
    public Dictionary<int, MonsterData> MonsterInfo = new Dictionary<int, MonsterData>();
    public Dictionary<int, PartData> PartInfo = new Dictionary<int, PartData>();
    public Dictionary<int, AttackType> AttackTypeInfo = new Dictionary<int, AttackType>();
    public Dictionary<int, TileData> AttackReadyCheckTableInfo = new Dictionary<int, TileData>();
    public Dictionary<int, TileData> AttackDamageCheckTableInfo = new Dictionary<int, TileData>();
    public Dictionary<int, Mode> ModeInfo = new Dictionary<int, Mode>();
    public Dictionary<int, SkillData> SkillInfo = new Dictionary<int, SkillData>();
    public Dictionary<int, SkillCondition> SkillConditionInfo = new Dictionary<int, SkillCondition>();
    public Dictionary<int, SkillActivation> SkillActivationInfo = new Dictionary<int, SkillActivation>();
    public Dictionary<int, SkillTarget> SkillTargetInfo = new Dictionary<int, SkillTarget>();
    public Dictionary<int, SkillEffect> SkillEffectInfo = new Dictionary<int, SkillEffect>();
    public Dictionary<int, DropItem> DropItemInfo = new Dictionary<int, DropItem>();
    public Dictionary<int, DropItemEffect> DropItemEffectInfo = new Dictionary<int, DropItemEffect>();
    public Dictionary<int, Stage> StageInfo = new Dictionary<int, Stage>();
    public Dictionary<int, StageEvent> StageEventInfo = new Dictionary<int, StageEvent>();
    public Dictionary<int, StageShop> StageShopInfo = new Dictionary<int, StageShop>();

    public List<Character> PlayerCryptureList = new List<Character>();

    public float CurrentScore;
    public int CurrentKillCount;
    public int CurrentCombo;

    public UserData User
    {
        get { return m_UserData; }
    }

    void Start()
    {
        PlayerPrefs.Save();
        m_UserData = new UserData();//플레이어 데이터 저장하는거 만들어야함

        SetString("string", StringTable);
        SetData("attack_type", AttackTypeInfo);
        SetData("character", CryptureInfo);
        SetData("monster", MonsterInfo);
        SetData("parts", PartInfo);
        SetMapData("attack_ready_check_table", AttackReadyCheckTableInfo);
        SetCheckTiles(ref AttackReadyCheckTableInfo);
        SetMapData("attack_damage_check_table", AttackDamageCheckTableInfo);
        SetCheckTiles(ref AttackDamageCheckTableInfo);

        SetData("skill_condition", SkillConditionInfo);
        SetData("skill_activation", SkillActivationInfo);
        SetData("skill_target", SkillTargetInfo);
        SetData("skill_effect", SkillEffectInfo);
        SetData("skill", SkillInfo);

        SetData("drop_item", DropItemInfo);
        SetData("drop_item_effect", DropItemEffectInfo);

        SetData("stage", StageInfo);
        SetData("stage_event", StageEventInfo);
        SetData("stage_shop", StageShopInfo);
        SetData("mode", ModeInfo);
        SetString("string", StringTable);
        DATALOADCOMPLETE = true;
    }

    public void Resume()//인스턴스 관리 들어가면 그쪽으로 옮길것
    {
        if (Time.timeScale <= 0)
            Time.timeScale = 1f;
        else
            Time.timeScale = 0f;
    }

    void SetData<T>(string csvName, Dictionary<int, T> classDic) where T : new()
    {
        string[] variables = new string[] { };
        string[] headers = new string[] { };
        List<List<string>> list = new List<List<string>>();
        CSVReader.Read(csvName, ref variables, ref headers, ref list);

        if (list == null)
        {
            Debug.Log($"테이블 {typeof(T)}를 읽지 못했습니다.");
            return;
        }

        for (int i = 0; i < list.Count; i++)
        {
            T newT = new T();
            int key = 0;
            int count = 0;

            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                try
                {
                    if (propertyInfo.Name.Equals("index"))
                    {
                        key = Convert.ToInt32(list[i][count]);
                    }

                    if (variables[count].Equals(propertyInfo.PropertyType.Name) && propertyInfo.PropertyType.Name.Equals("Int32") && headers[count].Equals(propertyInfo.Name))
                    {
                        if (list[i][count].Equals("0"))//Convert.ToInt32는 0을 컨버팅 할 수 없다
                            propertyInfo.SetValue(newT, 0);
                        else
                            propertyInfo.SetValue(newT, Convert.ToInt32(list[i][count]));
                    }
                    else if (variables[count].Equals(propertyInfo.PropertyType.Name) && propertyInfo.PropertyType.Name.Equals("Single") && headers[count].Equals(propertyInfo.Name))
                    {
                        propertyInfo.SetValue(newT, Convert.ToSingle(list[i][count]));
                    }
                    else if (variables[count].Equals(propertyInfo.PropertyType.Name) && propertyInfo.PropertyType.Name.Equals("Double") && headers[count].Equals(propertyInfo.Name))
                    {
                        propertyInfo.SetValue(newT, Convert.ToDouble(list[i][count]));
                    }
                    else if (variables[count].Equals(propertyInfo.PropertyType.Name) && propertyInfo.PropertyType.Name.Equals("String") && headers[count].Equals(propertyInfo.Name))
                    {
                        propertyInfo.SetValue(newT, list[i][count]);
                    }
                    else if (variables[count].Equals(propertyInfo.PropertyType.Name) && propertyInfo.PropertyType.Name.Equals("Bool") && headers[count].Equals(propertyInfo.Name))
                    {
                        propertyInfo.SetValue(newT, Convert.ToBoolean(list[i][count]));
                    }
                    else//enum을 위한 부분
                    {
                        if (list[i][count].Equals("0"))//Convert.ToInt32는 0을 컨버팅 할 수 없다
                            propertyInfo.SetValue(newT, 0);
                        else if (list[i][count].Equals("0.0"))
                            propertyInfo.SetValue(newT, 0.0f);
                        else
                            propertyInfo.SetValue(newT, Convert.ToInt32(list[i][count]));
                    }
                }
                catch
                {
                    Debug.Log(propertyInfo.ReflectedType.Name + ", " + propertyInfo.PropertyType.Name + ", " + propertyInfo.Name);
                }
                count++;
            }

            if (key == 0)
                continue;

            classDic.Add(key, newT);
        }
    }

    void SetMapData(string csvName, Dictionary<int, TileData> classDic)
    {
        Dictionary<int, TileData> table = CSVReader.ReadMap(csvName);

        if (table == null)
        {
            Debug.Log($"맵을 읽지 못했습니다.");
            return;
        }

        foreach (KeyValuePair<int, TileData> p in table)
        {
            classDic.Add(p.Key, p.Value);
        }
    }

    void SetString(string csvName, Dictionary<string, string> stringDic)
    {
        Dictionary<string, string> table = CSVReader.ReadString(csvName);

        if (table == null)
        {
            Debug.Log($"맵을 읽지 못했습니다.");
            return;
        }

        foreach (KeyValuePair<string, string> p in table)
        {
            stringDic.Add(p.Key, p.Value);
        }
    }

    public void SetCheckTiles(ref Dictionary<int, TileData> dic)
    {
        int startPos = -1;
        Dictionary<int, TileData> newDic = new Dictionary<int, TileData>();

        foreach (KeyValuePair<int, TileData> readyCheck in dic)
        {
            for (int i = 0; i < readyCheck.Value.tileData.Length; i++)
            {
                if (readyCheck.Value.tileData[i] == 1)//근거리 범위 공격일 때
                {
                    startPos = i;
                    break;
                }
                if (readyCheck.Value.tileData[i] == 2)
                    startPos = i;
            }

            if (startPos == -1)//원거리 투사체
                continue;

            if (startPos > 0)
            {
                int targetRow = 0;//공격자나 공격대상의 행렬 위치
                int targetCol = 0;

                targetRow = startPos % readyCheck.Value.col > 0 ? startPos / readyCheck.Value.col : (startPos / readyCheck.Value.col) + 1;
                targetCol = startPos % readyCheck.Value.col;

                TileData newData = readyCheck.Value;
                newData.targetIndex = startPos;
                newData.targetRow = targetRow;
                newData.targetCol = targetCol;

                newDic.Add(readyCheck.Key, newData);
            }
            else
            {
                newDic.Add(readyCheck.Key, readyCheck.Value);
            }
        }

        dic.Clear();
        dic = newDic;
    }
}

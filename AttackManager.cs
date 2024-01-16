using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AttackManager : Singleton<AttackManager>
{
    public static int TILE_SIZE = 1;

    public bool IsAttack = false;
    GameObject m_goCheckTile;
    GameObject m_goBullet;
    List<List<GameObject>> m_listReadyCheckTile;

    //얘는 캐릭터를 따라다녀야함
    public TileEventListener MakeReadyTile(AttackType attackType, Transform owner, Transform destination)
    {
        if (m_listReadyCheckTile == null)
            m_listReadyCheckTile = new List<List<GameObject>>();

        if (m_goCheckTile == null)
        {
            m_goCheckTile = Resources.Load<GameObject>("Prefabs/CheckTile");
        }

        for (int i = 0; i < destination.childCount; i++)
        {
            Destroy(destination.GetChild(i).gameObject);
        }

        destination.gameObject.layer = owner.gameObject.layer;
        int tileIndex = attackType.attack_ready_check_idx;
        TileEventListener tl = destination.gameObject.GetComponent<TileEventListener>();
        if (tl == null)
            tl = destination.gameObject.AddComponent<TileEventListener>();
        tl.SetData(owner, true, attackType, destination);

        for (int i = 0; i < DataManager.Instance.AttackReadyCheckTableInfo[tileIndex].row; i++)
        {
            for (int j = 0; j < DataManager.Instance.AttackReadyCheckTableInfo[tileIndex].col; j++)
            {
                int rowGap = i - DataManager.Instance.AttackReadyCheckTableInfo[tileIndex].targetRow;//행
                int colGap = j - DataManager.Instance.AttackReadyCheckTableInfo[tileIndex].targetCol;//열

                Vector3 pos = new Vector3(colGap * TILE_SIZE, rowGap * TILE_SIZE, 0f) + destination.position;//y축은 데이터와 반대로 해야 화면상 좌표에 제대로 뜬다
                GameObject tile = Instantiate(m_goCheckTile, pos, destination.rotation, destination);
                tile.layer = owner.gameObject.layer;
                EnTileType tileType = (EnTileType)DataManager.Instance.AttackReadyCheckTableInfo[tileIndex].tileData[i * DataManager.Instance.AttackReadyCheckTableInfo[tileIndex].col + j];
                tile.GetComponent<CheckTile>().SetData(tl, tileType);
            }
        }

        return tl;
    }

    //얘는 캐릭터를 따라다니면 안됨
    public void MakeDamageTile(AttackType attackType, float damageBuff, Transform owner, Transform destination, Quaternion rotation)
    {
        int tileIndex = attackType.attack_damage_check_idx;
        ParticleSystem EffectType1 = m_goCheckTile.GetComponentInChildren<ParticleSystem>();
        GameObject newTileGroup = new GameObject("DamageTiles");
        newTileGroup.transform.position = destination.position;
        newTileGroup.transform.rotation = rotation;

        if (owner.gameObject.layer == 6)//Player
            newTileGroup.gameObject.layer = 11;//PlayerTile
        else if (owner.gameObject.layer == 7)//Monster
            newTileGroup.gameObject.layer = 12;//MonsterTile

        TileEventListener tl = newTileGroup.AddComponent<TileEventListener>();
        tl.SetData(owner, false, attackType, destination, EffectType1.main.duration, damageBuff);
        for (int i = 0; i < DataManager.Instance.AttackDamageCheckTableInfo[tileIndex].row; i++)
        {
            for (int j = 0; j < DataManager.Instance.AttackDamageCheckTableInfo[tileIndex].col; j++)
            {
                EnTileType tileType = (EnTileType)DataManager.Instance.AttackDamageCheckTableInfo[tileIndex].tileData[i * DataManager.Instance.AttackDamageCheckTableInfo[tileIndex].col + j];
                if (tileType != EnTileType.damage) continue;
                int rowGap = i - DataManager.Instance.AttackDamageCheckTableInfo[tileIndex].targetRow;//행
                int colGap = j - DataManager.Instance.AttackDamageCheckTableInfo[tileIndex].targetCol;//열

                Vector3 pos = new Vector3(colGap * TILE_SIZE, rowGap * TILE_SIZE, 0f)/* + destination.localPosition*/;//y축은 데이터와 반대로 해야 화면상 좌표에 제대로 뜬다
                //https://m.blog.naver.com/dj3630/221447943453
                //쿼터니언 값으로 변환이 되는데 이건 그대로 적용하면 안되는걸까
                GameObject tile = Instantiate(m_goCheckTile, newTileGroup.transform);//빈 오브젝트 만들어서 그 엠티에 회전을 주고 그 아래에 타일을 배치하기
                tile.layer = newTileGroup.gameObject.layer;
                tile.transform.SetLocalPositionAndRotation(pos, newTileGroup.transform.rotation);
                tile.GetComponent<CheckTile>().SetData(tl, tileType);
            }
        }
    }

    public void MakeBullet(AttackType attackType, float damageBuff, Transform startTarget, Transform finishTarget)
    {
        if (m_goBullet == null)
        {
            m_goBullet = Resources.Load<GameObject>("Prefabs/Bullet");
        }

        //추적 원거리 공격
        if (attackType.attack_type == EnAttackType.missile)
        {
            GameObject newBullet = Instantiate(m_goBullet, startTarget.position, startTarget.rotation);
            newBullet.GetComponent<Bullet>().SetData(attackType, startTarget.parent, finishTarget, 10f, null, damageBuff);
            newBullet.gameObject.layer = startTarget.gameObject.layer;
        }
        else if (attackType.attack_type == EnAttackType.range)
        {
            GameObject newBullet = Instantiate(m_goBullet, startTarget.position, startTarget.rotation);
            newBullet.GetComponent<Bullet>().SetData(attackType, startTarget.parent, finishTarget, 10f, null, damageBuff);
            newBullet.gameObject.layer = startTarget.gameObject.layer;
        }
    }
}
using System.Collections;

using UnityEngine;

public class TileEventListener : MonoBehaviour
{
    public float Damage;
    public float DamageBuff;

    InstanceController m_Controller;
    AttackType m_AttackType;
    Transform DrawTarget;
    Transform Owner;

    bool m_bCanAttack = true;
    bool m_bIsReadyTile = false;
    bool m_bIsAttacked = false;

    float m_fCurTime;
    float m_delay;

    public void SetData(Transform owner, bool isReadyTile, AttackType attackType, Transform drawTarget, float damageBuff = 0f, float delay = 0f)
    {
        Owner = owner;
        m_AttackType = attackType;
        m_bIsReadyTile = isReadyTile;
        DrawTarget = drawTarget;
        m_delay = delay;
        DamageBuff = damageBuff;
        Damage = m_AttackType.attack_damage;
        //if (!m_bIsReadyTile)
        //    StartCoroutine(Dying());
    }

    void Update()
    {
        if (!m_bCanAttack && m_bIsReadyTile)
        {
            m_fCurTime += Time.deltaTime;
        }

        if (m_fCurTime > m_AttackType?.attack_cooltime)
        {
            m_fCurTime = 0;
            m_bCanAttack = true;
        }

        if (DataManager.Instance.KILLALL)
        {
            if (Owner != null)
            {
                if (Owner.name != "first")
                    Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

        }
    }

    public void CallEvent(Collider2D collision)
    {
        if (!m_bCanAttack)
            return;

        if (m_bIsReadyTile)
        {
            if (m_bCanAttack)
            {
                if (m_AttackType.attack_type == EnAttackType.range || m_AttackType.attack_type == EnAttackType.missile)
                {
                    AttackManager.Instance.MakeBullet(m_AttackType, DamageBuff, DrawTarget, collision.transform);
                    m_bCanAttack = false;
                }
                else
                {
                    AttackManager.Instance.MakeDamageTile(m_AttackType, DamageBuff, Owner, DrawTarget, transform.rotation);//그 자리에 생성, 공격자(타일의 부모)의 회전값을 넘겨줌
                    m_bCanAttack = false;
                }
            }
        }
        else
        {
            if (!m_bIsAttacked)
            {
                if (collision.CompareTag("Monster"))
                {
                    m_bIsAttacked = true;
                    Character Character = Owner.GetComponent<Character>();
                    collision.transform.GetComponent<Monster>().CalculateDamage(Damage + DamageBuff);
                    Character.CheckSkill(EnSkillConditionType.attack);
                    Destroy(gameObject);
                }
                if (collision.CompareTag("Player"))
                {
                    transform.parent.GetComponent<Monster>()?.CheckSkill(EnSkillConditionType.attack);
                    collision.transform.GetComponent<Character>().CalculateDamage(Damage + DamageBuff);
                    Destroy(gameObject);
                }
            }
        }
    }

    IEnumerator Dying()
    {
        yield return new WaitForSeconds(m_delay);
        Destroy(gameObject);
    }
}

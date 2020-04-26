using UnityEngine;
using UnityEngine.Assertions;

public class UnitAttack : MonoBehaviour
{
    [SerializeField]
    private int m_damage = 0;
    [SerializeField]
    private float m_shootRange = 0.0f;
    [SerializeField]
    private Rigidbody m_projectile = null;
    [SerializeField]
    private float m_projectileSpeed = 0.0f;
    [SerializeField]
    private float m_timeBetweenShot = 0.0f;
    
    private float m_elaspedTime = 0.0f;
    private Unit m_unit = null;

    private void Awake()
    {
        m_unit = GetComponent<Unit>();
        Assert.IsNotNull(m_unit);
    }

    private void Update()
    {
        m_elaspedTime += Time.deltaTime;
    }

    private void meleeAttack(Vector3 position)
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, (position - transform.position), out hit, m_shootRange, (1 << 12)))
        {
            Unit unit = hit.collider.gameObject.GetComponent<Unit>();
            Assert.IsNotNull(unit);
            if(unit)
            {
                GameManager.Instance.damageUnit(unit, m_damage);
            }
        }
    }

    private void fireProjectile(Vector3 position)
    {
        Rigidbody clone;
        clone = Instantiate(m_projectile, transform.position, Quaternion.identity);
        Vector3 vBetween = position - transform.position;
        clone.velocity = transform.TransformDirection(vBetween.normalized * m_projectileSpeed);

        Projectile projectile = clone.GetComponent<Projectile>();
        Assert.IsNotNull(projectile);
        projectile.setSenderID(m_unit.getID(), m_unit.m_factionName, m_damage);
    }

    public void attackPosition(Vector3 position)
    {
        //If enemy in range
        if (m_elaspedTime >= m_timeBetweenShot &&
            (position - transform.position).sqrMagnitude <= m_shootRange * m_shootRange)
        {
            m_elaspedTime = 0.0f;

            if(m_projectile)
            {
                fireProjectile(position);
            }
            else
            {
                meleeAttack(position);
            }
        }
    }

    public bool isTargetInAttackRange(Vector3 targetPosition)
    {
        Vector3 result = targetPosition - transform.position;
        return result.sqrMagnitude <= m_shootRange * m_shootRange;
    }
}
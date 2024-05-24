using UnityEngine;

[CreateAssetMenu(fileName = "CatAttacking Configuration", menuName = "Enemy Scriptable Object/CatAttacking Configuration")]
public class AttackingScriptableObject : ScriptableObject
{
    [Header("Attack Configuration")]
    [SerializeField] private int _damage = 100;

    [SerializeField] private float _attackRadius = 1.5f;
    [SerializeField] private float _attackDelay = 1.5f;

    [Header("Ranged Configuration")]
    [SerializeField] private bool _isRanged = false;

    [SerializeField] private bool _isHomingBullet = false;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private HomingBullet _homingBulletPrefab;
    [SerializeField] private Vector3 _bulletSpawnOffset = new(0, 1, 0);
    [SerializeField] private LayerMask _lineOfSightLayers = -1;

    #region Getters & Setters

    public int Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    public float AttackRadius
    {
        get { return _attackRadius; }
        set { _attackRadius = value; }
    }

    public float AttackDelay
    {
        get { return _attackDelay; }
        set { _attackDelay = value; }
    }

    public bool IsRanged
    {
        get { return _isRanged; }
        set { _isRanged = value; }
    }

    public bool IsHomingBullet
    {
        get { return _isHomingBullet; }
        set { _isHomingBullet = value; }
    }

    public HomingBullet HomingBulletPrefab
    {
        get { return _homingBulletPrefab; }
        set { _homingBulletPrefab = value; }
    }

    public Bullet BulletPrefab
    {
        get { return _bulletPrefab; }
        set { _bulletPrefab = value; }
    }

    public Vector3 BulletSpawnOffset
    {
        get { return _bulletSpawnOffset; }
        set { _bulletSpawnOffset = value; }
    }

    public LayerMask LineOfSightLayers
    {
        get { return _lineOfSightLayers; }
        set { _lineOfSightLayers = value; }
    }

    #endregion Getters & Setters

    public void SetupEnemy(Enemy enemy)
    {
        enemy.AttackRadius.SphereColliderAttackRadius = _attackRadius;
        enemy.AttackRadius.AttackDelay = _attackDelay;
        enemy.AttackRadius.Damage = _damage;

        if (_isRanged)
        {
            RangedAttackRadius rangedAttackRadius = enemy.AttackRadius.GetComponent<RangedAttackRadius>();

            if (_isHomingBullet)
            {
                rangedAttackRadius.BulletSpawnOffset = _bulletSpawnOffset;
                rangedAttackRadius.LineOfSightLayers = _lineOfSightLayers;
            }
            else
            {
                rangedAttackRadius.BulletSpawnOffset = _bulletSpawnOffset;
                rangedAttackRadius.LineOfSightLayers = _lineOfSightLayers;
            }
        }
    }
}
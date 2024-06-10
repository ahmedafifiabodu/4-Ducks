using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Enemy Configuration", menuName = "Enemy Scriptable Object/Enemy Configuration Type")]
public class EnemyScriptableObject : ScriptableObject
{
    #region Properiets

    [Header("Enemy")]
    [SerializeField] private AttackingScriptableObject _attackingConfiguration;

    [SerializeField] private int _health = 100;
    [SerializeField] private float _AIUpdateInvterval = 0.1f;

    [Header("Enemy States")]
    [SerializeField] private EnemyStates _defaultState;

    [SerializeField] private float _idleLocationRadius = 4f;
    [SerializeField] private float _idleMovingSpeedMultiplier = 0.5f;
    [SerializeField][Range(2, 10)] private int _wayPoints = 4;
    [SerializeField] private float _fieldOfView = 90f;
    [SerializeField] private float _lineOfSightRange = 6f;

    [Header("Navmesh Agent")]
    [SerializeField] private float _acceleration = 8;

    [SerializeField] private float _angularSpeed = 120;
    [SerializeField] private int _areamask = -1;

    [SerializeField] private int _avoidancePriority = 50;
    [SerializeField] private float _baseOffset = 0f;
    [SerializeField] private float _height = 2f;

    [SerializeField] private ObstacleAvoidanceType _obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;

    [SerializeField] private float _radius = 0.5f;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _stoppingDistance = 0.5f;

    #endregion Properiets

    #region Getters

    internal AttackingScriptableObject AttackingConfiguration => _attackingConfiguration;
    internal int Health => _health;
    internal float AIUpdateInvterval => _AIUpdateInvterval;
    internal float Acceleration => _acceleration;
    internal float AngularSpeed => _angularSpeed;
    internal int Areamask => _areamask;
    internal int AvoidancePriority => _avoidancePriority;
    internal float BaseOffset => _baseOffset;
    internal float Height => _height;
    internal ObstacleAvoidanceType ObstacleAvoidanceType => _obstacleAvoidanceType;
    internal float Radius => _radius;
    internal float Speed => _speed;
    internal float StoppingDistance => _stoppingDistance;

    #endregion Getters

    public void SetupEnemy(Enemy enemy)
    {
        _health = Health;

        _attackingConfiguration.SetupEnemy(enemy);

        enemy.NavmeshEnemyMovment.UpdateSpeed = AIUpdateInvterval;
        enemy.NavmeshEnemyMovment.DefaultState = _defaultState;
        enemy.NavmeshEnemyMovment.IdleLocationRadius = _idleLocationRadius;
        enemy.NavmeshEnemyMovment.IdleMovingSpeedMultiplier = _idleMovingSpeedMultiplier;
        enemy.NavmeshEnemyMovment.WayPointsIndex = new Vector3[_wayPoints];

        enemy.NavmeshEnemyMovment.LineOfSightChecker.FieldOfView = _fieldOfView;
        enemy.NavmeshEnemyMovment.LineOfSightChecker.ColliderRadius = _lineOfSightRange;
        enemy.NavmeshEnemyMovment.LineOfSightChecker.LineOfSightLayers = AttackingConfiguration.LineOfSightLayers;

        enemy.NavMeshAgent.acceleration = Acceleration;
        enemy.NavMeshAgent.angularSpeed = AngularSpeed;
        enemy.NavMeshAgent.areaMask = Areamask;
        enemy.NavMeshAgent.avoidancePriority = AvoidancePriority;
        enemy.NavMeshAgent.baseOffset = BaseOffset;
        enemy.NavMeshAgent.height = Height;
        enemy.NavMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType;
        enemy.NavMeshAgent.radius = Radius;
        enemy.NavMeshAgent.speed = Speed;
        enemy.NavMeshAgent.stoppingDistance = StoppingDistance;
    }
}
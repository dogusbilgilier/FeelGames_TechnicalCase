using System;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    [SerializeField] private LayerMask _collisionMask;
    public bool IsInitialized { get; private set; }
    private IObjectPool<Ball> _pool;
    private int _colorID;
    private bool _canMove;

    private Vector3 _currentDirection;
    private MaterialPropertyBlock _materialPropertyBlock;
    private Renderer _renderer;
    private float _radius;
    private PixelPiece _target;
    private float _nextRetargetTime;

    private float _skin = 0.005f;
    private int _maxHitsPerFrame = 6;
    private float _retargetInterval = 0.25f;
    private float _postBounceMaxDeg = 12f;

    public void Initialize()
    {
        _materialPropertyBlock = new MaterialPropertyBlock();
        _renderer = GetComponentInChildren<Renderer>();
        _radius = GameConfigs.Instance.PixelSize * GameConfigs.Instance.SmallBallScaleMultiplier;
        _skin = GameConfigs.Instance.Skin;
        _maxHitsPerFrame = GameConfigs.Instance.MaxHitsPerFrame;
        _retargetInterval = GameConfigs.Instance.RetargetInterval;
        _postBounceMaxDeg = GameConfigs.Instance.PostBounceMaxDeg;
        IsInitialized = true;
    }

    public void AssignPool(IObjectPool<Ball> pool) => _pool = pool;

    private void Release()
    {
        _canMove = false;
        _pool.Release(this);
    }

    public void Prepare(int colorID)
    {
        _colorID = colorID;

        _materialPropertyBlock.SetColor("_BaseColor", GameManager.Instance.GameplayController.LevelColorsInOrder[_colorID]);

        if (_renderer != null)
            _renderer.SetPropertyBlock(_materialPropertyBlock);
    }

    public void BallStartDirection(Vector3 startDirection)
    {
        _currentDirection = NormalizeDir(startDirection);
    }

    public void StartMove() => _canMove = true;

    private void Update()
    {
        if (!_canMove) return;

        float dt = Time.deltaTime;
        float speed = GameConfigs.Instance.SmallBallSpeed;
        MoveAndBounce(dt, speed);
    }

    private void MoveAndBounce(float dt, float speed)
    {
        Vector3 pos = transform.position;
        Vector3 dir = NormalizeDir(_currentDirection);
        float remaining = speed * dt;

        for (int i = 0; i < _maxHitsPerFrame && remaining > 0f; i++)
        {
            float castDist = remaining + _skin;

            if (Physics.SphereCast(pos, _radius, dir, out RaycastHit hit, castDist, _collisionMask, QueryTriggerInteraction.Collide))
            {
                float travel = Mathf.Max(0f, hit.distance - _skin);
                pos += dir * travel;
                remaining -= travel;

                if (hit.collider.TryGetComponent<PixelPiece>(out var pixel))
                {
                    if (!pixel.IsCleared && pixel.ColorID == _colorID)
                    {
                        pixel.ClearWithScaleOut();
                        transform.position = pos;
                        Release();
                        return;
                    }
                }

                dir = Vector3.Reflect(dir, hit.normal);
                dir = TinyJitter(dir, 2f);
                dir = NormalizeDir(dir);

                if (Time.time >= _nextRetargetTime || _target == null || _target.IsCleared || !_target.gameObject.activeInHierarchy)
                    AcquireTarget(pos);

                dir = SteerAfterBounce(dir, pos);

                pos += hit.normal * _skin;
                pos += dir * _skin;
                remaining = Mathf.Max(0f, remaining - (2f * _skin));
            }
            else
            {
                pos += dir * remaining;
                remaining = 0f;
            }
        }

        pos.y = 0;
        transform.position = pos;
        _currentDirection = dir;
    }

    private Vector3 TinyJitter(Vector3 v, float degrees)
    {
        float a = Random.Range(-degrees, degrees);
        return Quaternion.AngleAxis(a, Vector3.up) * v;
    }

    private void AcquireTarget(Vector3 fromPos)
    {
        _target = GameManager.Instance.GameplayController.GetNearestPixel(_colorID, fromPos);
        _nextRetargetTime = Time.time + _retargetInterval;
    }

    private Vector3 SteerAfterBounce(Vector3 dir, Vector3 pos)
    {
        if (_target == null || _target.IsCleared || !_target.gameObject.activeInHierarchy)
            return dir;

        Vector3 to = _target.transform.position - pos;
        to.y = 0f;
        if (to.sqrMagnitude < 1e-6f) return dir;

        Vector3 desired = to.normalized;

        if (_postBounceMaxDeg > 0f)
        {
            float maxRad = _postBounceMaxDeg * Mathf.Deg2Rad;
            dir = Vector3.RotateTowards(dir, desired, maxRad, 0f);
        }

        return NormalizeDir(dir);
    }

    private Vector3 NormalizeDir(Vector3 d)
    {
        d.y = 0f;
        if (d.sqrMagnitude < 1e-8f)
            d = Vector3.forward;

        return d.normalized;
    }
}
using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using Utilities.EventBus;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

public class HoleObject : MonoBehaviour
{
    [SerializeField] private Transform _ballJumpTransform;
    [SerializeField] private Transform _ballDropTransform;
    [SerializeField] private Ball _ballPrefab;
    public bool IsInitialized { get; private set; }

    private ObjectPool<Ball> _ballPool;
    private int _activeBallCount;
    private bool _isFailed;
    public event Action<Ball,int> OnBallCreated;
    public event Action<Ball,int> OnBallReleased;

    public void Initialize()
    {
        _ballPool = new ObjectPool<Ball>(OnCreateBall, OnGetBall, OnReleaseBall, OnDestroyBall);
        IsInitialized = true;
    }

    private void OnDestroy()
    {
        OnBallCreated = null;
    }

    public void OnBallJump(BigBall bigBall)
    {
        float jumpDuration = GameConfigs.Instance.BigBallToHoleJumpDuration;
        float jumpPower = 1f;
        int jumpCount = 1;

        Sequence seq = DOTween.Sequence();
        seq.SetLink(bigBall.gameObject);
        seq.Append(bigBall.transform.DOJump(_ballJumpTransform.position, jumpPower, jumpCount, jumpDuration).SetEase(Ease.Linear));
        seq.Append(bigBall.transform.DOMove(_ballDropTransform.position, jumpDuration).SetEase(Ease.Linear));
        seq.Insert(jumpDuration * 0.5f, bigBall.transform.DOScale(0f, jumpDuration).SetEase(Ease.Linear));
        seq.OnComplete(() =>
        {
            bigBall.OnDropCompleted();
            CreateSmallBalls(bigBall);
        });
    }

    private void CreateSmallBalls(BigBall bigBall)
    {
        for (int i = 0; i < bigBall.Data.capacity; i++)
        {
            Ball ball = _ballPool.Get();
            ball.transform.localScale = Vector3.zero;
            Vector2 randomOffsetV2 = Random.insideUnitCircle;
            Vector3 randomOffset = new Vector3(randomOffsetV2.x, 0, randomOffsetV2.y);
            ball.transform.position = _ballJumpTransform.position + (randomOffset * GameConfigs.Instance.HoleRadius * 0.75f);
            ball.Prepare(bigBall.Data.colorID);
            
            OnBallCreated?.Invoke(ball,_activeBallCount);
            _activeBallCount++;
            if (_activeBallCount >= GameConfigs.Instance.MaxBallCountInPixelArea)
            {
                Debug.Log("FAIL");
                Debug.Break();
                break;
            }
        }
    }

    #region BALL POOL

    private void OnDestroyBall(Ball ball)
    {
        Destroy(ball.gameObject);
    }

    private Ball OnCreateBall()
    {
        Ball ball = Instantiate(_ballPrefab);
        ball.Initialize();
        ball.AssignPool(_ballPool);
        return ball;
    }

    private void OnGetBall(Ball ball)
    {
        ball.gameObject.SetActive(true);
    }

    private void OnReleaseBall(Ball ball)
    {
        _activeBallCount--;
        ball.gameObject.SetActive(false);
        OnBallReleased?.Invoke(ball,_activeBallCount);
    }

    #endregion
}

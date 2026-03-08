using DG.Tweening;
using UnityEngine;

public class HoleObject : MonoBehaviour
{
    [SerializeField] private Transform _ballJumpTransform;
    [SerializeField] private Transform _ballDropTransform;

    public void OnBallJump(BigBall bigBall)
    {
        float jumpDuration = 0.5f;
        Sequence seq = DOTween.Sequence();
        seq.Append(bigBall.transform.DOJump(_ballJumpTransform.position, 1f, 1, jumpDuration).SetEase(Ease.Linear));
        seq.Append(bigBall.transform.DOMove(_ballDropTransform.position, jumpDuration).SetEase(Ease.Linear));
        seq.Insert(jumpDuration , bigBall.transform.DOScale(0f, jumpDuration).SetEase(Ease.Linear));
        seq.OnComplete(bigBall.OnDropCompleted);
    }
}
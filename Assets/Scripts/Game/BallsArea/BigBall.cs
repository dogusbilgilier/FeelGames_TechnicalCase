using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class BigBall : MonoBehaviour
{
    [Title("References")]
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private TextMeshPro _capacityText;
    [SerializeField] private Collider _collider;
    public bool IsInitialized { get; private set; }

    private BigBallData _bigBallData;
    public BigBallData Data => _bigBallData;
    public bool IsNext { get; private set; }
    public int LaneIndex { get; private set; }
    public event Action<BigBall> OnJumpRequested;
    public int LaneRowIndex { get; private set; }

    private MaterialPropertyBlock _materialPropertyBlock;
    private MaterialPropertyBlock MaterialPropertyBlock => _materialPropertyBlock ??= new MaterialPropertyBlock();
    private LinkObject _linkObject;

    public void Initialize(BigBallData bigBallData, int laneIndex)
    {
        LaneIndex = laneIndex;
        _bigBallData = bigBallData;
        _capacityText.alpha = 0.2f;
        SetVisuals();
        IsInitialized = true;
    }

    public void SetLinkObject(LinkObject linkObject)
    {
        _linkObject = linkObject;
    }

    public void BreakLink()
    {
        _linkObject.BreakLink();
    }

    private void OnDestroy()
    {
        OnJumpRequested = null;
    }

    private void SetVisuals()
    {
        Color color = GameManager.Instance.GameplayController.LevelColorsInOrder[Data.colorID];
        MaterialPropertyBlock.SetColor("_BaseColor", color);
        _meshRenderer.SetPropertyBlock(MaterialPropertyBlock);
        _capacityText.SetText(Data.capacity.ToString());
    }

    public void SetAsNext()
    {
        _capacityText.alpha = 1f;
        IsNext = true;
    }

    private void OnMouseDown()
    {
        if (CanJumpToHole())
        {
            OnJumpRequested?.Invoke(this);
        }
    }

    public bool CanJumpToHole()
    {
        return IsNext;
    }

    public void OnDropCompleted()
    {
        gameObject.SetActive(false);
    }

    public void OnDropStart()
    {
        _collider.enabled = false;
        _capacityText.transform.DOScale(0f, 0.5f).SetEase(Ease.Linear).SetLink(_capacityText.gameObject);
    }

    public void SetIndexInLane(int index)
    {
        LaneRowIndex = index;
    }
}
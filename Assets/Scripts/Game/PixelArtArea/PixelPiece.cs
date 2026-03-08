using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class PixelPiece : MonoBehaviour
{
    [Title("References")]
    [SerializeField] MeshRenderer _meshRenderer;
    private MaterialPropertyBlock _materialPropertyBlock;
    private MaterialPropertyBlock MaterialPropertyBlock => _materialPropertyBlock ??= new MaterialPropertyBlock();
    public bool IsInitialized { get; private set; }
    public PixelPieceData Data { get; private set; }
    public bool IsCleared { get; set; }
    public int ColorID { get; private set; }

    public event Action<PixelPiece> OnPixelPieceCleared; 

    public void Initialize(PixelPieceData data)
    {
        Data = data;
        ColorID = data.colorID;
        UpdateVisuals();
        IsInitialized = true;
    }

    private void OnDestroy()
    {
        OnPixelPieceCleared = null;
    }

    private void UpdateVisuals()
    {
        Color color = GameManager.Instance.GameplayController.LevelColorsInOrder[Data.colorID];
        MaterialPropertyBlock.SetColor("_BaseColor", color);
        _meshRenderer.SetPropertyBlock(MaterialPropertyBlock);
    }


    public void ClearWithScaleOut()
    {
        if (IsCleared)
            return;
        OnPixelPieceCleared?.Invoke(this);
        IsCleared = true;
        transform.DOScale(0f, 0.1f).SetEase(Ease.Linear).SetLink(this.gameObject).OnComplete(() => { gameObject.SetActive(false); });
    }
}
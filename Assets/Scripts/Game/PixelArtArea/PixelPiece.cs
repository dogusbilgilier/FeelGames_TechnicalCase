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

    public void Initialize(PixelPieceData data)
    {
        Data = data;
        UpdateVisuals();
        IsInitialized = true;
    }

    public void SetData(PixelPieceData data, Color? color = null)
    {
        Data = data;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        Color color = GameManager.Instance.GameplayController.LevelColorsInOrder[Data.colorID];
        MaterialPropertyBlock.SetColor("_BaseColor", color);
        _meshRenderer.SetPropertyBlock(MaterialPropertyBlock);
    }
}
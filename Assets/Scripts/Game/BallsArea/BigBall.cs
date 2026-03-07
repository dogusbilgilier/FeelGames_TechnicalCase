using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class BigBall : MonoBehaviour
{
    [Title("References")]
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private TextMeshPro _capacityText;

    public bool IsInitialized { get; private set; }

    private BigBallData _bigBallData;
    public BigBallData Data => _bigBallData;

    private MaterialPropertyBlock _materialPropertyBlock;
    private MaterialPropertyBlock MaterialPropertyBlock => _materialPropertyBlock ??= new MaterialPropertyBlock();


    public void Initialize(BigBallData bigBallData)
    {
        _bigBallData = bigBallData;
        SetVisuals();
        IsInitialized = true;
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
    }
}
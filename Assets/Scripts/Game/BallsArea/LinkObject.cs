using System;
using UnityEngine;

public class LinkObject : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;

    private BigBall _bigBall1;
    private BigBall _bigBall2;

    private bool _isLinkActive;

    public void SetLink(BigBall ball1, BigBall ball2)
    {
        _bigBall1 = ball1;
        _bigBall2 = ball2;
        MaterialPropertyBlock block1 = new MaterialPropertyBlock();
        MaterialPropertyBlock block2 = new MaterialPropertyBlock();
        block1.SetColor("_BaseColor", GameManager.Instance.GameplayController.LevelColorsInOrder[ball1.Data.colorID]);
        block2.SetColor("_BaseColor", GameManager.Instance.GameplayController.LevelColorsInOrder[ball2.Data.colorID]);
        _meshRenderer.SetPropertyBlock(block1, 0);
        _meshRenderer.SetPropertyBlock(block2, 1);
        _isLinkActive = true;
    }

    public void BreakLink()
    {
        _isLinkActive = false;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_isLinkActive)
            return;

        transform.position = (_bigBall2.transform.position + _bigBall1.transform.position) / 2f + Vector3.up * 0.5f;
        float distance = Vector3.Distance(_bigBall1.transform.position, _bigBall2.transform.position);
        Vector3 scale = new Vector3(0.5f, 0.5f, distance);
        transform.localScale = scale;
        transform.forward = ((_bigBall1.transform.position) - _bigBall2.transform.position).normalized;
    }
}
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private UnityEngine.Rendering.Universal.DecalProjector _decalRenderer;

    public void Show() { if (_decalRenderer) _decalRenderer.enabled = true; }
    public void Hide() { if (_decalRenderer) _decalRenderer.enabled = false; }

}
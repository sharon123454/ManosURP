using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private UnityEngine.Rendering.Universal.DecalProjector _decalRenderer;

    public void Show(Material material)
    {
        if (_decalRenderer)
        {
            _decalRenderer.enabled = true;
            _decalRenderer.material = material;
        }
    }
    public void Hide()
    {
        if (_decalRenderer)
        {
            _decalRenderer.enabled = false;
        }
    }

}
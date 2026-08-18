using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[DisallowMultipleComponent]
public sealed class RawImageRenderTextureAutoResizer : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;

    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        Cache();
    }

    private void OnValidate()
    {
        Cache();
    }

    private void OnEnable()
    {
        Cache();

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            ResizeRenderTextureToCurrentPixelSize();
        }
#endif
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Application.isPlaying)
        {
            return;
        }

        ResizeRenderTextureToCurrentPixelSize();
    }
#endif

    private void Cache()
    {
        if (rawImage == null)
        {
            rawImage = GetComponent<RawImage>();
        }

        if (rawImage == null)
        {
            return;
        }

        rectTransform = rawImage.rectTransform;
        canvas = rawImage.canvas;
    }

    /// <summary>
    /// Resize RenderTexture to exact pixel size of RawImage
    /// </summary>
    public void ResizeRenderTextureToCurrentPixelSize()
    {
        if (rawImage == null)
        {
            return;
        }

        RenderTexture rt = rawImage.texture as RenderTexture;
        if (rt == null)
        {
            return;
        }

        Vector2 pixelSize = GetPixelSize();
        int width = Mathf.Max(1, Mathf.RoundToInt(pixelSize.x));
        int height = Mathf.Max(1, Mathf.RoundToInt(pixelSize.y));

        if (rt.width == width && rt.height == height)
        {
            return;
        }

        rt.Release();
        rt.width = width;
        rt.height = height;
        rt.Create();
    }

    private Vector2 GetPixelSize()
    {
        Rect rect = rectTransform.rect;

        float scaleFactor = 1f;
        if (canvas != null)
        {
            scaleFactor = canvas.scaleFactor;
        }

        return rect.size * scaleFactor;
    }
}
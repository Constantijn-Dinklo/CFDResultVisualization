using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VOConfig
{
    private static UnityVolumeRendering.RenderMode renderMode = UnityVolumeRendering.RenderMode.DirectVolumeRendering;

    public static float Isosurface1ValueOffset = 0.01f;

    public static void setRenderMode(UnityVolumeRendering.RenderMode newMode)
    {
        renderMode = newMode;
    }

    public static UnityVolumeRendering.RenderMode getRenderMode()
    {
        return renderMode;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline
{
    CameraRenderer renderer = new CameraRenderer();
    //구현해야 하는 메소드
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        //각 카메라를 다른 방식(top-view, forward view or forward render, diferred render)으로 사용할 수 있으나, 지금은 그냥 간단하게 유지
        foreach(Camera cam in cameras)
        {
            renderer.Render(context, cam);
        }
    }

}

using UnityEngine;
using UnityEngine.Rendering;

//render pipeline의 카메라 렌더링 역할 수행 클래스
public class CameraRenderer
{
    ScriptableRenderContext context;

    Camera camera;

    public void Render(ScriptableRenderContext context, Camera camera)
    {
        this.context = context;
        this.camera = camera;

        //이제 실제로 그리도록 해야 함. 간단히 메소드 하나 분리
        DrawVisibleGeometry();

        //실제로 그려지는 것은 context queue의 작업이 submit 되었을 때
        Submit();
    }

    void DrawVisibleGeometry()
    {
        context.DrawSkybox(camera);
    }

    void Submit()
    {
        context.Submit();
    }
}
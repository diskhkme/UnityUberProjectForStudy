using UnityEngine;
using UnityEngine.Rendering;

//render pipeline의 카메라 렌더링 역할 수행 클래스
public class CameraRenderer
{
    ScriptableRenderContext context;

    Camera camera;

    const string bufferName = "Render Camera";
    CommandBuffer buffer = new CommandBuffer { name = bufferName };

    public void Render(ScriptableRenderContext context, Camera camera)
    {
        this.context = context;
        this.camera = camera;

        Setup();

        //이제 실제로 그리도록 해야 함. 간단히 메소드 하나 분리
        DrawVisibleGeometry();

        //실제로 그려지는 것은 context queue의 작업이 submit 되었을 때
        Submit();
    }

    void Setup()
    {
        buffer.BeginSample(bufferName);
        ExecuteBuffer();
        context.SetupCameraProperties(camera); //MVP matrix등 카메라 정보를 context에 적용
    }

    void DrawVisibleGeometry()
    {
        context.DrawSkybox(camera);
    }

    void Submit()
    {
        buffer.EndSample(bufferName);
        ExecuteBuffer();
        context.Submit();
    }

    void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
}
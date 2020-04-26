using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering; //RP 구현에 필요

[CreateAssetMenu(menuName = "Rendering/Custom Render Pipeline")]
public class CustomRenderPipelineAsset : RenderPipelineAsset //RenderPipelineAsset 상속
{
    //이 클래스의 역할은 render pipeline의 세팅을 저장하는 것일 뿐, 아래 메소드 오버라이딩 함으로써
    protected override RenderPipeline CreatePipeline()
    {
        return new CustomRenderPipeline();
    }
}

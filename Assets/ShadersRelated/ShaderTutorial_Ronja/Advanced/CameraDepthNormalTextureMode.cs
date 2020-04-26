using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraDepthNormalTextureMode : MonoBehaviour
{
    [SerializeField] Material postprocessMaterial;

    Camera cam;
    

    private void Start()
    {
        //get the camera and tell it to render a depthnormals texture
        cam = GetComponent<Camera>();
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.DepthNormals;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //view space normal을 world space normal로 변경 계산을 위해서 데이터 넘겨야 함
        Matrix4x4 viewToWorld = cam.cameraToWorldMatrix;
        postprocessMaterial.SetMatrix("_viewToWorld", viewToWorld);

        Graphics.Blit(source, destination, postprocessMaterial);

    }
}

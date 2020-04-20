using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ReplacementShaderEffect : MonoBehaviour
{
    public Shader ReplacementShader;
    public Color OverDrawColor;

    void OnValidate()
    {
        //Material이 아닌 상태의 Shader에 값을 할당하는 방법
        Shader.SetGlobalColor("_OverDrawColor", OverDrawColor);
    }

    void OnEnable()
    {
        if (ReplacementShader != null)
            GetComponent<Camera>().SetReplacementShader(ReplacementShader, ""); //두 번째 인자가 비어있으면, 처음 있는 렌더 타입에 맞는 셰이더 사용
    }

    void OnDisable()
    {
        GetComponent<Camera>().ResetReplacementShader();
    }
}
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CatlikeRendering/FirstShader"
{
	Properties
	{
		_Tint("Tint", Color) = (1, 1, 1, 1)
		_MainTex("Texture", 2D) = "white" {}
	}
    SubShader
	{
		Pass {
			CGPROGRAM

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			#include "UnityCG.cginc"

			struct VertexData {
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Interpolators {
				float4 position : SV_POSITION;
				//float3 localPosition : TEXCOORD0
				float2 uv  : TEXCOORD0;
			};

			float4 _Tint;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			//POSITION means vertex position (object space)
			//out으로 출력 데이터임을 명시
			//float4 MyVertexProgram(float4 position : POSITION, out float3 localPosition : TEXCOORD0) : SV_POSITION { //SystemValue, Vertex position. 어떤 값이 출력으로 나오는지를 indicate
			//Interpolators MyVertexProgram(float4 position : POSITION)
			Interpolators MyVertexProgram(VertexData v)
			{
				Interpolators i;
				i.position = UnityObjectToClipPos(v.position);
				//i.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw; //tiling & offset 적용 방법
				i.uv = TRANSFORM_TEX(v.uv, _MainTex); //or this
				return i;
			}

			//float4 MyFragmentProgram(float4 position : SV_POSITION, float3 localPosition : TEXCOORD0) : SV_TARGET { //system value target(frame buffer)
			float4 MyFragmentProgram(Interpolators i) : SV_TARGET
			{
				//return float4(i.uv,1,1);
				return tex2D(_MainTex,i.uv) * _Tint;
			}

			ENDCG
		}
	}
}

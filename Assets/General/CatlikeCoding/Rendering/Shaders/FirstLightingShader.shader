// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CatlikeRendering/First Lighting Shader"
{
	Properties
	{
		_Tint("Tint", Color) = (1, 1, 1, 1)
		_MainTex("Albedo", 2D) = "white" {}
		//_SpecularTint("Specular", Color) = (0.5, 0.5, 0.5)
		[Gamma] _Metallic("Metallic", Range(0,1)) = 0
		_Smoothness("Smoothness", Range(0,1)) = 0.5
	}
    SubShader
	{
		Pass {
			Tags{"LightMode" = "ForwardBase"}

			CGPROGRAM

			#pragma target 3.0

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			//#include "UnityCG.cginc"
			//#include "UnityStandardBRDF.cginc"
			//#include "UnityStandardUtils.cginc"
			#include "UnityPBSLighting.cginc"

			struct VertexData {
				float4 position : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct Interpolators {
				float4 position : SV_POSITION;
				float2 uv  : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
			};

			float4 _Tint;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			//float4 _SpecularTint;
			float _Metallic;
			float _Smoothness;

			Interpolators MyVertexProgram(VertexData v)
			{
				Interpolators i;
				i.uv = TRANSFORM_TEX(v.uv, _MainTex);
				i.position = UnityObjectToClipPos(v.position);
				i.worldPos = mul(unity_ObjectToWorld, v.position);
				//i.normal = v.normal;

				//World space normal
				i.normal = mul(transpose((float3x3)unity_WorldToObject), v.normal); //upper 3x3
				i.normal = normalize(i.normal);

				//or, in short
				//i.normal = UnityObjectToWorldNormal(v.normal);
				
				return i;
			}

			//float4 MyFragmentProgram(Interpolators i) : SV_TARGET
			//{
			//	i.normal = normalize(i.normal); // normal이 짧아지는 문제 해결하기 위해 다시 normalization 필요.
			//	//return float4(i.normal * 0.5 + 0.5,1);

			//	//Simple Lighting
			//	//return dot(float3(0, 1, 0), i.normal);

			//	//값을 0~1 사이로 한정
			//	//return saturate(dot(float3(0, 1, 0), i.normal));
			//	//같은 기능을 Unity제공 내장함수로 사용(in UnityStandardBRDF.cginc
			//	//return DotClamped(float3(0, 1, 0), i.normal);

			//	//Unity에서 정의된 빛의 방향 벡터 및 색상 등을 가져올 수 있게 되어있음.
			//	float3 lightDir = _WorldSpaceLightPos0.xyz;
			//	//마찬가지로 카메라 위치 또한 가져올 수 있음
			//	float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
			//	float3 lightColor = _LightColor0.rgb;
			//	float3 albedo = tex2D(_MainTex, i.uv).rgb * _Tint.rgb; //Albedo : color of diffuse reflectivity
			//	//albedo *= 1 - _SpecularTint.rgb; // Energy Conservation, 빛이 1 이상의 영향을 주지 않도록 조정 (채널별)
			//	//albedo *= 1 - max(_SpecularTint.r, max(_SpecularTint.g, _SpecularTint.b)); //전체
			//	//albedo = EnergyConservationBetweenDiffuseAndSpecular(albedo, _SpecularTint.rgb, oneMinusReflectivity); // 내장함수
			//	float3 specularTint = albedo * _Metallic;
			//	float oneMinusReflectivity;
			//	albedo = DiffuseAndSpecularFromMetallic(albedo, _Metallic, specularTint, oneMinusReflectivity);

			//	//float3 diffuse = albedo * lightColor * DotClamped(lightDir, i.normal);

			//	////float3 reflectionDir = reflect(-lightDir, i.normal); //Blinn
			//	//float3 halfVector = normalize(lightDir + viewDir);
			//	//float3 specular = specularTint * lightColor * pow(DotClamped(halfVector, i.normal), _Smoothness * 100);

			//	//return float4(diffuse, 1);
			//	//return float4(specular, 1);
			//	//return float4(diffuse + specular, 1);

			//	return UNITY_BRDF_PBS(
			//		albedo, specularTint,
			//		oneMinusReflectivity, _Smoothness,
			//		i.normal, viewDir
			//	);
			//}

			float4 MyFragmentProgram(Interpolators i) : SV_TARGET{
				i.normal = normalize(i.normal);
				float3 lightDir = _WorldSpaceLightPos0.xyz;
				float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

				float3 lightColor = _LightColor0.rgb;
				float3 albedo = tex2D(_MainTex, i.uv).rgb * _Tint.rgb;

				float3 specularTint;
				float oneMinusReflectivity;
				albedo = DiffuseAndSpecularFromMetallic(
					albedo, _Metallic, specularTint, oneMinusReflectivity
				);

				UnityLight light;
				light.color = lightColor;
				light.dir = lightDir;
				light.ndotl = DotClamped(i.normal, lightDir);
				UnityIndirect indirectLight;
				indirectLight.diffuse = 0;
				indirectLight.specular = 0;

				return UNITY_BRDF_PBS(
					albedo, specularTint,
					oneMinusReflectivity, _Smoothness,
					i.normal, viewDir,
					light, indirectLight
				);
			}

			ENDCG
		}
	}
}

Shader "Tutorial/15_VertexManipulation"
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (0,0,0,1)
		[HDR] _Emission("Emission", Color) = (0,0,0,1) //HDR 붙이면 1 이상의 값 설정 가능
		_Amplitude("Wave Size", Range(0,1)) = 0.4
		_Frequency("Wave Freqency", Range(1, 8)) = 2
		_AnimationSpeed("Animation Speed", Range(0,5)) = 1
	}
    SubShader
    {
		Tags{ "Rendertype"="Opaque" "Queue"="Geometry" }

		CGPROGRAM //HLSL 코드를 작성하겠다는 의미, ENDCG와 한쌍

		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow// vert라는 vertex shader 사용할 것이라고 명시, 
				  //addshadow는 vert에서 변경된 vertex position으로 shadow pass 계산을 하라는 의미. 없으면 여기서 변경한 vertex와 상관없이 원래 vertex위치로 shadow가 계산됨.
		#pragma target 3.0

		half4 _Color;
		sampler2D _MainTex;
		half3 _Emission; 
		float _Amplitude;
		float _Frequency;
		float _AnimationSpeed;

		struct Input // Surface Shader에서 사용할 새로운 Input이라는 구조체
		{
			float2 uv_MainTex; //uv_로 시작하는 텍스처 uv정보
		};

		// 이번에는 surface shader에서 vertex shader도 직접 만들어서 사용. 입출력 데이터는 appdata_full type
		void vert(inout appdata_full data)
		{
			float4 modifiedPos = data.vertex;
			modifiedPos.y += sin(data.vertex.x * _Frequency + _Time.y * _AnimationSpeed) * _Amplitude; //단순히 포지션만 바꾸면 이상함. normal이 그대로이기 때문
															//자동 변화 애니메이션을 위해 _Time을 사용. y의 값이 초 정보

			//normal을 새로 계산하기 위해 tangent space 정보 사용. data.tangent에 tangent vector가 포함되어 있음
			float3 posPlusTangent = data.vertex + data.tangent * 0.01;
			posPlusTangent.y += sin(posPlusTangent.x * _Frequency + _Time.y * _AnimationSpeed) * _Amplitude;

			float3 bitangent = cross(data.normal, data.tangent);
			float3 posPlusBitangent = data.vertex + bitangent * 0.01;
			posPlusBitangent.y += sin(posPlusBitangent.x * _Frequency + _Time.y * _AnimationSpeed) * _Amplitude;

			float3 modifiedTangent = posPlusTangent - modifiedPos;
			float3 modifiedBitangent = posPlusBitangent - modifiedPos;

			float3 modifiedNormal = cross(modifiedTangent, modifiedBitangent);
			data.normal = normalize(modifiedNormal);
			data.vertex = modifiedPos;
		}

		void surf (Input i, inout SurfaceOutputStandard o) { // o를 custum lighting 함수로 넘김
			fixed4 col = tex2D(_MainTex, i.uv_MainTex);
			col *= _Color; //Tint Color 적용
			o.Albedo = col.rgb;
			
		}

		

		ENDCG
        
    }
	FallBack "Standard" //fallback을 넣어주어야 다른 pass(shadow)등 사용 가능(?)
}

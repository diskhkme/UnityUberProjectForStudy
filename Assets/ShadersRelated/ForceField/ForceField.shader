//https://www.youtube.com/watch?v=NiOGWZXBg4Y&t=297s, surface shader로 구현.

Shader "ForceField/ForceField"
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Tint", Color) = (0,0,0,1)
		_GradientOffset("Gradient Offset", float) = 0
		[HDR] _EmissionColor("Emission Color", Color) = (0,0,0,1)
		_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
		[PowerSlider(4)] _FresnelExponent("Fresnel Exponent", Range(0.25, 4)) = 1
		_ForceGrid("ForceGrid", 2D) = "white" {}
		_FillIntensity("Fill Intensity", float) = 0.05
	}
	SubShader
	{
		ZWrite Off
		Tags{ "Rendertype" = "Transparent" "Queue" = "Transparent" }

		CGPROGRAM //HLSL 코드를 작성하겠다는 의미, ENDCG와 한쌍

		#pragma surface surf Standard fullforwardshadows alpha:blend
		#pragma target 3.0

		half4 _Color;
		sampler2D _MainTex;

		sampler2D _CameraDepthTexture;

		float _GradientOffset;
		half3 _EmissionColor;
		float3 _FresnelColor;
		float _FresnelExponent;
		sampler2D _ForceGrid;
		float4 _ForceGrid_ST;
		float _FillIntensity;

		struct Input
		{
			float2 uv_MainTex;
			float4 screenPos;
			float3 worldNormal; 
			float3 viewDir; 
			INTERNAL_DATA 
		};

		void surf(Input i, inout SurfaceOutputStandard o) { // o를 custum lighting 함수로 넘김
			fixed4 col = tex2D(_MainTex, i.uv_MainTex);
			col *= _Color; //Tint Color 적용
			o.Albedo = col.rgb;

			float3 normScreenPos = i.screenPos.xyz / i.screenPos.w; //!!!!여기 때문에 한참 헤멤...얘도 perspective division 해 주어야 함...

			//구의 각 vertex의 screen space position을 사용해, 해당 position의 depth texture에 저장된 depth 값을 가져옴 
			float depth = tex2D(_CameraDepthTexture, normScreenPos.xy);
			depth = Linear01Depth(depth); 
			depth = depth * _ProjectionParams.z;

			float vertDepth = normScreenPos.z;
			vertDepth = Linear01Depth(vertDepth);
			vertDepth = vertDepth * _ProjectionParams.z;
			vertDepth -= _GradientOffset;
			
			float depthDiff = (depth- vertDepth);
			depthDiff = smoothstep(0, 1, 1 - depthDiff);

			//float fresnel = dot(i.worldNormal, float3(0, 1, 0)); // y 단위벡터 기반의 fresnel 효과
			float fresnel = dot(i.worldNormal, i.viewDir); // 뷰 기반의 fresnel 효과
			//fresnel은 바라보는 방향과 수직일수록 빛나야 함. 1-fresnel 적용
			fresnel = saturate(1 - fresnel); //clamp와 같으나 어떤 GPU에서 약간 더 빠르다 함. 사용 이유는 dot에 의한 -1을 제거
			fresnel = pow(fresnel, _FresnelExponent); //fresnel 효과 제어 파라메터
			half3 fresnelColor = fresnel * _FresnelColor;
			
			i.uv_MainTex = TRANSFORM_TEX(i.uv_MainTex, _ForceGrid);
			i.uv_MainTex += float2(_Time.x, _Time.x);
			o.Alpha = depthDiff + fresnelColor * tex2D(_ForceGrid, i.uv_MainTex) + _FillIntensity; // 약간의 default fill color를 위해 0.1f 추가
			
			o.Emission = _EmissionColor;
		}



		ENDCG

	}
	FallBack "Standard" //fallback을 넣어주어야 다른 pass(shadow)등 사용 가능(?)
}

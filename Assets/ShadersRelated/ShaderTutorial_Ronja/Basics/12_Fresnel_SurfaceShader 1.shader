Shader "Tutorial/12_Fresnel_SurfaceShader"
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (0,0,0,1)
		_Smoothness("Smoothness", Range(0, 1)) = 0
		_Metallic("Metalness", Range(0, 1)) = 0
		[HDR] _Emission("Emission", Color) = (0,0,0,1) //HDR 붙이면 1 이상의 값 설정 가능
		_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
		[PowerSlider(4)] _FresnelExponent("Fresnel Exponent", Range(0.25, 4)) = 1
	}
    SubShader
    {
		Tags{ "Rendertype"="Opaque" "Queue"="Geometry" }

		CGPROGRAM //HLSL 코드를 작성하겠다는 의미, ENDCG와 한쌍

		#pragma surface surf Standard fullforwardshadows 
		#pragma target 3.0

		half4 _Color;
		sampler2D _MainTex;
		half _Smoothness; 
		half _Metallic;
		half3 _Emission; 
		float3 _FresnelColor;
		float _FresnelExponent;

		struct Input // Surface Shader에서 사용할 새로운 Input이라는 구조체
		{
			float2 uv_MainTex; //uv_로 시작하는 텍스처 uv정보
			float3 worldNormal; // world space normal
			float3 viewDir; //마찬가지고 그냥 사용 가능
			INTERNAL_DATA //Internal data 사용. 다른 internal data를 사용하지는 않지만, world space normal을 가져오기 위해서는 필요함.
		};

		void surf (Input i, inout SurfaceOutputStandard o) { // void 반환형의 surf 함수. Input은 위에 정의한 per-vertex data, o가 결과. 이 결과 데이터를 사용해 유니티에서 PBR 렌더링을 수행함
			fixed4 col = tex2D(_MainTex, i.uv_MainTex);
			col *= _Color; //Tint Color 적용
			o.Albedo = col.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			
			//float fresnel = dot(i.worldNormal, float3(0, 1, 0)); // y 단위벡터 기반의 fresnel 효과
			float fresnel = dot(i.worldNormal, i.viewDir); // 뷰 기반의 fresnel 효과
			//fresnel은 바라보는 방향과 수직일수록 빛나야 함. 1-fresnel 적용
			fresnel = saturate(1 - fresnel); //clamp와 같으나 어떤 GPU에서 약간 더 빠르다 함. 사용 이유는 dot에 의한 -1을 제거
			fresnel = pow(fresnel, _FresnelExponent); //fresnel 효과 제어 파라메터

			half3 fresnelColor = fresnel * _FresnelColor;
			o.Emission = _Emission + fresnelColor;

			//Albedo, Normal, Metallic 등에 대한 설명
			//https://www.ronja-tutorials.com/2018/03/30/simple-surface.html
		}

		ENDCG
        
    }
	FallBack "Standard" //fallback을 넣어주어야 다른 pass(shadow)등 사용 가능(?)
}

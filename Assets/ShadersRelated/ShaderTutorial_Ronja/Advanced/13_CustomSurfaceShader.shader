Shader "Tutorial/13_CustomSurfaceShader"
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (0,0,0,1)
		[HDR] _Emission("Emission", Color) = (0,0,0,1) //HDR 붙이면 1 이상의 값 설정 가능

		_Ramp("Toon Ramp", 2D) = "white" {}
	}
    SubShader
    {
		Tags{ "Rendertype"="Opaque" "Queue"="Geometry" }

		CGPROGRAM //HLSL 코드를 작성하겠다는 의미, ENDCG와 한쌍

		#pragma surface surf Custom fullforwardshadows // 이번에는 standard가 아닌 Custom
		#pragma target 3.0

		half4 _Color;
		sampler2D _MainTex;
		half3 _Emission; 
		sampler2D _Ramp;

		struct Input // Surface Shader에서 사용할 새로운 Input이라는 구조체
		{
			float2 uv_MainTex; //uv_로 시작하는 텍스처 uv정보
			float3 worldNormal; // world space normal
			float3 viewDir; //마찬가지고 그냥 사용 가능
			INTERNAL_DATA //Internal data 사용. 다른 internal data를 사용하지는 않지만, world space normal을 가져오기 위해서는 필요함.
		};

		void surf (Input i, inout SurfaceOutput o) { // o를 custum lighting 함수로 넘김
			fixed4 col = tex2D(_MainTex, i.uv_MainTex);
			col *= _Color; //Tint Color 적용
			o.Albedo = col.rgb;
			
		}

		//Custom lighting을 위한 함수 정의. Lighting으로 함수 이름이 시작해야 함. Surf()의 output을 인자로 받음
		//SurfaceOutputStandard가 아닌 이유는 metalness와 softness를 안쓸 것이기 때문. 써도 되는데 그러한경우 UnityPBSLighting.cginc를 추가해야 함.
		float4 LightingCustom(SurfaceOutput s, float3 lightDir, float atten) 
		{
			float towardsLight = dot(s.Normal, lightDir);
			towardsLight = towardsLight * 0.5 + 0.5; //-1~1 to 0~1

			float3 lightIntensity = tex2D(_Ramp, towardsLight).rgb; 

			float4 col;
			col.rgb = lightIntensity * s.Albedo * atten * _LightColor0.rgb; //atten light falloff and shadowcasting, albedo 색을 섞어줌
			col.a = s.Alpha;

			return col;
		}

		ENDCG
        
    }
	FallBack "Standard" //fallback을 넣어주어야 다른 pass(shadow)등 사용 가능(?)
}

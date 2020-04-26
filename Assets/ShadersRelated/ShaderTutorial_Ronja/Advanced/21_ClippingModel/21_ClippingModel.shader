Shader "Tutorial/21_ClippingModel"
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (0,0,0,1)
		_Smoothness("Smoothness", Range(0, 1)) = 0
		_Metallic("Metalness", Range(0, 1)) = 0
		[HDR] _Emission("Emission", Color) = (0,0,0,1) //HDR 붙이면 1 이상의 값 설정 가능

		[HDR]_CutoffColor("Cutoff Color", Color) = (1,0,0,0)
	}
    SubShader
    {
		Cull Off
		Tags{ "Rendertype"="Opaque" "Queue"="Geometry" }

		CGPROGRAM //HLSL 코드를 작성하겠다는 의미, ENDCG와 한쌍

		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		half4 _Color;
		sampler2D _MainTex;
		half _Smoothness;
		half _Metallic;
		half3 _Emission; 

		float4 _Plane; //Clip 평면 정의, xyz & distance
		float4 _CutoffColor;

		struct Input 
		{
			float2 uv_MainTex;
			float3 worldPos;
			float facing : VFACE; //삼각형(vertex?)이 카메라를 바라보는지 아닌지 정보를 담음
		};

		void surf (Input i, inout SurfaceOutputStandard o) { 
			float distance = dot(i.worldPos, _Plane.xyz);
			distance = distance + _Plane.w;
			clip(-distance);
			
			float facing = i.facing * 0.5 + 0.5;

			fixed4 col = tex2D(_MainTex, i.uv_MainTex);
			col *= _Color;
			o.Albedo = col.rgb * facing;
			o.Metallic = _Metallic * facing;
			o.Smoothness = _Smoothness * facing;
			o.Emission = lerp(_CutoffColor,_Emission,facing); //facing이면 Emission을, 아니면 CutoffColor 적용
		}

		ENDCG
        
    }
	FallBack "Standard" //fallback을 넣어주어야 다른 pass(shadow)등 사용 가능(?)
}

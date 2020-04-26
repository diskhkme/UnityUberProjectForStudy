Shader "Tutorial/22_Stencil_Buffer"
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (0,0,0,1)
		_Smoothness("Smoothness", Range(0, 1)) = 0
		_Metallic("Metalness", Range(0, 1)) = 0
		[HDR] _Emission("Emission", Color) = (0,0,0,1) 

		[IntRange] _StencilRef("Stencil Reference Value", Range(0,255)) = 0
	}
    SubShader
    {
		Tags{ "Rendertype"="Opaque" "Queue"="Geometry" }

		//stencil 버퍼 사용을 위한 코드
		Stencil
		{
			//Ref 0 //stencil로 사용할 값
			Ref [_StencilRef]
			Comp Equal //ref값과 같을(equal) 경우 오퍼레이션을 수행하겠다.
		}

		CGPROGRAM //HLSL 코드를 작성하겠다는 의미, ENDCG와 한쌍

		#pragma surface surf Standard fullforwardshadows //프로그램 정의.
		#pragma target 3.0

		half4 _Color;
		sampler2D _MainTex;
		half _Smoothness; // Suface shader에서는 half 데이터 타입 사용
		half _Metallic;
		half3 _Emission; //Emission은 알파값 없음

		struct Input // Surface Shader에서 사용할 새로운 Input이라는 구조체
		{
			float2 uv_MainTex; //uv_로 시작하는 텍스처 uv정보
		};

		void surf (Input i, inout SurfaceOutputStandard o) { // void 반환형의 surf 함수. Input은 위에 정의한 per-vertex data, o가 결과. 이 결과 데이터를 사용해 유니티에서 PBR 렌더링을 수행함
			fixed4 col = tex2D(_MainTex, i.uv_MainTex);
			col *= _Color; //Tint Color 적용
			o.Albedo = col.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Emission = _Emission;

			//Albedo, Normal, Metallic 등에 대한 설명
			//https://www.ronja-tutorials.com/2018/03/30/simple-surface.html
		}

		ENDCG
        
    }
	FallBack "Standard" //fallback을 넣어주어야 다른 pass(shadow)등 사용 가능(?)
}

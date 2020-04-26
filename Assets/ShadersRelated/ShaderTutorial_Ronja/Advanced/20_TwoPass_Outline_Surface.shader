Shader "Tutorial/20_TwoPass_Outline_Surface"
{
	Properties{
		_Color("Tint", Color) = (0, 0, 0, 1)
		_MainTex("Texture", 2D) = "white" {}
		_Smoothness("Smoothness", Range(0, 1)) = 0
		_Metallic("Metalness", Range(0, 1)) = 0
		[HDR] _Emission("Emission", color) = (0,0,0)

		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_OutlineThickness("Outline Thickness", Range(0,1)) = 0.1
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry"}

		CGPROGRAM
		
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;

		half _Smoothness;
		half _Metallic;
		half3 _Emission;

		//input struct which is automatically filled by unity
		struct Input {
			float2 uv_MainTex;
		};

		//the surface shader function which sets parameters the lighting function then uses
		void surf(Input i, inout SurfaceOutputStandard o) {
			//read albedo color from texture and apply tint
			fixed4 col = tex2D(_MainTex, i.uv_MainTex);
			col *= _Color;
			o.Albedo = col.rgb;
			//just apply the values for metalness, smoothness and emission
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Emission = _Emission;
		}
		ENDCG

		//-------------------------아웃라인을 그리기 위한 second pass--------------------------------
		//-------------------------이 두 번째 pass는 surf 함수 아님.그냥 하나의 pass일 뿐. surface shader에서 자동으로 생성하는 다른 pass들과 함께 하나의 추가 pass로 동작함
		Pass{
			Cull Front

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			fixed4 _OutlineColor;
			float _OutlineThickness;

			struct appdata {
				float4 vertex : POSITION;
				float4 normal : NORMAL;
			};

			struct v2f {
				float4 position : SV_POSITION;
			};

			v2f vert(appdata v) {
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex + normalize(v.normal) * _OutlineThickness);
				return o;
			}

			//the fragment shader
			fixed4 frag(v2f i) : SV_TARGET{
				return _OutlineColor;
			}

			ENDCG
		}
	}
	FallBack "Standard"
}
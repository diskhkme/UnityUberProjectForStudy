Shader "Tutorial/08_PlanarMapping"
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (0,0,0,1)
	}
    SubShader
    {
        //하나의 Subshader에서 패스가 순차적으로 그려짐
        Pass
        {
			//이 Pass를 어떻게 그릴 것인지
			Tags{ "Rendertype"="Opaque" "Queue"="Geometry" }

			CGPROGRAM //HLSL 코드를 작성하겠다는 의미, ENDCG와 한쌍

			#include "UnityCG.cginc" //유니티의 유틸리티 함수 include

			#pragma vertex vert //어떤 함수가 어떤 프로그램의 역할을 하는지 명시
			#pragma fragment frag

			struct appdata { 
				float4 vertex : POSITION; 
				//float2 uv : TEXCOORD0; // 이번에는 UV를 직접 조작할 것임
			};

			struct v2f {
				float4 position : SV_POSITION; 
				float2 uv : TEXCOORD0;
			};

			half4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST; 

			v2f vert(appdata v) { 
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);

				float4 worldPos = mul(unity_ObjectToWorld, v.vertex); //각 vertex의 world 좌표를 얻어오기
				o.uv = TRANSFORM_TEX(worldPos.xz,_MainTex);

				return o;
			}
			
			fixed4 frag(v2f i) : SV_TARGET{ 
				fixed4 col = tex2D(_MainTex, i.uv);
				col *= _Color; 
				return col;
			}

			ENDCG
        }
    }
}

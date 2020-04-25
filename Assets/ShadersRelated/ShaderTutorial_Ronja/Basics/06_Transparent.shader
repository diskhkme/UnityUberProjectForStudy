Shader "Tutorial/06_Transparent"
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (0,0,0,1)
	}
    SubShader
    {
		//---아래 태그와 정의를 Pass 안에 넣으면 그려지지 않음...왜일까?
		//투명한 물체는 rendertype과 renderqueue를 나중에 그리는 transparent로 분류해 주어야 함
		Tags{ "Rendertype" = "Transparent" "Queue" = "Transparent" }
		//알파 블렌딩 방법 정의
		Blend SrcAlpha OneMinusSrcAlpha
		//투명한 물체는 Z-buffering에 적용되지 않도록 설정
		ZWrite Off
        //하나의 Subshader에서 패스가 순차적으로 그려짐
        Pass
        {


			CGPROGRAM //HLSL 코드를 작성하겠다는 의미, ENDCG와 한쌍

			#include "UnityCG.cginc" //유니티의 유틸리티 함수 include

			#pragma vertex vert //어떤 함수가 어떤 프로그램의 역할을 하는지 명시
			#pragma fragment frag

			struct appdata { //Input data struct
				float4 vertex : POSITION; // :POSITION attribute를 통해 vertex 변수에 모델의 object space 좌표가 채워지도록 함
				float2 uv : TEXCOORD0; // :TEXCOORD0를 통해 uv 좌표가 채워지도록 함
			};

			struct v2f {
				float4 position : SV_POSITION; // :SV_POSITION attribute를 통해 vertex의 screen 좌표가 전달
				float2 uv : TEXCOORD0;
			};

			half4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST; //텍스처 이름 뒤에 _ST붙이면 offset과 tiling 파라메터 받아짐. 

			v2f vert(appdata v) { // appdata를 가지고 v2f를 만드는 프로그램 정의
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				//o.uv = v.uv;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex); // offset과 tiling 데이터 적용해서 UV 변환

				return o;
			}
			
			fixed4 frag(v2f i) : SV_TARGET{ // :SV_TARGET을 통해 이 함수가 최종적으로 스크린에 그려질 색을 결정한다는 것을 표시.
				fixed4 col = tex2D(_MainTex, i.uv);
				col *= _Color; //Tint Color 적용
				//return fixed4(i.uv.x, i.uv.y,0,1);
				return col;
			}

			ENDCG
        }
    }
}

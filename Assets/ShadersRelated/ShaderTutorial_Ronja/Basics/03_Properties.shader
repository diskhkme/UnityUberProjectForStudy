Shader "Tutorial/03_Properties"
{
	Properties{
		_Color ("Color", Color) = (0,0,0,1)
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

			struct appdata { //Input data struct
				float4 vertex : POSITION; // :POSITION attribute를 통해 vertex 변수에 모델의 object space 좌표가 입력되도록 함
			};

			struct v2f {
				float4 position : SV_POSITION; // :SV_POSITION attribute를 통해 vertex의 screen 좌표가 전달
			};

			v2f vert(appdata v) { // appdata를 가지고 v2f를 만드는 프로그램 정의
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				return o;
			}

			half4 _Color;

			fixed4 frag(v2f i) : SV_TARGET{ // :SV_TARGET을 통해 이 함수가 최종적으로 스크린에 그려질 색을 결정한다는 것을 표시.
				return _Color;
			}

			ENDCG
        }
    }
}

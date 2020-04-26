Shader "Tutorial/22_Stencil_BufferWrite"
{
	Properties{
		[IntRange] _StencilRef("Stencil Reference Value", Range(0,255)) = 0
	}
    SubShader
    {
		//실제로 그려지는 것은 stencil buffer를 참조해서 그리는 다른 셰이더이므로, 여기서는 queue를 하나 앞에 집어넣음
		Tags{ "Rendertype"="Opaque" "Queue"="Geometry-1" }

		Stencil{
			Ref[_StencilRef]
			Comp Always 
			Pass Replace //reference value를 stencil buffer에 써라
		}

		Pass{
			//don't draw color or depth
			Blend Zero One //이 셰이더에서 return한 값을 무시하라.
			ZWrite Off //이 셰이더에서는 Zbuffer write 안함

			CGPROGRAM
			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			struct appdata {
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 position : SV_POSITION;
			};

			v2f vert(appdata v) {
				v2f o;
				//calculate the position in clip space to render the object
				o.position = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET{
				return 0; //아무것도 하지 않음
			}

			ENDCG
		}
	}
        
   
}

Shader "ForceField/ForceFieldShader"
{
    Properties
    {
        
    }
	SubShader{
		// markers that specify that we don't need culling 
		// or comparing/writing to the depth buffer
		Cull Off
		ZWrite Off
		ZTest Always

		Pass{
			CGPROGRAM
			//include useful shader functions
			#include "UnityCG.cginc"

			//define vertex and fragment shader
			#pragma vertex vert
			#pragma fragment frag

			//the rendered screen so far
			sampler2D _MainTex;

			//the depth texture
			sampler2D _CameraDepthTexture;
			

			//the object data that's put into the vertex shader
			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			//the data that's used to generate fragments and can be read by the fragment shader
			struct v2f {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 screenPos : TEXCOORD1;
				float depth : Depth;
			};

			//the vertex shader
			v2f vert(appdata v) {
				v2f o;
				//convert the vertex positions from object space to clip space so they can be rendered
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.depth = -mul(UNITY_MATRIX_MV, v.vertex).z *_ProjectionParams.w;
				return o;
			}

			//the fragment shader
			fixed4 frag(v2f i) : SV_TARGET{
				//get depth from depth texture
				
				float bufferDepth = tex2D(_CameraDepthTexture, i.screenPos).r;
				//float depth = Linear01Depth(i.screenPos.z) * _ProjectionParams;

				////linear depth between camera and far clipping plane
				bufferDepth = Linear01Depth(bufferDepth);
				////depth as distance from camera in units 
				bufferDepth = bufferDepth * _ProjectionParams.z;

				fixed4 col;
				if (bufferDepth - i.depth> 0)
				{
					col = fixed4(1, 0, 0, 1);
				}
				else
				{
					col = fixed4(1, 1, 0, 1);
				}
				//col = fixed4(i.depth, i.depth, i.depth, 1);

				

				return col;
			}
		ENDCG
		}
	}
}

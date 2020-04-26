Shader "Tutorial/18_DepthNormalPostprocessing"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth --> Image Effect Post processing 특성상 당연한 것.
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 position : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

			//이번에는 카메라로부터 Depth와 normal을 생성하도록 하였음(CameraDepthNormalTextureMode.cs 참고)
			//DepthNormals Texture에서는 이미 depth가 linearize 되어 있음
			sampler2D _CameraDepthNormalsTexture;

			//script를 통해 넘겨준 view to world matrix
			uniform float4x4 _viewToWorld;


            fixed4 frag (v2f i) : SV_Target
            {
				float4 depthnormal = tex2D(_CameraDepthNormalsTexture, i.uv); //여기서 i.uv는 screen space uv임. object가 아니고
				
				float3 normal;
				float depth;
				DecodeDepthNormal(depthnormal, depth, normal); //depth와 normal로 값을 decode해주는 내장 함수

				depth = depth * _ProjectionParams.z;

				//return depth;

				//view space normal을 world space normal로 transform. translate는 상관없으므로 3x3만 사용
				normal = mul((float3x3)_viewToWorld, normal);
				//return float4(normal, 1);

				float up = dot(float3(0, 1, 0), normal);
				up = step(0.5, up);

				float4 source = tex2D(_MainTex, i.uv); //원래 렌더링된 이미지 색상
				float4 col = lerp(source, float4(0, 0, 1, 1), up);

				return col;
            }
            ENDCG
        }
    }
}

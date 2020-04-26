Shader "Tutorial/19_Outline_Using_DepthNormal"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
		_NormalMult("Normal Outline Multiplier", Range(0,4)) = 1
		_NormalBias("Normal Outline Bias", Range(1,4)) = 1
		_DepthMult("Depth Outline Multiplier", Range(0,4)) = 1
		_DepthBias("Depth Outline Bias", Range(1,4)) = 1
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
			//카메라 렌더링 텍스처의 크기를 얻어올 수 있는 변수, uv단위로 변환 해 주는 듯
			float4 _CameraDepthNormalsTexture_TexelSize;

			float _NormalMult;
			float _NormalBias;
			float _DepthMult;
			float _DepthBias;


			//주변 텍셀의 depth값과 비교하는 함수
			//float Compare(float baseDepth, float2 uv, float2 offset) {
			//기능 확장. inout으로 reference return과 같이 사용 가능
			void Compare(inout float depthOutline, inout float normalOutline,
				float baseDepth, float3 baseNormal, float2 uv, float2 offset)
			{
				//read neighbor pixel
				float4 neighborDepthnormal = tex2D(_CameraDepthNormalsTexture,
					uv + _CameraDepthNormalsTexture_TexelSize.xy * offset);
				float3 neighborNormal;
				float neighborDepth;
				DecodeDepthNormal(neighborDepthnormal, neighborDepth, neighborNormal);
				neighborDepth = neighborDepth * _ProjectionParams.z;

				float depthDifference = baseDepth - neighborDepth;
				depthOutline = depthOutline + depthDifference;

				//깊이값의 차이 뿐만 아니라 normal의 차이도 계산
				float3 normalDifference = 1-abs(dot(baseNormal,neighborNormal));
				normalDifference = normalDifference.r + normalDifference.g + normalDifference.b;
				normalOutline = normalOutline + normalDifference;

			}

            fixed4 frag (v2f i) : SV_Target
            {
				float4 depthnormal = tex2D(_CameraDepthNormalsTexture, i.uv); //여기서 i.uv는 screen space uv임. object가 아니고
				float3 normal;
				float depth;
				DecodeDepthNormal(depthnormal, depth, normal); //depth와 normal로 값을 decode해주는 내장 함수
				depth = depth * _ProjectionParams.z;

				float depthDifference = 0;
				float normalDifference = 0;

				Compare(depthDifference, normalDifference, depth, normal, i.uv, float2(1, 0));
				Compare(depthDifference, normalDifference, depth, normal, i.uv, float2(-1, 0));
				Compare(depthDifference, normalDifference, depth, normal, i.uv, float2(0, 1));
				Compare(depthDifference, normalDifference, depth, normal, i.uv, float2(0, -1));

				//차이를 강조한 후 제곱하는 변환
				depthDifference = depthDifference * _DepthMult;
				depthDifference = saturate(depthDifference);
				depthDifference = pow(depthDifference, _DepthBias);

				normalDifference = normalDifference * _NormalMult;
				normalDifference = saturate(normalDifference);
				normalDifference = pow(normalDifference, _NormalBias);

				//return normalDifference + depthDifference;

				//outline과 원래 렌더링된 이미지 합치기
				float outline = normalDifference + depthDifference;
				float4 sourceColor = tex2D(_MainTex, i.uv);
				float4 color = sourceColor * float4(1 - outline, 1 - outline, 1 - outline,0);

				return color;
            }
            ENDCG
        }
    }
}

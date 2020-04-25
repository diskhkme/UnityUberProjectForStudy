Shader "Tutorial/17_DepthPostprocessing"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
		[Header(Wave)]
		_WaveDistance("Distance from player", float) = 10
		_WaveTrail("Length of the trail", Range(0,5)) = 1
		_WaveColor("Color", Color) = (1,0,0,1)
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

			//카메라로부터 생성된 Depth Texture에 접근
			sampler2D _CameraDepthTexture;

			float _WaveDistance;
			float _WaveTrail;
			float4 _WaveColor;

            fixed4 frag (v2f i) : SV_Target
            {
                float depth = tex2D(_CameraDepthTexture, i.uv).r;
				depth = Linear01Depth(depth); //linearize depth
				depth = depth * _ProjectionParams.z; // linear 0~farclippingplane depth, unity unit 단위로 encoding 되었으므로 거의 다 흰색으로 보이는 상태

				float waveFront = step(depth, _WaveDistance); //첫 번째 번째 인자가 크면 1, 아니면 0을 반환하는 step 함수
				float waveTrail = smoothstep(_WaveDistance - _WaveTrail, _WaveDistance, depth); //첫 번째 인자보다 작으면 0, 두 번째 인자보다 크면 1, 그 사이면 lerp
				float wave = waveFront * waveTrail; //lerp값 빼고 나머지는 다 0으로 만듬
				//return wave;

				fixed4 source = tex2D(_MainTex, i.uv);
				if (depth >= _ProjectionParams.z) //skybox는 예외 처리
					return source;
				
				fixed4 col = lerp(source, _WaveColor, wave);

				return col;
            }
            ENDCG
        }
    }
}

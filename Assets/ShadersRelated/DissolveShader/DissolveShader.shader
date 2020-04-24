Shader "Dissolve/DissolveShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_DissolveTex("Dissolve", 2D) = "white" {}
		_Amount("Amount", Range(0,1)) = 0
		_Color("Diffuse", Color) = (1,1,1,1)
		_DissolveEdgeColor("DissolveEdgeColor",Color) = (1,0,0,1)
		_DissolveEdgeOffset("DissolveEdgeOffset", Range(0,0.3)) = 0
    }
    SubShader
    {
        // No culling or depth
        //Cull Off ZWrite Off ZTest Always
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }

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
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			sampler2D _DissolveTex;
			float _Amount;
			fixed4 _Color;
			fixed4 _DissolveEdgeColor;
			float _DissolveEdgeOffset;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color;
				fixed alpha = tex2D(_DissolveTex, i.uv).x;
				
				//isEdge
				if (alpha > _Amount + _DissolveEdgeOffset)
				{
					col.w = 1;
				}
				//isInside
				else if (alpha > _Amount - _DissolveEdgeOffset && alpha < _Amount + _DissolveEdgeOffset)
				{
					col = fixed4(_DissolveEdgeColor.rgb, 1);
				}
				else
				{
					col.w = 0;
				}
				
                return col;
            }
            ENDCG
        }
    }
}

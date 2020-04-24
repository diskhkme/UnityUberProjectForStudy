Shader "Dissolve/StandardDissolveShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_DissolveTex("Dissolve", 2D) = "white" {}
		_Amount("Amount", Range(0,1)) = 0
		_DissolveEdgeColor("DissolveEdgeColor",Color) = (1,0,0,1)
		_DissolveEdgeOffset("DissolveEdgeOffset", Range(0,0.3)) = 0
    }
    SubShader
    {
		//Cull Off
		Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:blend

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _DissolveTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		fixed4 _DissolveEdgeColor;
		float _DissolveEdgeOffset;
		float _Amount;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed alpha = tex2D(_DissolveTex, IN.uv_MainTex).x;

            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

			if (alpha > _Amount + _DissolveEdgeOffset)
			{
				o.Alpha = 1;
			}
			//isInside
			else if (alpha > _Amount - _DissolveEdgeOffset && alpha < _Amount + _DissolveEdgeOffset)
			{
				o.Alpha = 1;
				o.Emission = _DissolveEdgeColor;
			}
			else
			{
				o.Alpha = 0;
			}

            
        }
        ENDCG
    }
    FallBack "Diffuse"
}

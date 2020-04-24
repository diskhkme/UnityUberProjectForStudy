Shader "Hologram/HologramShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

		_HologramTex("Hologram Texture", 2D) = "white" {}
		_ScrollSpeed("Hologram Texture Y Offset", Range(0,10)) = 1

		_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
		[PowerSlider(4)] _FresnelExponent("Fresnel Exponent", Range(0.25, 4)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"}
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:blend

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
			float4 screenPos;
			float3 worldNormal;
			float3 viewDir;
			INTERNAL_DATA // Fresnel 계산을 위한 내부 데이터
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		float _ScrollSpeed;
		half3 _Emission;

		sampler2D _HologramTex;
		//Texture에 부여된 offset, tiling 값 가져오는 변수 ~~_ST
		float4 _HologramTex_ST;

		float3 _FresnelColor;
		float _FresnelExponent;

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
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

			// Screen Space UV로 바꾸어 적용
			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			screenUV.y += _Time* _ScrollSpeed;

			float2 screenUV_ST = screenUV * _HologramTex_ST.xy + _HologramTex_ST.zw; //Tiling과 ST값 적용
			float alpha = tex2D(_HologramTex, screenUV_ST).r;
			o.Alpha = alpha;
			
			//--------------Fresnel Part---------------------------------------//
			//get the dot product between the normal and the view direction
			float fresnel = dot(IN.worldNormal, IN.viewDir);
			//invert the fresnel so the big values are on the outside
			fresnel = saturate(1 - fresnel);
			//raise the fresnel value to the exponents power to be able to adjust it
			fresnel = pow(fresnel, _FresnelExponent);
			//combine the fresnel value with a color
			float3 fresnelColor = fresnel * _FresnelColor;
			//apply the fresnel value to the emission
			o.Emission = fresnelColor + (1 - alpha)*c.rgb;
			
        }
        ENDCG
    }
    FallBack "Diffuse"
}

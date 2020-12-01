Shader "Custom/ToonShaderPlanetAlpha"
{
    Properties
    {
        _Color1("Color", Color) = (1,1,1,1)
        _Color2("Color2", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Tex2("Albedo (RGB)2", 2D) = "white" {}
        [Normal]_Normal("Normal", 2D) = "bump" {}
        _LightCutoff("Light cutoff", Range(-1,1)) = 0.5
        _ShadowBands("Shadow bands", Range(0,4)) = 1

        [Header(Specular)]
        _SpecularMap("Specular map", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        [HDR]_SpecularColor("Specular color", Color) = (0,0,0,1)

        _Scroll1("Scroll1", Range(0, 5)) = 1
        _Scroll2("Scroll2", Range(0, 5)) = 1

        [Header(Rim)]
        _RimSize("Rim size", Range(0,1)) = 0
        [HDR]_RimColor("Rim color", Color) = (0,0,0,1)
        [Toggle(SHADOWED_RIM)]
        _ShadowedRim("Rim affected by shadow", float) = 0

        [Header(Emission)]
        [HDR]_Emission("Emission", Color) = (0,0,0,1)
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue"="Transparent" }
            LOD 200
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma surface surf CelShaded vertex:vert fullforwardshadows alpha
            #pragma shader_feature SHADOWED_RIM
            #pragma target 3.0


            fixed4 _Color1;
            fixed4 _Color2;
            sampler2D _MainTex;
            sampler2D _Tex2;
            sampler2D _Normal;
            float _LightCutoff;
            float _ShadowBands;

            float _Scroll1;
            float _Scroll2;

            sampler2D _SpecularMap;
            half _Glossiness;
            fixed4 _SpecularColor;

            float _RimSize;
            fixed4 _RimColor;

            fixed4 _Emission;

            struct Input
            {
                float2 uv_MainTex;
                float2 uv_Normal;
                float2 uv_SpecularMap;
                float4 vertexColor;
            };

            struct SurfaceOutputCelShaded
            {
                fixed3 Albedo;
                fixed3 Normal;
                float Smoothness;
                half3 Emission;
                fixed Alpha;
            };

            void vert(inout appdata_full v, out Input o)
            {
                UNITY_INITIALIZE_OUTPUT(Input, o);
                o.vertexColor = v.color; // Save the Vertex Color in the Input for the surf() method
            }

            half4 LightingCelShaded(SurfaceOutputCelShaded s, half3 lightDir, half3 viewDir, half atten) {
                half nDotL = saturate(dot(s.Normal, normalize(lightDir)));
                half diff = round(saturate(nDotL / _LightCutoff) * _ShadowBands) / _ShadowBands;

                float3 refl = reflect(normalize(lightDir), s.Normal);
                float vDotRefl = dot(viewDir, -refl);
                float3 specular = _SpecularColor.rgb * step(1 - s.Smoothness, vDotRefl);

                float3 rim = _RimColor * step(1 - _RimSize ,1 - saturate(dot(normalize(viewDir), s.Normal)));


                half stepAtten = round(atten);
                half shadow = diff * stepAtten;

                shadow = shadow + 1.5;

                half3 col = (s.Albedo + specular) * _LightColor0;

                half4 c;
                #ifdef SHADOWED_RIM
                c.rgb = (col + rim) * shadow;
                #else
                c.rgb = col * shadow + rim;
                #endif            
                c.a = s.Alpha;
                return c;
            }

            // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
                // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)

            void surf(Input IN, inout SurfaceOutputCelShaded o)
            {
                fixed4 c1 = tex2D(_MainTex, IN.uv_MainTex + _Scroll1 * float2(1,0) * _Time) * _Color1;
                fixed4 c2 = tex2D(_Tex2, IN.uv_MainTex + _Scroll2 * float2(1, 0) * _Time) * _Color2;
                c2.a = c2.r;
                c1 = c1 * (1 - c2.a) + c2;
                c1.a = (c2.a * 8) * _Color1.a;
                o.Albedo = c1.rgb * IN.vertexColor;
                o.Normal = UnpackNormal(tex2D(_Normal, IN.uv_Normal));
                o.Smoothness = tex2D(_SpecularMap, IN.uv_SpecularMap).x * _Glossiness;
                o.Emission = o.Albedo * _Emission;
                o.Alpha = c1.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}
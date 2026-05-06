Shader "AetherCell/AetherCell_StandardShader"
{
    Properties
    {
        _ColorTint("Color Tint",Color) = (1,1,1,1)
        _EmissionTint("Emission Tint",Color) = (1,1,1,1)
        _RoughnessAjust("RoughnessAjustment",Range(-1,1)) = 0

        [NoscaleOffset]_BasecolorMap("Basecolor Map",2D) = "white"
        [NoscaleOffset]_MetallicMap("Metallic Map",2D) = "black"
        [NoscaleOffset]_RoughnessMap("Roughness Map",2D) = "white"
        [NoscaleOffset][Normal]_NormalMap("Normal Map",2D) = "bump"
        [NoscaleOffset]_EmissionMap("EmissionMap",2D) = "black"
        
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        half3 _ColorTint;
        half3 _EmissionTint;
        half _RoughnessAjust;
        
        sampler2D _BasecolorMap;
        sampler2D _MetallicMap;
        sampler2D _RoughnessMap;
        sampler2D _NormalMap;
        sampler2D _EmissionMap;
        

        struct Input
        {
            float2 uv_BasecolorMap;
        };


        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.uv_BasecolorMap;

            half3 color = tex2D(_BasecolorMap, uv);
            half metallic = tex2D(_MetallicMap, uv);
            half roughness = tex2D(_RoughnessMap, uv);
            half3 normal = UnpackNormal(tex2D(_NormalMap, uv));
            half3 emission = tex2D(_EmissionMap, uv);

            o.Albedo = color * _ColorTint;
            o.Metallic = metallic;
            o.Smoothness = 1 - saturate((roughness + _RoughnessAjust));
            o.Normal = normal;
            o.Emission = emission * _EmissionTint;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

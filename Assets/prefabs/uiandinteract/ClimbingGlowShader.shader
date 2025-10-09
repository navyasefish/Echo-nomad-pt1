Shader "Custom/ClimbingGlowShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _Color ("Base Color", Color) = (1,1,1,1)
        
        // Glow Properties
        _GlowColor ("Glow Color", Color) = (1, 0.84, 0, 1) // Gold
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 2
        _RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
        
        // Climbing Effect
        _ClimbHeight ("Climb Height", Range(0, 1)) = 0
        _ClimbSmoothness ("Climb Smoothness", Range(0.01, 0.5)) = 0.1
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
        
        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _GlowColor;
        float _GlowIntensity;
        float _RimPower;
        float _ClimbHeight;
        float _ClimbSmoothness;
        
        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 viewDir;
        };
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Base texture
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            
            // Calculate normalized height (0 at bottom, 1 at top)
            float objHeight = IN.worldPos.y;
            float normalizedHeight = frac(objHeight * 0.5); // Adjust multiplier based on your seaweed size
            
            // Climbing wave effect
            float climbMask = smoothstep(_ClimbHeight - _ClimbSmoothness, _ClimbHeight + _ClimbSmoothness, normalizedHeight);
            climbMask = 1.0 - climbMask; // Invert so glow is below the climb line
            
            // Rim lighting (outline glow)
            float rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            rim = pow(rim, _RimPower);
            
            // Combine rim and climb mask
            float finalGlow = rim * climbMask * _GlowIntensity;
            
            // Add glow to emission
            o.Emission = _GlowColor.rgb * finalGlow;
        }
        ENDCG
    }
    
    FallBack "Diffuse"
}
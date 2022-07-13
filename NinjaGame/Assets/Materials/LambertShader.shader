Shader "Custom/LambertShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white"
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf LambertShader

        //We will need to create a function called Lighting + the name that we gave
        //here with instructions on how to light the object, which we will be writing later on in this
        //recipe. In this case, it would be LightingSimpleLambert.



        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
        }

        // Allows us to use the SimpleLambert lighting mode
        half4 LightingLambertShader (SurfaceOutput s, half3 lightDir, half atten)
        {
            
            // First calculate the dot product of the light direction and the
            // surface's normal
            half NdotL = dot(s.Normal, lightDir);
            // Next, set what color should be returned
            half4 color;
            color.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten);
            color.a = s.Alpha;
            // Return the calculated color
            return color;
            /*
            half4 color = half4(100,5,10,1);
            return color;
            */
        }

        ENDCG
    }
    FallBack "Diffuse"
}

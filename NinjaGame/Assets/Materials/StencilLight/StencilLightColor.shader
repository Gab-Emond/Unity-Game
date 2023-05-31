Shader "Custom/StencilLight Color"//(could add a render pass to renderer for these, then for shadows)
{
    Properties
    {
        [HDR]_Color("Color",Color) = (1,1,1,1) 
        
    }
    HLSLINCLUDE
    
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"        
    struct appdata
    {
        float4 vertex : POSITION;
    };
    
    struct v2f
    {
        float4 vertex : SV_POSITION;
    };
    
    float Toon(float3 normal, float3 lightDir){
        float NdotL = max(0.0, dot(normalize(normal),normalize(lightDir)));
        // Below the NdotL declaration.
        float lightIntensity = NdotL > 0 ? 1 : 0;

        //return NdotL;
        return lightIntensity;
    }
    
    v2f vert(appdata v)
    {
        v2f o;
        o.vertex = TransformObjectToHClip(v.vertex.xyz);
        return o;
    }

    CBUFFER_START(UnityPerMaterial)
        float4 _Color;    
    CBUFFER_END
    
    float4 frag(v2f i) : SV_Target
    {
        return _Color * _Color.a;
        //check light hit

        //light falloff possibility:
        //https://answers.unity.com/questions/1676725/how-to-have-a-point-light-with-no-falloff-in-unity.html

        //or check spotlight falloff
    }
    ENDHLSL

    SubShader
    {
        Tags{"Queue" = "Transparent" "RenderType" = "Transparent"}  
        Pass
        {
            Tags
            {
                "RenderType" = "Transparent"          
                "RenderPipeline" = "UniversalPipeline"            
            }         
            Zwrite off
            Ztest Lequal
            Cull Back
            Blend DstColor One
            
            Stencil
            {
                comp equal
                ref 1
                pass zero
                fail zero
                zfail zero
            }         
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag         
            ENDHLSL
        }      
        Pass
        {
            Tags
            {
                "RenderPipeline" = "UniversalPipeline"
                "LightMode" = "UniversalForward"
            }
            ZTest always
            ZWrite on
            Cull Front
            Blend DstColor One

            Stencil
            {
                Ref 1
                Comp equal
                Pass zero
            }         
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }     
    }
}
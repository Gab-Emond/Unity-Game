Shader "Custom/StencilLight Mask"
{
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        LOD 100
        
        Pass
        {
            
            Ztest Greater//shows only where intersection
            Zwrite off
            Cull Off
            Colormask 0
            Stencil
            {
                Comp Always//always compare
                Ref 1
                Pass Replace
            }
        }

        //https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
        //https://github.com/Unity-Technologies/Graphics/blob/master/Packages/com.unity.render-pipelines.universal/ShaderLibrary/RealtimeLights.hlsl

        // float3 Position;
        // Light light;
        
        // int pixelLightCount = GetAdditionalLightsCount();
        // for(int i=0;i<pixelLightCount;i++){
        //     light = GetAdditionalLight(i,Position,1);
        // }
        //todo: check how to get lightpass/shadows
        //or how to calculate intersect between current vertex point&&lightsource point
        
        //_WorldSpaceLightPos0 (float4)

        //

        //float4x4 shadowMatrix = unity_WorldToShadow;(light?) (float 4x4)
        
    }
}
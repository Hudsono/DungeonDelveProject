Shader "FullScreen/FullscreenTexture"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTexture("MainTexture", 2D) = "white" {}
    }

        HLSLINCLUDE

#pragma vertex Vert

#pragma target 4.5
#pragma only_renderers d3d11 ps4 xboxone vulkan metal switch

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"

        // The PositionInputs struct allow you to retrieve a lot of useful information for your fullScreenShader:
        // struct PositionInputs
        // {
        //     float3 positionWS;  // World space position (could be camera-relative)
        //     float2 positionNDC; // Normalized screen coordinates within the viewport    : [0, 1) (with the half-pixel offset)
        //     uint2  positionSS;  // Screen space pixel coordinates                       : [0, NumPixels)
        //     uint2  tileCoord;   // Screen tile coordinates                              : [0, NumTiles)
        //     float  deviceDepth; // Depth from the depth buffer                          : [0, 1] (typically reversed)
        //     float  linearDepth; // View space Z coordinate                              : [Near, Far]
        // };

        // To sample custom buffers, you have access to these functions:
        // But be careful, on most platforms you can't sample to the bound color buffer. It means that you
        // can't use the SampleCustomColor when the pass color buffer is set to custom (and same for camera the buffer).
        // float4 SampleCustomColor(float2 uv);
        // float4 LoadCustomColor(uint2 pixelCoords);
        // float LoadCustomDepth(uint2 pixelCoords);
        // float SampleCustomDepth(float2 uv);

        // There are also a lot of utility function you can use inside Common.hlsl and Color.hlsl,
        // you can check them out in the source code of the core SRP package.

        TEXTURE2D(_MainTexture);
    float4 _Color;

    float4 FullScreenPass(Varyings varyings) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(varyings);
        float depth = LoadCameraDepth(varyings.positionCS.xy);
        PositionInputs posInput = GetPositionInput(varyings.positionCS.xy, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
        float4 screenColor = float4(0.0, 0.0, 0.0, 0.0);
        if (_CustomPassInjectionPoint != CUSTOMPASSINJECTIONPOINT_BEFORE_RENDERING)
            screenColor = float4(CustomPassLoadCameraColor(varyings.positionCS.xy, 0), 1);
        float4 maskCol = SAMPLE_TEXTURE2D(_MainTexture, s_trilinear_clamp_sampler, posInput.positionNDC);
        return lerp(screenColor, maskCol * _Color, maskCol.a * _Color.a);
    }

        ENDHLSL

        SubShader
    {
        Pass
        {
            Name "Full screen texture"

            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM
                #pragma fragment FullScreenPass
            ENDHLSL
        }
    }
    Fallback Off
}
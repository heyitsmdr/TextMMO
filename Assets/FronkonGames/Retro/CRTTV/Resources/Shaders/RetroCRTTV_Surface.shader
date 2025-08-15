////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Martin Bustos @FronkonGames <fronkongames@gmail.com>. All rights reserved.
//
// THIS FILE CAN NOT BE HOSTED IN PUBLIC REPOSITORIES.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
Shader "Fronkon Games/Retro/CRT TV Surface"
{
  Properties
  {
    _MainTex("Albedo", 2D) = "white" {}
    _BaseColor("Color", Color) = (1, 1, 1, 1)

    _EmissionStrength("Emission strength", Range(0.0, 2.0)) = 0.5
        
    _Intensity("Intensity", Range(0.0, 1.0)) = 1.0
    
    _ShadowmaskStrength("Shadowmask", Range(0.0, 1.0)) = 1.0
    _ShadowmaskLuminosity("Luminosity", Range(0.0, 2.0)) = 1.0
    _ShadowmaskScale("Scale", Range(0.0, 1.0)) = 1.0
    _ShadowmaskColorOffset("Color offset", Vector) = (0.0, -0.3, -0.6)
    _ShadowmaskVerticalGapHardness("Horizontal hardness", Range(0.0, 1.0)) = 0.8
    _ShadowmaskHorizontalGapHardness("Vertical hardness", Range(0.0, 1.0)) = 0.1
    
    _FishEyeStrength("FishEye", Range(-1.0, 5.0)) = 0.2
    _FishEyeZoom("Zoom", Vector) = (1.0, 1.0, 0.0, 0.0)

    _Distortion("Distortion", Range(0.0, 2.0)) = 0.17
    _DistortionSpeed("Speed", Range(-10.0, 10.0)) = 1.0
    _DistortionAmplitude("Amplitude", Range(0.0, 10.0)) = 2.0

    _VignetteSmoothness("Vignette smoothness", Range(0.0, 2.0)) = 0.7
    _VignetteRounding("Rounding", Range(0.0, 1.0)) = 0.5
    _VignetteBorders("Borders", Vector) = (0.0, 0.5, 0.0, 0.0)
    
    _ShineStrength("Shine", Range(0.0, 1.0)) = 0.3
    _ShineColor("Color", Color) = (0.5, 0.5, 0.5, 0.5)
    _ShinePosition("Position", Vector) = (0.5, 1.0, 0.0)
    
    _GammaColor("Gamma color", Vector) = (0.9, 0.7, 1.2, 1.0)
    
    _ColorCurves("Color curves", Color) = (0.95, 1.05, 0.95, 1.0)
    _RGBOffsetStrength("Shine", Range(0.0, 1.0)) = 0.25
    
    _ColorBleedingStrength("Color bleeding", Range(0.0, 1.0)) = 0.25
    _ColorBleedingDistance("Distance", Range(-1.0, 1.0)) = 0.5
    
    _Scanlines("Scanlines", Range(0.0, 1.0)) = 1.0
    _ScanlinesCount("Density", Range(0.0, 2.0)) = 1.25
    _ScanlinesVelocity("Velocity", Range(-10.0, 10.0)) = 3.5
    
    _InterferenceStrength("Signal interference", Range(0.0, 1.0)) = 0.2
    _InterferencePeakStrength("Peak strength", Range(0.0, 1.0)) = 0.3
    _InterferencePeakPosition("Peak position", Range(0.0, 1.0)) = 0.2
        
    _ShakeStrength("Frame shaking", Range(0.0, 1.0)) = 1.0
    _ShakeRate("Rate", Range(0.0, 1.0)) = 0.2
    
    _MovementStrength("Frame movement", Range(0.0, 1.0)) = 1.0
    _MovementRate("Rate", Range(0.0, 1.0)) = 0.2
    _MovementSpeed("Speed", Range(0.0, 1.0)) = 0.4
    
    _Grain("Grain", Range(0.0, 1.0)) = 0.0
    _StaticNoise("Static noise", Range(0.0, 1.0)) = 0.0
    
    _BarStrength("Bar effect", Range(0.0, 1.0)) = 0.1
    _BarHeight("Height", Range(0.0, 10.0)) = 6.0
    _BarSpeed("Speed", Range(-10.0, 10.0)) = 4.0
    _BarOverflow("Overflow", Range(0.0, 4.0)) = 1.2
    
    _FlickerStrength("Flicker", Range(0.0, 1.0)) = 0.1
    _FlickerSpeed("Rate", Range(0.0, 100.0)) = 50.0
  }

  SubShader
  {
    Tags
    {
      "RenderPipeline" = "UniversalPipeline"
      "RenderType" = "Opaque"
      "Queue" = "Geometry"
    }
    LOD 100

    Pass
    {
      Name "Universal Forward"
      Tags
      {
        "LightMode" = "UniversalForward"
      }

      Cull Back
      Blend One Zero
      ZWrite On
      ZTest LEqual
      
      HLSLPROGRAM
      #pragma prefer_hlslcc gles
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_instancing      
      #pragma vertex vert
      #pragma fragment frag

      #include "RetroCRTTV.hlsl"

      CBUFFER_START(UnityPerMaterial)
      float4 _MainTex_ST;
      half4 _BaseColor;      
      CBUFFER_END

      float _EmissionStrength;

      struct Attributes
      {
        float3 positionOS : POSITION;
        float3 normalOS   : NORMAL;
        float2 uv0        : TEXCOORD;
        UNITY_VERTEX_INPUT_INSTANCE_ID
      };

      struct Interpolators
      {
        float4 positionCS : SV_POSITION;
        float2 uv0        : TEXCOORD0;
        float3 normalWS   : TEXCOORD1;
        float fogCoord    : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
      };

      Interpolators vert(Attributes input)
      {
        Interpolators output = (Interpolators)0;

        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_TRANSFER_INSTANCE_ID(input, output);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

        const VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS);
        const VertexNormalInputs normalInputs = GetVertexNormalInputs(input.positionOS);

        output.positionCS = positionInputs.positionCS;
        output.uv0 = TRANSFORM_TEX(input.uv0, _MainTex);
        output.normalWS = normalInputs.normalWS;
        output.fogCoord = ComputeFogFactor(positionInputs.positionCS.z);

        return output;
      }

      half4 frag(const Interpolators input) : SV_Target
      {
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        const half4 color = SAMPLE_MAIN(input.uv0) * _BaseColor;
        half4 pixel = CRTTV(color, input.uv0); 

        InputData lightingInput = (InputData)0;
        lightingInput.normalWS = input.normalWS;

        pixel = lerp(color, pixel, _Intensity);

        pixel.rgb = MixFogColor(pixel.rgb, half3(1.0, 1.0, 1.0), input.fogCoord);

        SurfaceData surfaceInput = (SurfaceData)0;
        surfaceInput.albedo = pixel.rgb;
        surfaceInput.emission = pixel.rgb * _EmissionStrength;

        surfaceInput.alpha = pixel.a;

        return UniversalFragmentBlinnPhong(lightingInput, surfaceInput);
      }
      ENDHLSL
    }

    UsePass "Universal Render Pipeline/Lit/DepthOnly"
    UsePass "Universal Render Pipeline/Lit/ShadowCaster"
    UsePass "Universal Render Pipeline/Simple Lit/Meta"
  }
  
  FallBack "Hidden/Universal Render Pipeline/FallbackError"
  CustomEditor "FronkonGames.Retro.CRTTV.Editor.CRTTVSurfaceGUI"
}


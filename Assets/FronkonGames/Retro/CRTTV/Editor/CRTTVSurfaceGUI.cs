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
using UnityEditor;
using UnityEngine;
using static FronkonGames.Retro.CRTTV.Inspector;

namespace FronkonGames.Retro.CRTTV.Editor
{
  /// <summary> Retro CRT TV Shader GUI. </summary>
  public class CRTTVSurfaceGUI : SurfaceGUI
  {
    protected override void InspectorGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
      SliderProperty("_EmissionStrength", "Emission strength [0.0, 2.0]. Default 0.5.", 1.0f);
      
      Separator();
      
      SliderProperty("_ShadowmaskStrength", "Shadowmask strength [0.0, 1.0]. Default 1.0.", 1.0f);
      IndentLevel++;
      SliderProperty("_ShadowmaskScale", "Shadowmask cells scale [0.0, 1.0]. Default 1.", 1.0f);
      VectorProperty("_ShadowmaskColorOffset", "Shadowmask cells scale [0.0, 1.0]. Default 0.5.", CRTTV.Settings.DefaultShadowmaskColorOffset);
      SliderProperty("_ShadowmaskHorizontalGapHardness", "Shadowmask cells horizontal gap hardness [0.0, 1.0]. Default 0.8.", 0.8f);
      SliderProperty("_ShadowmaskVerticalGapHardness", "Shadowmask cells vertical gap hardness [0.0, 1.0]. Default 0.1.", 0.1f);
      IndentLevel--;

      SliderProperty("_FishEyeStrength", "Fisheye strength [-1.0, 5.0]. Default 0.2.", 0.2f);
      IndentLevel++;
      VectorProperty("_FishEyeZoom", "Fisheye zoom.", Vector2.one);
      IndentLevel--;

      SliderProperty("_Distortion", "Sinusoidal distortion of the vertical axis [0, 2.0]. Default 0.17.", 0.17f);
      IndentLevel++;
      SliderProperty("_DistortionSpeed", "Distortion speed [-10, 10]. Default 1.", 1.0f);
      SliderProperty("_DistortionAmplitude", "Wave amplitude of the distortion [0, 10]. Default 2.", 2.0f);
      IndentLevel--;

      SliderProperty("_VignetteSmoothness", "Vignette smoothness [0.0, 2.0]. Default 0.7.", 0.7f);
      IndentLevel++;
      SliderProperty("_VignetteRounding", "Vignette rounding [0.0, 1.0]. Default 0.5.", 0.5f);
      VectorProperty("_VignetteBorders", "Screen size.", CRTTV.Settings.DefaultVignetteBorders);
      IndentLevel--;
      
      SliderProperty("_ShineStrength", "Screen shine strength [0.0, 1.0]. Default 0.3.", 0.3f);
      IndentLevel++;
      ColorProperty("_ShineColor", "Screen shine color.", CRTTV.Settings.DefaultShineColor);
      VectorProperty("_ShinePosition", "Screen shine position. Default (0.5, 1.0).", CRTTV.Settings.DefaultShinePosition);
      IndentLevel--;

      MaterialProperty property = FindProperty("_GammaColor", properties, true);
      if (property != null)
      {
        EditorGUI.BeginChangeCheck();

        Vector4 gammaColor = property.vectorValue;
        gammaColor.w = Slider("Gamma color", "Gamma color tint.", gammaColor.w, 0.0f, 1.0f, 1.0f);
        IndentLevel++;
        gammaColor.x = Slider("Red", "Red channel.", gammaColor.x, 0.0f, 2.0f, CRTTV.Settings.DefaultGammaColor.x);
        gammaColor.y = Slider("Green", "Green channel", gammaColor.y, 0.0f, 2.0f, CRTTV.Settings.DefaultGammaColor.y);
        gammaColor.z = Slider("Blue", "Blue channel", gammaColor.z, 0.0f, 2.0f, CRTTV.Settings.DefaultGammaColor.z);
        IndentLevel--;

        if (EditorGUI.EndChangeCheck() == true)
          property.vectorValue = gammaColor;
      }
      
      ColorProperty("_ColorCurves", "Color curves adjustment.", CRTTV.Settings.DefaultColorCurves);
      SliderProperty("_RGBOffsetStrength", "Color channels offset strength [0.0, 1.0]. Default 0.25.", 0.25f);
      
      SliderProperty("_ColorBleedingStrength", "Color bleeding strength [0.0, 1.0]. Default 0.25.", 0.25f);
      IndentLevel++;
      SliderProperty("_ColorBleedingDistance", "Color bleeding distance [-1.0, 1.0]. Default 0.5.", 0.5f);
      IndentLevel--;
      
      SliderProperty("_Scanlines", "Scanlines strength [0.0, 1.0]. Default 1.", 1.0f);
      IndentLevel++;
      SliderProperty("_ScanlinesCount", "Scanlines density [0.0, 2.0]. Default 1.25.", 1.25f);
      SliderProperty("_ScanlinesVelocity", "Scanlines velocity [-10.0, 10.0]. Default 3.5.", 3.5f);
      IndentLevel--;
      
      SliderProperty("_InterferenceStrength", "Signal interference strength [0.0, 1.0]. Default 0.2.", 0.2f);
      IndentLevel++;
      SliderProperty("_InterferencePeakStrength", "Peak interference strength [0.0, 1.0]. Default 0.3.", 0.3f);
      SliderProperty("_InterferencePeakPosition", "Peak interference y position [0.0, 1.0]. Default 0.2.", 0.2f);
      IndentLevel--;
      
      SliderProperty("_ShakeStrength", "Vertical shake strength [0.0, 1.0]. Default 1.0.", 1.0f);
      IndentLevel++;
      SliderProperty("_ShakeRate", "Vertical shake rate [0.0, 1.0]. Default 0.2.", 0.2f);
      IndentLevel--;

      SliderProperty("_MovementStrength", "Vertical frame movement strength [0.0, 1.0]. Default 1.0.", 1.0f);
      IndentLevel++;
      SliderProperty("_MovementRate", "Vertical frame movement rate [0.0, 1.0]. Default 0.2.", 0.2f);
      SliderProperty("_MovementSpeed", "Vertical frame movement speed [0.0, 1.0]. Default 0.4.", 0.4f);
      IndentLevel--;
      
      SliderProperty("_Grain", "Signal noise [0.0, 1.0]. Default 0.", 0.0f);
      SliderProperty("_StaticNoise", "Static noise [0.0, 1.0]. Default 0.", 0.0f);
      
      SliderProperty("_BarStrength", "Bar effect strength [0.0, 1.0]. Default 0.1.", 0.1f);
      IndentLevel++;
      SliderProperty("_BarHeight", "Bar height [0.0, 10.0]. Default 6.", 6.0f);
      SliderProperty("_BarSpeed", "Bar speed [-10.0, 10.0]. Default 4.", 4.0f);
      SliderProperty("_BarOverflow", "Bar overflow [0.0, 4.0]. Default 1.2.", 1.2f);
      IndentLevel--;
      
      SliderProperty("_FlickerStrength", "Flicker strength [0.0, 1.0]. Default 0.1.", 0.1f);
      IndentLevel++;
      SliderProperty("_FlickerSpeed", "Flicker speed [0.0, 100.0]. Default 50.", 50.0f);
      IndentLevel--;
    }
  }
}

using UnityEngine;
using Verse;

namespace FreeWill
{
    /// <summary>
    /// Holds texture resources for FreeWill UI elements.
    /// Custom textures should be placed in: Textures/UI/FreeWill/
    /// </summary>
    [StaticConstructorOnStartup]
    public static class FreeWillResources
    {
        public static readonly Texture2D SettingsIcon = ContentFinder<Texture2D>.Get("UI/Icons/Options/OptionsGeneral", false);
        
        public static readonly Texture2D RefreshIcon = ContentFinder<Texture2D>.Get("UI/Commands/Autoload", false);
        
        public static readonly Texture2D FocusIcon = ContentFinder<Texture2D>.Get("UI/Icons/Study", false) ;
        
        public static readonly Texture2D FreeWillOn = Widgets.CheckboxOnTex;
        public static readonly Texture2D FreeWillOff = Widgets.CheckboxOffTex;
        public static readonly Texture2D WorkTypeOn = Widgets.CheckboxOnTex;
        public static readonly Texture2D WorkTypeOff = Widgets.CheckboxOffTex;
        public static readonly Texture2D FocusHighlight = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 0.8f, 0.2f, 0.3f));
    }
}

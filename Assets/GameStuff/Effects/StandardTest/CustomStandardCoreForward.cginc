#ifndef CUSTOM_STANDARD_CORE_FORWARD_INCLUDED
#define CUSTOM_STANDARD_CORE_FORWARD_INCLUDED

#include "UnityStandardConfig.cginc"
#include "UnityStandardCore.cginc"

half4 fragCustomInternal(VertexOutputForwardBase i)
{
    UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);

    FRAGMENT_SETUP(s)

    UNITY_SETUP_INSTANCE_ID(i);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

    UnityLight mainLight = MainLight ();
    UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);

    half occlusion = Occlusion(i.tex.xy);
    UnityGI gi = FragmentGI (s, occlusion, i.ambientOrLightmapUV, atten, mainLight);
    
    half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect);
    c.rgb += Emission(i.tex.xy);

    UNITY_EXTRACT_FOG_FROM_EYE_VEC(i);
    UNITY_APPLY_FOG(_unity_fogCoord, c.rgb);
    return OutputForward (c, s.alpha);
}

half4 fragCustom (VertexOutputForwardBase i) : SV_Target   // backward compatibility (this used to be the fragment entry function)
{
    return fragCustomInternal(i);
}


#endif // CUSTOM_STANDARD_CORE_FORWARD_INCLUDED
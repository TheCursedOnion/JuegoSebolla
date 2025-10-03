Shader "Custom/StencilWriter"
{
    SubShader
    {
        ZWrite Off
        ColorMask 0
        Cull Off
        ZTest LEqual
        
        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
            ZFail Replace
        }
        
        Pass
        {
            
        }
    }
}

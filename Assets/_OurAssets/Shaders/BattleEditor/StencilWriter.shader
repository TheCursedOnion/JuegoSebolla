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
            Ref 2             // Usa bit 1
            Comp Always       // Siempre pasa
            Pass Replace      // Escribe el valor Ref
            Fail Keep
            ZFail Keep
            ReadMask 2        // Solo lee el bit 1
            WriteMask 2       // Solo escribe en el bit 1
        }
        
        Pass
        {
            
        }
    }
}

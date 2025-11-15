Shader "Custom/StencilWriterWall"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Cull Back
        ZWrite Off          // No escribimos profundidad, solo leemos
        ZTest LEqual          // Solo pasa si el fragmento está más cercano que lo dibujado previamente
        ColorMask 0         // No pintamos color

        Stencil
        {
            Ref 2             // Usa bit 1
            Comp Equal       // Siempre pasa
            Pass Zero      // Escribe el valor Ref
            Fail Keep
            ZFail Keep
            ReadMask 2        // Solo lee el bit 1
            WriteMask 2       // Solo escribe en el bit 1
        }

        Pass {}              // Necesario para renderizar el pass
    }
}
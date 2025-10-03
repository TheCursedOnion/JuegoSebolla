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
            Ref 1
            Comp Always      // Evalúa stencil siempre que el fragmento pase ZTest
            Pass DecrSat     // Incrementa el stencil en lugar de reemplazar
            Fail Keep        // Si falla stencil, mantener valor
            ZFail Replace
        }

        Pass {}              // Necesario para renderizar el pass
    }
}
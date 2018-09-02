Shader "Custom/StencilWrite"
{
	Properties
	{
            //Stencil Buffer Property
        //[Enum(SeeAll,0, SeeThrough,1)] _StencilRef ("Stencil State", int) = 1
	}
	SubShader
	{
        Tags{ "RenderType"="Opaque"}      

        //stencil operation
        Stencil{
            Ref 1
            Comp Always
            Pass Replace
        }

        Pass{
            //don't draw color or depth and do not cull faces
            ColorMask 0
            ZWrite Off
            Cull Off

        }
	}
}

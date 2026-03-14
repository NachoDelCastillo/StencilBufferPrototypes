Shader "Unlit/StencilMask"
{
	Properties
	{
		[IntRange] _StencilID("Stencil ID", Range(0,255)) = 1
	}
		SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"Queue" = "Geometry-1"
			"RenderPipeline" = "UniversalPipeline"
		}

		Pass
		{
			Blend Zero One
			ZWrite On

			Stencil
			{
				Ref 1
				Comp Equal
			}
			ZTest Always
		}
	}
}
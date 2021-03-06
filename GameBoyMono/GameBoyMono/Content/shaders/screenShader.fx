﻿#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

int spriteWidth, spriteHeight;
int scale;

sampler2D s0
{
    MipFilter = Point;
    MinFilter = Point;
    MagFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};

float4 MainPS(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords: TEXCOORD0) : COLOR
{
	float4 spriteColor = tex2D(s0,coords);
	
	if(	(int)(coords.x * spriteWidth) % scale == 0 ||
		(int)(coords.y * spriteHeight) % scale == 0 )
		spriteColor.rgba =  spriteColor * 0.4f; //float4(0,0,0,0);//
	//else if(	(int)(coords.x * spriteWidth) % scale == 1 ||
	//	(int)(coords.y * spriteHeight) % scale == 1 )
	//	spriteColor.rgba = spriteColor * 0.3f;
		
	//spriteColor.rgba = ((spriteColor.rgba - 0.5f) * 1.2f) + 0.5f;
	return spriteColor;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
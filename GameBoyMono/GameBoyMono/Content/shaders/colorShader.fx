#if OPENGL 
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

int spriteWidth, spriteHeight;
int scale;
float4 color1, color2, color3, color4;

sampler2D s0
{
    MipFilter = Point;
    MinFilter = Point;
    MagFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};

float4 MainPS(float4 pos : SV_POSITION, float4 sColor1 : COLOR0, float2 coords: TEXCOORD0) : COLOR
{
	float4 spriteColor = tex2D(s0,coords);
	
	if(spriteColor.a == 0)
		return float4(0, 0, 0, 0);
	else if(spriteColor.r < 0.25)	// 0
		return color4;
	else if(spriteColor.r < 0.50)	// 0.33
		return color3;
	else if(spriteColor.r < 0.75)	// 0.66
		return color2;
	else
		return color1;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
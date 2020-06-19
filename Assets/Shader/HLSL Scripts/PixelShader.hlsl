void PixelShaderFunction_float(float4 IN, out float4 OUT)
{
    OUT = IN;
    float4 scaleOffset = unity_StereoScaleOffset[unity_StereoEyeIndex];
 
    if (scaleOffset.x > 0){
 
        OUT.x /= 2;
        OUT.x += scaleOffset.z;
    }
}
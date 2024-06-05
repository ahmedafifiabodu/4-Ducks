// Declare the depth texture of the camera
TEXTURE2D(_CameraDepthTexture);
// Declare the sampler for the depth texture
SAMPLER(sampler_CameraDepthTexture);
// Declare the texel size of the depth texture
float4 _CameraDepthTexture_TexelSize;

// Declare the depth normals texture of the camera
TEXTURE2D(_CameraDepthNormalsTexture);
// Declare the sampler for the depth normals texture
SAMPLER(sampler_CameraDepthNormalsTexture);

// Declare the depth texture for transparent objects
TEXTURE2D(_TransparentDepthTexture);
// Declare the sampler for the depth texture of transparent objects
SAMPLER(sampler_TransparentDepthTexture);

// Function to decode normal from encoded value
float3 DecodeNormal(float4 enc)
{
    // Scale factor for decoding
    float kScale = 1.7777;
    // Decode the normal
    float3 nn = enc.xyz*float3(2*kScale,2*kScale,0) + float3(-kScale,-kScale,1);
    float g = 2.0 / dot(nn.xyz,nn.xyz);
    float3 n;
    n.xy = g*nn.xy;
    n.z = g-1;
    return n;
}

// Function to outline an object
void OutlineObject_float(float2 UV, float OutlineThickness, float DepthSensitivity, float NormalsSensitivity, bool isTransparent, out float Out)
{
    // Calculate the half scale for the outline thickness
    float halfScaleFloor = floor(OutlineThickness * 0.5);
    float halfScaleCeil = ceil(OutlineThickness * 0.5);
    
    // Declare arrays to store UV, depth and normal samples
    float2 uvSamples[4];
    float depthSamples[4];
    float3 normalSamples[4];

    // Calculate the UV samples
    uvSamples[0] = UV - float2(_CameraDepthTexture_TexelSize.x, _CameraDepthTexture_TexelSize.y) * halfScaleFloor;
    uvSamples[1] = UV + float2(_CameraDepthTexture_TexelSize.x, _CameraDepthTexture_TexelSize.y) * halfScaleCeil;
    uvSamples[2] = UV + float2(_CameraDepthTexture_TexelSize.x * halfScaleCeil, -_CameraDepthTexture_TexelSize.y * halfScaleFloor);
    uvSamples[3] = UV + float2(-_CameraDepthTexture_TexelSize.x * halfScaleFloor, _CameraDepthTexture_TexelSize.y * halfScaleCeil);

    // Loop over the samples
    for(int i = 0; i < 4 ; i++)
    {
        // If the object is transparent, sample the depth from the transparent depth texture
        if (isTransparent)
        {
            depthSamples[i] = SAMPLE_TEXTURE2D(_TransparentDepthTexture, sampler_TransparentDepthTexture, uvSamples[i]).r;
        }
        // Otherwise, sample the depth from the camera depth texture
        else
        {
            depthSamples[i] = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uvSamples[i]).r;
        }
        // Sample the normal from the camera depth normals texture
        normalSamples[i] = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, uvSamples[i]));
    }

    // Calculate the finite differences for depth
    float depthFiniteDifference0 = depthSamples[1] - depthSamples[0];
    float depthFiniteDifference1 = depthSamples[3] - depthSamples[2];
    // Calculate the edge depth
    float edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2)) * 100;
    // Calculate the depth threshold
    float depthThreshold = (1/DepthSensitivity) * depthSamples[0];
    // If the edge depth is greater than the threshold, set it to 1, otherwise 0
    edgeDepth = edgeDepth > depthThreshold ? 1 : 0;

    // Calculate the finite differences for normals
    float3 normalFiniteDifference0 = normalSamples[1] - normalSamples[0];
    float3 normalFiniteDifference1 = normalSamples[3] - normalSamples[2];
    // Calculate the edge normal
    float edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
    // If the edge normal is greater than the threshold, set it to 1, otherwise 0
    edgeNormal = edgeNormal > (1/NormalsSensitivity) ? 1 : 0;

    // Calculate the edge by taking the maximum of edge depth and edge normal
    float edge = max(edgeDepth, edgeNormal);
    // Output the edge
    Out = edge;
}
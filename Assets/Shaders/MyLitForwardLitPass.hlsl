// This file contains the vertex and fragment functions for the forward lit pass
// This is the shader pass that computes visible colors for a material
// by reading material, light, shadow, etc. data

// This attributes struct receives data about the mesh we're currently rendering
// Data is automatically placed in fields according to their semantics

// Pull in URP library functions and our own common functions
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

struct Attributes {
	float3 position : POSITION; // Position in object space
};

// This struct is output by the vertex function and input to the fragment function
// Note that fields will be transformed by the intermediary rasteration stage
struct Interpolators {
	// This value should contain the position in clip space (which is similar to a position on screen)
	// when output from the vertex function. It will be transformed into pixel position of the current
	// fragment on the screen when read from the fragment function
	float4 positionCS : SV_POSITION;
};

// The Vertex function. This runs for each vertex on mesh.
// It must output the position on the screen each vertex should appear at,
// as well as any data the fragment function will need
Interpolators Vertex(Attributes input) {
	Interpolators output;

	// These helper functions, found in URP/ShaderLab/ShaderVariablesFunctions.hlsl
	// transform object space values into world and clip space
	VertexPositionInputs posnInputs = GetVertexPositionInputs(input.position);

	// Pass position and orientation data to the fragment function
	output.positionCS = posnInputs.positionCS;

	return output;
}

// The fragment function. This runs once per fragment, which you can think of as a pixel on the screen.
// It must output the final color of this pixel.
float4 Fragment(Interpolators input) : SV_TARGET {
	return float4(1, 1, 1, 1); // The color of the pixel (just coloring pixel white)
}
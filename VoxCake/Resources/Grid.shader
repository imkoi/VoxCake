Shader "VoxCake/Grid" {
	Properties{
		_GridThickness("Grid Thickness", Float) = 0.05
		_GridColour("Grid Colour", Color) = (1.0, 1.0, 1.0, 1.0)
	}

		SubShader{
			Tags{ "Queue" = "Transparent" }

			Pass{
				ZWrite Off
				Blend SrcAlpha OneMinusSrcAlpha
				Cull Off
				CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag

		uniform float _GridThickness;
		uniform float4 _GridColour;

		struct vertexInput {
			float4 vertex : POSITION;
		};
		struct vertexOutput {
			float4 pos : SV_POSITION;
			float4 worldPos : TEXCOORD0;
		};

		vertexOutput vert(vertexInput input) {
			vertexOutput output;
			output.pos = UnityObjectToClipPos(input.vertex);
			output.worldPos = mul(unity_ObjectToWorld, input.vertex);
			output.worldPos.x -= _GridThickness / 2;
			output.worldPos.z -= _GridThickness / 2;
			output.worldPos.y -= _GridThickness / 2;

			return output;
		}

		float4 frag(vertexOutput input) : COLOR{
			if (frac(input.worldPos.x) < _GridThickness || frac(input.worldPos.y) < _GridThickness || frac(input.worldPos.z) < _GridThickness) {
				return _GridColour;
			}
			else {
				return float4(0,0,0,0);
			}
		}
		ENDCG
	}
	}
}
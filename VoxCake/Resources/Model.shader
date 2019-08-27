Shader "VoxCake/Model" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
	}
		SubShader{

		CGPROGRAM
		#pragma surface surf Standard vertex:vert fullforwardshadows
		#pragma target 3.0

		struct Input {
			 float3 vertexColor;
		};

		struct v2f {
		   float4 pos : SV_POSITION;
		   fixed4 color : COLOR;
		};

		void vert(inout appdata_full v, out Input o)
		{
			 UNITY_INITIALIZE_OUTPUT(Input,o);
			 o.vertexColor = v.color;
		}

		fixed4 _Color;
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			 fixed4 c = _Color;
			 o.Albedo = c.rgb * IN.vertexColor * 1.25;
			 o.Alpha = c.a;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
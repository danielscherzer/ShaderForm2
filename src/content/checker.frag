#version 130
uniform vec2 u_resolution;

void main() 
{
	vec2 uv = gl_FragCoord.xy / u_resolution;
	vec2 uv10 = floor(uv * 10.0f);
	bool black = 1.0 > mod(uv10.x + uv10.y, 2.0f);
	gl_FragColor = black ? vec4(0, 0, 0, 1) : vec4(1, 1, 0, 1);
//	gl_FragColor = vec4(uv, 0, 1);
}
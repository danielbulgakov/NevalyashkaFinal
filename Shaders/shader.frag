#version 330

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;


void main()
{
    outputColor = mix(texture(texture0, texCoord), texture(texture0, texCoord), 0.2);
}
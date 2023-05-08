#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNorm;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 FragPos;
out vec3 FragNorm;
out vec3 vPos;
out vec3 vNorm;

void main()
{
	vPos  = aPos;
	vNorm = normalize(aNorm);

	FragPos  = vec3(model * vec4(aPos, 1.));
	FragNorm = mat3(transpose(inverse(model))) * aNorm;

	gl_Position = projection * view * model * vec4(aPos, 1.);
}
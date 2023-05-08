#version 330 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 FragNorm;
in vec3 vPos;
in vec3 vNorm;

uniform sampler2D tex;
uniform vec3 lightPos;

void main()
{
    vec4 colX = texture(tex, vPos.zy * 1f);
    vec4 colY = texture(tex, vPos.xz * 1f);
    vec4 colZ = texture(tex, vPos.xy * 1f);

    vec3 bw = pow(abs(vNorm), vec3(10f));
    bw /= dot(bw, vec3(1));

    vec4 surfColor = colX * bw.x + colY * bw.y + colZ * bw.z;

    vec3 norm = normalize(FragNorm);
    vec3 lightDir = normalize(lightPos - FragPos);

    float diff = max(dot(norm, lightDir), 0.4);

    FragColor = vec4(1) * diff* surfColor;

}
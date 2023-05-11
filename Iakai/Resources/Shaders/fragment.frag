#version 330 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 FragNorm;
in vec3 vPos;
in vec3 vNorm;

uniform sampler2D dirt;
uniform sampler2D wall;
uniform float texScale;
uniform vec3 lightDir;

void main()
{
    vec3 fp = FragPos;
    vec3 fn = FragNorm;
    
    vec3 bw = pow(abs(fn), vec3(2));
    bw /= dot(bw, vec3(1));

    vec4 colX = texture(dirt, fp.zy * texScale);
    vec4 colY = texture(dirt, fp.xz * texScale);
    vec4 colZ = texture(dirt, fp.xy * texScale);

    vec4 surfColor = colX * bw.x + colY * bw.y + colZ * bw.z;

    vec3 norm = normalize(FragNorm);
    vec3 dir  = normalize(-lightDir);

    float diff = max(dot(norm, dir), 0.4);

    FragColor = vec4(1) * diff * surfColor;
}
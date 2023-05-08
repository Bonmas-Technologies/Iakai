using StbImageSharp;
using OpenTK.Graphics.OpenGL;

namespace Iakai.Core
{
    public class TextureContainer
    {
        private int _handle;

        static TextureContainer()
        {
            StbImage.stbi_set_flip_vertically_on_load(1);
        }

        public TextureContainer(int glHandle)
        {
            _handle = glHandle;
        }

        public TextureContainer(string path, TextureWrapMode wrapMode)
        {
            GL.ActiveTexture(TextureUnit.Texture0);

            _handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _handle);
            
            ImageResult tempImageContainer;

            using (var imageFile = File.OpenRead(path))
            {
                tempImageContainer = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, tempImageContainer.Width, tempImageContainer.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, tempImageContainer.Data);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, _handle);
        }
    }
}

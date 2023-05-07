using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iakai.Core
{
    internal class ShaderProgram : IDisposable
    {
        public string Log => _logger.ToString();

        public bool IsDisposed => _disposed;

        private int _handle;

        private bool _disposed = false;

        private StringBuilder _logger = new StringBuilder();

        public ShaderProgram(string vertexPath, string fragmentPath)
        {
            var vertexHandle = CreateShader(ShaderType.VertexShader, vertexPath);
            var fragmentHandle = CreateShader(ShaderType.FragmentShader, fragmentPath);

            if (CreateProgram(vertexHandle, fragmentHandle) != 1)
                Dispose();

            DeleteAttachedShader(vertexHandle);
            DeleteAttachedShader(fragmentHandle);
        }


        public ShaderProgram(string vertexPath, string geometryPath, string fragmentPath)
        {
            var vertexHandle   = CreateShader(ShaderType.VertexShader, vertexPath);
            var geometryHandle = CreateShader(ShaderType.GeometryShader, geometryPath);
            var fragmentHandle = CreateShader(ShaderType.FragmentShader, fragmentPath);

            if (CreateProgram(vertexHandle, fragmentHandle, geometryHandle) != 1)
                Dispose();

            DeleteAttachedShader(vertexHandle);
            DeleteAttachedShader(geometryHandle);
            DeleteAttachedShader(fragmentHandle);
        }

        private static string ReadShaderFile(string sourcePath)
        {
            string shaderSource;
            using (var sourcePlace = new StreamReader(sourcePath))
                shaderSource = sourcePlace.ReadToEnd();
            return shaderSource;
        }

        public static void UnUse()
        {
            GL.UseProgram(0);
        }

        public void Use()
        {
            if (_disposed)
                throw new ObjectDisposedException("Shader");
            GL.UseProgram(_handle);
        }

        private int CreateShader(ShaderType type, string shaderPath)
        {
            var shaderHandle = GL.CreateShader(type);
            GL.ShaderSource(shaderHandle, ReadShaderFile(shaderPath));
            GL.CompileShader(shaderHandle);

            GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out int success);
            _logger.Append($"{type.ToString().Remove(10)}. \t status: {success} ");
            _logger.AppendLine(GL.GetShaderInfoLog(shaderHandle));

            return shaderHandle;
        }

        private int CreateProgram(int vert, int frag, params int[] otherShaders)
        {
            _handle = GL.CreateProgram();

            GL.AttachShader(_handle, vert);
            GL.AttachShader(_handle, frag);
            
            foreach (var shader in otherShaders)
                GL.AttachShader(_handle, shader);

            GL.LinkProgram(_handle);
            return ProgramStatus();
        }

        private int ProgramStatus()
        {
            GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out int success);
            _logger.Append($"PROGRAM \t status: {success} ");
            _logger.AppendLine(GL.GetProgramInfoLog(_handle));
            return success;
        }

        private void DeleteAttachedShader(int shaderHandle)
        {
            GL.DetachShader(_handle, shaderHandle);
            GL.DeleteShader(shaderHandle);
        }

        public void Dispose()
        {
            GL.DeleteProgram(_handle);
            _disposed = true;
        }
    }
}

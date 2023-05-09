using System;
using System.Drawing;
using System.Reflection;
using Iakai.Core;
using OpenTK;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Iakai
{
    internal class Game : GameWindow
    {
        private GLMesh _mesh;
        private ShaderProgram _prog;
        private TextureContainer _tex;

        private Matrix4 proj;
        private Matrix4 view;
        private Matrix4 model;
        private Vector3 ls;

        public Game(GameWindowSettings gameSettings, NativeWindowSettings nativeSettings) : base(gameSettings, nativeSettings)
        {
            var size = nativeSettings.Size;

            model = Matrix4.Identity;
            view = Matrix4.Identity;
            view *= Matrix4.CreateTranslation(0f, 0f, -5f);

            proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), (float)size.X / size.Y, 0.1f, 100f);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Cw);

            GL.ClearColor(0f, 0f, 0f, 0f);

            _mesh = new GLMesh();

            var meshData = ObjectArray.ObjectArrayLoader.LoadObject(@".\Resources\Objects\obj.oa");

            _mesh.InsertData(meshData.elements, meshData.indices);

            _prog = new ShaderProgram(@".\Resources\Shaders\vertex.vert", @".\Resources\Shaders\fragment.frag");
            _prog.Use();
            _prog.SetUniform("tex", 0);

            Console.WriteLine(_prog.Log);

            _tex = new TextureContainer(@".\Resources\Textures\dirt.png", TextureWrapMode.Repeat);

            ShaderProgram.UnUse();
        }
        float acc = 0;
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            KeyboardState input = KeyboardState;

            model *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians((float)args.Time * -40f));
            acc += (float)args.Time;
            ls.X = (float)MathHelper.Sin(MathHelper.DegreesToRadians(acc * 400f));
            ls.Z = 1;
            ls.Y = 1;

            if (input.IsKeyDown(Keys.Escape) | _prog.IsDisposed)
            {
                Close();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _prog.Use();
            _tex.Use(TextureUnit.Texture0);
            _mesh.Use();

            _prog.SetUniform("model", model);
            _prog.SetUniform("view", view);
            _prog.SetUniform("projection", proj);
            _prog.SetUniform("lightPos", ls);

            GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);


            _prog.SetUniform("model", Matrix4.CreateTranslation(ls));
            _prog.SetUniform("view", view);
            _prog.SetUniform("projection", proj);
            _prog.SetUniform("lightPos", ls);
            GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), (float)e.Width / e.Height, 0.1f, 100f);
            GL.Viewport(0, 0, e.Width, e.Height);
        }

    }
}

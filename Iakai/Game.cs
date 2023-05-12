using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using Iakai.Core;
using Iakai.Generation;
using ObjectArray;
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
        private GLCamera _cam;
        private GLWorldRenderer _renderer;
        private TextureContainer _dirt;
        private TextureContainer _wall;

        public Game(GameWindowSettings gameSettings, NativeWindowSettings nativeSettings) : base(gameSettings, nativeSettings)
        {
            var size = nativeSettings.Size;

            _cam = new GLCamera()
            {
                AspectRatio = (float)size.X / size.Y,
                Fov = 80f,
                Position = new Vector3(0, 1f, 0f),
            };
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            _renderer = new GLWorldRenderer();
            _renderer.Init();
            _renderer.DistanceOfView = 5;

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Cw);
            GL.ClearColor(0.19f, 0.49f, 0.78f, 0f);

            ShaderProgram.UnUse();
            _dirt = new TextureContainer(@".\Resources\Textures\dirt.png", TextureWrapMode.Repeat);
            _wall = new TextureContainer(@".\Resources\Textures\wall.png", TextureWrapMode.Repeat);

            _renderer.Update(_cam.Position.Xz);

        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            KeyboardState input = KeyboardState;
            MouseState mouse = MouseState;

            _cam.Yaw   += mouse.Delta.X * 0.1f;
            _cam.Pitch -= mouse.Delta.Y * 0.1f;

            float speed = 5f;
            if (input.IsKeyDown(Keys.LeftShift))
                speed *= 10f;

            if (input.IsKeyDown(Keys.W))
                _cam.Position += ((float)args.Time * speed) *  _cam.Front;

            if (input.IsKeyDown(Keys.S))
                _cam.Position += ((float)args.Time * speed) * -_cam.Front;
            
            if (input.IsKeyDown(Keys.A))
                _cam.Position += ((float)args.Time * speed) * -_cam.Right;
            
            if (input.IsKeyDown(Keys.D))
                _cam.Position += ((float)args.Time * speed) *  _cam.Right;
            
            if (input.IsKeyDown(Keys.Space))
                _cam.Position += ((float)args.Time * speed) *  Vector3.UnitY;
            
            if (input.IsKeyDown(Keys.LeftControl))
                _cam.Position += ((float)args.Time * speed) * -Vector3.UnitY;

            if (input.IsKeyPressed(Keys.F1))
            {
                switch (CursorState)
                {
                    case CursorState.Normal:
                        CursorState = CursorState.Grabbed;
                        break;
                    case CursorState.Grabbed:
                        CursorState = CursorState.Normal;
                        break;
                }
            }

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            //if (input.IsKeyPressed(Keys.Tab))
            {
                _renderer.Update(_cam.Position.Xz);
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _dirt.Use(TextureUnit.Texture0);
            _wall.Use(TextureUnit.Texture1);

            _renderer.Render(_cam);

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            _cam.AspectRatio = (float)e.Width / e.Height;

            GL.Viewport(0, 0, e.Width, e.Height);
        }

    }
}

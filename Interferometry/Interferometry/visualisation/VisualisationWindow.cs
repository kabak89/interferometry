﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Interferometry.Visualisation
{
    class VisualisationWindow : GameWindow
    {
        int shaderProgram;

        Vector2 oldMousePos;
        Mesh mesh;
        FreeCamera cam;
        PerspectiveProjeciton proj;
        int projLoc, viewLoc;
        
        public VisualisationWindow(Interferometry.math_classes.ZArrayDescriptor desc, int width, int height, int fsaa_samples, bool vsync)
            : base(width, height, new GraphicsMode(32, 24, 0, fsaa_samples))
        {
            mesh = new Mesh(desc);
            if(!vsync)
                this.VSync = VSyncMode.Off;
        }

        private void CreateProgram()
        {
            ShaderWrapper vshader = ShaderWrapper.FromFile(ShaderType.VertexShader, "..\\..\\visualisation\\shaders\\shader.vert");
            ShaderWrapper fshader = ShaderWrapper.FromFile(ShaderType.FragmentShader, "..\\..\\visualisation\\shaders\\shader.frag");

            vshader.Compile();
            fshader.Compile();

            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vshader.handle);
            GL.AttachShader(shaderProgram, fshader.handle);

            int status;
            GL.LinkProgram(shaderProgram);
            GL.GetProgram(shaderProgram, ProgramParameter.LinkStatus, out status);
            if (status == 0)
                throw new Exception(GL.GetProgramInfoLog(shaderProgram));

            projLoc = GL.GetUniformLocation(shaderProgram, "projection");
            viewLoc = GL.GetUniformLocation(shaderProgram, "view");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CreateProgram();

            cam = new FreeCamera(new Vector3(0, 0, 1), new Vector3(0, 0, -5));
            proj = new PerspectiveProjeciton(3.14159f / 4, 0.01f, 5000f, (float)this.Width / this.Height);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.DepthRange(0.0f, 1.0f);
            //GL.Enable(EnableCap.DepthClamp);
            /*
            GL.Enable(EnableCap.CullFace);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.CullFace(CullFaceMode.Back);
            */
        }

        float shift = 0.01f;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            cam.update();

            if (Mouse[MouseButton.Left])
            {
                cam.rotLeft((oldMousePos.X - Mouse.X) / this.Width * (float) Math.PI);
                cam.rotDown((oldMousePos.Y - Mouse.Y) / this.Height * (float) Math.PI);
            }

            oldMousePos.X = Mouse.X;
            oldMousePos.Y = Mouse.Y;

            shift += 0.1f *shift * (float)Mouse.WheelDelta;
            if (shift < 0.003)
                shift = 0.003f;

            if (Keyboard[Key.W])
                cam.moveForward(shift);
            if (Keyboard[Key.S])
                cam.moveForward(-shift);
            if (Keyboard[Key.A])
                cam.moveRight(-shift);
            if (Keyboard[Key.D])
                cam.moveRight(shift);
            if (Keyboard[Key.Space])
                cam.moveUp(shift);
            if (Keyboard[Key.C])
                cam.moveUp(-shift);

            if (Keyboard[Key.R])
            {
                proj.FieldOfView += 0.01f;
                if (proj.FieldOfView > 3.14f)
                    proj.FieldOfView = 3.14f;
                setProjectionUniform();
            }

            if (Keyboard[Key.F])
            {
                proj.FieldOfView -= 0.01f;
                if (proj.FieldOfView < 0.01f)
                    proj.FieldOfView = 0.01f;
                setProjectionUniform();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            this.Title = this.RenderFrequency.ToString() + " fps";

            GL.UseProgram(shaderProgram);
            Matrix4 hack = cam.Matrix;
            GL.UniformMatrix4(viewLoc, false, ref hack);
            mesh.render();
            GL.UseProgram(0);

            SwapBuffers();
        }

        private void setProjectionUniform()
        {
            GL.UseProgram(shaderProgram);
            Matrix4 hack = proj.Matrix;
            GL.UniformMatrix4(projLoc, false, ref hack);
            GL.UseProgram(0);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);

            proj.AspectRatio = (float)this.Width / this.Height;
            setProjectionUniform();

            OnUpdateFrame(null);
            OnRenderFrame(null);
        }
    }
}

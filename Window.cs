
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using Nevalyashka.Object;
using Nevalyashka.Common;
using Nevalyashka.Render;

namespace Nevalyashka
{
    // Be warned, there is a LOT of stuff here. It might seem complicated, but just take it slow and you'll be fine.
    // OpenGL's initial hurdle is quite large, but once you get past that, things will start making more sense.
    public class Window : GameWindow
    {
        Vector3 LightPos = new Vector3(0.0f, 2.6f, 0.0f);

        Shader Shader;

        Texture DiffuseTail, SpecularTail;
        Texture DiffuseHead, SpecularHead;

        List<ObjectRender> ObjectRenderList = new List<ObjectRender>();

        double Time;
        int Side = 1;
        const double Degrees = 40;
        


        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        private void DefineShader(Shader Shader)
        {
            Shader.SetInt("material.diffuse", 0);
            Shader.SetInt("material.specular", 1);
            Shader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            Shader.SetFloat("material.shininess", 100000.0f);
            Shader.SetVector3("light.position", LightPos);
            Shader.SetFloat("light.constant", 0.1f);
            Shader.SetFloat("light.linear", 0.09f);
            Shader.SetFloat("light.quadratic", 0.032f);
            Shader.SetVector3("light.ambient", new Vector3(0.2f));
            Shader.SetVector3("light.diffuse", new Vector3(0.5f));
            Shader.SetVector3("light.specular", new Vector3(1.0f));
            Shader.Use();
        }



        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            Sphere Butt = new Sphere(0.2f, 0.0f, 0.5f, 0f);
            Sphere Head = new Sphere(0.4f, 0.0f, 0.0f, 0f);

            Shader = new Shader("../../../Shaders/shader.vert", "../../../Shaders/lighting.frag");
            DefineShader(Shader);

            DiffuseHead = Texture.LoadFromFile("../../../Resources/red.jpg");
            SpecularHead = Texture.LoadFromFile("../../../Resources/red_specular.jpg");

            DiffuseTail = Texture.LoadFromFile("../../../Resources/head.jpg");
            SpecularTail = Texture.LoadFromFile("../../../Resources/head_specular.jpg");

            var ButtVert = Butt.GetAll(); var ButtInd = Butt.GetIndices();
            var HeadVert = Head.GetAll(); var HeadInd = Head.GetIndices();
           
            ObjectRenderList.Add(new ObjectRender(ButtVert, ButtInd, Shader, DiffuseTail, SpecularTail));
            ObjectRenderList.Add(new ObjectRender(HeadVert, HeadInd, Shader, DiffuseHead , SpecularHead));
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Time += 35.0 * e.Time * Side * Math.Cos(Time / 30);

            if (Math.Abs(Time) > Degrees) Side *= -1;

            var RotationMatrixZ = Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(Time));
            var RotationMatrixY = Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(90));
            var TranslationMatrix = Matrix4.CreateTranslation(0, 0, (float)(Time / 80));

            var model = Matrix4.Identity * RotationMatrixZ * TranslationMatrix * RotationMatrixY;

            foreach (var Obj in ObjectRenderList)
            {
                Obj.Bind();
                Obj.ApplyTexture();
                Obj.UpdateShaderModel(model);
                Obj.ShaderAttribute();
                Obj.Render();
            }

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused) // Check to see if the window is focused
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

        }

        // In the mouse wheel function, we manage all the zooming of the camera.
        // This is simply done by changing the FOV of the camera.
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
        }
    }
}


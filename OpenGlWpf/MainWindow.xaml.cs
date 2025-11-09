using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
namespace OpenGlWpf
{
	public partial class MainWindow : Window
	{
		private GLControl glControl;
		private DispatcherTimer timer;
		private float angle = 0f;

		public MainWindow()
		{
			InitializeComponent();
			var graphicsMode = new GraphicsMode(32, 24, 0, 4);
			glControl = new GLControl(graphicsMode)
			{
				Dock = System.Windows.Forms.DockStyle.Fill,
				VSync = true  
			};
			glHost.Child = glControl;

			glControl.Load += GlControl_Load;
			glControl.Paint += GlControl_Paint;
			glControl.Resize += GlControl_Resize;

			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(64);
			timer.Tick += (s, e) =>
			{
				angle = (float)ScroolValue.Value;
				glControl.Invalidate(); 
			};
			timer.Start();
		}

		private void GlControl_Load(object sender, EventArgs e)
		{
			GL.ClearColor(System.Drawing.Color.CornflowerBlue);
			GL.Enable(EnableCap.DepthTest);
			SetupViewport();
		}

		private void GlControl_Resize(object sender, EventArgs e)
		{
			SetupViewport();
		}

		private void SetupViewport()
		{
			int w = glControl.Width;
			int h = glControl.Height > 0 ? glControl.Height : 1;
			GL.Viewport(0, 0, w, h);

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GLU.Perspective(45, (float)w / h, 0.1f, 100);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
		}

		private void GlControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.LoadIdentity();
			GL.Translate(0.0f, 0.0f, -6.0f);
			GL.Rotate(angle, 0.0f, 1.0f, 0.0f);

			DrawPyramid();

			glControl.SwapBuffers();
		}

		 

		private void DrawPyramid()
		{
			GL.Begin(BeginMode.Triangles);

			var top = new float[] { 0f, 1f, 0f };

			var baseVertices = new[]
			{
				new float[] { -1f, 0f,  1f }, 
				new float[] {  1f, 0f,  1f },
				new float[] {  1f, 0f, -1f }, 
				new float[] { -1f, 0f, -1f } 
			};

			
			GL.Color3(1.0, 0.0, 0.0);
			GL.Vertex3(top[0], top[1], top[2]);
			GL.Vertex3(baseVertices[0][0], baseVertices[0][1], baseVertices[0][2]);
			GL.Vertex3(baseVertices[1][0], baseVertices[1][1], baseVertices[1][2]);

			GL.Color3(0.0, 1.0, 0.0);
			GL.Vertex3(top[0], top[1], top[2]);
			GL.Vertex3(baseVertices[1][0], baseVertices[1][1], baseVertices[1][2]);
			GL.Vertex3(baseVertices[2][0], baseVertices[2][1], baseVertices[2][2]);

			GL.Color3(0.0, 0.0, 1.0); 
			GL.Vertex3(top[0], top[1], top[2]);
			GL.Vertex3(baseVertices[2][0], baseVertices[2][1], baseVertices[2][2]);
			GL.Vertex3(baseVertices[3][0], baseVertices[3][1], baseVertices[3][2]);

			GL.Color3(1.0, 1.0, 0.0);
			GL.Vertex3(top[0], top[1], top[2]);
			GL.Vertex3(baseVertices[3][0], baseVertices[3][1], baseVertices[3][2]);
			GL.Vertex3(baseVertices[0][0], baseVertices[0][1], baseVertices[0][2]);

			GL.End();

			
			GL.Begin(BeginMode.Quads);
			GL.Color3(0.5, 0.5, 0.5);
			foreach (var v in baseVertices)
			{
				GL.Vertex3(v[0], v[1], v[2]);
			}	
			GL.End();
		}
	}
	public static class GLU
	{
		public static void Perspective(double fov, double aspect, double zNear, double zFar)
		{
			double fH = Math.Tan(fov / 360 * Math.PI) * zNear;
			double fW = fH * aspect;
			GL.Frustum(-fW, fW, -fH, fH, zNear, zFar);
		}
	}
}
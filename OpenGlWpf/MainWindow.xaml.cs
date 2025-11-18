using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
namespace OpenGlWpf
{
	public partial class MainWindow : Window
	{
		private GLControl glControl;
		private DispatcherTimer timer;
		private float angle = 0f;
		float y = -6.0f;
		float x = 0.0f;
		float z = -1.0f;
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
			timer.Interval = TimeSpan.FromMilliseconds(16);
			timer.Tick += (s, e) =>
			{
				angle = (float)ScroolValue.Value;
				glControl.Invalidate(); 
			};
			timer.Start();
		}

		private void GlControl_Load(object sender, EventArgs e)
		{
			GL.ClearColor(System.Drawing.Color.LightGray);
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


		private float[] LookAt(float[] eye, float[] center, float[] up)
		{
			var f = Normalize(Subtract(center, eye));
			var s = Normalize(Cross(f, up));
			var u = Cross(s, f);

			return new float[]
			{
		s[0], u[0], -f[0], 0,
		s[1], u[1], -f[1], 0,
		s[2], u[2], -f[2], 0,
		-Dot(s, eye), -Dot(u, eye), Dot(f, eye), 1
			};
		}

		private float[] Subtract(float[] a, float[] b)
		{
			return new float[] { a[0] - b[0], a[1] - b[1], a[2] - b[2] };
		}

		private float[] Normalize(float[] v)
		{
			float length = (float)Math.Sqrt(Dot(v, v));
			return new float[] { v[0] / length, v[1] / length, v[2] / length };
		}

		private float Dot(float[] a, float[] b)
		{
			return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
		}

		private float[] Cross(float[] a, float[] b)
		{
			return new float[]
			{
		a[1] * b[2] - a[2] * b[1],
		a[2] * b[0] - a[0] * b[2],
		a[0] * b[1] - a[1] * b[0]
			};
		}



		private void GlControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.LoadIdentity();
			GL.Translate(x, z, y);
			GL.Rotate(angle, 0.0f, 1.0f, 0.0f);
			GL.LoadMatrix(LookAt(new float[] { 0, 0, 0 }, new float[] { 0,0,0 }, new float[] { 0, 1, 0 }));
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
			GL.Vertex3(0f, 1f, 0f);
			GL.Vertex3(-1f, 0f, 1f);
			GL.Vertex3(1f, 0f, 1f);

			GL.Color3(0.0, 1.0, 0.0);
			GL.Vertex3(0f, 1f, 0f);
			GL.Vertex3(1f, 0f, 1f);
			GL.Vertex3(1f, 0f, -1f);

			GL.Color3(0.0, 0.0, 1.0);
			GL.Vertex3(0f, 1f, 0f);
			GL.Vertex3(1f, 0f, -1f);
			GL.Vertex3(-1f, 0f, -1f);

			GL.Color3(1.0, 1.0, 0.0);
			GL.Vertex3(0f, 1f, 0f);
			GL.Vertex3(-1f, 0f, -1f);
			GL.Vertex3(-1f, 0f, 1f);
			GL.End();

			GL.Begin(BeginMode.Quads);
			GL.Color3(0.5, 0.5, 0.5);
			GL.Vertex3(-1f, 0f, 1f);
			GL.Vertex3(1f, 0f, 1f);
			GL.Vertex3(1f, 0f, -1f);
			GL.Vertex3(-1f, 0f, -1f);
			GL.End();
		}
		private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			switch(e.Key)
			{
				case System.Windows.Input.Key.W:
					y += 0.1f;
					break;
				case System.Windows.Input.Key.S:
					y += -0.1f;
					break;
				case System.Windows.Input.Key.D:
					x += -0.1f;
					break;
				case System.Windows.Input.Key.A:
					x += 0.1f;
					break;
				case System.Windows.Input.Key.Space:
					z += -0.1f;
					break;
				case System.Windows.Input.Key.LeftShift:
					z += 0.1f;
					break;
			}
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
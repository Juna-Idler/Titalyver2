using System;

using System.Windows;
using System.Windows.Input;


namespace Juna
{
    public class WindowMoveSize
	{
		private Window This;

		private bool isDragLeft = false;
		public bool IsDragLeft { get { return isDragLeft; } }

		private Point DragPoint;
		private Size DragSize;


		private int sizingBarSize = 8;
		private int cornerSize = 16;

		public int SizingBarSize
		{
			get { return sizingBarSize; }
			set
			{
				if (value < 0)
					sizingBarSize = 0;
				else if (value > This.ActualWidth / 4 || value > This.ActualHeight / 4)
				{
					sizingBarSize = (int)Math.Min(This.ActualWidth, This.ActualHeight) / 4;
				}
				else
					sizingBarSize = value;
			}
		}
		public int CornerSize
		{
			get { return cornerSize; }
			set
			{
				if (value < 0)
					cornerSize = 0;
				else if (value > This.ActualWidth / 2 || value > This.ActualHeight / 2)
				{
					cornerSize = (int)Math.Min(This.ActualWidth, This.ActualHeight) / 2;
				}
				else
					cornerSize = value;
			}
		}

		private Cursor DefaultCursor;


		enum enumDragType
		{
			bit_left = 1,
			bit_right = 2,
			bit_top = 4,
			bit_bottom = 8,

			TopLeft = bit_top + bit_left,
			TopMiddle = bit_top,
			TopRight = bit_top + bit_right,
			MiddleLeft = bit_left,
			MiddleMiddle = 0,
			MiddleRight = bit_right,
			BottomLeft = bit_bottom + bit_left,
			BottomMiddle = bit_bottom,
			BottomRight = bit_bottom + bit_right,
		}

		private enumDragType DragType;


		public WindowMoveSize(Window window, int sizing_bar_size, int corner_size)
		{
			This = window;
			sizingBarSize = sizing_bar_size;
			cornerSize = corner_size;
			DefaultCursor = window.Cursor;

			This.MouseDown += new MouseButtonEventHandler(MouseDown);
			This.MouseMove += new MouseEventHandler(MouseMove);
			This.MouseUp += new MouseButtonEventHandler(MouseUp);
		}

		/*
				public const int WM_NCHITTEST = 0x0084;
				public const int HTCLIENT = 1;
				public void WndProc_WM_NCHITTEST(ref Message m)
				{
					m.Result = (IntPtr)HTCLIENT;
				}
		*/
		private void MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				DragPoint = e.GetPosition(This);
				DragSize = new(This.ActualWidth, This.ActualHeight);

				isDragLeft = true;
				Mouse.Capture(This);
			}
		}

		private void MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Source != sender)
			{
				isDragLeft = false;
				This.Cursor = DefaultCursor;
				return;
			}

			if (This.WindowState == WindowState.Maximized)
			{
				isDragLeft = false;
				This.Cursor = DefaultCursor;
				return;
			}

			if (isDragLeft)
			{
				Point pt = e.GetPosition(This);
				double move_x = pt.X - DragPoint.X;
				double move_y = pt.Y - DragPoint.Y;
//				Rect wa = new(SystemParameters.VirtualScreenLeft,
//								SystemParameters.VirtualScreenTop,
//								SystemParameters.VirtualScreenWidth,
//								SystemParameters.VirtualScreenHeight);

				Rect wa = SystemParameters.WorkArea;

				//				Rectangle wa = Screen.GetWorkingArea( This );

				if (DragType == enumDragType.MiddleMiddle)
				{
					Point oldp =  new(This.Left, This.Top);
					Point newp = new(oldp.X + move_x, oldp.Y + move_y);
					if (e.RightButton != MouseButtonState.Pressed)
					{
						if (oldp.X >= wa.Left && newp.X < wa.Left)
						{
							newp.X = wa.Left;
						}
						if (oldp.Y >= wa.Top && newp.Y < wa.Top)
						{
							newp.Y = wa.Top;
						}
						if (oldp.X + This.ActualWidth <= wa.Right && newp.X + This.ActualWidth > wa.Right)
						{
							newp.X = wa.Right - This.ActualWidth;
						}
						if (oldp.Y + This.ActualHeight <= wa.Bottom && newp.Y + This.ActualHeight > wa.Bottom)
						{
							newp.Y = wa.Bottom - This.ActualHeight;
						}
					}

					This.Left = newp.X;
					This.Top = newp.Y;
				}
				else
				{
					Point newp = new(This.Left, This.Top);
					double new_width = This.ActualWidth;
					double new_height = This.ActualHeight;

					if ((DragType & enumDragType.bit_left) != 0)
					{
						newp.X += move_x;
						new_width -= move_x;

						if (e.RightButton != MouseButtonState.Pressed)
						{
							if (This.Left >= wa.Left && newp.X < wa.Left)
							{
								new_width -= wa.Left - newp.X;
								newp.X = wa.Left;
							}
						}
						if (new_width < This.MinWidth)
						{
							newp.X -= This.MinWidth - new_width;
							new_width = This.MinWidth;
						}
					}
					else if ((DragType & enumDragType.bit_right) != 0)
					{
						double right = newp.X + DragSize.Width + move_x;
						new_width = DragSize.Width + move_x;
						if (e.RightButton != MouseButtonState.Pressed)
						{
							if (This.Left + This.ActualWidth <= wa.Right && right > wa.Right)
							{
								new_width -= right - wa.Right;
							}
						}
						if (new_width < This.MinWidth)
						{
							new_width = This.MinWidth;
						}
					}

					if ((DragType & enumDragType.bit_top) != 0)
					{
						newp.Y += move_y;
						new_height -= move_y;

						if (e.RightButton != MouseButtonState.Pressed)
						{
							if (This.Top >= wa.Top && newp.Y < wa.Top)
							{
								new_height -= wa.Top - newp.Y;
								newp.Y = wa.Top;
							}
						}

						if (new_height < This.MinHeight)
						{
							newp.Y -= This.MinHeight - new_height;
							new_height = This.MinHeight;
						}
					}
					else if ((DragType & enumDragType.bit_bottom) != 0)
					{
						double bottom = newp.Y + DragSize.Height + move_y;
						new_height = DragSize.Height + move_y;
						if (e.RightButton != MouseButtonState.Pressed)
						{
							if (This.Top + This.ActualHeight <= wa.Bottom && bottom > wa.Bottom)
							{
								new_height -= bottom - wa.Bottom;
							}
						}
						if (new_height < This.MinHeight)
						{
							new_height = This.MinHeight;
						}
					}

					This.Width = new_width;
					This.Height = new_height;
					This.Left = newp.X;
					This.Top = newp.Y;
				}
			}
			else
			{
				Point p = e.GetPosition(This);

				if (p.X < sizingBarSize)
				{
					if (p.Y < cornerSize)
					{
						DragType = enumDragType.TopLeft;
						This.Cursor = Cursors.SizeNWSE;
						return;
					}
					if (p.Y >= This.ActualHeight - cornerSize)
					{
						DragType = enumDragType.BottomLeft;
						This.Cursor = Cursors.SizeNESW;
						return;
					}
					DragType = enumDragType.MiddleLeft;
					This.Cursor = Cursors.SizeWE;
					return;
				}
				if (p.X >= This.ActualWidth - sizingBarSize)
				{
					if (p.Y < cornerSize)
					{
						DragType = enumDragType.TopRight;
						This.Cursor = Cursors.SizeNESW;
						return;
					}
					if (p.Y >= This.ActualHeight - cornerSize)
					{
						DragType = enumDragType.BottomRight;
						This.Cursor = Cursors.SizeNWSE;
						return;
					}
					DragType = enumDragType.MiddleRight;
					This.Cursor = Cursors.SizeWE;
					return;
				}
				if (p.Y < sizingBarSize)
				{
					if (p.X < cornerSize)
					{
						DragType = enumDragType.TopLeft;
						This.Cursor = Cursors.SizeNWSE;
						return;
					}
					if (p.X >= This.ActualWidth - cornerSize)
					{
						DragType = enumDragType.TopRight;
						This.Cursor = Cursors.SizeNESW;
						return;
					}
					DragType = enumDragType.TopMiddle;
					This.Cursor = Cursors.SizeNS;
					return;
				}
				if (p.Y >= This.ActualHeight - sizingBarSize)
				{
					if (p.X < cornerSize)
					{
						DragType = enumDragType.BottomLeft;
						This.Cursor = Cursors.SizeNESW;
						return;
					}
					if (p.X >= This.ActualWidth - cornerSize)
					{
						DragType = enumDragType.BottomRight;
						This.Cursor = Cursors.SizeNWSE;
						return;
					}
					DragType = enumDragType.BottomMiddle;
					This.Cursor = Cursors.SizeNS;
					return;
				}
				DragType = enumDragType.MiddleMiddle;
				This.Cursor = DefaultCursor;
				return;
			}

		}

		private void MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				isDragLeft = false;
				Mouse.Capture(null);
			}
		}
	}
}

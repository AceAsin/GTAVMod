//
// Copyright (C) 2015 crosire & contributors
// License: https://github.com/crosire/scripthookvdotnet#license
//

using GTA.Math;
using GTA.Native;
using System;

namespace GTA
{
	public sealed class Checkpoint : PoolObject
	{
		public Checkpoint(int handle) : base(handle)
		{
		}

		/// <summary>
		/// Gets the memory address of this <see cref="Checkpoint"/>.
		/// </summary>
		public IntPtr MemoryAddress => SHVDN.NativeMemory.GetCheckpointAddress(Handle);

		/// <summary>
		/// Gets or sets the position of this <see cref="Checkpoint"/>.
		/// </summary>
		public Vector3 Position
		{
			get
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return Vector3.Zero;
				}
				return new Vector3(SHVDN.NativeMemory.ReadVector3(memoryAddress));
			}
			set
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return;
				}
				SHVDN.NativeMemory.WriteVector3(memoryAddress, value.ToArray());
			}
		}

		/// <summary>
		/// Gets or sets the position where this <see cref="Checkpoint"/> points to.
		/// </summary>
		public Vector3 TargetPosition
		{
			get
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return Vector3.Zero;
				}
				return new Vector3(SHVDN.NativeMemory.ReadVector3(memoryAddress + 16));
			}
			set
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return;
				}
				SHVDN.NativeMemory.WriteVector3(memoryAddress + 16, value.ToArray());
			}
		}

		/// <summary>
		/// Gets or sets the icon drawn in this <see cref="Checkpoint"/>.
		/// </summary>
		public CheckpointIcon Icon
		{
			get
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return 0;
				}
				return (CheckpointIcon)SHVDN.NativeMemory.ReadInt32(memoryAddress + 56);
			}
			set
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return;
				}
				SHVDN.NativeMemory.WriteInt32(memoryAddress + 56, (int)value);
			}
		}

		/// <summary>
		/// Gets or sets a custom icon to be drawn in this <see cref="Checkpoint"/>.
		/// </summary>
		public CheckpointCustomIcon CustomIcon
		{
			get
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return 0;
				}
				return SHVDN.NativeMemory.ReadByte(memoryAddress + 52);
			}
			set
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return;
				}
				SHVDN.NativeMemory.WriteByte(memoryAddress + 52, value);
				SHVDN.NativeMemory.WriteInt32(memoryAddress + 56, 42); // Sets the icon to a custom icon
			}
		}

		/// <summary>
		/// Gets or sets the radius of this <see cref="Checkpoint"/>.
		/// </summary>
		public float Radius
		{
			get
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return 0.0f;
				}
				return SHVDN.NativeMemory.ReadFloat(memoryAddress + 60);
			}
			set
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return;
				}
				SHVDN.NativeMemory.WriteFloat(memoryAddress + 60, value);
			}
		}

		/// <summary>
		/// Gets or sets the color of this <see cref="Checkpoint"/>.
		/// </summary>
		public System.Drawing.Color Color
		{
			get
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return System.Drawing.Color.Transparent;
				}
				return System.Drawing.Color.FromArgb(SHVDN.NativeMemory.ReadInt32(memoryAddress + 80));
			}
			set
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return;
				}
				SHVDN.NativeMemory.WriteInt32(memoryAddress + 80, Color.ToArgb());
			}
		}
		/// <summary>
		/// Gets or sets the color of the icon in this <see cref="Checkpoint"/>.
		/// </summary>
		public System.Drawing.Color IconColor
		{
			get
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return System.Drawing.Color.Transparent;
				}
				return System.Drawing.Color.FromArgb(SHVDN.NativeMemory.ReadInt32(memoryAddress + 84));
			}
			set
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return;
				}
				SHVDN.NativeMemory.WriteInt32(memoryAddress + 84, Color.ToArgb());
			}
		}

		/// <summary>
		/// Gets or sets the radius of the cylinder in this <see cref="Checkpoint"/>.
		/// </summary>
		public float CylinderRadius
		{
			get
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return 0.0f;
				}
				return SHVDN.NativeMemory.ReadFloat(memoryAddress + 76);
			}
			set
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return;
				}
				SHVDN.NativeMemory.WriteFloat(memoryAddress + 76, value);
			}
		}
		/// <summary>
		/// Gets or sets the near height of the cylinder of this <see cref="Checkpoint"/>.
		/// </summary>
		public float CylinderNearHeight
		{
			get
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return 0.0f;
				}
				return SHVDN.NativeMemory.ReadFloat(memoryAddress + 68);
			}
			set
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return;
				}
				SHVDN.NativeMemory.WriteFloat(memoryAddress + 68, value);
			}
		}
		/// <summary>
		/// Gets or sets the far height of the cylinder of this <see cref="Checkpoint"/>.
		/// </summary>
		public float CylinderFarHeight
		{
			get
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return 0.0f;
				}
				return SHVDN.NativeMemory.ReadFloat(memoryAddress + 72);
			}
			set
			{
				IntPtr memoryAddress = MemoryAddress;
				if (memoryAddress == IntPtr.Zero)
				{
					return;
				}
				SHVDN.NativeMemory.WriteFloat(memoryAddress + 72, value);
			}
		}

		/// <summary>
		/// Removes this <see cref="Checkpoint"/>.
		/// </summary>
		public override void Delete()
		{
			Function.Call(Hash.DELETE_CHECKPOINT, Handle);
		}

		/// <summary>
		/// Determines if this <see cref="Checkpoint"/> exists.
		/// </summary>
		/// <returns><c>true</c> if this <see cref="Checkpoint"/> exists; otherwise, <c>false</c>.</returns>
		public override bool Exists()
		{
			return Handle != 0 && MemoryAddress != IntPtr.Zero;
		}

		/// <summary>
		/// Determines if an <see cref="object"/> refers to the same checkpoint as this <see cref="Checkpoint"/>.
		/// </summary>
		/// <param name="obj">The <see cref="object"/> to check.</param>
		/// <returns><c>true</c> if the <paramref name="obj"/> is the same checkpoint as this <see cref="Checkpoint"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			if (obj is Checkpoint checkpoint)
			{
				return Handle == checkpoint.Handle;
			}

			return false;
		}

		/// <summary>
		/// Determines if two <see cref="Checkpoint"/>s refer to the same checkpoint.
		/// </summary>
		/// <param name="left">The left <see cref="Checkpoint"/>.</param>
		/// <param name="right">The right <see cref="Checkpoint"/>.</param>
		/// <returns><c>true</c> if <paramref name="left"/> is the same checkpoint as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
		public static bool operator ==(Checkpoint left, Checkpoint right)
		{
			return left is null ? right is null : left.Equals(right);
		}
		/// <summary>
		/// Determines if two <see cref="Checkpoint"/>s don't refer to the same checkpoint.
		/// </summary>
		/// <param name="left">The left <see cref="Checkpoint"/>.</param>
		/// <param name="right">The right <see cref="Checkpoint"/>.</param>
		/// <returns><c>true</c> if <paramref name="left"/> is not the same checkpoint as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
		public static bool operator !=(Checkpoint left, Checkpoint right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Converts a <see cref="Checkpoint"/> to a native input argument.
		/// </summary>
		public static implicit operator InputArgument(Checkpoint value)
		{
			return new InputArgument((ulong)value.Handle);
		}

		public override int GetHashCode()
		{
			return Handle.GetHashCode();
		}
	}
}

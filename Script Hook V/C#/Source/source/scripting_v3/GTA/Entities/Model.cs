//
// Copyright (C) 2015 crosire & contributors
// License: https://github.com/crosire/scripthookvdotnet#license
//

using GTA.Math;
using GTA.Native;
using System;
using SHVDN;

namespace GTA
{
	public struct Model : IEquatable<Model>, INativeValue
	{
		public Model(int hash) : this()
		{
			Hash = hash;
		}
		public Model(string name) : this(Game.GenerateHash(name))
		{
		}
		public Model(PedHash hash) : this((int)hash)
		{
		}
		public Model(WeaponHash hash) : this((int)hash)
		{
		}
		public Model(VehicleHash hash) : this((int)hash)
		{
		}

		/// <summary>
		/// Gets the hash for this <see cref="Model"/>.
		/// </summary>
		public int Hash
		{
			get; private set;
		}

		/// <summary>
		/// Gets the native representation of this <see cref="Model"/>.
		/// </summary>
		public ulong NativeValue
		{
			get => (ulong)Hash;
			set => Hash = unchecked((int)value);
		}

		/// <summary>
		/// Gets if this <see cref="Model"/> is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if this <see cref="Model"/> is valid; otherwise, <c>false</c>.
		/// </value>
		public bool IsValid => Function.Call<bool>(Native.Hash.IS_MODEL_VALID, Hash);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is in the CD image.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is in the CD image; otherwise, <c>false</c>.
		/// </value>
		public bool IsInCdImage => Function.Call<bool>(Native.Hash.IS_MODEL_IN_CDIMAGE, Hash);

		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is loaded so it can be spawned.
		/// </summary>
		/// <value>
		///   <c>true</c> if this <see cref="Model"/> is loaded; otherwise, <c>false</c>.
		/// </value>
		public bool IsLoaded => Function.Call<bool>(Native.Hash.HAS_MODEL_LOADED, Hash);
		/// <summary>
		/// Gets a value indicating whether the collision for this <see cref="Model"/> is loaded.
		/// </summary>
		/// <value>
		/// <c>true</c> if the collision is loaded; otherwise, <c>false</c>.
		/// </value>
		public bool IsCollisionLoaded => Function.Call<bool>(Native.Hash.HAS_COLLISION_FOR_MODEL_LOADED, Hash);

		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is an amphibious car.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is an amphibious car; otherwise, <c>false</c>.
		/// </value>
		public bool IsAmphibiousCar => Game.Version >= GameVersion.v1_0_944_2_Steam && Function.Call<bool>(Native.Hash._IS_THIS_MODEL_AN_AMPHIBIOUS_CAR, Hash);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is an amphibious quad bike.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is an amphibious quad bike; otherwise, <c>false</c>.
		/// </value>
		public bool IsAmphibiousQuadBike => Game.Version >= GameVersion.v1_0_944_2_Steam && Function.Call<bool>(Native.Hash._IS_THIS_MODEL_AN_AMPHIBIOUS_QUADBIKE, Hash);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is an amphibious vehicle.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is an amphibious vehicle; otherwise, <c>false</c>.
		/// </value>
		public bool IsAmphibiousVehicle => IsAmphibiousCar || IsAmphibiousQuadBike;
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a bicycle.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a bicycle; otherwise, <c>false</c>.
		/// </value>
		public bool IsBicycle => Function.Call<bool>(Native.Hash.IS_THIS_MODEL_A_BICYCLE, Hash);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a big vehicle whose vehicle flag has "FLAG_BIG".
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a big vehicle; otherwise, <c>false</c>.
		/// </value>
		public bool IsBigVehicle => SHVDN.NativeMemory.HasVehicleFlag(Hash, NativeMemory.VehicleFlag1.Big);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a motorbike.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a motorbike; otherwise, <c>false</c>.
		/// </value>
		public bool IsBike => Function.Call<bool>(Native.Hash.IS_THIS_MODEL_A_BIKE, Hash);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a blimp.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a blimp; otherwise, <c>false</c>.
		/// </value>
		public bool IsBlimp => SHVDN.NativeMemory.IsModelABlimp(Hash);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a boat.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a boat; otherwise, <c>false</c>.
		/// </value>
		public bool IsBoat => Function.Call<bool>(Native.Hash.IS_THIS_MODEL_A_BOAT, Hash);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is an emergency vehicle.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is an emergency vehicle; otherwise, <c>false</c>.
		/// </value>
		public bool IsBus => SHVDN.NativeMemory.HasVehicleFlag(Hash, NativeMemory.VehicleFlag1.IsBus);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a car.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a car; otherwise, <c>false</c>.
		/// </value>
		public bool IsCar => Function.Call<bool>(Native.Hash.IS_THIS_MODEL_A_CAR, Hash);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a cargobob.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a cargobob; otherwise, <c>false</c>.
		/// </value>
		public bool IsCargobob => (VehicleHash)Hash == VehicleHash.Cargobob || (VehicleHash)Hash == VehicleHash.Cargobob2 || (VehicleHash)Hash == VehicleHash.Cargobob3 || (VehicleHash)Hash == VehicleHash.Cargobob4;
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a donk car.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a donk car; otherwise, <c>false</c>.
		/// </value>
		public bool IsDonk => SHVDN.NativeMemory.HasVehicleFlag(Hash, NativeMemory.VehicleFlag2.HasLowriderDonkHydraulics);

		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is an electric vehicle.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is an electric vehicle; otherwise, <c>false</c>.
		/// </value>
		public bool IsElectricVehicle => SHVDN.NativeMemory.HasVehicleFlag(Hash, NativeMemory.VehicleFlag1.IsElectric);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is an emergency vehicle.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is an emergency vehicle; otherwise, <c>false</c>.
		/// </value>
		public bool IsEmergencyVehicle => SHVDN.NativeMemory.HasVehicleFlag(Hash, NativeMemory.VehicleFlag1.EmergencyService);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a helicopter.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a helicopter; otherwise, <c>false</c>.
		/// </value>
		public bool IsHelicopter => Function.Call<bool>(Native.Hash.IS_THIS_MODEL_A_HELI, Hash);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a jet ski.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a jet ski; otherwise, <c>false</c>.
		/// </value>
		public bool IsJetSki => Function.Call<bool>(Native.Hash.IS_THIS_MODEL_A_JETSKI, Hash);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a law enforcement vehicle.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a law enforcement vehicle; otherwise, <c>false</c>.
		/// </value>
		public bool IsLawEnforcementVehicle => SHVDN.NativeMemory.HasVehicleFlag(Hash, NativeMemory.VehicleFlag1.LawEnforcement);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a regular lowrider.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a regular lowrider; otherwise, <c>false</c>.
		/// </value>
		public bool IsLowrider => SHVDN.NativeMemory.HasVehicleFlag(Hash, NativeMemory.VehicleFlag2.HasLowriderHydraulics);

		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is an off-road vehicle.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is an off-road vehicle; otherwise, <c>false</c>.
		/// </value>
		public bool IsOffRoadVehicle => SHVDN.NativeMemory.HasVehicleFlag(Hash, NativeMemory.VehicleFlag1.IsOffroadVehicle);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a pedestrian.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a pedestrian; otherwise, <c>false</c>.
		/// </value>
		public bool IsPed => SHVDN.NativeMemory.IsModelAPed(Hash);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a plane.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a plane; otherwise, <c>false</c>.
		/// </value>
		public bool IsPlane => Function.Call<bool>(Native.Hash.IS_THIS_MODEL_A_PLANE, Hash);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a prop.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a prop; otherwise, <c>false</c>.
		/// </value>
		public bool IsProp => IsValid && !IsPed && !IsVehicle;
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a quad bike.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a quad bike; otherwise, <c>false</c>.
		/// </value>
		public bool IsQuadBike => Function.Call<bool>(Native.Hash.IS_THIS_MODEL_A_QUADBIKE, Hash);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a submarine car.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is an submarine car; otherwise, <c>false</c>.
		/// </value>
		public bool IsSubmarineCar => SHVDN.NativeMemory.IsModelASubmarineCar(Hash);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a tank.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a tank; otherwise, <c>false</c>.
		/// </value>
		public bool IsTank => SHVDN.NativeMemory.HasVehicleFlag(Hash, NativeMemory.VehicleFlag2.IsTank);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a train.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a train; otherwise, <c>false</c>.
		/// </value>
		public bool IsTrain => Function.Call<bool>(Native.Hash.IS_THIS_MODEL_A_TRAIN, Hash);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a trailer.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a trailer; otherwise, <c>false</c>.
		/// </value>
		public bool IsTrailer => SHVDN.NativeMemory.IsModelATrailer(Hash);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a van.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a van; otherwise, <c>false</c>.
		/// </value>
		public bool IsVan => SHVDN.NativeMemory.HasVehicleFlag(Hash, NativeMemory.VehicleFlag1.IsVan);
		/// <summary>
		/// Gets a value indicating whether this <see cref="Model"/> is a vehicle.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Model"/> is a vehicle; otherwise, <c>false</c>.
		/// </value>
		public bool IsVehicle => Function.Call<bool>(Native.Hash.IS_MODEL_A_VEHICLE, Hash);

		/// <summary>
		/// Gets the dimensions of this <see cref="Model"/>.
		/// </summary>
		/// <returns>
		/// rearBottomLeft is the minimum dimensions, which contains the rear bottom left relative offset from the origin of the model,
		///  frontTopRight is the maximum dimensions, which contains the front top right relative offset from the origin of the model.
		/// </returns>
		public (Vector3 rearBottomLeft, Vector3 frontTopRight) Dimensions
		{
			get
			{
				NativeVector3 min, max;
				unsafe
				{
					Function.Call(Native.Hash.GET_MODEL_DIMENSIONS, Hash, &min, &max);
				}

				return (min, max);
			}
		}

		/// <summary>
		/// Attempts to load this <see cref="Model"/> into memory.
		/// </summary>
		public void Request()
		{
			Function.Call(Native.Hash.REQUEST_MODEL, Hash);
		}
		/// <summary>
		/// Attempts to load this <see cref="Model"/> into memory for a given period of time.
		/// </summary>
		/// <param name="timeout">The time (in milliseconds) before giving up trying to load this <see cref="Model"/>.</param>
		/// <returns><c>true</c> if this <see cref="Model"/> is loaded; otherwise, <c>false</c>.</returns>
		public bool Request(int timeout)
		{
			Request();

			DateTime endtime = timeout >= 0 ? DateTime.UtcNow + new TimeSpan(0, 0, 0, 0, timeout) : DateTime.MaxValue;

			while (!IsLoaded)
			{
				Script.Yield();
				Request();

				if (DateTime.UtcNow >= endtime)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Attempts to load this <see cref="Model"/>'s collision into memory.
		/// </summary>
		public void RequestCollision()
		{
			Function.Call(Native.Hash.REQUEST_COLLISION_FOR_MODEL, Hash);
		}
		/// <summary>
		/// Attempts to load this <see cref="Model"/>'s collision into memory for a given period of time.
		/// </summary>
		/// <param name="timeout">The time (in milliseconds) before giving up trying to load this <see cref="Model"/>.</param>
		/// <returns><c>true</c> if this <see cref="Model"/>'s collision is loaded; otherwise, <c>false</c>.</returns>
		public bool RequestCollision(int timeout)
		{
			Request();

			DateTime endtime = timeout >= 0 ? DateTime.UtcNow + new TimeSpan(0, 0, 0, 0, timeout) : DateTime.MaxValue;

			while (!IsLoaded)
			{
				Script.Yield();
				RequestCollision();

				if (DateTime.UtcNow >= endtime)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Tells the game we have finished using this <see cref="Model"/> and it can be freed from memory.
		/// </summary>
		public void MarkAsNoLongerNeeded()
		{
			Function.Call(Native.Hash.SET_MODEL_AS_NO_LONGER_NEEDED, Hash);
		}

		public bool Equals(Model model)
		{
			return Hash == model.Hash;
		}
		public override bool Equals(object obj)
		{
			if (obj is Model model)
			{
				return Equals(model);
			}

			return false;
		}

		public static bool operator ==(Model left, Model right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(Model left, Model right)
		{
			return !left.Equals(right);
		}

		public static implicit operator int(Model source)
		{
			return source.Hash;
		}
		public static implicit operator PedHash(Model source)
		{
			return (PedHash)source.Hash;
		}
		public static implicit operator WeaponHash(Model source)
		{
			return (WeaponHash)source.Hash;
		}
		public static implicit operator VehicleHash(Model source)
		{
			return (VehicleHash)source.Hash;
		}

		public static implicit operator Model(int source)
		{
			return new Model(source);
		}
		public static implicit operator Model(string source)
		{
			return new Model(source);
		}
		public static implicit operator Model(PedHash source)
		{
			return new Model(source);
		}
		public static implicit operator Model(WeaponHash source)
		{
			return new Model(source);
		}
		public static implicit operator Model(VehicleHash source)
		{
			return new Model(source);
		}

		public static implicit operator InputArgument(Model value)
		{
			return new InputArgument((ulong)value.Hash);
		}

		public override int GetHashCode()
		{
			return Hash;
		}

		public override string ToString()
		{
			return "0x" + Hash.ToString("X");
		}
	}
}

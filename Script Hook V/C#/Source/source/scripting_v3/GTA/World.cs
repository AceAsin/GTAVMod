//
// Copyright (C) 2015 crosire & contributors
// License: https://github.com/crosire/scripthookvdotnet#license
//

using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace GTA
{
	public static class World
	{
		#region Fields
		static readonly string[] weatherNames = {
			"EXTRASUNNY",
			"CLEAR",
			"CLOUDS",
			"SMOG",
			"FOGGY",
			"OVERCAST",
			"RAIN",
			"THUNDER",
			"CLEARING",
			"NEUTRAL",
			"SNOW",
			"BLIZZARD",
			"SNOWLIGHT",
			"XMAS",
			"HALLOWEEN"
		};

		static readonly GregorianCalendar calendar = new GregorianCalendar();
		#endregion

		#region Time & Day

		/// <summary>
		/// Pauses or resumes the in-game clock.
		/// </summary>
		/// <param name="value">Pauses the game clock if set to <c>true</c>; otherwise, resumes the game clock.</param>
		public static void PauseClock(bool value)
		{
			Function.Call(Hash.PAUSE_CLOCK, value);
		}

		/// <summary>
		/// Gets or sets the current date and time in the GTA World.
		/// </summary>
		/// <value>
		/// The current date and time.
		/// </value>
		public static DateTime CurrentDate
		{
			get
			{
				int year = Function.Call<int>(Hash.GET_CLOCK_YEAR);
				int month = Function.Call<int>(Hash.GET_CLOCK_MONTH) + 1;
				int day = System.Math.Min(Function.Call<int>(Hash.GET_CLOCK_DAY_OF_MONTH), calendar.GetDaysInMonth(year, month));
				int hour = Function.Call<int>(Hash.GET_CLOCK_HOURS);
				int minute = Function.Call<int>(Hash.GET_CLOCK_MINUTES);
				int second = Function.Call<int>(Hash.GET_CLOCK_SECONDS);

				return new DateTime(year, month, day, hour, minute, second);
			}
			set
			{
				Function.Call(Hash.SET_CLOCK_DATE, value.Day, value.Month - 1, value.Year);
				Function.Call(Hash.SET_CLOCK_TIME, value.Hour, value.Minute, value.Second);
			}
		}

		/// <summary>
		/// Gets or sets the current time of day in the GTA World.
		/// </summary>
		/// <value>
		/// The current time of day
		/// </value>
		public static TimeSpan CurrentTimeOfDay
		{
			get
			{
				int hours = Function.Call<int>(Hash.GET_CLOCK_HOURS);
				int minutes = Function.Call<int>(Hash.GET_CLOCK_MINUTES);
				int seconds = Function.Call<int>(Hash.GET_CLOCK_SECONDS);

				return new TimeSpan(hours, minutes, seconds);
			}
			set => Function.Call(Hash.SET_CLOCK_TIME, value.Hours, value.Minutes, value.Seconds);
		}

		#endregion

		#region Weather & Effects

		/// <summary>
		/// Sets a value indicating whether lights in the <see cref="World"/> should be rendered.
		/// </summary>
		/// <value>
		///   <c>true</c> if blackout; otherwise, <c>false</c>.
		/// </value>
		public static bool Blackout
		{
			set => Function.Call(Hash.SET_ARTIFICIAL_LIGHTS_STATE, value);
		}

		/// <summary>
		/// Gets or sets the weather.
		/// </summary>
		/// <value>
		/// The weather.
		/// </value>
		public static Weather Weather
		{
			get
			{
				for (int i = 0; i < weatherNames.Length; i++)
				{
					if (Function.Call<int>(Hash.GET_PREV_WEATHER_TYPE_HASH_NAME) == Game.GenerateHash(weatherNames[i]))
					{
						return (Weather)i;
					}
				}

				return Weather.Unknown;
			}
			set
			{
				if (Enum.IsDefined(typeof(Weather), value) && value != Weather.Unknown)
				{
					Function.Call(Hash.SET_WEATHER_TYPE_NOW, weatherNames[(int)value]);
				}
			}
		}
		/// <summary>
		/// Gets or sets the next weather.
		/// </summary>
		/// <value>
		/// The next weather.
		/// </value>
		public static Weather NextWeather
		{
			get
			{
				for (int i = 0; i < weatherNames.Length; i++)
				{
					if (Function.Call<bool>(Hash.IS_NEXT_WEATHER_TYPE, weatherNames[i]))
					{
						return (Weather)i;
					}
				}

				return Weather.Unknown;
			}
			set
			{
				if (Enum.IsDefined(typeof(Weather), value) && value != Weather.Unknown)
				{
					int currentWeatherHash, nextWeatherHash;
					float weatherTransition;
					unsafe
					{
						Function.Call(Hash._GET_WEATHER_TYPE_TRANSITION, &currentWeatherHash, &nextWeatherHash, &weatherTransition);
					}
					Function.Call(Hash._SET_WEATHER_TYPE_TRANSITION, currentWeatherHash, Game.GenerateHash(weatherNames[(int)value]), 0.0f);
				}
			}
		}

		/// <summary>
		/// Transitions to weather.
		/// </summary>
		/// <param name="weather">The weather.</param>
		/// <param name="duration">The duration.</param>
		public static void TransitionToWeather(Weather weather, float duration)
		{
			if (Enum.IsDefined(typeof(Weather), weather) && weather != Weather.Unknown)
			{
				Function.Call(Hash.SET_WEATHER_TYPE_OVERTIME_PERSIST, weatherNames[(int)weather], duration);
			}
		}

		/// <summary>
		/// Sets the gravity level for all <see cref="World"/> objects.
		/// </summary>
		/// <value>
		/// The gravity level:
		/// 9.8f - Default gravity.
		/// 2.4f - Moon gravity.
		/// 0.1f - Very low gravity.
		/// 0.0f - No gravity.
		/// </value>
		public static float GravityLevel
		{
			get => SHVDN.NativeMemory.WorldGravity;
			set
			{
				// Write the value you want to the first item in the array where the native reads the gravity level choices from
				SHVDN.NativeMemory.WorldGravity = value;
				// Call set_gravity_level normally using 0 as gravity type
				// The native will then set the gravity level to what we just wrote
				Function.Call(Hash.SET_GRAVITY_LEVEL, 0);
				// Reset the array item back to 9.8 so as to restore behavior of the native
				SHVDN.NativeMemory.WorldGravity = 9.800000f;
			}
		}

		#endregion

		#region Blips

		/// <summary>
		/// Gets the waypoint blip.
		/// </summary>
		/// <returns>The <see cref="Vector3"/> coordinates of the Waypoint <see cref="Blip"/></returns>
		/// <remarks>
		/// Returns <c>null</c> if a waypoint <see cref="Blip"/> hasn't been set
		/// </remarks>
		public static Blip WaypointBlip
		{
			get
			{
				if (!Game.IsWaypointActive)
				{
					return null;
				}

				for (int it = Function.Call<int>(Hash._GET_BLIP_INFO_ID_ITERATOR), blip = Function.Call<int>(Hash.GET_FIRST_BLIP_INFO_ID, it); Function.Call<bool>(Hash.DOES_BLIP_EXIST, blip); blip = Function.Call<int>(Hash.GET_NEXT_BLIP_INFO_ID, it))
				{
					if (Function.Call<int>(Hash.GET_BLIP_INFO_ID_TYPE, blip) == 4)
					{
						return new Blip(blip);
					}
				}

				return null;
			}
		}

		/// <summary>
		/// Removes the waypoint.
		/// </summary>
		public static void RemoveWaypoint()
		{
			Function.Call(Hash.SET_WAYPOINT_OFF);
		}

		/// <summary>
		/// Gets or sets the waypoint position.
		/// </summary>
		/// <returns>The <see cref="Vector3"/> coordinates of the Waypoint <see cref="Blip"/></returns>
		/// <remarks>
		/// Returns an empty <see cref="Vector3"/> if a waypoint <see cref="Blip"/> hasn't been set
		/// If the game engine cant extract height information the Z component will be 0.0f
		/// </remarks>
		public static Vector3 WaypointPosition
		{
			get
			{
				Blip waypointBlip = WaypointBlip;
				if (waypointBlip == null)
				{
					return Vector3.Zero;
				}

				Vector3 position = waypointBlip.Position;
				position.Z = GetGroundHeight((Vector2)position);
				return position;
			}
			set
			{
				Function.Call(Hash.SET_NEW_WAYPOINT, value.X, value.Y);
			}
		}

		/// <summary>
		/// Gets an <c>array</c> of all the <see cref="Blip"/>s on the map with a given <see cref="BlipSprite"/>.
		/// </summary>
		/// <param name="blipTypes">The blip types to include, leave blank to get all <see cref="Blip"/>s.</param>
		public static Blip[] GetAllBlips(params BlipSprite[] blipTypes)
		{
			int[] blipTypesInt = Array.ConvertAll(blipTypes, blipType => (int)blipType);
			return Array.ConvertAll(SHVDN.NativeMemory.GetNonCriticalRadarBlipHandles(blipTypesInt), handle => new Blip(handle));
		}

		/// <summary>
		/// Gets an <c>array</c> of all <see cref="Blip"/>s in a given region in the World.
		/// </summary>
		/// <param name="position">The position to check the <see cref="Blip"/> against.</param>
		/// <param name="radius">The maximum distance from the <paramref name="position"/> to detect <see cref="Blip"/>s.</param>
		/// <param name="blipTypes">The blip types to include, leave blank to get all <see cref="Blip"/>s.</param>
		public static Blip[] GetNearbyBlips(Vector3 position, float radius, params BlipSprite[] blipTypes)
		{
			int[] blipTypesInt = Array.ConvertAll(blipTypes, blipType => (int)blipType);
			return Array.ConvertAll(SHVDN.NativeMemory.GetNonCriticalRadarBlipHandles(position.ToArray(), radius, blipTypesInt), handle => new Blip(handle));
		}

		/// <summary>
		/// Creates a <see cref="Blip"/> at the given position on the map.
		/// </summary>
		/// <param name="position">The position of the blip on the map.</param>
		public static Blip CreateBlip(Vector3 position)
		{
			return new Blip(Function.Call<int>(Hash.ADD_BLIP_FOR_COORD, position.X, position.Y, position.Z));
		}
		/// <summary>
		/// Creates a <see cref="Blip"/> for a circular area at the given position on the map.
		/// </summary>
		/// <param name="position">The position of the blip on the map.</param>
		/// <param name="radius">The radius of the area on the map.</param>
		public static Blip CreateBlip(Vector3 position, float radius)
		{
			return new Blip(Function.Call<int>(Hash.ADD_BLIP_FOR_RADIUS, position.X, position.Y, position.Z, radius));
		}

		#endregion

		#region Entities

		/// <summary>
		/// A fast way to get the total number of <see cref="Vehicle"/>s spawned in the world.
		/// </summary>
		public static int VehicleCount => SHVDN.NativeMemory.GetVehicleCount();
		/// <summary>
		/// A fast way to get the total number of <see cref="Ped"/>s spawned in the world.
		/// </summary>
		public static int PedCount => SHVDN.NativeMemory.GetPedCount();
		/// <summary>
		/// A fast way to get the total number of <see cref="Prop"/>s spawned in the world.
		/// </summary>
		public static int PropCount => SHVDN.NativeMemory.GetObjectCount();
		/// <summary>
		/// A fast way to get the total number of <see cref="Prop"/>s in the world associated with a <see cref="Pickup"/>.
		/// </summary>
		public static int PickupObjectCount => SHVDN.NativeMemory.GetPickupObjectCount();

		/// <summary>
		/// The total number of <see cref="Vehicle"/>s that can exist in the world.
		/// </summary>
		/// <remarks>The game will crash when the number of <see cref="Vehicle"/> is the same as this limit and the game tries to create a <see cref="Vehicle"/>.</remarks>
		public static int VehicleCapacity => SHVDN.NativeMemory.GetVehicleCapacity();
		/// <summary>
		/// The total number of <see cref="Ped"/>s that can exist in the world.
		/// </summary>
		/// <remarks>The game will crash when the number of <see cref="Ped"/> is the same as this limit and the game tries to create a <see cref="Ped"/>.</remarks>
		public static int PedCapacity => SHVDN.NativeMemory.GetPedCapacity();
		/// <summary>
		/// The total number of <see cref="Prop"/>s that can exist in the world.
		/// </summary>
		/// <remarks>The game will crash when the number of <see cref="Prop"/> is the same as this limit and the game tries to create a <see cref="Prop"/>.</remarks>
		public static int PropCapacity => SHVDN.NativeMemory.GetObjectCapacity();
		/// <summary>
		/// The total number of <see cref="Prop"/>s in the world associated with a <see cref="Pickup"/> that can exist in the world.
		/// </summary>
		public static int PickupObjectCapacity => SHVDN.NativeMemory.GetPickupObjectCapacity();

		/// <summary>
		/// Gets the closest <see cref="Ped"/> to a given position in the World.
		/// </summary>
		/// <param name="position">The position to find the nearest <see cref="Ped"/>.</param>
		/// <param name="radius">The maximum distance from the <paramref name="position"/> to detect <see cref="Ped"/>s.</param>
		/// <param name="models">The <see cref="Model"/> of <see cref="Ped"/>s to get, leave blank for all <see cref="Ped"/> <see cref="Model"/>s.</param>
		/// <remarks>Returns <c>null</c> if no <see cref="Ped"/> was in the given region.</remarks>
		public static Ped GetClosestPed(Vector3 position, float radius, params Model[] models)
		{
			return GetClosest(position, GetNearbyPeds(position, radius, models));
		}

		/// <summary>
		/// Gets an <c>array</c>of all <see cref="Ped"/>s in the World.
		/// </summary>
		/// <param name="models">The <see cref="Model"/> of <see cref="Ped"/>s to get, leave blank for all <see cref="Ped"/> <see cref="Model"/>s.</param>
		public static Ped[] GetAllPeds(params Model[] models)
		{
			int[] hashes = Array.ConvertAll(models, model => model.Hash);
			return Array.ConvertAll(SHVDN.NativeMemory.GetPedHandles(hashes), handle => new Ped(handle));
		}
		/// <summary>
		/// Gets an <c>array</c> of all <see cref="Ped"/>s near a given <see cref="Ped"/> in the world
		/// </summary>
		/// <param name="ped">The ped to check.</param>
		/// <param name="radius">The maximun distance from the <paramref name="ped"/> to detect <see cref="Ped"/>s.</param>
		/// <param name="models">The <see cref="Model"/> of <see cref="Ped"/>s to get, leave blank for all <see cref="Ped"/> <see cref="Model"/>s.</param>
		/// <remarks>Doesnt include the <paramref name="ped"/> in the result</remarks>
		public static Ped[] GetNearbyPeds(Ped ped, float radius, params Model[] models)
		{
			int[] hashes = Array.ConvertAll(models, model => model.Hash);
			int[] handles = SHVDN.NativeMemory.GetPedHandles(ped.Position.ToArray(), radius, hashes);

			var result = new List<Ped>();

			foreach (int handle in handles)
			{
				if (handle == ped.Handle)
				{
					continue;
				}

				result.Add(new Ped(handle));
			}

			return result.ToArray();
		}
		/// <summary>
		/// Gets an <c>array</c> of all <see cref="Ped"/>s in a given region in the World.
		/// </summary>
		/// <param name="position">The position to check the <see cref="Ped"/> against.</param>
		/// <param name="radius">The maximun distance from the <paramref name="position"/> to detect <see cref="Ped"/>s.</param>
		/// <param name="models">The <see cref="Model"/> of <see cref="Ped"/>s to get, leave blank for all <see cref="Ped"/> <see cref="Model"/>s.</param>
		public static Ped[] GetNearbyPeds(Vector3 position, float radius, params Model[] models)
		{
			int[] hashes = Array.ConvertAll(models, model => model.Hash);
			return Array.ConvertAll(SHVDN.NativeMemory.GetPedHandles(position.ToArray(), radius, hashes), handle => new Ped(handle));
		}

		/// <summary>
		/// Gets the closest <see cref="Vehicle"/> to a given position in the World.
		/// </summary>
		/// <param name="position">The position to find the nearest <see cref="Vehicle"/>.</param>
		/// <param name="radius">The maximum distance from the <paramref name="position"/> to detect <see cref="Vehicle"/>s.</param>
		/// <param name="models">The <see cref="Model"/> of <see cref="Vehicle"/>s to get, leave blank for all <see cref="Vehicle"/> <see cref="Model"/>s.</param>
		/// <remarks>Returns <c>null</c> if no <see cref="Vehicle"/> was in the given region.</remarks>
		public static Vehicle GetClosestVehicle(Vector3 position, float radius, params Model[] models)
		{
			return GetClosest(position, GetNearbyVehicles(position, radius, models));
		}

		/// <summary>
		/// Gets an <c>array</c> of all <see cref="Vehicle"/>s in the World.
		/// </summary>
		/// <param name="models">The <see cref="Model"/> of <see cref="Vehicle"/>s to get, leave blank for all <see cref="Vehicle"/> <see cref="Model"/>s.</param>
		public static Vehicle[] GetAllVehicles(params Model[] models)
		{
			int[] hashes = Array.ConvertAll(models, model => model.Hash);
			return Array.ConvertAll(SHVDN.NativeMemory.GetVehicleHandles(hashes), handle => new Vehicle(handle));
		}
		/// <summary>
		/// Gets an <c>array</c> of all <see cref="Vehicle"/>s near a given <see cref="Ped"/> in the world
		/// </summary>
		/// <param name="ped">The ped to check.</param>
		/// <param name="radius">The maximun distance from the <paramref name="ped"/> to detect <see cref="Vehicle"/>s.</param>
		/// <param name="models">The <see cref="Model"/> of <see cref="Vehicle"/>s to get, leave blank for all <see cref="Vehicle"/> <see cref="Model"/>s.</param>
		/// <remarks>Doesnt include the <see cref="Vehicle"/> the <paramref name="ped"/> is using in the result</remarks>
		public static Vehicle[] GetNearbyVehicles(Ped ped, float radius, params Model[] models)
		{
			int[] hashes = Array.ConvertAll(models, model => model.Hash);
			int[] handles = SHVDN.NativeMemory.GetVehicleHandles(ped.Position.ToArray(), radius, hashes);

			var result = new List<Vehicle>();
			Vehicle ignore = ped.CurrentVehicle;

			foreach (int handle in handles)
			{
				if (ignore != null && handle == ignore.Handle)
				{
					continue;
				}

				result.Add(new Vehicle(handle));
			}

			return result.ToArray();
		}
		/// <summary>
		/// Gets an <c>array</c> of all <see cref="Vehicle"/>s in a given region in the World.
		/// </summary>
		/// <param name="position">The position to check the <see cref="Vehicle"/> against.</param>
		/// <param name="radius">The maximun distance from the <paramref name="position"/> to detect <see cref="Vehicle"/>s.</param>
		/// <param name="models">The <see cref="Model"/> of <see cref="Vehicle"/>s to get, leave blank for all <see cref="Vehicle"/> <see cref="Model"/>s.</param>
		public static Vehicle[] GetNearbyVehicles(Vector3 position, float radius, params Model[] models)
		{
			int[] hashes = Array.ConvertAll(models, model => model.Hash);
			return Array.ConvertAll(SHVDN.NativeMemory.GetVehicleHandles(position.ToArray(), radius, hashes), handle => new Vehicle(handle));
		}

		/// <summary>
		/// Gets the closest <see cref="Prop"/> to a given position in the World.
		/// </summary>
		/// <param name="position">The position to find the nearest <see cref="Prop"/>.</param>
		/// <param name="radius">The maximum distance from the <paramref name="position"/> to detect <see cref="Prop"/>s.</param>
		/// <param name="models">The <see cref="Model"/> of <see cref="Prop"/>s to get, leave blank for all <see cref="Prop"/> <see cref="Model"/>s.</param>
		/// <remarks>Returns <c>null</c> if no <see cref="Prop"/> was in the given region.</remarks>
		public static Prop GetClosestProp(Vector3 position, float radius, params Model[] models)
		{
			return GetClosest(position, GetNearbyProps(position, radius, models));
		}

		/// <summary>
		/// Gets an <c>array</c> of all <see cref="Prop"/>s in the World.
		/// </summary>
		/// <param name="models">The <see cref="Model"/> of <see cref="Prop"/>s to get, leave blank for all <see cref="Prop"/> <see cref="Model"/>s.</param>
		public static Prop[] GetAllProps(params Model[] models)
		{
			int[] hashes = Array.ConvertAll(models, model => model.Hash);
			return Array.ConvertAll(SHVDN.NativeMemory.GetPropHandles(hashes), handle => new Prop(handle));
		}
		/// <summary>
		/// Gets an <c>array</c> of all <see cref="Prop"/>s in a given region in the World.
		/// </summary>
		/// <param name="position">The position to check the <see cref="Prop"/> against.</param>
		/// <param name="radius">The maximun distance from the <paramref name="position"/> to detect <see cref="Prop"/>s.</param>
		/// <param name="models">The <see cref="Model"/> of <see cref="Prop"/>s to get, leave blank for all <see cref="Prop"/> <see cref="Model"/>s.</param>
		public static Prop[] GetNearbyProps(Vector3 position, float radius, params Model[] models)
		{
			int[] hashes = Array.ConvertAll(models, model => model.Hash);
			return Array.ConvertAll(SHVDN.NativeMemory.GetPropHandles(position.ToArray(), radius, hashes), handle => new Prop(handle));
		}

		/// <summary>
		/// Gets the closest <see cref="Prop"/> to a given position in the World associated with a <see cref="Pickup"/>.
		/// </summary>
		/// <param name="position">The position to find the nearest <see cref="Prop"/>.</param>
		/// <param name="radius">The maximum distance from the <paramref name="position"/> to detect <see cref="Prop"/>s.</param>
		/// <remarks>Returns <c>null</c> if no <see cref="Prop"/> was in the given region.</remarks>
		public static Prop GetClosestPickupObject(Vector3 position, float radius)
		{
			return GetClosest(position, GetNearbyPickupObjects(position, radius));
		}

		/// <summary>
		/// Gets an <c>array</c> of all <see cref="Prop"/>s in the World associated with a <see cref="Pickup"/>.
		/// </summary>
		public static Prop[] GetAllPickupObjects()
		{
			return Array.ConvertAll(SHVDN.NativeMemory.GetPickupObjectHandles(), handle => new Prop(handle));
		}
		/// <summary>
		/// Gets an <c>array</c> of all <see cref="Prop"/>s in a given region in the World associated with a <see cref="Pickup"/>.
		/// </summary>
		/// <param name="position">The position to check the <see cref="Entity"/> against.</param>
		/// <param name="radius">The maximun distance from the <paramref name="position"/> to detect <see cref="Prop"/>s.</param>
		public static Prop[] GetNearbyPickupObjects(Vector3 position, float radius)
		{
			return Array.ConvertAll(SHVDN.NativeMemory.GetPickupObjectHandles(position.ToArray(), radius), handle => new Prop(handle));
		}

		/// <summary>
		/// Gets an <c>array</c> of all <see cref="Entity"/>s in the World.
		/// </summary>
		public static Entity[] GetAllEntities()
		{
			return Array.ConvertAll(SHVDN.NativeMemory.GetEntityHandles(), Entity.FromHandle);
		}
		/// <summary>
		/// Gets an <c>array</c> of all <see cref="Entity"/>s in a given region in the World.
		/// </summary>
		/// <param name="position">The position to check the <see cref="Entity"/> against.</param>
		/// <param name="radius">The maximun distance from the <paramref name="position"/> to detect <see cref="Entity"/>s.</param>
		public static Entity[] GetNearbyEntities(Vector3 position, float radius)
		{
			return Array.ConvertAll(SHVDN.NativeMemory.GetEntityHandles(position.ToArray(), radius), Entity.FromHandle);
		}

		/// <summary>
		/// Gets the closest <see cref="ISpatial"/> to a given position in the World.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="position">The position to check against.</param>
		/// <param name="spatials">The spatials to check.</param>
		/// <returns>The closest <see cref="ISpatial"/> to the <paramref name="position"/></returns>
		public static T GetClosest<T>(Vector3 position, params T[] spatials) where T : ISpatial
		{
			ISpatial closest = null;
			float closestDistance = 3e38f;

			foreach (var spatial in spatials)
			{
				var distance = position.DistanceToSquared(spatial.Position);
				if (distance <= closestDistance)
				{
					closest = spatial;
					closestDistance = distance;
				}
			}
			return (T)closest;
		}
		/// <summary>
		/// Gets the closest <see cref="ISpatial"/> to a given position in the World ignoring height.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="position">The position to check against.</param>
		/// <param name="spatials">The spatials to check.</param>
		/// <returns>The closest <see cref="ISpatial"/> to the <paramref name="position"/></returns>
		public static T GetClosest<T>(Vector2 position, params T[] spatials) where T : ISpatial
		{
			ISpatial closest = null;
			float closestDistance = 3e38f;
			var position3D = new Vector3(position.X, position.Y, 0.0f);

			foreach (var spatial in spatials)
			{
				var distance = position3D.DistanceToSquared2D(spatial.Position);
				if (distance <= closestDistance)
				{
					closest = spatial;
					closestDistance = distance;
				}
			}
			return (T)closest;
		}

		/// <summary>
		/// Spawns a <see cref="Ped"/> of the given <see cref="Model"/> at the position and heading specified.
		/// </summary>
		/// <param name="model">The <see cref="Model"/> of the <see cref="Ped"/>.</param>
		/// <param name="position">The position to spawn the <see cref="Ped"/> at.</param>
		/// <param name="heading">The heading of the <see cref="Ped"/>.</param>
		/// <remarks>returns <c>null</c> if the <see cref="Ped"/> could not be spawned</remarks>
		public static Ped CreatePed(Model model, Vector3 position, float heading = 0f)
		{
			if (PedCount >= PedCapacity || !model.IsPed || !model.Request(1000))
			{
				return null;
			}

			return new Ped(Function.Call<int>(Hash.CREATE_PED, 26, model.Hash, position.X, position.Y, position.Z, heading, false, false));
		}
		/// <summary>
		/// Spawns a <see cref="Ped"/> of a random <see cref="Model"/> at the position specified.
		/// </summary>
		/// <param name="position">The position to spawn the <see cref="Ped"/> at.</param>
		public static Ped CreateRandomPed(Vector3 position)
		{
			if (PedCount >= PedCapacity)
			{
				return null;
			}

			return new Ped(Function.Call<int>(Hash.CREATE_RANDOM_PED, position.X, position.Y, position.Z));
		}

		/// <summary>
		/// Spawns a <see cref="Vehicle"/> of the given <see cref="Model"/> at the position and heading specified.
		/// </summary>
		/// <param name="model">The <see cref="Model"/> of the <see cref="Vehicle"/>.</param>
		/// <param name="position">The position to spawn the <see cref="Vehicle"/> at.</param>
		/// <param name="heading">The heading of the <see cref="Vehicle"/>.</param>
		/// <remarks>returns <c>null</c> if the <see cref="Vehicle"/> could not be spawned</remarks>
		public static Vehicle CreateVehicle(Model model, Vector3 position, float heading = 0f)
		{
			if (VehicleCount >= VehicleCapacity || !model.IsVehicle || !model.Request(1000))
			{
				return null;
			}

			return new Vehicle(Function.Call<int>(Hash.CREATE_VEHICLE, model.Hash, position.X, position.Y, position.Z, heading, false, false));
		}
		/// <summary>
		/// Spawns a <see cref="Vehicle"/> of a random <see cref="Model"/> at the position specified. Does not work currently.
		/// </summary>
		/// <param name="position">The position to spawn the <see cref="Vehicle"/> at.</param>
		/// <param name="heading">The heading of the <see cref="Vehicle"/>.</param>
		/// <remarks>returns <c>null</c> if the <see cref="Vehicle"/> could not be spawned</remarks>
		public static Vehicle CreateRandomVehicle(Vector3 position, float heading = 0f)
		{
			// GET_RANDOM_VEHICLE_MODEL_IN_MEMORY is not present but just a nullsub in the retail version
			// We need certain memory patterns to get this method to spawn random ambient vehicles properly
			return null;

			int outModel, outInt;
			unsafe
			{
				Function.Call(Hash.GET_RANDOM_VEHICLE_MODEL_IN_MEMORY, 1, &outModel, &outInt);
			}
			Model model = outModel;
			if (model.IsVehicle && model.IsLoaded)
			{
				return
					new Vehicle(Function.Call<int>(Hash.CREATE_VEHICLE, model.Hash, position.X, position.Y, position.Z, heading, false, false));
			}
			return null;
		}

		/// <summary>
		/// Spawns a <see cref="Prop"/> of the given <see cref="Model"/> at the specified position.
		/// </summary>
		/// <param name="model">The <see cref="Model"/> of the <see cref="Prop"/>.</param>
		/// <param name="position">The position to spawn the <see cref="Prop"/> at.</param>
		/// <param name="dynamic">if set to <c>true</c> the <see cref="Prop"/> will have physics; otherwise, it will be static.</param>
		/// <param name="placeOnGround">if set to <c>true</c> place the prop on the ground nearest to the <paramref name="position"/>.</param>
		/// <remarks>returns <c>null</c> if the <see cref="Prop"/> could not be spawned</remarks>
		public static Prop CreateProp(Model model, Vector3 position, bool dynamic, bool placeOnGround)
		{
			if (PropCount >= PropCapacity || !model.Request(1000))
			{
				return null;
			}

			if (placeOnGround)
			{
				position.Z = GetGroundHeight(position);
			}

			return new Prop(Function.Call<int>(Hash.CREATE_OBJECT, model.Hash, position.X, position.Y, position.Z, 1, 1, dynamic));
		}
		/// <summary>
		/// Spawns a <see cref="Prop"/> of the given <see cref="Model"/> at the specified position.
		/// </summary>
		/// <param name="model">The <see cref="Model"/> of the <see cref="Prop"/>.</param>
		/// <param name="position">The position to spawn the <see cref="Prop"/> at.</param>
		/// <param name="rotation">The rotation of the <see cref="Prop"/>.</param>
		/// <param name="dynamic">if set to <c>true</c> the <see cref="Prop"/> will have physics; otherwise, it will be static.</param>
		/// <param name="placeOnGround">if set to <c>true</c> place the prop on the ground nearest to the <paramref name="position"/>.</param>
		/// <remarks>returns <c>null</c> if the <see cref="Prop"/> could not be spawned</remarks>
		public static Prop CreateProp(Model model, Vector3 position, Vector3 rotation, bool dynamic, bool placeOnGround)
		{
			Prop prop = CreateProp(model, position, dynamic, placeOnGround);

			if (prop != null)
			{
				prop.Rotation = rotation;
			}

			return prop;
		}
		/// <summary>
		/// Spawns a <see cref="Prop"/> of the given <see cref="Model"/> at the specified position without any offset.
		/// </summary>
		/// <param name="model">The <see cref="Model"/> of the <see cref="Prop"/>.</param>
		/// <param name="position">The position to spawn the <see cref="Prop"/> at.</param>
		/// <param name="dynamic">if set to <c>true</c> the <see cref="Prop"/> will have physics; otherwise, it will be static.</param>
		/// <remarks>returns <c>null</c> if the <see cref="Prop"/> could not be spawned</remarks>
		public static Prop CreatePropNoOffset(Model model, Vector3 position, bool dynamic)
		{
			if (PropCount >= PropCapacity || !model.Request(1000))
			{
				return null;
			}

			return new Prop(Function.Call<int>(Hash.CREATE_OBJECT_NO_OFFSET, model.Hash, position.X, position.Y, position.Z, 1, 1, dynamic));
		}
		/// <summary>
		/// Spawns a <see cref="Prop"/> of the given <see cref="Model"/> at the specified position without any offset.
		/// </summary>
		/// <param name="model">The <see cref="Model"/> of the <see cref="Prop"/>.</param>
		/// <param name="position">The position to spawn the <see cref="Prop"/> at.</param>
		/// <param name="rotation">The rotation of the <see cref="Prop"/>.</param>
		/// <param name="dynamic">if set to <c>true</c> the <see cref="Prop"/> will have physics; otherwise, it will be static.</param>
		/// <remarks>returns <c>null</c> if the <see cref="Prop"/> could not be spawned</remarks>
		public static Prop CreatePropNoOffset(Model model, Vector3 position, Vector3 rotation, bool dynamic)
		{
			Prop prop = CreatePropNoOffset(model, position, dynamic);

			if (prop != null)
			{
				prop.Rotation = rotation;
			}

			return prop;
		}

		/// <summary>
		/// Spawns a pickup <see cref="Prop"/> at the specified position.
		/// </summary>
		public static Prop CreateAmbientPickup(PickupType type, Vector3 position, Model model, int value)
		{
			if (!model.Request(1000))
			{
				return null;
			}

			int handle = Function.Call<int>(Hash.CREATE_AMBIENT_PICKUP, type, position.X, position.Y, position.Z, 0, value, model.Hash, false, true);

			if (handle == 0)
			{
				return null;
			}

			return new Prop(handle);
		}

		/// <summary>
		/// Spawns a <see cref="Pickup"/> at the specified position.
		/// </summary>
		public static Pickup CreatePickup(PickupType type, Vector3 position, Model model, int value)
		{
			if (!model.Request(1000))
			{
				return null;
			}

			int handle = Function.Call<int>(Hash.CREATE_PICKUP, type, position.X, position.Y, position.Z, 0, value, true, model.Hash);

			if (handle == 0)
			{
				return null;
			}

			return new Pickup(handle);
		}
		/// <summary>
		/// Spawns a <see cref="Pickup"/> at the specified position.
		/// </summary>
		public static Pickup CreatePickup(PickupType type, Vector3 position, Vector3 rotation, Model model, int value)
		{
			if (!model.Request(1000))
			{
				return null;
			}

			int handle = Function.Call<int>(Hash.CREATE_PICKUP_ROTATE, type, position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, 0, value, 2, true, model.Hash);

			if (handle == 0)
			{
				return null;
			}

			return new Pickup(handle);
		}

		#endregion

		#region Checkpoints

		/// <summary>
		/// Gets an <c>array</c> of all the <see cref="Checkpoint"/>s.
		/// </summary>
		public static Checkpoint[] GetAllCheckpoints()
		{
			return Array.ConvertAll(SHVDN.NativeMemory.GetCheckpointHandles(), element => new Checkpoint(element));
		}

		/// <summary>
		/// Creates a <see cref="Checkpoint"/> in the world.
		/// </summary>
		/// <param name="icon">The <see cref="CheckpointIcon"/> to display inside the <see cref="Checkpoint"/>.</param>
		/// <param name="position">The position in the World.</param>
		/// <param name="pointTo">The position in the world where this <see cref="Checkpoint"/> should point.</param>
		/// <param name="radius">The radius of the <see cref="Checkpoint"/>.</param>
		/// <param name="color">The color of the <see cref="Checkpoint"/>.</param>
		/// <remarks>returns <c>null</c> if the <see cref="Checkpoint"/> could not be created</remarks>
		public static Checkpoint CreateCheckpoint(CheckpointIcon icon, Vector3 position, Vector3 pointTo, float radius, System.Drawing.Color color)
		{
			int handle = Function.Call<int>(Hash.CREATE_CHECKPOINT, icon, position.X, position.Y, position.Z, pointTo.X, pointTo.Y, pointTo.Z, radius, color.R, color.G, color.B, color.A, 0);
			return handle != 0 ? new Checkpoint(handle) : null;
		}
		/// <summary>
		/// Creates a <see cref="Checkpoint"/> in the world.
		/// </summary>
		/// <param name="icon">The <see cref="CheckpointCustomIcon"/> to display inside the <see cref="Checkpoint"/>.</param>
		/// <param name="position">The position in the World.</param>
		/// <param name="pointTo">The position in the world where this <see cref="Checkpoint"/> should point.</param>
		/// <param name="radius">The radius of the <see cref="Checkpoint"/>.</param>
		/// <param name="color">The color of the <see cref="Checkpoint"/>.</param>
		/// <remarks>returns <c>null</c> if the <see cref="Checkpoint"/> could not be created</remarks>
		public static Checkpoint CreateCheckpoint(CheckpointCustomIcon icon, Vector3 position, Vector3 pointTo, float radius, System.Drawing.Color color)
		{
			int handle = Function.Call<int>(Hash.CREATE_CHECKPOINT, 42, position.X, position.Y, position.Z, pointTo.X, pointTo.Y, pointTo.Z, radius, color.R, color.G, color.B, color.A, icon);
			return handle != 0 ? new Checkpoint(handle) : null;
		}

		#endregion

		#region Cameras

		/// <summary>
		/// Destroys all user created <see cref="Camera"/>s.
		/// </summary>
		public static void DestroyAllCameras()
		{
			Function.Call(Hash.DESTROY_ALL_CAMS, 0);
		}

		/// <summary>
		/// Creates a <see cref="Camera"/>, use <see cref="World.RenderingCamera"/> to switch to this camera
		/// </summary>
		/// <param name="position">The position of the camera.</param>
		/// <param name="rotation">The rotation of the camera.</param>
		/// <param name="fov">The field of view of the camera.</param>
		public static Camera CreateCamera(Vector3 position, Vector3 rotation, float fov)
		{
			return new Camera(Function.Call<int>(Hash.CREATE_CAM_WITH_PARAMS, "DEFAULT_SCRIPTED_CAMERA", position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, fov, 1, 2));
		}

		/// <summary>
		/// Gets or sets the rendering camera.
		/// </summary>
		/// <value>
		/// The rendering <see cref="Camera"/>.
		/// </value>
		/// <remarks>
		/// Setting to <c>null</c> sets the rendering <see cref="Camera"/> to <see cref="GameplayCamera"/>.
		/// </remarks>
		public static Camera RenderingCamera
		{
			get
			{
				return new Camera(Function.Call<int>(Hash.GET_RENDERING_CAM));
			}
			set
			{
				if (value == null)
				{
					Function.Call(Hash.RENDER_SCRIPT_CAMS, false, 0, 3000, 1, 0);
				}
				else
				{
					value.IsActive = true;
					Function.Call(Hash.RENDER_SCRIPT_CAMS, true, 0, 3000, 1, 0);
				}
			}
		}

		#endregion

		#region Particle Effects

		/// <summary>
		/// Starts a Particle Effect that runs once at a given position then is destroyed.
		/// </summary>
		/// <param name="asset">The effect asset to use.</param>
		/// <param name="effectName">The name of the effect.</param>
		/// <param name="pos">The World position where the effect is.</param>
		/// <param name="rot">What rotation to apply to the effect.</param>
		/// <param name="scale">How much to scale the size of the effect by.</param>
		/// <param name="invertAxis">Which axis to flip the effect in.</param>
		/// <returns><c>true</c>If the effect was able to start; otherwise, <c>false</c>.</returns>
		public static bool CreateParticleEffectNonLooped(ParticleEffectAsset asset, string effectName, Vector3 pos, Vector3 rot = default, float scale = 1.0f, InvertAxisFlags invertAxis = InvertAxisFlags.None)
		{
			if (!asset.UseNext())
			{
				return false;
			}

			return Function.Call<bool>(Hash.START_PARTICLE_FX_NON_LOOPED_AT_COORD, effectName, pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, scale, invertAxis.HasFlag(InvertAxisFlags.X), invertAxis.HasFlag(InvertAxisFlags.Y), invertAxis.HasFlag(InvertAxisFlags.Z));
		}
		/// <summary>
		/// Starts a Particle Effect on an <see cref="Entity"/> that runs once then is destroyed.
		/// </summary>
		/// <param name="asset">The effect asset to use.</param>
		/// <param name="effectName">the name of the effect.</param>
		/// <param name="entity">The <see cref="Entity"/> the effect is attached to.</param>
		/// <param name="off">The offset from the <paramref name="entity"/> to attach the effect.</param>
		/// <param name="rot">The rotation, relative to the <paramref name="entity"/>, the effect has.</param>
		/// <param name="scale">How much to scale the size of the effect by.</param>
		/// <param name="invertAxis">Which axis to flip the effect in. For a car side exahust you may need to flip in the Y Axis</param>
		/// <returns><c>true</c>If the effect was able to start; otherwise, <c>false</c>.</returns>
		public static bool CreateParticleEffectNonLooped(ParticleEffectAsset asset, string effectName, Entity entity, Vector3 off = default, Vector3 rot = default, float scale = 1.0f, InvertAxisFlags invertAxis = InvertAxisFlags.None)
		{
			if (!asset.UseNext())
			{
				return false;
			}

			return Function.Call<bool>(Hash.START_PARTICLE_FX_NON_LOOPED_ON_PED_BONE, effectName, entity.Handle, off.X, off.Y, off.Z, rot.X, rot.Y, rot.Z, -1, scale, invertAxis.HasFlag(InvertAxisFlags.X), invertAxis.HasFlag(InvertAxisFlags.Y), invertAxis.HasFlag(InvertAxisFlags.Z));
		}
		/// <summary>
		/// Starts a Particle Effect on an <see cref="EntityBone"/> that runs once then is destroyed.
		/// </summary>
		/// <param name="asset">The effect asset to use.</param>
		/// <param name="effectName">the name of the effect.</param>
		/// <param name="entityBone">The <see cref="EntityBone"/> the effect is attached to.</param>
		/// <param name="off">The offset from the <paramref name="entityBone"/> to attach the effect.</param>
		/// <param name="rot">The rotation, relative to the <paramref name="entityBone"/>, the effect has.</param>
		/// <param name="scale">How much to scale the size of the effect by.</param>
		/// <param name="invertAxis">Which axis to flip the effect in. For a car side exahust you may need to flip in the Y Axis</param>
		/// <returns><c>true</c>If the effect was able to start; otherwise, <c>false</c>.</returns>
		public static bool CreateParticleEffectNonLooped(ParticleEffectAsset asset, string effectName, EntityBone entityBone, Vector3 off = default, Vector3 rot = default, float scale = 1.0f, InvertAxisFlags invertAxis = InvertAxisFlags.None)
		{
			if (!asset.UseNext())
			{
				return false;
			}

			return Function.Call<bool>(Hash.START_PARTICLE_FX_NON_LOOPED_ON_PED_BONE, effectName, entityBone.Owner.Handle, off.X, off.Y, off.Z, rot.X, rot.Y, rot.Z, entityBone, scale, invertAxis.HasFlag(InvertAxisFlags.X), invertAxis.HasFlag(InvertAxisFlags.Y), invertAxis.HasFlag(InvertAxisFlags.Z));
		}

		/// <summary>
		/// Creates a <see cref="ParticleEffect"/> on an <see cref="Entity"/> that runs looped.
		/// </summary>
		/// <param name="asset">The effect asset to use.</param>
		/// <param name="effectName">The name of the Effect</param>
		/// <param name="entity">The <see cref="Entity"/> the effect is attached to.</param>
		/// <param name="offset">The offset from the <paramref name="entity"/> to attach the effect.</param>
		/// <param name="rotation">The rotation, relative to the <paramref name="entity"/>, the effect has.</param>
		/// <param name="scale">How much to scale the size of the effect by.</param>
		/// <param name="invertAxis">Which axis to flip the effect in. For a car side exhaust you may need to flip in the Y Axis.</param>
		public static ParticleEffect CreateParticleEffect(ParticleEffectAsset asset, string effectName, Entity entity, Vector3 offset = default, Vector3 rotation = default, float scale = 1.0f, InvertAxisFlags invertAxis = InvertAxisFlags.None)
		{
			return CreateParticleEffect(asset, effectName, entity.Bones.Core, offset, rotation, scale, invertAxis);
		}
		/// <summary>
		/// Creates a <see cref="ParticleEffect"/> on an <see cref="EntityBone"/> that runs looped.
		/// </summary>
		/// <param name="asset">The effect asset to use.</param>
		/// <param name="effectName">The name of the Effect</param>
		/// <param name="entityBone">The <see cref="EntityBone"/> the effect is attached to.</param>
		/// <param name="offset">The offset from the <paramref name="entityBone"/> to attach the effect.</param>
		/// <param name="rotation">The rotation, relative to the <paramref name="entityBone"/>, the effect has.</param>
		/// <param name="scale">How much to scale the size of the effect by.</param>
		/// <param name="invertAxis">Which axis to flip the effect in. For a car side exhaust you may need to flip in the Y Axis.</param>
		public static ParticleEffect CreateParticleEffect(ParticleEffectAsset asset, string effectName, EntityBone entityBone, Vector3 offset = default, Vector3 rotation = default, float scale = 1.0f, InvertAxisFlags invertAxis = InvertAxisFlags.None)
		{
			if (!asset.UseNext())
			{
				return null;
			}

			int handle = Function.Call<int>((entityBone.Owner is Ped) ? Hash.START_PARTICLE_FX_LOOPED_ON_PED_BONE : Hash.START_PARTICLE_FX_LOOPED_ON_ENTITY_BONE,
				effectName, entityBone.Owner.Handle, offset.X, offset.Y, offset.Z, rotation.X, rotation.Y, rotation.Z, entityBone.Index, scale, invertAxis.HasFlag(InvertAxisFlags.X), invertAxis.HasFlag(InvertAxisFlags.Y), invertAxis.HasFlag(InvertAxisFlags.Z));
			if (handle == 0)
			{
				return null;
			}

			return new ParticleEffect(handle, asset.AssetName, effectName, entityBone);
		}
		/// <summary>
		/// Creates a <see cref="ParticleEffect"/> at a position that runs looped.
		/// </summary>
		/// <param name="asset">The effect asset to use.</param>
		/// <param name="effectName">The name of the effect.</param>
		/// <param name="position">The world coordinates where the effect is.</param>
		/// <param name="rotation">What rotation to apply to the effect.</param>
		/// <param name="scale">How much to scale the size of the effect by.</param>
		/// <param name="invertAxis">Which axis to flip the effect in.</param>
		public static ParticleEffect CreateParticleEffect(ParticleEffectAsset asset, string effectName, Vector3 position, Vector3 rotation = default, float scale = 1.0f, InvertAxisFlags invertAxis = InvertAxisFlags.None)
		{
			if (!asset.UseNext())
			{
				return null;
			}

			int handle = Function.Call<int>(Hash.START_PARTICLE_FX_LOOPED_AT_COORD, effectName, position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, scale, invertAxis.HasFlag(InvertAxisFlags.X), invertAxis.HasFlag(InvertAxisFlags.Y), invertAxis.HasFlag(InvertAxisFlags.Z), false);
			if (handle == 0)
			{
				return null;
			}

			return new ParticleEffect(handle, asset.AssetName, effectName, null);
		}

		/// <summary>
		/// Stops all particle effects in a range.
		/// </summary>
		/// <param name="pos">The position in the world to stop particle effects.</param>
		/// <param name="range">The maximum distance from the <paramref name="pos"/> to stop particle effects.</param>
		public static void RemoveAllParticleEffectsInRange(Vector3 pos, float range)
		{
			Function.Call(Hash.REMOVE_PARTICLE_FX_IN_RANGE, pos.X, pos.Y, pos.Z, range);
		}

		#endregion

		#region Others

		/// <summary>
		/// Spawns a <see cref="Rope"/>.
		/// </summary>
		/// <param name="type">The type of <see cref="Rope"/>.</param>
		/// <param name="position">The position of the <see cref="Rope"/>.</param>
		/// <param name="rotation">The rotation of the <see cref="Rope"/>.</param>
		/// <param name="length">The length of the <see cref="Rope"/>.</param>
		/// <param name="minLength">The minimum length of the <see cref="Rope"/>.</param>
		/// <param name="breakable">if set to <c>true</c> the <see cref="Rope"/> will break if shot.</param>
		public static Rope AddRope(RopeType type, Vector3 position, Vector3 rotation, float length, float minLength, bool breakable)
		{
			Function.Call(Hash.ROPE_LOAD_TEXTURES);

			return new Rope(Function.Call<int>(Hash.ADD_ROPE, position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, length, type, length, minLength, 0.5f, false, false, true, 1.0f, breakable, 0));
		}

		/// <summary>
		/// Fires a single bullet in the world
		/// </summary>
		/// <param name="sourcePosition">Where the bullet is fired from.</param>
		/// <param name="targetPosition">Where the bullet is fired to.</param>
		/// <param name="owner">The <see cref="Ped"/> who fired the bullet, leave <c>null</c> for no one.</param>
		/// <param name="weaponAsset">The weapon that the bullet is fired from.</param>
		/// <param name="damage">The damage the bullet will cause.</param>
		/// <param name="speed">The speed, only affects projectile weapons, leave -1 for default.</param>
		public static void ShootBullet(Vector3 sourcePosition, Vector3 targetPosition, Ped owner, WeaponAsset weaponAsset, int damage, float speed = -1f)
		{
			Function.Call(Hash.SHOOT_SINGLE_BULLET_BETWEEN_COORDS, sourcePosition.X, sourcePosition.Y, sourcePosition.Z, targetPosition.X, targetPosition.Y, targetPosition.Z, damage, 1, weaponAsset.Hash, (owner == null ? 0 : owner.Handle), 1, 0, speed);
		}

		/// <summary>
		/// Creates an explosion in the world
		/// </summary>
		/// <param name="position">The position of the explosion.</param>
		/// <param name="type">The type of explosion.</param>
		/// <param name="radius">The radius of the explosion.</param>
		/// <param name="cameraShake">The amount of camera shake to apply to nearby cameras.</param>
		/// <param name="owner">The <see cref="Ped"/> who caused the explosion, leave null if no one caused the explosion.</param>
		/// <param name="aubidble">if set to <c>true</c> explosion can be heard.</param>
		/// <param name="invisible">if set to <c>true</c> explosion is invisible.</param>
		public static void AddExplosion(Vector3 position, ExplosionType type, float radius, float cameraShake, Ped owner = null, bool aubidble = true, bool invisible = false)
		{
			if (owner?.Exists() == true)
			{
				Function.Call(Hash.ADD_OWNED_EXPLOSION, owner.Handle, position.X, position.Y, position.Z, type, radius, aubidble, invisible, cameraShake);
			}
			else
			{
				Function.Call(Hash.ADD_EXPLOSION, position.X, position.Y, position.Z, type, radius, aubidble, invisible, cameraShake);
			}
		}

		/// <summary>
		/// Creates a <see cref="RelationshipGroup"/> with the given name.
		/// </summary>
		/// <param name="name">The name of the relationship group.</param>
		public static RelationshipGroup AddRelationshipGroup(string name)
		{
			int resultArg;
			unsafe
			{
				Function.Call(Hash.ADD_RELATIONSHIP_GROUP, name, &resultArg);
			}

			return new RelationshipGroup(resultArg);
		}

		#endregion

		#region Drawing

		/// <summary>
		/// Draws a marker in the world, this needs to be done on a per frame basis
		/// </summary>
		/// <param name="type">The type of marker.</param>
		/// <param name="pos">The position of the marker.</param>
		/// <param name="dir">The direction the marker points in.</param>
		/// <param name="rot">The rotation of the marker.</param>
		/// <param name="scale">The amount to scale the marker by.</param>
		/// <param name="color">The color of the marker.</param>
		/// <param name="bobUpAndDown">if set to <c>true</c> the marker will bob up and down.</param>
		/// <param name="faceCamera">if set to <c>true</c> the marker will always face the camera, regardless of its rotation.</param>
		/// <param name="rotateY">if set to <c>true</c> rotates only on the y axis(heading).</param>
		/// <param name="textueDict">Name of texture dictionary to load the texture from, leave null for no texture in the marker.</param>
		/// <param name="textureName">Name of texture inside the dictionary to load the texture from, leave null for no texture in the marker.</param>
		/// <param name="drawOnEntity">if set to <c>true</c> draw on any <see cref="Entity"/> that intersects the marker.</param>
		public static void DrawMarker(MarkerType type, Vector3 pos, Vector3 dir, Vector3 rot, Vector3 scale, Color color, bool bobUpAndDown = false, bool faceCamera = false, bool rotateY = false, string textueDict = null, string textureName = null, bool drawOnEntity = false)
		{
			if (!string.IsNullOrEmpty(textueDict) && !string.IsNullOrEmpty(textureName))
			{
				Function.Call(Hash.DRAW_MARKER, type, pos.X, pos.Y, pos.Z, dir.X, dir.Y, dir.Z, rot.X, rot.Y, rot.Z, scale.X,
					scale.Y, scale.Z, color.R, color.G, color.B, color.A, bobUpAndDown, faceCamera, 2, rotateY, textueDict,
					textureName, drawOnEntity);
			}
			else
			{
				Function.Call(Hash.DRAW_MARKER, type, pos.X, pos.Y, pos.Z, dir.X, dir.Y, dir.Z, rot.X, rot.Y, rot.Z, scale.X,
					scale.Y, scale.Z, color.R, color.G, color.B, color.A, bobUpAndDown, faceCamera, 2, rotateY, 0, 0, drawOnEntity);
			}
		}

		/// <summary>
		/// Draws light around a region.
		/// </summary>
		/// <param name="position">The position to center the light around.</param>
		/// <param name="color">The color of the light.</param>
		/// <param name="range">How far the light should extend to.</param>
		/// <param name="intensity">The intensity: <c>0.0f</c> being no intensity, <c>1.0f</c> being full intensity.</param>
		public static void DrawLightWithRange(Vector3 position, Color color, float range, float intensity)
		{
			Function.Call(Hash.DRAW_LIGHT_WITH_RANGE, position.X, position.Y, position.Z, color.R, color.G, color.B, range,
				intensity);
		}

		public static void DrawSpotLight(Vector3 pos, Vector3 dir, Color color, float distance, float brightness, float roundness, float radius, float fadeout)
		{
			Function.Call(Hash.DRAW_SPOT_LIGHT, pos.X, pos.Y, pos.Z, dir.X, dir.Y, dir.Z, color.R, color.G, color.B, distance, brightness, roundness, radius, fadeout);
		}

		public static void DrawSpotLightWithShadow(Vector3 pos, Vector3 dir, Color color, float distance, float brightness, float roundness, float radius, float fadeout)
		{
			Function.Call(Hash._DRAW_SPOT_LIGHT_WITH_SHADOW, pos.X, pos.Y, pos.Z, dir.X, dir.Y, dir.Z, color.R, color.G, color.B, distance, brightness, roundness, radius, fadeout);
		}

		public static void DrawLine(Vector3 start, Vector3 end, Color color)
		{
			Function.Call(Hash.DRAW_LINE, start.X, start.Y, start.Z, end.X, end.Y, end.Z, color.R, color.G, color.B, color.A);
		}

		public static void DrawPolygon(Vector3 vertexA, Vector3 vertexB, Vector3 vertexC, Color color)
		{
			Function.Call(Hash.DRAW_POLY, vertexA.X, vertexA.Y, vertexA.Z, vertexB.X, vertexB.Y, vertexB.Z, vertexC.X, vertexC.Y, vertexC.Z, color.R, color.G, color.B, color.A);
		}

		#endregion

		#region Raycasting

		/// <summary>
		/// Creates a raycast between 2 points.
		/// </summary>
		/// <param name="source">The source of the raycast.</param>
		/// <param name="target">The target of the raycast.</param>
		/// <param name="options">What type of objects the raycast should intersect with.</param>
		/// <param name="ignoreEntity">Specify an <see cref="Entity"/> that the raycast should ignore, leave null for no entities ignored.</param>
		public static RaycastResult Raycast(Vector3 source, Vector3 target, IntersectFlags options, Entity ignoreEntity = null)
		{
			return new RaycastResult(Function.Call<int>(Hash._START_SHAPE_TEST_RAY, source.X, source.Y, source.Z, target.X, target.Y, target.Z, options, ignoreEntity == null ? 0 : ignoreEntity.Handle, 7));
		}
		/// <summary>
		/// Creates a raycast between 2 points.
		/// </summary>
		/// <param name="source">The source of the raycast.</param>
		/// <param name="direction">The direction of the raycast.</param>
		/// <param name="maxDistance">How far the raycast should go out to.</param>
		/// <param name="options">What type of objects the raycast should intersect with.</param>
		/// <param name="ignoreEntity">Specify an <see cref="Entity"/> that the raycast should ignore, leave null for no entities ignored.</param>
		public static RaycastResult Raycast(Vector3 source, Vector3 direction, float maxDistance, IntersectFlags options, Entity ignoreEntity = null)
		{
			Vector3 target = source + direction * maxDistance;

			return new RaycastResult(Function.Call<int>(Hash._START_SHAPE_TEST_RAY, source.X, source.Y, source.Z, target.X, target.Y, target.Z, options, ignoreEntity == null ? 0 : ignoreEntity.Handle, 7));
		}

		/// <summary>
		/// Creates a 3D raycast between 2 points.
		/// </summary>
		/// <param name="source">The source of the raycast.</param>
		/// <param name="target">The target of the raycast.</param>
		/// <param name="radius">The radius of the raycast.</param>
		/// <param name="options">What type of objects the raycast should intersect with.</param>
		/// <param name="ignoreEntity">Specify an <see cref="Entity"/> that the raycast should ignore, leave null for no entities ignored.</param>
		public static RaycastResult RaycastCapsule(Vector3 source, Vector3 target, float radius, IntersectFlags options, Entity ignoreEntity = null)
		{
			return new RaycastResult(Function.Call<int>(Hash.START_SHAPE_TEST_CAPSULE, source.X, source.Y, source.Z, target.X, target.Y, target.Z, radius, options, ignoreEntity == null ? 0 : ignoreEntity.Handle, 7));
		}
		/// <summary>
		/// Creates a 3D raycast between 2 points.
		/// </summary>
		/// <param name="source">The source of the raycast.</param>
		/// <param name="direction">The direction of the raycast.</param>
		/// <param name="radius">The radius of the raycast.</param>
		/// <param name="maxDistance">How far the raycast should go out to.</param>
		/// <param name="options">What type of objects the raycast should intersect with.</param>
		/// <param name="ignoreEntity">Specify an <see cref="Entity"/> that the raycast should ignore, leave null for no entities ignored.</param>
		public static RaycastResult RaycastCapsule(Vector3 source, Vector3 direction, float maxDistance, float radius, IntersectFlags options, Entity ignoreEntity = null)
		{
			Vector3 target = source + direction * maxDistance;

			return new RaycastResult(Function.Call<int>(Hash.START_SHAPE_TEST_CAPSULE, source.X, source.Y, source.Z, target.X, target.Y, target.Z, radius, options, ignoreEntity == null ? 0 : ignoreEntity.Handle, 7));
		}

		/// <summary>
		/// Determines where the crosshair intersects with the world.
		/// </summary>
		/// <returns>A <see cref="RaycastResult"/> containing information about where the crosshair intersects with the world.</returns>
		public static RaycastResult GetCrosshairCoordinates()
		{
			return Raycast(GameplayCamera.Position, GameplayCamera.GetOffsetPosition(new Vector3(0f, 1000f, 0f)), IntersectFlags.Everything, null);
		}
		/// <summary>
		/// Determines where the crosshair intersects with the world.
		/// </summary>
		/// <param name="intersectOptions">Type of <see cref="IntersectFlags">environment</see> the raycast should intersect with.</param>
		/// <param name="ignoreEntity">Prevent the raycast detecting a specific <see cref="Entity"/>.</param>
		/// <returns>A <see cref="RaycastResult"/> containing information about where the crosshair intersects with the world.</returns>
		public static RaycastResult GetCrosshairCoordinates(IntersectFlags intersectOptions = IntersectFlags.Everything, Entity ignoreEntity = null)
		{
			return Raycast(GameplayCamera.Position, GameplayCamera.GetOffsetPosition(new Vector3(0f, 1000f, 0f)), intersectOptions, ignoreEntity);
		}

		#endregion

		#region Positioning

		/// <summary>
		/// Gets the straight line distance between 2 positions.
		/// </summary>
		/// <param name="origin">The origin.</param>
		/// <param name="destination">The destination.</param>
		/// <returns>The distance</returns>
		public static float GetDistance(Vector3 origin, Vector3 destination)
		{
			return Function.Call<float>(Hash.GET_DISTANCE_BETWEEN_COORDS, origin.X, origin.Y, origin.Z, destination.X, destination.Y, destination.Z, 1);
		}
		/// <summary>
		/// Calculates the travel distance using roads and paths between 2 positions.
		/// </summary>
		/// <param name="origin">The origin.</param>
		/// <param name="destination">The destination.</param>
		/// <returns>The travel distance</returns>
		public static float CalculateTravelDistance(Vector3 origin, Vector3 destination)
		{
			return Function.Call<float>(Hash.CALCULATE_TRAVEL_DISTANCE_BETWEEN_POINTS, origin.X, origin.Y, origin.Z, destination.X, destination.Y, destination.Z);
		}

		/// <summary>
		/// Gets the height of the ground at a given position.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <returns>The height measured in meters</returns>
		public static float GetGroundHeight(Vector2 position)
		{
			return GetGroundHeight(new Vector3(position.X, position.Y, 1000f));
		}
		/// <summary>
		/// Gets the height of the ground at a given position.
		/// Note : If the Vector3 is already below the ground, this will return 0.
		/// You may want to use the other overloaded function to be safe.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <returns>The height measured in meters</returns>
		public static float GetGroundHeight(Vector3 position)
		{
			float resultArg;
			unsafe
			{
				Function.Call(Hash.GET_GROUND_Z_FOR_3D_COORD, position.X, position.Y, position.Z, &resultArg, false);
			}
			return resultArg;
		}

		/// <summary>
		/// Gets the nearest safe coordinate to position a <see cref="Ped"/>.
		/// </summary>
		/// <param name="position">The position to check around.</param>
		/// <param name="sidewalk">if set to <c>true</c> Only find positions on the sidewalk.</param>
		/// <param name="flags">The flags.</param>
		public static Vector3 GetSafeCoordForPed(Vector3 position, bool sidewalk = true, int flags = 0)
		{
			NativeVector3 outPos;
			unsafe
			{
				if (Function.Call<bool>(Hash.GET_SAFE_COORD_FOR_PED, position.X, position.Y, position.Z, sidewalk, &outPos, flags))
				{
					return outPos;
				}
			}
			return Vector3.Zero;
		}

		/// <summary>
		/// Gets the next position on the street where a <see cref="Vehicle"/> can be placed.
		/// </summary>
		/// <param name="position">The position to check around.</param>
		/// <param name="unoccupied">if set to <c>true</c> only find positions that dont already have a vehicle in them.</param>
		public static Vector3 GetNextPositionOnStreet(Vector2 position, bool unoccupied = false)
		{
			return GetNextPositionOnStreet(new Vector3(position.X, position.Y, 0f), unoccupied);
		}
		/// <summary>
		/// Gets the next position on the street where a <see cref="Vehicle"/> can be placed.
		/// </summary>
		/// <param name="position">The position to check around.</param>
		/// <param name="unoccupied">if set to <c>true</c> only find positions that dont already have a vehicle in them.</param>
		public static Vector3 GetNextPositionOnStreet(Vector3 position, bool unoccupied = false)
		{
			NativeVector3 outPos;

			unsafe
			{
				if (unoccupied)
				{
					for (int i = 1; i < 40; i++)
					{
						Function.Call(Hash.GET_NTH_CLOSEST_VEHICLE_NODE, position.X, position.Y, position.Z, i, &outPos, 1, 0x40400000, 0);

						position = outPos;

						if (!Function.Call<bool>(Hash.IS_POINT_OBSCURED_BY_A_MISSION_ENTITY, position.X, position.Y, position.Z, 5.0f,
							5.0f, 5.0f, 0))
						{
							return position;
						}
					}
				}
				else if (Function.Call<bool>(Hash.GET_NTH_CLOSEST_VEHICLE_NODE, position.X, position.Y, position.Z, 1, &outPos, 1,
					0x40400000, 0))
				{
					return outPos;
				}
			}

			return Vector3.Zero;
		}

		/// <summary>
		/// Gets the next position on the street where a <see cref="Ped"/> can be placed.
		/// </summary>
		/// <param name="position">The position to check around.</param>
		public static Vector3 GetNextPositionOnSidewalk(Vector2 position)
		{
			return GetNextPositionOnSidewalk(new Vector3(position.X, position.Y, 0f));
		}
		/// <summary>
		/// Gets the next position on the street where a <see cref="Ped"/> can be placed.
		/// </summary>
		/// <param name="position">The position to check around.</param>
		public static Vector3 GetNextPositionOnSidewalk(Vector3 position)
		{
			NativeVector3 outPos;

			unsafe
			{
				if (Function.Call<bool>(Hash.GET_SAFE_COORD_FOR_PED, position.X, position.Y, position.Z, true, &outPos, 0))
				{
					return outPos;
				}
				else if (Function.Call<bool>(Hash.GET_SAFE_COORD_FOR_PED, position.X, position.Y, position.Z, false, &outPos, 0))
				{
					return outPos;
				}
			}

			return Vector3.Zero;
		}

		/// <summary>
		/// Determines the name of the street which is the closest to the given coordinates.
		/// </summary>
		public static string GetStreetName(Vector2 position)
		{
			return GetStreetName(new Vector3(position.X, position.Y, 0f));
		}
		/// <summary>
		/// Determines the name of the street which is the closest to the given coordinates.
		/// </summary>
		public static string GetStreetName(Vector3 position)
		{
			int streetHash, crossingHash;
			unsafe
			{
				Function.Call(Hash.GET_STREET_NAME_AT_COORD, position.X, position.Y, position.Z, &streetHash, &crossingHash);
			}
			return Function.Call<string>(Hash.GET_STREET_NAME_FROM_HASH_KEY, streetHash);
		}
		/// <summary>
		/// Determines the name of the street which is the closest to the given coordinates.
		/// </summary>
		/// <param name="position">The coordinates of the street</param>
		/// <param name="crossingRoadName">If the coordinates are on an intersection, the name of the crossing road</param>
		/// <returns>Returns the name of the street the coordinates are on.</returns>
		public static string GetStreetName(Vector3 position, out string crossingRoadName)
		{
			int streetHash, crossingHash;
			unsafe
			{
				Function.Call(Hash.GET_STREET_NAME_AT_COORD, position.X, position.Y, position.Z, &streetHash, &crossingHash);
			}
			crossingRoadName = Function.Call<string>(Hash.GET_STREET_NAME_FROM_HASH_KEY, crossingHash);
			return Function.Call<string>(Hash.GET_STREET_NAME_FROM_HASH_KEY, streetHash);
		}

		/// <summary>
		/// Gets the display name of the a zone in the map.
		/// Use <see cref="Game.GetLocalizedString(string)"/> to convert to the localized name.
		/// </summary>
		/// <param name="position">The position on the map.</param>
		public static string GetZoneDisplayName(Vector2 position)
		{
			return GetZoneDisplayName(new Vector3(position.X, position.Y, 0f));
		}
		/// <summary>
		/// Gets the display name of the a zone in the map.
		/// Use <see cref="Game.GetLocalizedString(string)"/> to convert to the localized name.
		/// </summary>
		/// <param name="position">The position on the map.</param>
		public static string GetZoneDisplayName(Vector3 position)
		{
			return Function.Call<string>(Hash.GET_NAME_OF_ZONE, position.X, position.Y, position.Z);
		}

		/// <summary>
		/// Gets the localized name of the a zone in the map.
		/// </summary>
		/// <param name="position">The position on the map.</param>
		public static string GetZoneLocalizedName(Vector2 position)
		{
			return GetZoneLocalizedName(new Vector3(position.X, position.Y, 0f));
		}
		/// <summary>
		/// Gets the localized name of the a zone in the map.
		/// </summary>
		/// <param name="position">The position on the map.</param>
		public static string GetZoneLocalizedName(Vector3 position)
		{
			return Game.GetLocalizedString(GetZoneDisplayName(position));
		}

		#endregion
	}
}

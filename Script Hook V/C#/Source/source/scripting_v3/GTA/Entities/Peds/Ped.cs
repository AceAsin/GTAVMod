//
// Copyright (C) 2015 crosire & contributors
// License: https://github.com/crosire/scripthookvdotnet#license
//

using GTA.Math;
using GTA.Native;
using GTA.NaturalMotion;
using System;
using System.Linq;

namespace GTA
{
	public sealed class Ped : Entity
	{
		#region Fields
		TaskInvoker _tasks;
		Euphoria _euphoria;
		WeaponCollection _weapons;
		Style _style;
		PedBoneCollection _pedBones;

		internal static readonly string[] _speechModifierNames = {
			"SPEECH_PARAMS_STANDARD",
			"SPEECH_PARAMS_ALLOW_REPEAT",
			"SPEECH_PARAMS_BEAT",
			"SPEECH_PARAMS_FORCE",
			"SPEECH_PARAMS_FORCE_FRONTEND",
			"SPEECH_PARAMS_FORCE_NO_REPEAT_FRONTEND",
			"SPEECH_PARAMS_FORCE_NORMAL",
			"SPEECH_PARAMS_FORCE_NORMAL_CLEAR",
			"SPEECH_PARAMS_FORCE_NORMAL_CRITICAL",
			"SPEECH_PARAMS_FORCE_SHOUTED",
			"SPEECH_PARAMS_FORCE_SHOUTED_CLEAR",
			"SPEECH_PARAMS_FORCE_SHOUTED_CRITICAL",
			"SPEECH_PARAMS_FORCE_PRELOAD_ONLY",
			"SPEECH_PARAMS_MEGAPHONE",
			"SPEECH_PARAMS_HELI",
			"SPEECH_PARAMS_FORCE_MEGAPHONE",
			"SPEECH_PARAMS_FORCE_HELI",
			"SPEECH_PARAMS_INTERRUPT",
			"SPEECH_PARAMS_INTERRUPT_SHOUTED",
			"SPEECH_PARAMS_INTERRUPT_SHOUTED_CLEAR",
			"SPEECH_PARAMS_INTERRUPT_SHOUTED_CRITICAL",
			"SPEECH_PARAMS_INTERRUPT_NO_FORCE",
			"SPEECH_PARAMS_INTERRUPT_FRONTEND",
			"SPEECH_PARAMS_INTERRUPT_NO_FORCE_FRONTEND",
			"SPEECH_PARAMS_ADD_BLIP",
			"SPEECH_PARAMS_ADD_BLIP_ALLOW_REPEAT",
			"SPEECH_PARAMS_ADD_BLIP_FORCE",
			"SPEECH_PARAMS_ADD_BLIP_SHOUTED",
			"SPEECH_PARAMS_ADD_BLIP_SHOUTED_FORCE",
			"SPEECH_PARAMS_ADD_BLIP_INTERRUPT",
			"SPEECH_PARAMS_ADD_BLIP_INTERRUPT_FORCE",
			"SPEECH_PARAMS_FORCE_PRELOAD_ONLY_SHOUTED",
			"SPEECH_PARAMS_FORCE_PRELOAD_ONLY_SHOUTED_CLEAR",
			"SPEECH_PARAMS_FORCE_PRELOAD_ONLY_SHOUTED_CRITICAL",
			"SPEECH_PARAMS_SHOUTED",
			"SPEECH_PARAMS_SHOUTED_CLEAR",
			"SPEECH_PARAMS_SHOUTED_CRITICAL",
		};
		#endregion

		internal Ped(int handle) : base(handle)
		{
		}

		/// <summary>
		/// Spawn an identical clone of this <see cref="Ped"/>.
		/// </summary>
		/// <param name="heading">The direction the clone should be facing.</param>
		public Ped Clone(float heading = 0.0f)
		{
			return new Ped(Function.Call<int>(Hash.CLONE_PED, Handle, heading, false, false));
		}

		/// <summary>
		/// Kills this <see cref="Ped"/> immediately.
		/// </summary>
		public void Kill()
		{
			Health = 0;
		}

		/// <summary>
		/// Resurrects this <see cref="Ped"/> from death.
		/// </summary>
		public void Resurrect()
		{
			int health = MaxHealth;
			bool isCollisionEnabled = IsCollisionEnabled;

			Function.Call(Hash.RESURRECT_PED, Handle);
			Health = MaxHealth = health;
			IsCollisionEnabled = isCollisionEnabled;
			Function.Call(Hash.CLEAR_PED_TASKS_IMMEDIATELY, Handle);
		}

		/// <summary>
		/// Determines if this <see cref="Ped"/> exists.
		/// </summary>
		/// <returns><c>true</c> if this <see cref="Ped"/> exists; otherwise, <c>false</c></returns>
		public new bool Exists()
		{
			return EntityType == EntityType.Ped;
		}

		#region Styling

		/// <summary>
		/// Gets a value indicating whether this <see cref="Ped"/> is human.
		/// </summary>
		/// <value>
		///   <c>true</c> if this <see cref="Ped"/> is human; otherwise, <c>false</c>.
		/// </value>
		public bool IsHuman => Function.Call<bool>(Hash.IS_PED_HUMAN, Handle);

		public bool IsCuffed => Function.Call<bool>(Hash.IS_PED_CUFFED, Handle);

		public bool CanWearHelmet
		{
			set => Function.Call(Hash.SET_PED_HELMET, Handle, value);
		}

		public bool IsWearingHelmet => Function.Call<bool>(Hash.IS_PED_WEARING_HELMET, Handle);

		public void ClearBloodDamage()
		{
			Function.Call(Hash.CLEAR_PED_BLOOD_DAMAGE, Handle);
		}

		public void ClearVisibleDamage()
		{
			Function.Call(Hash.RESET_PED_VISIBLE_DAMAGE, Handle);
		}

		public void GiveHelmet(bool canBeRemovedByPed, Helmet helmetType, int textureIndex)
		{
			Function.Call(Hash.GIVE_PED_HELMET, Handle, !canBeRemovedByPed, helmetType, textureIndex);
		}

		public void RemoveHelmet(bool instantly)
		{
			Function.Call(Hash.REMOVE_PED_HELMET, Handle, instantly);
		}

		/// <summary>
		/// Opens a list of clothing and prop configurations that this <see cref="Ped"/> can wear.
		/// </summary>
		public Style Style => _style ?? (_style = new Style(this));

		/// <summary>
		/// Gets the gender of this <see cref="Ped"/>.
		/// </summary>
		public Gender Gender => Function.Call<bool>(Hash.IS_PED_MALE, Handle) ? Gender.Male : Gender.Female;

		/// <summary>
		/// Gets or sets the how much sweat should be rendered on this <see cref="Ped"/>.
		/// </summary>
		/// <value>
		/// The sweat from 0 to 100, 0 being no sweat, 100 being saturated.
		/// </value>
		public float Sweat
		{
			get
			{
				var address = MemoryAddress;
				if (address == IntPtr.Zero)
				{
					return 0;
				}

				int offset = (Game.Version >= GameVersion.v1_0_877_1_Steam ? 0x11A0 : 0x1170);
				offset = Game.Version >= GameVersion.v1_0_944_2_Steam ? 0x11B0 : offset;
				offset = Game.Version >= GameVersion.v1_0_2060_0_Steam ? 0x11C0 : offset;

				return SHVDN.NativeMemory.ReadFloat(address + offset);
			}
			set
			{
				if (value < 0)
				{
					value = 0;
				}
				if (value > 100)
				{
					value = 100;
				}

				Function.Call(Hash.SET_PED_SWEAT, Handle, value);
			}
		}

		/// <summary>
		/// Sets how high up on this <see cref="Ped"/>s body water should be visible.
		/// </summary>
		/// <value>
		/// The height ranges from 0.0f to 1.99f, 0.0f being no water visible, 1.99f being covered in water.
		/// </value>
		public float WetnessHeight
		{
			set
			{
				if (value == 0.0f)
				{
					Function.Call(Hash.CLEAR_PED_WETNESS, Handle);
				}
				else
				{
					Function.Call<float>(Hash.SET_PED_WETNESS_HEIGHT, Handle, value);
				}
			}
		}

		#endregion

		#region Configuration

		/// <summary>
		/// Gets or sets how much Armor this <see cref="Ped"/> is wearing.
		/// </summary>
		/// <remarks>if you need to get or set the value strictly, use <see cref="ArmorFloat"/> instead.</remarks>
		public int Armor
		{
			get => Function.Call<int>(Hash.GET_PED_ARMOUR, Handle);
			set => Function.Call(Hash.SET_PED_ARMOUR, Handle, value);
		}

		public float ArmorFloat
		{
			get
			{
				var address = MemoryAddress;
				if (address == IntPtr.Zero)
				{
					return 0.0f;
				}

				int offset = Game.Version >= GameVersion.v1_0_372_2_Steam ? 0x1474 : 0x1464;
				offset = Game.Version >= GameVersion.v1_0_877_1_Steam ? 0x14A0 : offset;
				offset = Game.Version >= GameVersion.v1_0_944_2_Steam ? 0x14B0 : offset;
				offset = Game.Version >= GameVersion.v1_0_1290_1_Steam ? 0x14B8 : offset;
				offset = Game.Version >= GameVersion.v1_0_2060_0_Steam ? 0x14E0 : offset;

				return SHVDN.NativeMemory.ReadFloat(address + offset);
			}
			set
			{
				var address = MemoryAddress;
				if (address == IntPtr.Zero)
				{
					return;
				}

				int offset = Game.Version >= GameVersion.v1_0_372_2_Steam ? 0x1474 : 0x1464;
				offset = Game.Version >= GameVersion.v1_0_877_1_Steam ? 0x14A0 : offset;
				offset = Game.Version >= GameVersion.v1_0_944_2_Steam ? 0x14B0 : offset;
				offset = Game.Version >= GameVersion.v1_0_1290_1_Steam ? 0x14B8 : offset;
				offset = Game.Version >= GameVersion.v1_0_2060_0_Steam ? 0x14E0 : offset;

				SHVDN.NativeMemory.WriteFloat(address + offset, value);
			}
		}

		/// <summary>
		/// Gets or sets how much money this <see cref="Ped"/> is carrying.
		/// </summary>
		public int Money
		{
			get => Function.Call<int>(Hash.GET_PED_MONEY, Handle);
			set => Function.Call(Hash.SET_PED_MONEY, Handle, value);
		}

		/// <summary>
		/// Gets or sets the maximum health of this <see cref="Ped"/> as an <see cref="int"/>.
		/// </summary>
		/// <value>
		/// The maximum health as an integer.
		/// </value>
		public override int MaxHealth
		{
			get => Function.Call<int>(Hash.GET_PED_MAX_HEALTH, Handle);
			set => Function.Call(Hash.SET_PED_MAX_HEALTH, Handle, value);
		}

		public bool IsPlayer => Function.Call<bool>(Hash.IS_PED_A_PLAYER, Handle);

		public bool GetConfigFlag(int flagID)
		{
			return Function.Call<bool>(Hash.GET_PED_CONFIG_FLAG, Handle, flagID, true);
		}

		public void SetConfigFlag(int flagID, bool value)
		{
			Function.Call(Hash.SET_PED_CONFIG_FLAG, Handle, flagID, value);
		}

		public void ResetConfigFlag(int flagID)
		{
			Function.Call(Hash.SET_PED_RESET_FLAG, Handle, flagID, true);
		}

		/// <summary>
		/// Gets a collection of the <see cref="PedBone"/>s in this <see cref="Ped"/>.
		/// </summary>
		public new PedBoneCollection Bones => _pedBones ?? (_pedBones = new PedBoneCollection(this));

		#endregion

		#region Tasks

		public bool IsIdle => !IsInjured && !IsRagdoll && !IsInAir && !IsOnFire && !IsDucking && !IsGettingIntoVehicle && !IsInCombat && !IsInMeleeCombat && (!IsInVehicle() || IsSittingInVehicle());

		public bool IsProne => Function.Call<bool>(Hash.IS_PED_PRONE, Handle);

		public bool IsGettingUp => Function.Call<bool>(Hash.IS_PED_GETTING_UP, Handle);

		public bool IsDiving => Function.Call<bool>(Hash.IS_PED_DIVING, Handle);

		public bool IsJumping => Function.Call<bool>(Hash.IS_PED_JUMPING, Handle);

		public bool IsFalling => Function.Call<bool>(Hash.IS_PED_FALLING, Handle);

		public bool IsVaulting => Function.Call<bool>(Hash.IS_PED_VAULTING, Handle);

		public bool IsClimbing => Function.Call<bool>(Hash.IS_PED_CLIMBING, Handle);

		public bool IsWalking => Function.Call<bool>(Hash.IS_PED_WALKING, Handle);

		public bool IsRunning => Function.Call<bool>(Hash.IS_PED_RUNNING, Handle);

		public bool IsSprinting => Function.Call<bool>(Hash.IS_PED_SPRINTING, Handle);

		public bool IsStopped => Function.Call<bool>(Hash.IS_PED_STOPPED, Handle);

		public bool IsSwimming => Function.Call<bool>(Hash.IS_PED_SWIMMING, Handle);

		public bool IsSwimmingUnderWater => Function.Call<bool>(Hash.IS_PED_SWIMMING_UNDER_WATER, Handle);

		public bool IsDucking
		{
			get => Function.Call<bool>(Hash.IS_PED_DUCKING, Handle);
			set => Function.Call(Hash.SET_PED_DUCKING, Handle, value);
		}

		public bool IsHeadtracking(Entity entity)
		{
			return Function.Call<bool>(Hash.IS_PED_HEADTRACKING_ENTITY, Handle, entity.Handle);
		}

		public bool AlwaysKeepTask
		{
			set => Function.Call(Hash.SET_PED_KEEP_TASK, Handle, value);
		}

		/// <summary>
		/// Sets whether permanent events are blocked for this <see cref="Ped"/>.
		///  If permanent events are blocked, this <see cref="Ped"/> will only do as it's told, and won't flee when shot at, etc.
		/// </summary>
		/// <value>
		///   <c>true</c> if permanent events are blocked; otherwise, <c>false</c>.
		/// </value>
		public bool BlockPermanentEvents
		{
			set => Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, Handle, value);
		}

		/// <summary>
		/// Opens a list of <see cref="TaskInvoker"/> that this <see cref="Ped"/> can carry out.
		/// </summary>
		public TaskInvoker Task => _tasks ?? (_tasks = new TaskInvoker(this));

		/// <summary>
		/// Gets the stage of the <see cref="TaskSequence"/> this <see cref="Ped"/> is currently executing.
		/// </summary>
		public int TaskSequenceProgress => Function.Call<int>(Hash.GET_SEQUENCE_PROGRESS, Handle);

		#endregion

		#region Euphoria & Ragdoll

		public void Ragdoll(int duration = -1, RagdollType ragdollType = RagdollType.Normal)
		{
			CanRagdoll = true;
			Function.Call(Hash.SET_PED_TO_RAGDOLL, Handle, duration, duration, ragdollType, false, false, false);
		}

		public void CancelRagdoll()
		{
			Function.Call(Hash.SET_PED_TO_RAGDOLL, Handle, 1, 1, 1, false, false, false);
		}

		public bool IsRagdoll => Function.Call<bool>(Hash.IS_PED_RAGDOLL, Handle);

		public bool CanRagdoll
		{
			get => Function.Call<bool>(Hash.CAN_PED_RAGDOLL, Handle);
			set => Function.Call(Hash.SET_PED_CAN_RAGDOLL, Handle, value);
		}

		/// <summary>
		/// Opens a list of <see cref="GTA.NaturalMotion.Euphoria"/> Helpers which can be applied to this <see cref="Ped"/>.
		/// </summary>
		public Euphoria Euphoria => _euphoria ?? (_euphoria = new Euphoria(this));

		#endregion

		#region Weapon Interaction

		/// <summary>
		/// Gets or sets how accurate this <see cref="Ped"/>s shooting ability is.
		///  The higher the value of this property is, the more likely it is that this <see cref="Ped"/> will shoot at exactly where they are aiming at.
		/// </summary>
		/// <value>
		/// The accuracy from 0 to 100, 0 being very inaccurate, which means this <see cref="Ped"/> cannot shoot at exactly where they are aiming at,
		///  100 being perfectly accurate.
		/// </value>
		public int Accuracy
		{
			get => Function.Call<int>(Hash.GET_PED_ACCURACY, Handle);
			set => Function.Call(Hash.SET_PED_ACCURACY, Handle, value);
		}

		/// <summary>
		/// Sets the rate this <see cref="Ped"/> will shoot at.
		/// </summary>
		/// <value>
		/// The shoot rate from 0.0f to 1000.0f, 100.0f is the default value.
		/// </value>
		public int ShootRate
		{
			set => Function.Call(Hash.SET_PED_SHOOT_RATE, Handle, value);
		}

		/// <summary>
		/// Sets the pattern this <see cref="Ped"/> uses to fire weapons.
		/// </summary>
		public FiringPattern FiringPattern
		{
			set => Function.Call(Hash.SET_PED_FIRING_PATTERN, Handle, value);
		}

		/// <summary>
		/// Gets a collection of all this <see cref="Ped"/>s <see cref="Weapon"/>s.
		/// </summary>
		public WeaponCollection Weapons => _weapons ?? (_weapons = new WeaponCollection(this));

		/// <summary>
		/// Gets the vehicle weapon this <see cref="Ped"/> is using.
		/// <remarks>The vehicle weapon, returns <see cref="VehicleWeaponHash.Invalid"/> if this <see cref="Ped"/> isnt using a vehicle weapon.</remarks>
		/// </summary>
		public VehicleWeaponHash VehicleWeapon
		{
			get
			{
				unsafe
				{
					int hash;
					return Function.Call<bool>(Hash.GET_CURRENT_PED_VEHICLE_WEAPON, Handle, &hash) ?
						(VehicleWeaponHash)hash : VehicleWeaponHash.Invalid;
				}
			}
			set
			{
				Function.Call(Hash.SET_CURRENT_PED_VEHICLE_WEAPON, Handle, value);
			}
		}

		/// <summary>
		/// Sets if this <see cref="Ped"/> can switch between different weapons.
		/// </summary>
		public bool CanSwitchWeapons
		{
			set => Function.Call(Hash.SET_PED_CAN_SWITCH_WEAPON, Handle, value);
		}

		#endregion

		#region Vehicle Interaction

		public bool IsOnBike => Function.Call<bool>(Hash.IS_PED_ON_ANY_BIKE, Handle);

		public bool IsOnFoot => Function.Call<bool>(Hash.IS_PED_ON_FOOT, Handle);

		public bool IsInSub => Function.Call<bool>(Hash.IS_PED_IN_ANY_SUB, Handle);

		public bool IsInTaxi => Function.Call<bool>(Hash.IS_PED_IN_ANY_TAXI, Handle);

		public bool IsInTrain => Function.Call<bool>(Hash.IS_PED_IN_ANY_TRAIN, Handle);

		public bool IsInHeli => Function.Call<bool>(Hash.IS_PED_IN_ANY_HELI, Handle);

		public bool IsInPlane => Function.Call<bool>(Hash.IS_PED_IN_ANY_PLANE, Handle);

		public bool IsInFlyingVehicle => Function.Call<bool>(Hash.IS_PED_IN_FLYING_VEHICLE, Handle);

		public bool IsInBoat => Function.Call<bool>(Hash.IS_PED_IN_ANY_BOAT, Handle);

		public bool IsInPoliceVehicle => Function.Call<bool>(Hash.IS_PED_IN_ANY_POLICE_VEHICLE, Handle);

		public bool IsGettingIntoVehicle => Function.Call<bool>(Hash.IS_PED_GETTING_INTO_A_VEHICLE, Handle);

		/// <summary>
		/// Gets a value indicating whether this <see cref="Ped"/> is jumping out of their vehicle.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Ped"/> is jumping out of their vehicle; otherwise, <c>false</c>.
		/// </value>
		public bool IsJumpingOutOfVehicle => Function.Call<bool>(Hash.IS_PED_JUMPING_OUT_OF_VEHICLE, Handle);

		public bool IsTryingToEnterALockedVehicle => Function.Call<bool>(Hash.IS_PED_TRYING_TO_ENTER_A_LOCKED_VEHICLE, Handle);

		public bool CanBeDraggedOutOfVehicle
		{
			set => Function.Call(Hash.SET_PED_CAN_BE_DRAGGED_OUT, Handle, value);
		}

		public bool CanBeKnockedOffBike
		{
			set => Function.Call(Hash.SET_PED_CAN_BE_KNOCKED_OFF_VEHICLE, Handle, value);
		}

		public bool CanFlyThroughWindscreen
		{
			get => Function.Call<bool>(Hash.GET_PED_CONFIG_FLAG, Handle, 32, true);
			set => Function.Call(Hash.SET_PED_CONFIG_FLAG, Handle, 32, value);
		}

		public bool IsInVehicle()
		{
			return Function.Call<bool>(Hash.IS_PED_IN_ANY_VEHICLE, Handle, 0);
		}
		public bool IsInVehicle(Vehicle vehicle)
		{
			return Function.Call<bool>(Hash.IS_PED_IN_VEHICLE, Handle, vehicle.Handle, 0);
		}

		public bool IsSittingInVehicle()
		{
			return Function.Call<bool>(Hash.IS_PED_SITTING_IN_ANY_VEHICLE, Handle);
		}
		public bool IsSittingInVehicle(Vehicle vehicle)
		{
			return Function.Call<bool>(Hash.IS_PED_SITTING_IN_VEHICLE, Handle, vehicle.Handle);
		}

		public void SetIntoVehicle(Vehicle vehicle, VehicleSeat seat)
		{
			Function.Call(Hash.SET_PED_INTO_VEHICLE, Handle, vehicle.Handle, seat);
		}

		/// <summary>
		/// Gets the last <see cref="Vehicle"/> this <see cref="Ped"/> used.
		/// </summary>
		/// <remarks>returns <c>null</c> if the last vehicle doesn't exist.</remarks>
		public Vehicle LastVehicle
		{
			get
			{
				var veh = new Vehicle(Function.Call<int>(Hash.GET_VEHICLE_PED_IS_IN, Handle, true));
				return veh.Exists() ? veh : null;
			}
		}

		/// <summary>
		/// Gets the current <see cref="Vehicle"/> this <see cref="Ped"/> is using.
		/// </summary>
		/// <remarks>returns <c>null</c> if this <see cref="Ped"/> isn't in a <see cref="Vehicle"/>.</remarks>
		public Vehicle CurrentVehicle
		{
			get
			{
				var veh = new Vehicle(Function.Call<int>(Hash.GET_VEHICLE_PED_IS_IN, Handle, false));
				return veh.Exists() ? veh : null;
			}
		}

		/// <summary>
		/// Gets the <see cref="Vehicle"/> this <see cref="Ped"/> is trying to enter.
		/// </summary>
		/// <remarks>returns <c>null</c> if this <see cref="Ped"/> isn't trying to enter a <see cref="Vehicle"/>.</remarks>
		public Vehicle VehicleTryingToEnter
		{
			get
			{
				var veh = new Vehicle(Function.Call<int>(Hash.GET_VEHICLE_PED_IS_TRYING_TO_ENTER, Handle));
				return veh.Exists() ? veh : null;
			}
		}

		/// <summary>
		/// Gets the <see cref="VehicleSeat"/> this <see cref="Ped"/> is in.
		/// </summary>
		/// <value>
		/// The <see cref="VehicleSeat"/> this <see cref="Ped"/> is in if this <see cref="Ped"/> is in a <see cref="Vehicle"/>; otherwise, <see cref="VehicleSeat.None"/>.
		/// </value>
		public VehicleSeat SeatIndex
		{
			get
			{
				var address = MemoryAddress;
				if (address == IntPtr.Zero)
				{
					return VehicleSeat.None;
				}

				int offset = (Game.Version >= GameVersion.v1_0_877_1_Steam ? 0x1588 : 0x1540);
				offset = Game.Version >= GameVersion.v1_0_944_2_Steam ? 0x1598 : offset;
				offset = Game.Version >= GameVersion.v1_0_1290_1_Steam ? 0x15A0 : offset;
				offset = Game.Version >= GameVersion.v1_0_2060_0_Steam ? 0x15C8 : offset;

				int seatIndex = (sbyte)SHVDN.NativeMemory.ReadByte(address + offset);
				return (seatIndex >= 0 && IsInVehicle()) ? (VehicleSeat)(seatIndex - 1) : VehicleSeat.None;
			}
		}

		#endregion

		#region Driving

		public float DrivingSpeed
		{
			set => Function.Call(Hash.SET_DRIVE_TASK_CRUISE_SPEED, Handle, value);
		}

		/// <summary>
		/// Sets the maximum driving speed this <see cref="Ped"/> can drive at.
		/// </summary>
		public float MaxDrivingSpeed
		{
			set => Function.Call(Hash.SET_DRIVE_TASK_MAX_CRUISE_SPEED, Handle, value);
		}

		public DrivingStyle DrivingStyle
		{
			set => Function.Call(Hash.SET_DRIVE_TASK_DRIVING_STYLE, Handle, value);
		}

		public VehicleDrivingFlags VehicleDrivingFlags
		{
			set => Function.Call(Hash.SET_DRIVE_TASK_DRIVING_STYLE, Handle, value);
		}

		#endregion

		#region Jacking

		public bool IsJacking => Function.Call<bool>(Hash.IS_PED_JACKING, Handle);

		public bool IsBeingJacked => Function.Call<bool>(Hash.IS_PED_BEING_JACKED, Handle);

		/// <summary>
		/// Sets a value indicating whether this <see cref="Ped"/> will stay in the vehicle when the driver gets jacked.
		/// </summary>
		/// <value>
		/// <c>true</c> if <see cref="Ped"/> stays in vehicle when jacked; otherwise, <c>false</c>.
		/// </value>
		public bool StaysInVehicleWhenJacked
		{
			set => Function.Call(Hash.SET_PED_STAY_IN_VEHICLE_WHEN_JACKED, Handle, value);
		}

		public Ped Jacker
		{
			get
			{
				var ped = new Ped(Function.Call<int>(Hash.GET_PEDS_JACKER, Handle));
				return ped.Exists() ? ped : null;
			}
		}

		public Ped JackTarget
		{
			get
			{
				var ped = new Ped(Function.Call<int>(Hash.GET_JACK_TARGET, Handle));
				return ped.Exists() ? ped : null;
			}
		}

		#endregion

		#region Parachuting

		public bool IsInParachuteFreeFall => Function.Call<bool>(Hash.IS_PED_IN_PARACHUTE_FREE_FALL, Handle);

		public void OpenParachute()
		{
			Function.Call(Hash.FORCE_PED_TO_OPEN_PARACHUTE, Handle);
		}

		public ParachuteState ParachuteState => Function.Call<ParachuteState>(Hash.GET_PED_PARACHUTE_STATE, Handle);

		public ParachuteLandingType ParachuteLandingType => Function.Call<ParachuteLandingType>(Hash.GET_PED_PARACHUTE_LANDING_TYPE, Handle);

		#endregion

		#region Combat

		public bool IsEnemy
		{
			set => Function.Call(Hash.SET_PED_AS_ENEMY, Handle, value);
		}

		public bool IsPriorityTargetForEnemies
		{
			set => Function.Call(Hash.SET_ENTITY_IS_TARGET_PRIORITY, Handle, value, 0);
		}

		public bool IsFleeing => Function.Call<bool>(Hash.IS_PED_FLEEING, Handle);

		public bool IsInjured => Function.Call<bool>(Hash.IS_PED_INJURED, Handle);

		public bool IsInStealthMode => Function.Call<bool>(Hash.GET_PED_STEALTH_MOVEMENT, Handle);

		public bool IsInCombat => Function.Call<bool>(Hash.IS_PED_IN_COMBAT, Handle);

		public bool IsInMeleeCombat => Function.Call<bool>(Hash.IS_PED_IN_MELEE_COMBAT, Handle);

		public bool IsAiming => GetConfigFlag(78);

		public bool IsPlantingBomb => Function.Call<bool>(Hash.IS_PED_PLANTING_BOMB, Handle);

		public bool IsShooting => Function.Call<bool>(Hash.IS_PED_SHOOTING, Handle);

		public bool IsReloading => Function.Call<bool>(Hash.IS_PED_RELOADING, Handle);

		public bool IsDoingDriveBy => Function.Call<bool>(Hash.IS_PED_DOING_DRIVEBY, Handle);

		public bool IsGoingIntoCover => Function.Call<bool>(Hash.IS_PED_GOING_INTO_COVER, Handle);

		public bool IsAimingFromCover => Function.Call<bool>(Hash.IS_PED_AIMING_FROM_COVER, Handle);

		public bool IsBeingStunned => Function.Call<bool>(Hash.IS_PED_BEING_STUNNED, Handle);

		public bool IsBeingStealthKilled => Function.Call<bool>(Hash.IS_PED_BEING_STEALTH_KILLED, Handle);

		public bool IsPerformingStealthKill => Function.Call<bool>(Hash.IS_PED_PERFORMING_STEALTH_KILL, Handle);

		public bool IsInCover => Function.Call<bool>(Hash.IS_PED_IN_COVER, Handle, false);

		public bool IsInCoverFacingLeft => Function.Call<bool>(Hash.IS_PED_IN_COVER_FACING_LEFT, Handle);

		public bool CanBeTargetted
		{
			set => Function.Call(Hash.SET_PED_CAN_BE_TARGETTED, Handle, value);
		}

		public bool CanBeShotInVehicle
		{
			set => Function.Call(Hash.SET_PED_CAN_BE_SHOT_IN_VEHICLE, Handle, value);
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Ped"/> was killed by a stealth attack.
		/// </summary>
		/// <value>
		///   <c>true</c> if this <see cref="Ped"/> was killed by stealth; otherwise, <c>false</c>.
		/// </value>
		public bool WasKilledByStealth => Function.Call<bool>(Hash.WAS_PED_KILLED_BY_STEALTH, Handle);

		/// <summary>
		/// Gets a value indicating whether this <see cref="Ped"/> was killed by a takedown.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Ped"/> was killed by a takedown; otherwise, <c>false</c>.
		/// </value>
		public bool WasKilledByTakedown => Function.Call<bool>(Hash.WAS_PED_KILLED_BY_TAKEDOWN, Handle);

		public Ped MeleeTarget
		{
			get
			{
				var ped = new Ped(Function.Call<int>(Hash.GET_MELEE_TARGET_FOR_PED, Handle));
				return ped.Exists() ? ped : null;
			}
		}

		public bool IsInCombatAgainst(Ped target)
		{
			return Function.Call<bool>(Hash.IS_PED_IN_COMBAT, Handle, target.Handle);
		}

		public Entity Killer => FromHandle(Function.Call<int>(Hash.GET_PED_SOURCE_OF_DEATH, Handle));

		#endregion

		#region Damaging

		public bool CanWrithe
		{
			get => !GetConfigFlag(281);
			set => SetConfigFlag(281, !value);
		}

		/// <summary>
		/// Gets or Sets whether this <see cref="Ped"/> can suffer critical damage (which deals 1000 times base damages to non-player characters with default weapon configs) when bullets hit this <see cref="Ped"/>'s head bone or its child bones.
		///  If this <see cref="Ped"/> can't suffer critical damage, they will take base damage of weapons when bullets hit their head bone or its child bones, just like when bullets hit a bone other than their head bone, its child bones, or limb bones.
		/// </summary>
		/// <value>
		///   <c>true</c> if this <see cref="Ped"/> can suffer critical damage; otherwise, <c>false</c>.
		/// </value>
		public bool CanSufferCriticalHits
		{
			get
			{
				var address = MemoryAddress;
				if (address == IntPtr.Zero)
				{
					return false;
				}

				int offset = Game.Version >= GameVersion.v1_0_372_2_Steam ? 0x13BC : 0x13AC;
				offset = (Game.Version >= GameVersion.v1_0_877_1_Steam ? 0x13E4 : offset);
				offset = Game.Version >= GameVersion.v1_0_944_2_Steam ? 0x13F4 : offset;
				offset = Game.Version >= GameVersion.v1_0_2060_0_Steam ? 0x1414 : offset;

				return (SHVDN.NativeMemory.ReadByte(address + offset) & (1 << 2)) == 0;
			}
			set => Function.Call(Hash.SET_PED_SUFFERS_CRITICAL_HITS, Handle, value);
		}

		public bool DiesOnLowHealth
		{
			set => Function.Call(Hash.SET_PED_DIES_WHEN_INJURED, Handle, value);
		}

		public bool DiesInstantlyInWater
		{
			set => Function.Call(Hash.SET_PED_DIES_INSTANTLY_IN_WATER, Handle, value);
		}

		public bool DrownsInWater
		{
			set => Function.Call(Hash.SET_PED_DIES_IN_WATER, Handle, value);
		}

		public bool DrownsInSinkingVehicle
		{
			set => Function.Call(Hash.SET_PED_DIES_IN_SINKING_VEHICLE, Handle, value);
		}

		/// <summary>
		/// Sets whether this <see cref="Ped"/> will drop the equipped weapon when they get killed.
		///  Note that <see cref="Ped"/>s will drop only their equipped weapon when they get killed.
		/// </summary>
		/// <value>
		/// <c>true</c> if <see cref="Ped"/> drops the equipped weapon when killed; otherwise, <c>false</c>.
		/// </value>
		public bool DropsEquippedWeaponOnDeath
		{
			get
			{
				var address = MemoryAddress;
				if (address == IntPtr.Zero)
				{
					return false;
				}

				int offset = (Game.Version >= GameVersion.v1_0_877_1_Steam ? 0x13E5 : 0x13BD);
				offset = Game.Version >= GameVersion.v1_0_944_2_Steam ? 0x13F5 : offset;
				offset = Game.Version >= GameVersion.v1_0_2060_0_Steam ? 0x1415 : offset;

				return (SHVDN.NativeMemory.ReadByte(address + offset) & (1 << 6)) == 0;
			}
			set => Function.Call(Hash.SET_PED_DROPS_WEAPONS_WHEN_DEAD, Handle, value);
		}

		public void ApplyDamage(int damageAmount)
		{
			Function.Call(Hash.APPLY_DAMAGE_TO_PED, Handle, damageAmount, true);
		}

		public override bool HasBeenDamagedBy(WeaponHash weapon)
		{
			return Function.Call<bool>(Hash.HAS_PED_BEEN_DAMAGED_BY_WEAPON, Handle, weapon, 0);
		}

		public override bool HasBeenDamagedByAnyWeapon()
		{
			return Function.Call<bool>(Hash.HAS_PED_BEEN_DAMAGED_BY_WEAPON, Handle, 0, 2);
		}

		public override bool HasBeenDamagedByAnyMeleeWeapon()
		{
			return Function.Call<bool>(Hash.HAS_PED_BEEN_DAMAGED_BY_WEAPON, Handle, 0, 1);
		}

		public override void ClearLastWeaponDamage()
		{
			Function.Call(Hash.CLEAR_PED_LAST_WEAPON_DAMAGE, Handle);
		}

		/// <summary>
		/// Gets or sets the injury health threshold for this <see cref="Ped"/>.
		/// The pedestrian is considered injured when its health drops below this value.
		/// The pedestrian dies on attacks when its health is below this value.
		/// </summary>
		/// <value>
		/// The injury health threshold. Should be below <see cref="Entity.MaxHealth"/>.
		/// </value>
		/// <remarks>
		/// Note on player controlled pedestrians: One of the game scripts will consider the player wasted when their health drops below this setting value.
		/// </remarks>
		public float InjuryHealthThreshold
		{
			get
			{
				var address = MemoryAddress;
				if (address == IntPtr.Zero)
				{
					return 0.0f;
				}

				int offset = Game.Version >= GameVersion.v1_0_372_2_Steam ? 0x1480 : 0x1470;
				offset = Game.Version >= GameVersion.v1_0_877_1_Steam ? 0x14C8 : offset;
				offset = Game.Version >= GameVersion.v1_0_944_2_Steam ? 0x14D8 : offset;
				offset = Game.Version >= GameVersion.v1_0_1290_1_Steam ? 0x14E0 : offset;
				offset = Game.Version >= GameVersion.v1_0_2060_0_Steam ? 0x1508 : offset;

				return SHVDN.NativeMemory.ReadFloat(address + offset);
			}
			set
			{
				var address = MemoryAddress;
				if (address == IntPtr.Zero)
				{
					return;
				}

				int offset = Game.Version >= GameVersion.v1_0_372_2_Steam ? 0x1480 : 0x1470;
				offset = Game.Version >= GameVersion.v1_0_877_1_Steam ? 0x14C8 : offset;
				offset = Game.Version >= GameVersion.v1_0_944_2_Steam ? 0x14D8 : offset;
				offset = Game.Version >= GameVersion.v1_0_1290_1_Steam ? 0x14E0 : offset;
				offset = Game.Version >= GameVersion.v1_0_2060_0_Steam ? 0x1508 : offset;

				SHVDN.NativeMemory.WriteFloat(address + offset, value);
			}
		}

		/// <summary>
		/// Gets or sets the fatal injury health threshold for this <see cref="Ped"/>.
		/// The pedestrian health will be set to 0.0 when it drops below this value.
		/// </summary>
		/// <value>
		/// The fatal injury health threshold. Should be below <see cref="Entity.MaxHealth"/>.
		/// </value>
		/// <remarks>
		/// Note on player controlled pedestrians: One of the game scripts will consider the player wasted when their health drops below <see cref="Ped.InjuryHealthThreshold"/>, regardless of this setting.
		/// </remarks>
		public float FatalInjuryHealthThreshold
		{
			get
			{
				var address = MemoryAddress;
				if (address == IntPtr.Zero)
				{
					return 0.0f;
				}

				int offset = Game.Version >= GameVersion.v1_0_372_2_Steam ? 0x1484 : 0x1474;
				offset = Game.Version >= GameVersion.v1_0_877_1_Steam ? 0x14CC : offset;
				offset = Game.Version >= GameVersion.v1_0_944_2_Steam ? 0x14DC : offset;
				offset = Game.Version >= GameVersion.v1_0_1290_1_Steam ? 0x14E4 : offset;
				offset = Game.Version >= GameVersion.v1_0_2060_0_Steam ? 0x150C : offset;

				return SHVDN.NativeMemory.ReadFloat(address + offset);
			}
			set
			{
				var address = MemoryAddress;
				if (address == IntPtr.Zero)
				{
					return;
				}

				int offset = Game.Version >= GameVersion.v1_0_372_2_Steam ? 0x1484 : 0x1474;
				offset = Game.Version >= GameVersion.v1_0_877_1_Steam ? 0x14CC : offset;
				offset = Game.Version >= GameVersion.v1_0_944_2_Steam ? 0x14DC : offset;
				offset = Game.Version >= GameVersion.v1_0_1290_1_Steam ? 0x14E4 : offset;
				offset = Game.Version >= GameVersion.v1_0_2060_0_Steam ? 0x150C : offset;

				SHVDN.NativeMemory.WriteFloat(address + offset, value);
			}
		}

		public Vector3 LastWeaponImpactPosition
		{
			get
			{
				NativeVector3 position;
				unsafe
				{
					if (Function.Call<bool>(Hash.GET_PED_LAST_WEAPON_IMPACT_COORD, Handle, &position))
					{
						return position;
					}
				}
				return Vector3.Zero;
			}
		}

		#endregion

		#region Relationship

		public Relationship GetRelationshipWithPed(Ped ped)
		{
			return (Relationship)Function.Call<int>(Hash.GET_RELATIONSHIP_BETWEEN_PEDS, Handle, ped.Handle);
		}

		public RelationshipGroup RelationshipGroup
		{
			get => new RelationshipGroup(Function.Call<int>(Hash.GET_PED_RELATIONSHIP_GROUP_HASH, Handle));
			set => Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, Handle, value.Hash);
		}

		#endregion

		#region Group

		/// <summary>
		/// Gets if this <see cref="Ped"/> is in a <see cref="PedGroup"/>.
		/// </summary>
		public bool IsInGroup => Function.Call<bool>(Hash.IS_PED_IN_GROUP, Handle);

		public void LeaveGroup()
		{
			Function.Call(Hash.REMOVE_PED_FROM_GROUP, Handle);
		}

		public bool NeverLeavesGroup
		{
			set => Function.Call(Hash.SET_PED_NEVER_LEAVES_GROUP, Handle, value);
		}

		/// <summary>
		/// Gets the PedGroup this <see cref="Ped"/> is in.
		/// </summary>
		public PedGroup PedGroup => IsInGroup ? new PedGroup(Function.Call<int>(Hash.GET_PED_GROUP_INDEX, Handle, false)) : null;

		#endregion

		#region Speech & Animation

		public bool CanPlayGestures
		{
			set => Function.Call(Hash.SET_PED_CAN_PLAY_GESTURE_ANIMS, Handle, value);
		}

		public bool IsPainAudioEnabled
		{
			set => Function.Call(Hash.DISABLE_PED_PAIN_AUDIO, Handle, !value);
		}

		public bool IsAmbientSpeechPlaying => Function.Call<bool>(Hash.IS_AMBIENT_SPEECH_PLAYING, Handle);

		public bool IsScriptedSpeechPlaying => Function.Call<bool>(Hash.IS_SCRIPTED_SPEECH_PLAYING, Handle);

		public bool IsAnySpeechPlaying => Function.Call<bool>(Hash.IS_ANY_SPEECH_PLAYING, Handle);

		public bool IsAmbientSpeechEnabled => !Function.Call<bool>(Hash.IS_AMBIENT_SPEECH_DISABLED, Handle);

		public void PlayAmbientSpeech(string speechName, SpeechModifier modifier = SpeechModifier.Standard)
		{
			if (modifier < 0 || (int)modifier >= _speechModifierNames.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(modifier));
			}

			Function.Call(Hash._PLAY_AMBIENT_SPEECH1, Handle, speechName, _speechModifierNames[(int)modifier]);
		}
		public void PlayAmbientSpeech(string speechName, string voiceName, SpeechModifier modifier = SpeechModifier.Standard)
		{
			if (modifier < 0 || (int)modifier >= _speechModifierNames.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(modifier));
			}

			Function.Call(Hash._PLAY_AMBIENT_SPEECH_WITH_VOICE, Handle, speechName, voiceName, _speechModifierNames[(int)modifier], 0);
		}

		/// <summary>
		/// Sets the voice to use when this <see cref="Ped"/> speaks.
		/// </summary>
		public string Voice
		{
			set => Function.Call(Hash.SET_AMBIENT_VOICE_NAME, Handle, value);
		}

		/// <summary>
		/// Sets the animation dictionary or set this <see cref="Ped"/> should use or <c>null</c> to clear it.
		/// </summary>
		public string MovementAnimationSet
		{
			set
			{
				if (value == null)
				{
					Function.Call(Hash.RESET_PED_MOVEMENT_CLIPSET, 0.25f);
					Task.ClearAll();
				}
				else
				{
					// Movement sets can be applied from animation dictionaries and animation sets (also clip sets but they use the same native as animation sets).
					// So check if the string is a valid dictionary, if so load it as such. Otherwise load it as an animation set.
					bool isDict = Function.Call<bool>(Hash.DOES_ANIM_DICT_EXIST, value);

					Function.Call(isDict ? Hash.REQUEST_ANIM_DICT : Hash.REQUEST_ANIM_SET, value);
					var endtime = DateTime.UtcNow + new TimeSpan(0, 0, 0, 0, 1000);

					while (!Function.Call<bool>(isDict ? Hash.HAS_ANIM_DICT_LOADED : Hash.HAS_ANIM_SET_LOADED, value))
					{
						Script.Yield();

						if (DateTime.UtcNow >= endtime)
						{
							return;
						}
					}

					Function.Call(Hash.SET_PED_MOVEMENT_CLIPSET, value, 0.25f);
				}
			}
		}

		#endregion

		public static PedHash[] GetAllModels()
		{
			return SHVDN.NativeMemory.PedModels.Select(x => (PedHash)x).ToArray();
		}
	}
}

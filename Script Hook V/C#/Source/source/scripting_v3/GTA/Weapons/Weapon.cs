//
// Copyright (C) 2015 crosire & contributors
// License: https://github.com/crosire/scripthookvdotnet#license
//

using System.Linq;
using GTA.Native;

namespace GTA
{
	public sealed class Weapon
	{
		#region Fields
		readonly Ped owner;
		WeaponComponentCollection components;
		#endregion

		internal Weapon()
		{
			Hash = WeaponHash.Unarmed;
		}
		internal Weapon(Ped owner, WeaponHash hash)
		{
			this.owner = owner;
			Hash = hash;
		}

		public WeaponHash Hash
		{
			get;
		}

		public string DisplayName => GetDisplayNameFromHash(Hash);

		public string LocalizedName => Game.GetLocalizedString(DisplayName);

		public bool IsPresent => Hash == WeaponHash.Unarmed || Function.Call<bool>(Native.Hash.HAS_PED_GOT_WEAPON, owner.Handle, Hash);

		public Model Model => new Model(Function.Call<int>(Native.Hash.GET_WEAPONTYPE_MODEL, Hash));

		public WeaponTint Tint
		{
			get => Function.Call<WeaponTint>(Native.Hash.GET_PED_WEAPON_TINT_INDEX, owner.Handle, Hash);
			set => Function.Call(Native.Hash.SET_PED_WEAPON_TINT_INDEX, owner.Handle, Hash, value);
		}

		public WeaponGroup Group => Function.Call<WeaponGroup>(Native.Hash.GET_WEAPONTYPE_GROUP, Hash);

		public int Ammo
		{
			get
			{
				if (Hash == WeaponHash.Unarmed)
				{
					return 1;
				}

				if (!IsPresent)
				{
					return 0;
				}

				return Function.Call<int>(Native.Hash.GET_AMMO_IN_PED_WEAPON, owner.Handle, Hash);
			}
			set
			{
				if (Hash == WeaponHash.Unarmed)
				{
					return;
				}

				if (IsPresent)
				{
					Function.Call(Native.Hash.SET_PED_AMMO, owner.Handle, Hash, value);
				}
				else
				{
					Function.Call(Native.Hash.GIVE_WEAPON_TO_PED, owner.Handle, Hash, value, false, true);
				}
			}
		}
		public int AmmoInClip
		{
			get
			{
				if (Hash == WeaponHash.Unarmed)
				{
					return 1;
				}

				if (!IsPresent)
				{
					return 0;
				}

				int ammoInClip;
				unsafe
				{
					Function.Call(Native.Hash.GET_AMMO_IN_CLIP, owner.Handle, Hash, &ammoInClip);
				}
				return ammoInClip;
			}
			set
			{
				if (Hash == WeaponHash.Unarmed)
				{
					return;
				}

				if (IsPresent)
				{
					Function.Call(Native.Hash.SET_AMMO_IN_CLIP, owner.Handle, Hash, value);
				}
				else
				{
					Function.Call(Native.Hash.GIVE_WEAPON_TO_PED, owner.Handle, Hash, value, true, false);
				}
			}
		}

		public int MaxAmmo
		{
			get
			{
				if (Hash == WeaponHash.Unarmed)
				{
					return 1;
				}

				int maxAmmo;
				unsafe
				{
					Function.Call(Native.Hash.GET_MAX_AMMO, owner.Handle, Hash, &maxAmmo);
				}
				return maxAmmo;
			}
		}
		public int MaxAmmoInClip
		{
			get
			{
				if (Hash == WeaponHash.Unarmed)
				{
					return 1;
				}

				if (!IsPresent)
				{
					return 0;
				}

				return Function.Call<int>(Native.Hash.GET_MAX_AMMO_IN_CLIP, owner.Handle, Hash, true);
			}
		}

		public int DefaultClipSize => Function.Call<int>(Native.Hash.GET_WEAPON_CLIP_SIZE, Hash);

		public bool InfiniteAmmo
		{
			set
			{
				if (Hash == WeaponHash.Unarmed)
				{
					return;
				}

				Function.Call(Native.Hash.SET_PED_INFINITE_AMMO, owner.Handle, value, Hash);
			}
		}
		public bool InfiniteAmmoClip
		{
			set => Function.Call(Native.Hash.SET_PED_INFINITE_AMMO_CLIP, owner.Handle, value);
		}

		public bool CanUseOnParachute => Function.Call<bool>(Native.Hash.CAN_USE_WEAPON_ON_PARACHUTE, Hash);

		public WeaponComponentCollection Components => components ?? (components = new WeaponComponentCollection(owner, this));

		public static implicit operator WeaponHash(Weapon weapon)
		{
			return weapon.Hash;
		}

		public static string GetDisplayNameFromHash(WeaponHash hash)
		{
			switch (hash)
			{
				case WeaponHash.Pistol:
					return "WT_PIST";
				case WeaponHash.CombatPistol:
					return "WT_PIST_CBT";
				case WeaponHash.APPistol:
					return "WT_PIST_AP";
				case WeaponHash.SMG:
					return "WT_SMG";
				case WeaponHash.MicroSMG:
					return "WT_SMG_MCR";
				case WeaponHash.AssaultRifle:
					return "WT_RIFLE_ASL";
				case WeaponHash.CarbineRifle:
					return "WT_RIFLE_CBN";
				case WeaponHash.AdvancedRifle:
					return "WT_RIFLE_ADV";
				case WeaponHash.MG:
					return "WT_MG";
				case WeaponHash.CombatMG:
					return "WT_MG_CBT";
				case WeaponHash.PumpShotgun:
					return "WT_SG_PMP";
				case WeaponHash.SawnOffShotgun:
					return "WT_SG_SOF";
				case WeaponHash.AssaultShotgun:
					return "WT_SG_ASL";
				case WeaponHash.HeavySniper:
					return "WT_SNIP_HVY";
				case WeaponHash.SniperRifle:
					return "WT_SNIP_RIF";
				case WeaponHash.GrenadeLauncher:
					return "WT_GL";
				case WeaponHash.RPG:
					return "WT_RPG";
				case WeaponHash.Minigun:
					return "WT_MINIGUN";
				case WeaponHash.AssaultSMG:
					return "WT_SMG_ASL";
				case WeaponHash.BullpupShotgun:
					return "WT_SG_BLP";
				case WeaponHash.Pistol50:
					return "WT_PIST_50";
				case WeaponHash.Bottle:
					return "WT_BOTTLE";
				case WeaponHash.Gusenberg:
					return "WT_GUSENBERG";
				case WeaponHash.SNSPistol:
					return "WT_SNSPISTOL";
				case WeaponHash.VintagePistol:
					return "TT_VPISTOL";
				case WeaponHash.Dagger:
					return "WT_DAGGER";
				case WeaponHash.FlareGun:
					return "WT_FLAREGUN";
				case WeaponHash.Musket:
					return "WT_MUSKET";
				case WeaponHash.Firework:
					return "WT_FWRKLNCHR";
				case WeaponHash.MarksmanRifle:
					return "WT_HMKRIFLE";
				case WeaponHash.HeavyShotgun:
					return "WT_HVYSHOT";
				case WeaponHash.ProximityMine:
					return "WT_PRXMINE";
				case WeaponHash.HomingLauncher:
					return "WT_HOMLNCH";
				case WeaponHash.CombatPDW:
					return "WT_COMBATPDW";
				case WeaponHash.KnuckleDuster:
					return "WT_KNUCKLE";
				case WeaponHash.MarksmanPistol:
					return "WT_MKPISTOL";
				case WeaponHash.Machete:
					return "WT_MACHETE";
				case WeaponHash.MachinePistol:
					return "WT_MCHPIST";
				case WeaponHash.Flashlight:
					return "WT_FLASHLIGHT";
				case WeaponHash.DoubleBarrelShotgun:
					return "WT_DBSHGN";
				case WeaponHash.CompactRifle:
					return "WT_CMPRIFLE";
				case WeaponHash.SwitchBlade:
					return "WT_SWBLADE";
				case WeaponHash.Revolver:
					return "WT_REVOLVER";
			}

			DlcWeaponData data;
			for (int i = 0, max = Function.Call<int>(Native.Hash.GET_NUM_DLC_WEAPONS); i < max; i++)
			{
				unsafe
				{
					if (Function.Call<bool>(Native.Hash.GET_DLC_WEAPON_DATA, i, &data))
					{
						if (data.Hash == hash)
						{
							return data.DisplayName;
						}
					}
				}
			}

			return "WT_INVALID";
		}

		public static Model[] GetAllModels()
		{
			return SHVDN.NativeMemory.WeaponModels.Select(x => new Model(x)).ToArray();
		}
	}
}

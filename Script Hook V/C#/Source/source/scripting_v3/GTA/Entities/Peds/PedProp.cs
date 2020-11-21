//
// Copyright (C) 2015 crosire & contributors
// License: https://github.com/crosire/scripthookvdotnet#license
//

using GTA.Native;

namespace GTA
{
	public class PedProp : IPedVariation
	{
		#region Fields
		readonly Ped _ped;
		#endregion

		internal PedProp(Ped ped, PedPropType propId)
		{
			_ped = ped;
			Type = propId;
		}

		public string Name => Type.ToString();

		public PedPropType Type
		{
			get;
		}

		public int Count => Function.Call<int>(Hash.GET_NUMBER_OF_PED_PROP_DRAWABLE_VARIATIONS, _ped.Handle, Type) + 1;

		public int Index
		{
			get => Function.Call<int>(Hash.GET_PED_PROP_INDEX, _ped.Handle, Type) + 1;
			set => SetVariation(value);
		}

		public int TextureCount => Function.Call<int>(Hash.GET_NUMBER_OF_PED_PROP_TEXTURE_VARIATIONS, _ped.Handle, Type, Index - 1);

		public int TextureIndex
		{
			get
			{
				return Index == 0 ? 0 : Function.Call<int>(Hash.GET_PED_PROP_TEXTURE_INDEX, _ped.Handle, Type);
			}
			set
			{
				if (Index > 0)
				{
					SetVariation(Index, value);
				}
			}
		}

		public bool SetVariation(int index, int textureIndex = 0)
		{
			if (index == 0)
			{
				Function.Call(Hash.CLEAR_PED_PROP, _ped.Handle, Type);
				return true;
			}

			if (!IsVariationValid(index, textureIndex))
			{
				return false;
			}

			Function.Call(Hash.SET_PED_PROP_INDEX, _ped.Handle, Type, index - 1, textureIndex, 1);
			return true;
		}

		public bool IsVariationValid(int index, int textureIndex = 0)
		{
			if (index == 0)
			{
				return true; // No prop is always valid
			}

			return Function.Call<bool>(Hash._IS_PED_PROP_VALID, _ped.Handle, Type, index - 1, textureIndex);
		}

		public bool HasVariations => Count > 1;

		public bool HasTextureVariations => Count > 1 && TextureCount > 1;

		public bool HasAnyVariations => HasVariations;

		public override string ToString()
		{
			return Type.ToString();
		}
	}
}

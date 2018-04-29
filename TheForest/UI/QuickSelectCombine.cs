﻿using System;
using TheForest.Items;
using TheForest.Items.Craft;
using TheForest.Items.Craft.Interfaces;
using TheForest.Items.Inventory;
using TheForest.Items.World;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

namespace TheForest.UI
{
	
	public class QuickSelectCombine : MonoBehaviour, ICraftOverride
	{
		
		private void Update()
		{
			if (this._stopCombining)
			{
				this._stopCombining = false;
				this._isCombining = false;
			}
			if (this._hovered || (this._isCraft && this._isCombining))
			{
				if (this._isCombining)
				{
					this.CombineUpdate();
				}
				else if (TheForest.Utils.Input.GetButtonDown("Combine"))
				{
					this.ToggleView();
				}
			}
			if (this._isCraft)
			{
				bool flag = !this._isCombining;
				if (flag && TheForest.Utils.Input.GetButtonDown("Drop"))
				{
					this.ToggleView();
				}
				if (Scene.HudGui.DropToRemoveAllInfo.activeSelf != flag)
				{
					Scene.HudGui.DropToRemoveAllInfo.SetActive(flag);
				}
			}
		}

		
		private void OnEnable()
		{
			if (this._isCraft)
			{
				for (int i = 0; i < LocalPlayer.Inventory.QuickSelectItemIds.Length; i++)
				{
					int itemId = LocalPlayer.Inventory.QuickSelectItemIds[i];
					if (itemId > 0)
					{
						if (this._crafting.Ingredients.Any((ReceipeIngredient ing) => ing._itemID == itemId))
						{
							ItemProperties propertiesOf = this._crafting.GetPropertiesOf(itemId);
							this._crafting.Remove(itemId, 1, propertiesOf);
							LocalPlayer.Inventory.AddItem(itemId, 1, true, true, propertiesOf);
							LocalPlayer.Inventory.GetLastActiveView(itemId).gameObject.SetActive(false);
							this._hiddenInventoryItemView[i] = true;
						}
						else if (LocalPlayer.Inventory.AmountOf(itemId, false) >= 1 && LocalPlayer.Inventory.InventoryItemViewsCache[itemId][0].gameObject.activeSelf)
						{
							LocalPlayer.Inventory.InventoryItemViewsCache[itemId][0].gameObject.SetActive(false);
							this._hiddenInventoryItemView[i] = true;
						}
					}
				}
			}
		}

		
		private void OnDisable()
		{
			if (Scene.HudGui)
			{
				Scene.HudGui.HideQuickSelectInfo();
			}
			if (this._isCraft)
			{
				for (int i = 0; i < LocalPlayer.Inventory.QuickSelectItemIds.Length; i++)
				{
					int num = LocalPlayer.Inventory.QuickSelectItemIds[i];
					if (this._hiddenInventoryItemView[i] && num > 0)
					{
						LocalPlayer.Inventory.InventoryItemViewsCache[num][0].gameObject.SetActive(true);
					}
					this._hiddenInventoryItemView[i] = false;
				}
				this.StopCombining();
				this.OnMouseExitCollider();
				this.ToggleView();
			}
		}

		
		private void OnMouseExitCollider()
		{
			if (this._hovered && !this._isCombining)
			{
				Scene.HudGui.HideQuickSelectInfo();
				Scene.HudGui.ClickToRemoveInfo.SetActive(false);
				base.gameObject.GetComponent<Renderer>().sharedMaterial = this._normalMaterial;
				this._hovered = false;
			}
		}

		
		private void OnMouseEnterCollider()
		{
			if (!this._hovered && !this._isCombining)
			{
				this._hovered = true;
				if (base.gameObject.GetComponent<Renderer>() && this._normalMaterial != base.gameObject.GetComponent<Renderer>().sharedMaterial)
				{
					this._normalMaterial = base.gameObject.GetComponent<Renderer>().sharedMaterial;
				}
				base.gameObject.GetComponent<Renderer>().sharedMaterial = this._selectedMaterial;
				if (!this._isCraft)
				{
					Scene.HudGui.ShowQuickSelectInfo(LocalPlayer.InventoryCam.ScreenToViewportPoint(TheForest.Utils.Input.mousePosition));
				}
				else
				{
					Scene.HudGui.ClickToRemoveInfo.SetActive(true);
				}
			}
		}

		
		private void ToggleView()
		{
			if (this._isCraft)
			{
				this._crafting.CraftOverride = null;
			}
			else
			{
				this._crafting.CraftOverride = this._other;
			}
			this._crafting.CheckForValidRecipe();
			LocalPlayer.Sfx.PlayWhoosh();
			this.OnMouseExitCollider();
			this._other.gameObject.SetActive(true);
			base.gameObject.SetActive(false);
		}

		
		private void SetItemAsQuickSlot(int slotNum)
		{
			try
			{
				for (int i = 0; i < LocalPlayer.Inventory.QuickSelectItemIds.Length; i++)
				{
					if (LocalPlayer.Inventory.QuickSelectItemIds[i] == this._combiningItemId && i != slotNum)
					{
						LocalPlayer.Inventory.QuickSelectItemIds[i] = -1;
						this._hiddenInventoryItemView[i] = false;
					}
				}
				if (LocalPlayer.Inventory.QuickSelectItemIds[slotNum] != this._combiningItemId)
				{
					if (this._hiddenInventoryItemView[slotNum] && LocalPlayer.Inventory.QuickSelectItemIds[slotNum] > 0)
					{
						LocalPlayer.Inventory.InventoryItemViewsCache[LocalPlayer.Inventory.QuickSelectItemIds[slotNum]][0].gameObject.SetActive(true);
					}
					LocalPlayer.Inventory.QuickSelectItemIds[slotNum] = this._combiningItemId;
					LocalPlayer.Inventory.AddItem(this._combiningItemId, 1, true, true, this._combiningItemProperties);
					InventoryItemView lastActiveView = LocalPlayer.Inventory.GetLastActiveView(this._combiningItemId);
					LocalPlayer.Inventory.BubbleDownInventoryView(lastActiveView);
					lastActiveView.gameObject.SetActive(false);
					this._hiddenInventoryItemView[slotNum] = true;
					this._combiningItemId = 0;
				}
				LocalPlayer.Sfx.PlayTwinkle();
			}
			finally
			{
				this._startingCombine = false;
				this._isCombining = true;
				this.StopCombining();
			}
			foreach (QuickSelectViews quickSelectViews2 in LocalPlayer.QuickSelectViews)
			{
				quickSelectViews2.ShowLocalPlayerViews();
			}
		}

		
		private void CombineUpdate()
		{
			int targetSlot = this.GetTargetSlot();
			for (int i = 0; i < this._cogs.Length; i++)
			{
				this._cogs[i].sharedMaterial = ((i != targetSlot) ? this._cogDefaultMaterial : this._cogSelectedMaterial);
			}
			if (TheForest.Utils.Input.GetButtonDown("AltFire"))
			{
				if (targetSlot >= 0)
				{
					this.SetItemAsQuickSlot(targetSlot);
				}
				else
				{
					LocalPlayer.Sfx.PlayWhoosh();
					this.StopCombining();
				}
			}
			bool flag = targetSlot >= 0;
			if (Scene.HudGui._quickSelectInfoView._selectButton.activeSelf != flag)
			{
				Scene.HudGui._quickSelectInfoView._selectButton.SetActive(flag);
			}
			if (Scene.HudGui._quickSelectInfoView._exitButton.activeSelf == flag)
			{
				Scene.HudGui._quickSelectInfoView._exitButton.SetActive(!flag);
			}
		}

		
		private int GetTargetSlot()
		{
			int result = -1;
			float num = float.MaxValue;
			for (int i = 0; i < this._slots.Length; i++)
			{
				Vector3 vector = LocalPlayer.InventoryCam.WorldToScreenPoint(this._slots[i].position);
				float num2 = Mathf.Abs(vector.x - TheForest.Utils.Input.mousePosition.x) / (float)Screen.width;
				float num3 = Mathf.Abs(vector.y - TheForest.Utils.Input.mousePosition.y) / (float)Screen.height;
				if (num2 < 0.05f && num3 < 0.05f && num2 + num3 < num)
				{
					num = num2 + num3;
					result = i;
				}
			}
			return result;
		}

		
		public bool CanCombine()
		{
			if (!this._isCombining && this._crafting.Ingredients.Count == 1)
			{
				ReceipeIngredient receipeIngredient = this._crafting.Ingredients.First<ReceipeIngredient>();
				if (receipeIngredient._amount == 1)
				{
					int itemID = receipeIngredient._itemID;
					Item item = ItemDatabase.ItemById(itemID);
					if ((item.MatchType(this._acceptedTypes) && item._equipmentSlot == Item.EquipmentSlot.RightHand) || item._allowQuickSelect)
					{
						return true;
					}
				}
			}
			return false;
		}

		
		public void Combine()
		{
			if (!this._isCombining)
			{
				LocalPlayer.Sfx.PlayWhoosh();
				this._slotsGo.SetActive(true);
				this._combiningItemId = this._crafting.Ingredients.First<ReceipeIngredient>()._itemID;
				this._combiningItemProperties = this._crafting.GetPropertiesOf(this._combiningItemId);
				this._crafting.Remove(this._combiningItemId, 1, null);
				for (int i = 0; i < LocalPlayer.Inventory.QuickSelectItemIds.Length; i++)
				{
					if (LocalPlayer.Inventory.QuickSelectItemIds[i] == this._combiningItemId)
					{
						LocalPlayer.Inventory.QuickSelectItemIds[i] = -1;
						this._hiddenInventoryItemView[i] = false;
						foreach (QuickSelectViews quickSelectViews2 in LocalPlayer.QuickSelectViews)
						{
							quickSelectViews2.ShowLocalPlayerViews();
						}
						break;
					}
				}
				this._isCombining = true;
				this._hovered = false;
				this._crafting.CheckForValidRecipe();
				Scene.HudGui._quickSelectInfoView._root.SetActive(true);
				Scene.HudGui._quickSelectInfoView._selectButton.SetActive(false);
				this._startingCombine = true;
				LocalPlayer.Inventory.QuickSelectGamepadSwitch = true;
				LocalPlayer.Inventory.BlockTogglingInventory = true;
			}
		}

		
		private void StopCombining()
		{
			if (this._startingCombine)
			{
				this._startingCombine = false;
			}
			else if (this._isCombining)
			{
				this._stopCombining = true;
				this._slotsGo.SetActive(false);
				if (this._combiningItemId > 0)
				{
					this._crafting.Add(this._combiningItemId, 1, this._combiningItemProperties);
				}
				this._crafting.Close();
				this._crafting.CheckForValidRecipe();
				Scene.HudGui._quickSelectInfoView._root.SetActive(false);
				LocalPlayer.Inventory.QuickSelectGamepadSwitch = false;
				LocalPlayer.Inventory.BlockTogglingInventory = false;
			}
		}

		
		
		public bool IsCombining
		{
			get
			{
				return this._isCombining;
			}
		}

		
		
		public Item.Types AcceptedTypes
		{
			get
			{
				return this._acceptedTypes;
			}
		}

		
		[EnumFlags]
		public Item.Types _acceptedTypes;

		
		public bool _isCraft;

		
		public Material _selectedMaterial;

		
		public CraftingCog _crafting;

		
		public QuickSelectCombine _other;

		
		public GameObject _slotsGo;

		
		public Transform[] _slots;

		
		public Renderer[] _cogs;

		
		public Material _cogDefaultMaterial;

		
		public Material _cogSelectedMaterial;

		
		private bool[] _hiddenInventoryItemView = new bool[4];

		
		private int _combiningItemId;

		
		private ItemProperties _combiningItemProperties;

		
		private bool _startingCombine;

		
		private bool _stopCombining;

		
		private bool _isCombining;

		
		private bool _hovered;

		
		private Material _normalMaterial;

		
		private Vector3 _initialObjectPosition;
	}
}

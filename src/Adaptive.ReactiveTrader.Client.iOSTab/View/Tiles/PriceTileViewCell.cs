﻿
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Adaptive.ReactiveTrader.Shared.DTO.ReferenceData;
using System.ComponentModel;
using MonoTouch.CoreGraphics;
using Adaptive.ReactiveTrader.Client.iOSTab.Tiles;
using System.IO;
using Adaptive.ReactiveTrader.Client.UI.SpotTiles;

namespace Adaptive.ReactiveTrader.Client.iOSTab
{
	public partial class PriceTileViewCell : UITableViewCell, IPriceTileCell
	{
		public static readonly UINib Nib = UINib.FromName ("PriceTileViewCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("PriceTileViewCell");

		private PriceTileModel _priceTileModel;
		private static UserModel _userModel;

		public PriceTileViewCell (IntPtr handle) : base (handle)
		{
		}

		public static PriceTileViewCell Create (UserModel userModel)
		{
			PriceTileViewCell created = (PriceTileViewCell)Nib.Instantiate (null, null) [0];
			created.ContentView.BackgroundColor = Styles.RTDarkerBlue;
			_userModel = userModel;
			return created;
		}
			
		public void UpdateFrom (PriceTileModel model)
		{
			_priceTileModel = model;

			this.CurrencyPair.Text = model.Symbol;

			this.LeftSideAction.Text = "SELL";

			this.RightSideAction.Text = "BUY";

			this.LeftSideNumber.Text = model.LeftSideNumber;
			this.LeftSideBigNumber.Text = model.LeftSideBigNumber;
			this.LeftSidePips.Text = model.LeftSidePips;

			this.RightSideNumber.Text = model.RightSideNumber;
			this.RightSideBigNumber.Text = model.RightSideBigNumber;
			this.RightSidePips.Text = model.RightSidePips;

			this.Notional.Text = model.Notional;

			this.Executing.Hidden = model.Status != PriceTileStatus.Executing;

			this.Spread.Text = model.Spread;

			switch (model.Movement) {
				case PriceMovement.Down:
					this.PriceMovementDown.Hidden = false;
					this.PriceMovementUp.Hidden = true;
					break;
				case PriceMovement.Up:
					this.PriceMovementDown.Hidden = true;
					this.PriceMovementUp.Hidden = false;
					break;
				case PriceMovement.None:
					this.PriceMovementDown.Hidden = true;
					this.PriceMovementUp.Hidden = true;
					break;
			}
			model.Rendered ();
		}

		partial void LeftSideButtonTouchUpInside (NSObject sender)
		{
			// TODO 
			var model = _priceTileModel;
			if (model != null && model.Status == PriceTileStatus.Streaming) {
				//
				// TODO: Determine where to best place the check for trading enabled.
				// Where would we implenet two-touch, or more complex order entry?
				//
				if (_userModel.GetOneTouchTradingEnabled() && model.Bid()) {
					_userModel.SetOneTouchTradingEnabled(false);
				}
			}
		}

		partial void RightSideButtonTouchUpInside (NSObject sender)
		{
			var model = _priceTileModel;
			if (model != null && model.Status == PriceTileStatus.Streaming) {
				if (_userModel.GetOneTouchTradingEnabled() && model.Ask()); {
					_userModel.SetOneTouchTradingEnabled(false);
				}
			}
		}

		partial void NotionalValueChanged (NSObject sender)
		{
			var model = _priceTileModel;
			if (model != null)
				model.Notional = Notional.Text;
		}
	}
}


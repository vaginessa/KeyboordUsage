using System;
using System.Linq;
using System.Windows.Forms;
using KeyboordUsage.Gui;

namespace KeyboordUsage
{
	public class KeyPressController
	{
		private string cachedKeyPopularity = "";
		private GuiKeyboard keyboard;

		private readonly Func<bool> isWindowMinimized;
		private readonly Action<string> updateCurrentKey;
		private readonly Action<string> updateKeyPopularity;
		private readonly Action<double> updateProductiveRatio;
		private readonly Action<double> updateDestructiveRatio;
		private readonly Action<double> updateNaviationRatio;
		private readonly Action<double> updateMetaRatio;
		private readonly KeysCounter counter;
		private string currentKey = "";

		public KeyPressController(
			Func<bool> isWindowMinimized, 
			Action<string> updateCurrentKey, 
			Action<string> updateKeyPopularity, 
			Action<double> updateProductiveRatio,
			Action<double> updateDestructiveRatio,
			Action<double> updateNaviationRatio,
			Action<double> updateMetaRatio,
			KeysCounter counter, 
			GuiKeyboard keyboard)
		{
			this.isWindowMinimized = isWindowMinimized;
			this.updateCurrentKey = updateCurrentKey;
			this.updateKeyPopularity = updateKeyPopularity;
			this.updateProductiveRatio = updateProductiveRatio;
			this.updateDestructiveRatio = updateDestructiveRatio;
			this.updateNaviationRatio = updateNaviationRatio;
			this.updateMetaRatio = updateMetaRatio;
			this.counter = counter;
			this.keyboard = keyboard;
		}

		public void ForceRepaint()
		{
			updateCurrentKey(currentKey);

			updateProductiveRatio(counter.GetProductiveRatio());
			updateDestructiveRatio(counter.GetDestructiveRatio());
			updateNaviationRatio(counter.GetNavigationRatio());
			updateMetaRatio(counter.GetMetaRatio());

			cachedKeyPopularity = GetKeyPopularity();
			updateKeyPopularity(cachedKeyPopularity);

			new HeatmapPainter(keyboard, counter).Do();
		}

		public void ChangeKeyboard(GuiKeyboard newKeyboard)
		{
			keyboard = newKeyboard;
		}

		private string GetKeyPopularity()
		{
			int no = 0;
			return string.Join("\n", counter.GetAccumulatedKeyPopularity().Select(x => (++no) + ". " + x.ToString()));
		}

		public void Paint(Keys keyData)
		{
			currentKey = keyData.ToString(); //string current = "code " + e.KeyCode + " value: " + e.KeyValue + " data: "+e.KeyData ;

			if (RepaintAlmostOnlyWhenVisible())
			{
				updateCurrentKey(currentKey); 
				ForceRepaint();
			}
		}

		private bool RepaintAlmostOnlyWhenVisible()
		{
			if (!isWindowMinimized())
				return true;

			return false;
		}
	}
}

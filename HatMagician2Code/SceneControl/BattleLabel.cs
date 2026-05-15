using MegaCrit.Sts2.addons.mega_text;

namespace HatMagician2.HatMagician2Code.SceneControl;

public partial class BattleLabel : MegaLabel
{
	public void TrySetText(string text)
	{
		if (this.Text == text)
			return;
		this.Text = text;
	}
}
